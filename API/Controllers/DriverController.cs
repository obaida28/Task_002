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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    public DriverController(IUnitOfWork unitOfWork , IMapper map)
    {
        _unitOfWork = unitOfWork;
        _map = map;
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<PagingResult<DriverDTO>> GetListAsync(DriverRequestDTO input)
    {
        if(input == null)
            throw new BadHttpRequestException("Requied input");

        var query = _unitOfWork.Drivers.GetQueryable();

        bool withSearching = input.SearchingValue != null;
        if(withSearching) query = query.Where(c => 
            c.Name.ToLower().Contains(input.SearchingValue));

        int countFilterd = await query.CountAsync();

        bool withSorting = input.OrderByData != null;
        if(withSorting) query = input.ASC ? query.OrderBy(c => c.Name) :
             query.OrderByDescending(c => c.Name);
        
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
        var getOne = await _unitOfWork.Drivers.GetByIdAsync(id);
        var result = _map.Map<DriverDTO>(getOne);
        return result;
    }

    [HttpPost]
    public async Task<DriverDTO> CreateAsync(DriverCreateDTO input)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        Driver driver = _map.Map<Driver>(input);
        await _unitOfWork.Drivers.AddAsync(driver);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
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
        await _unitOfWork.Drivers.UpdateAsync(driver);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        var entity = await _unitOfWork.Drivers.GetByIdAsync(id) ?? throw new BadHttpRequestException("This id is invalid");
        await _unitOfWork.Drivers.DeleteAsync(entity);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
    }
}