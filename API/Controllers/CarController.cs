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
    public async Task<PagingResult<CarDTO>> GetListAsync(CarRequestDTO input) 
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
        if(withSorting) query = input.OrderByData switch
         {
            "Type" => input.ASC ? query.OrderBy(c => c.Type) : query.OrderByDescending(c => c.Type),
            "Color" => input.ASC ? query.OrderBy(c => c.Color) : query.OrderByDescending(c => c.Color),
            "EngineCapacity" => input.ASC ? query.OrderBy(c => c.EngineCapacity) : query.OrderByDescending(c => c.EngineCapacity),
            "DailyRate" => input.ASC ? query.OrderBy(c => c.DailyRate) : query.OrderByDescending(c => c.DailyRate),
            _ => input.ASC ? query.OrderBy(c => c.Number) : query.OrderByDescending(c => c.Number),
        };

        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await query.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<CarDTO>>(entityResult);
        return dtoResult;
    }
    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            return new ApiBadRequestResponse("Id is Required");
        var getOne = await _unitOfWork.Cars.GetByIdAsync(id);
        var result = _map.Map<CarDTO>(getOne);
        return new ApiOkResponse(result);
    }
    
    [HttpPost]
    public async Task<ApiResponse> CreateAsync(CarCreateDto carCreateDto)
    {
        bool isExist = await _unitOfWork.Cars.IsExistNumberAsync(carCreateDto.Number);
        if(isExist) return new ApiBadRequestResponse( "The car number is unique !");
        var entity = _map.Map<Car>(carCreateDto);
        await _unitOfWork.Cars.AddAsync(entity);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) return new ApiBadRequestResponse( "bad request!");
        var res = _map.Map<CarDTO>(entity);
        return new ApiOkResponse(res);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id, CarUpdateDto carUpdateDTO)
    {
         if (id == Guid.Empty)
             return new ApiBadRequestResponse("Id is Required");
        if (id != carUpdateDTO.Id)
            return new ApiBadRequestResponse("Object id is not compatible with the pass id");
        Car car = _map.Map<Car>(carUpdateDTO);
        _unitOfWork.Cars.Update(car);
        var result = await _unitOfWork.SaveAsync();
        return result == 0 ? new ApiBadRequestResponse( "Bad Request") : new ApiOkResponse();
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return new ApiBadRequestResponse("Id is Required");
        var entity = await _unitOfWork.Cars.GetByIdAsync(id);
        if(entity == null)
            return new ApiNotFoundResponse("This id is invalid");
        _unitOfWork.Cars.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return result == 0 ? new ApiBadRequestResponse( "Bad Request") : new ApiOkResponse();
    }
}