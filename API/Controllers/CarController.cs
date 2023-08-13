using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using System.Linq.Dynamic.Core;
using System.Net;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
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
        if(input == null)
            throw new BadHttpRequestException("Requied input");
        
        var query = _unitOfWork.Cars.GetQueryable();

        bool withSearching = input.SearchingValue != null;
        if(withSearching) 
        {
            bool withDecimal = decimal.TryParse(input.SearchingValue, out decimal decimalValue);
            bool withInt = int.TryParse(input.SearchingValue, out int intValue);
             query = query.Where(c => 
                c.Type.ToLower().Contains(input.SearchingValue) || 
                c.Color.ToLower().Contains(input.SearchingValue) || 
                c.CarNumber.ToLower().Contains(input.SearchingValue) ||
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
            _ => input.ASC ? query.OrderBy(c => c.CarNumber) : query.OrderByDescending(c => c.CarNumber),
        };

        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await query.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<CarDTO>>(entityResult);
        return dtoResult;
    }
    [HttpGet("{id}")]
    public async Task<CarDTO> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        var getOne = await _unitOfWork.Cars.GetByIdAsync(id);
        var result = _map.Map<CarDTO>(getOne);
        return result;
    }
    
    [HttpPost]
    public async Task<CarDTO> CreateAsync(CarCreateDto carCreateDto)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        bool isExist = await _unitOfWork.Cars.IsExistAsync(carCreateDto.Number);
        if(isExist) throw new BadHttpRequestException("The car number is unique !");
        var entity = _map.Map<Car>(carCreateDto);
        await _unitOfWork.Cars.AddAsync(entity);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
        var res = _map.Map<CarDTO>(entity);
        return res;
    }

    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, CarUpdateDto carUpdateDTO)
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        if (id != carUpdateDTO.Id)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Car car = _map.Map<Car>(carUpdateDTO);
        await _unitOfWork.Cars.UpdateAsync(car);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        var entity = await _unitOfWork.Cars.GetByIdAsync(id) ?? 
            throw new BadHttpRequestException("This id is invalid");
        await _unitOfWork.Cars.DeleteAsync(entity);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
    }
}