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
    public async Task<PagingModel<Car>> GetListAsync(CarRequestDTO input) 
    {
        var query = _repository.GetQueryable();
        var searchingResult = input.ApplySearching(query);
        int countFilterd = searchingResult.Count();
        var sortingResult = input.ApplySorting(searchingResult);
        var pagingResult = sortingResult.ApplyPaging(input.CurrentPage, input.RowsPerPage , false);
        var finalQuery = await pagingResult.GetResultAsync(input.CurrentPage, input.RowsPerPage , countFilterd);
        return finalQuery;
    }
    [HttpGet("{id}")]
    public async Task<CarDTO> GetAsync(Guid id) 
    {
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
        if (id != carUpdateDTO.Id)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Car car = _map.Map<Car>(carUpdateDTO);
        await _repository.UpdateAsync(car);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? 
            throw new BadHttpRequestException("This id is invalid");
        await _repository.DeleteAsync(entity);
    }
}