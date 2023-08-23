using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.ErrorResponse;
using API.CustomFilters;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilterAttribute]
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
    public async Task<ApiResponse> GetListAsync(DriverRequestDTO input)
    {
        if(input == null)
            return ApiBadRequestResponse.BADresponse("Requied input");

        var query = _unitOfWork.Drivers.GetQueryable();

        bool withSearching = !string.IsNullOrEmpty(input.SearchingValue);
        if(withSearching) query = query.Where(d => d.Name.ToLower().Contains(input.SearchingValue));

        int countFilterd = await query.CountAsync();

        bool withSorting = !string.IsNullOrEmpty(input.OrderByData);
        if(withSorting) 
        {
            string dataOrder = input.OrderByData.ToLower();
            string[] orderResult = dataOrder.Split(" ");
            bool IsDesc = orderResult.Last() == "desc";
            query = !IsDesc ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name);
        }
        
        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await query.GetResultAsync
            (withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<DriverDTO>>(entityResult);
        return ApiOkResponse.OKresponse(dtoResult);
    }
    
    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        var getOne = await _unitOfWork.Drivers.GetByIdAsync(id);
        var result = _map.Map<DriverDTO>(getOne);
        return ApiOkResponse.OKresponse(result);
    }

    [HttpPost]
    public async Task<ApiResponse> CreateAsync(DriverCreateDTO input)
    {
        Driver driver = _map.Map<Driver>(input);
        await _unitOfWork.Drivers.AddAsync(driver);
        var result = await _unitOfWork.SaveAsync();
        var dto = _map.Map<DriverDTO>(driver);
        return ApiResponse.response(result , dto);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id, DriverUpdateDTO input)
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        if (id != input.Id)
            return ApiBadRequestResponse.BADresponse("Object id is not compatible with the pass id");
        Driver driver = _map.Map<Driver>(input);
        _unitOfWork.Drivers.Update(driver);
        var result = await _unitOfWork.SaveAsync();
       return ApiResponse.response(result);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        var entity = await _unitOfWork.Drivers.GetByIdAsync(id);
        if(entity == null)
            return ApiNotFoundResponse.NOTresponse("This id is invalid");
        _unitOfWork.Drivers.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.response(result);
    }
}