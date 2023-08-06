using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DriverController : ControllerBase //RootController<Car,ICarRepository>
{
    // private readonly ICarService _service;
    private readonly IGenericRepository<Driver> _repository;
    protected readonly IMapper _map;
    public DriverController(IGenericRepository<Driver> repository , IMapper map) //: base(repository)
    {
        _repository = repository;
        _map = map;
    } 
       
    [HttpGet(template : "All")]
    public async Task<PagingModel<Driver>> GetListAsync(DriverRequestDTO input) 
    {
        var query = _repository.GetQueryable();
        var searchingResult = query.ApplySearching(input.SearchingColumn, input.SearchingValue);
        int countFilterd = searchingResult.Count();
        var sortingResult = searchingResult.ApplySorting(input.OrderByData);
        var pagingResult = sortingResult.ApplyPaging(input.CurrentPage, input.RowsPerPage , false);
        var finalQuery = await pagingResult.GetResult(input.CurrentPage, input.RowsPerPage , countFilterd);
        return finalQuery;
    }
    [HttpGet("{id}")]
    public async Task<DriverDTO> GetAsync(Guid id) 
    {
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<DriverDTO>(getOne);
        return result;
    }

    [HttpPost]
    public DriverDTO Create(DriverCreateDTO driverDTO)
    {
        if (!ModelState.IsValid)
            throw new Exception("Validation failed. Please check the input and correct any errors.");
        Driver driver = _map.Map<Driver>(driverDTO);
        _repository.Add(driver);
        var res = _map.Map<DriverDTO>(driver);
        return res;
    }

    [HttpPut("{id}")]
    public void Update(Guid id, DriverUpdateDTO driverDTO)
    {
        if (id != driverDTO.DriverId)
            throw new Exception("Object id is not compatible with the pass id");
        Driver driver = _map.Map<Driver>(driverDTO);
        _repository.Update(driver);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("This id is invalid");
        _repository.Delete(entity);
    }
}