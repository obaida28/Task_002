using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase 
{
    private readonly ICarRepository _repository;
    protected readonly IMapper _map;
    public CarController(ICarRepository repository , IMapper map) //: base(repository)
    {
        _repository = repository;
        _map = map;
    } 
    
    [HttpGet(template : "All")]
    public async Task<PagingModel<Car>> GetListAsync(CarRequestDTO input) 
    {
        var query = _repository.GetQueryable();
        var searchingResult = query.ApplySearching(input.SearchingColumn, input.SearchingValue);
        int countFilterd = await searchingResult.CountAsync();
        var sortingResult = searchingResult.ApplySorting(input.OrderByData);
        var pagingResult = sortingResult.ApplyPaging(input.CurrentPage, input.RowsPerPage , false);
        var finalQuery = await pagingResult.GetResult(input.CurrentPage, input.RowsPerPage , countFilterd);
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
    public async Task<CarDTO> CreateAsync(CarCreateDto carDTO)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        Car car = _map.Map<Car>(carDTO);
        bool isExist = await _repository.IsExistAsync(carDTO.CarNumber);
        if(isExist) throw new BadHttpRequestException("The car number is unique !");
        await _repository.AddAsync(car);
        var res = _map.Map<CarDTO>(car);
        return res;
    }

    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, CarUpdateDto carDTO)
    {
        if (id != carDTO.CarId)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Car car = _map.Map<Car>(carDTO);
        await _repository.UpdateAsync(car);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if(entity is null) 
        {

            throw new Exception("Not found");
            //throw new NotFound();//("Not Found");
            //  return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "Fill the name!");
//             var responseMsg = new HttpResponseMessage(HttpStatusCode.Conflict);
//   responseMsg.Content = new StringContent("Custom error message");
//   throw new HttpResponseException(responseMsg);
           // throw new HttpRequestException(HttpStatusCode.NotFound,);
        }
        await _repository.DeleteAsync(entity);
        //return Notfound();
    }
}
