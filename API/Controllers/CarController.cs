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
    public async Task<IActionResult> GetListAsync(CarRequestDTO input) 
    {
        var query = _repository.GetAll().Where($"{input.SearchingColumn} = @0", input.SearchingValue);
        var getCars = await query.GetPagedResult(input.CurrentPage, input.RowsPerPage, input.OrderByData, false);
        var carList = _map.Map<List<CarDTO>>(getCars);
        return Ok(carList);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(Guid id) 
    {
        var getCar = await _repository.GetByIdAsync(id);
        var car = _map.Map<CarDTO>(getCar);
        return Ok(car);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CarCreateDto carDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Car car = _map.Map<Car>(carDTO);
        bool isExist = await _repository.IsExistAsync(carDTO.CarNumber);
        if(isExist)
            return BadRequest("The car number is unique !");
        _repository.Add(car);
        return Ok(car);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, CarUpdateDto carDTO)
    {
        if (id != carDTO.CarId)
            return BadRequest();
        Car car = _map.Map<Car>(carDTO);
        _repository.Update(car);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var car = await _repository.DeleteAsync(id);
        if(car == null) return NotFound();
        return NoContent();
    }
}
// [HttpGet(template : "All")]
    // public override async Task<IActionResult> Get() 
    // {
    //     var allCars = await base.Get();
    //     var cars = _map.Map<List<CarView>>(allCars);
    //     return (IActionResult)cars;
    // }
    
    // [HttpGet(template : "AllByCache")]
    // public async Task<IEnumerable<CarView>> GetByCache() 
    // {
    //     var allCars = await _service.GetCarsByCache();
    //     var cars = _map.Map<List<CarView>>(allCars);
    //     return cars;
    // } 
    
    // [HttpGet]
    // public async Task<PagingModel<CarView>> GetUserRecords(PagingModel<CarDTO> userInput)
    // {    
    //     return await _userService.GetUserData(userInput);
    // }
    

    // [HttpGet(template : "CarFilter")]
    // public IEnumerable<CarView> CarSer(PageParameters param) 
    // {
    //     // var all = _service.Getfilter(dto);
    //     // var cars = _map.Map<List<CarView>>(all);
    //     // return cars;
    // } 