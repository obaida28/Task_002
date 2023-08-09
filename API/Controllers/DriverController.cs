using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class DriverController : ControllerBase 
{
    private readonly IDriverRepository _repository; 
    private readonly IMapper _map;
    public DriverController(IDriverRepository repository , IMapper map)
    {
        _repository = repository;
        _map = map;
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<PagingModel<Driver>> GetListAsync(DriverRequestDTO input) 
    {
        var query = _repository.GetQueryable();
        var searchingResult = ApplySearching(query , input.SearchingColumn, input.SearchingValue);
        int countFilterd = searchingResult.Count();
        var sortingResult = searchingResult.ApplySorting(input.OrderByData);
        var pagingResult = sortingResult.ApplyPaging(input.CurrentPage, input.RowsPerPage , false);
        var finalQuery = await pagingResult.GetResult(input.CurrentPage, input.RowsPerPage , countFilterd);
        return finalQuery;
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    private IQueryable<Driver> ApplySearching(IQueryable<Driver> query , string colName , string value)
    {
        switch(colName)
        {
            case "Id" : 
                return query.Where(d => d.Id == new Guid(value));
            default :
                return query.Where(d => d.DriverName.Contains(value.ToString()));
        }
    }
    [HttpGet("{id}")]
    public async Task<DriverDTO> GetAsync(Guid id) 
    {
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<DriverDTO>(getOne);
        return result;
    }

    [HttpPost]
    public async Task<DriverDTO> CreateAsync(DriverCreateDTO driverCreateDTO)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        Driver driver = _map.Map<Driver>(driverCreateDTO);
        await _repository.AddAsync(driver);
        var res = _map.Map<DriverDTO>(driver);
        return res;
    }

    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, DriverUpdateDTO driverUpdateDTO)
    {
        if (id != driverUpdateDTO.Id)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Driver driver = _map.Map<Driver>(driverUpdateDTO);
        await _repository.UpdateAsync(driver);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new BadHttpRequestException("This id is invalid");
        await _repository.DeleteAsync(entity);
    }
}