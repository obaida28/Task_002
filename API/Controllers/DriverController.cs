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
[Route("api/[controller]")]
[ApiController]
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
    public async Task<PagingResult<DriverDTO>> GetListAsync(DriverRequestDTO input)
    {
        if(input == null)
            throw new BadHttpRequestException("Requied input");

        var query = _repository.GetQueryable();

        bool withSearching = input.SearchingValue != null;
        if(withSearching) query = query.Where(c => 
            c.DriverName.ToLower().Contains(input.SearchingValue));

        int countFilterd = await query.CountAsync();

        bool withSorting = input.OrderByData != null;
        if(withSorting) query = input.ASC ? query.OrderBy(c => c.DriverName) :
             query.OrderByDescending(c => c.DriverName);
        
        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await query.GetResultAsync
            (withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<DriverDTO>>(entityResult);
        return dtoResult;
    }
    // [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("{id}")]
    public async Task<DriverDTO> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<DriverDTO>(getOne);
        return result;
    }

    [HttpPost]
    public async Task<DriverDTO> CreateAsync(DriverCreateDTO input)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        Driver driver = _map.Map<Driver>(input);
        await _repository.AddAsync(driver);
        var res = _map.Map<DriverDTO>(driver);
        return res;
    }

    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, DriverUpdateDTO input)
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        if (id != input.Id)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Driver driver = _map.Map<Driver>(input);
        await _repository.UpdateAsync(driver);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        var entity = await _repository.GetByIdAsync(id) ?? throw new BadHttpRequestException("This id is invalid");
        await _repository.DeleteAsync(entity);
    }
}