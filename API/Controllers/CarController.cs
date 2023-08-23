using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using API.CustomFilters;
using API.ErrorResponse;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using System.Linq.Dynamic.Core;
using System.Net;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilterAttribute]
public class CarController : ControllerBase 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    public CarController(IUnitOfWork unitOfWork , IMapper map)
    {
        _unitOfWork = unitOfWork;
        _map = map;
    }
    
    [HttpGet(template: "GetListAsync")]
    public async Task<ApiResponse> GetListAsync(CarRequestDTO input) 
    {
        var query = _unitOfWork.Cars.GetQueryable();

        bool withSearching = input.SearchingValue != null;
        if(withSearching) 
        {
            bool withDecimal = decimal.TryParse(input.SearchingValue, out decimal decimalValue);
            bool withInt = int.TryParse(input.SearchingValue, out int intValue);
             query = query.Where(c => 
                c.Type.ToLower().Contains(input.SearchingValue) || 
                c.Color.ToLower().Contains(input.SearchingValue) || 
                c.Number.ToLower().Contains(input.SearchingValue) ||
                (withDecimal && c.EngineCapacity == decimalValue) || (withInt && c.DailyRate == intValue));
        }
           
        int countFilterd = await query.CountAsync();

        bool withSorting = input.OrderByData != null;
        if(withSorting) 
        {
            string dataOrder = input.OrderByData.ToLower();
            string[] orderResult = dataOrder.Split(" ");
            bool IsDesc = orderResult.Last() == "desc";
            query = orderResult.First() switch
            {
                "type" => !IsDesc ? query.OrderBy(c => c.Type) : query.OrderByDescending(c => c.Type),
                "color" => !IsDesc ? query.OrderBy(c => c.Color) : query.OrderByDescending(c => c.Color),
                "enginecapacity" => !IsDesc ? query.OrderBy(c => c.EngineCapacity) : query.OrderByDescending(c => c.EngineCapacity),
                "dailyrate" => !IsDesc ? query.OrderBy(c => c.DailyRate) : query.OrderByDescending(c => c.DailyRate),
                _ => !IsDesc ? query.OrderBy(c => c.Number) : query.OrderByDescending(c => c.Number),
            };
        }
        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await query.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<CarDTO>>(entityResult);
        return ApiOkResponse.OKresponse(dtoResult);
    }
    
    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        var getOne = await _unitOfWork.Cars.GetByIdAsync(id);
        var result = _map.Map<CarDTO>(getOne);
        return ApiOkResponse.OKresponse(result);
    }
    
    [HttpPost]
    public async Task<ApiResponse> CreateAsync(CarCreateDto carCreateDto)
    {
        bool isExist = await _unitOfWork.Cars.IsExistNumberAsync(carCreateDto.Number);
        if(isExist) return ApiBadRequestResponse.BADresponse( "The car number is unique !");
        var entity = _map.Map<Car>(carCreateDto);
        await _unitOfWork.Cars.AddAsync(entity);
        var result = await _unitOfWork.SaveAsync();
        var dto = _map.Map<CarDTO>(entity);
        return ApiResponse.response(result , dto);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id, CarUpdateDto carUpdateDTO)
    {
         if (id == Guid.Empty)
             return ApiBadRequestResponse.BADresponse("Id is Required");
        if (id != carUpdateDTO.Id)
            return ApiBadRequestResponse.BADresponse("Object id is not compatible with the pass id");
        Car car = _map.Map<Car>(carUpdateDTO);
        _unitOfWork.Cars.Update(car);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.response(result);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        var entity = await _unitOfWork.Cars.GetByIdAsync(id);
        if(entity == null)
            return ApiNotFoundResponse.NOTresponse("This id is invalid");
        _unitOfWork.Cars.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.response(result);
    }
}