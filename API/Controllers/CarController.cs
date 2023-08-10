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
    private readonly ICarRepository _repository; 
    private readonly IMapper _map;
    public CarController(ICarRepository repository , IMapper map)
    {
        _repository = repository;
        _map = map;
    }
    
    [HttpGet(template: "GetListAsync")]
    public async Task<PagingResult<CarDTO>> GetListAsync(CarRequestDTO input) 
    {
        if(input == null)
            throw new BadHttpRequestException("Requied input");
        var query = _repository.GetQueryable();
        bool withSearching = input.SearchingColumn != null && input.SearchingValue != null;
        if(withSearching) query = input.ApplySearching(query);
        int countFilterd = query.Count();
        bool withSorting = input.OrderByData != null;
        if(withSorting) query = input.ApplySorting(query);
        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);
        var entityResult = await query.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var carResult = entityResult.Results.Select(c =>
            new CarDTO { Number = c.CarNumber, Type = c.Type, Color = c.Color ,
                DailyRate = c.DailyRate, EngineCapacity = c.EngineCapacity });
        var dtoResult = new PagingResult<CarDTO>
        {
            CurrentPage = entityResult.CurrentPage ,
            RowsPerPage = entityResult.RowsPerPage,
            TotalPages = entityResult.TotalPages,
            TotalRows = entityResult.TotalRows ,
            Results = carResult
        };
        return dtoResult;
    }
    [HttpGet("{id}")]
    public async Task<CarDTO> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<CarDTO>(getOne);
        return result;
    }
    
    [HttpPost]
    public async Task<CarDTO> CreateAsync(CarCreateDto carCreateDto)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        bool isExist = await _repository.IsExistAsync(carCreateDto.Number);
        if(isExist) throw new BadHttpRequestException("The car number is unique !");
        var entity = _map.Map<Car>(carCreateDto);
        await _repository.AddAsync(entity);
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
        await _repository.UpdateAsync(car);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new BadHttpRequestException("Id is Required");
        var entity = await _repository.GetByIdAsync(id) ?? 
            throw new BadHttpRequestException("This id is invalid");
        await _repository.DeleteAsync(entity);
    }
}