using API.DTOs;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase //RootController<Car,ICarRepository>
{
    private readonly ICarService _service;
    protected readonly IMapper _map;
    public CarController(ICarService service , IMapper map) 
    {
        _service = service;
        _map = map;
    } 
    
    [HttpGet(template : "All")]
    public IEnumerable<CarView> Get() 
    {
        var allCars = _service.GetAllCars();
        var cars = _map.Map<List<CarView>>(allCars);
        return cars;
    }
    
    [HttpGet(template : "AllByCache")]
    public async Task<IEnumerable<CarView>> GetByCache() 
    {
        var allCars = await _service.GetCarsByCache();
        var cars = _map.Map<List<CarView>>(allCars);
        return cars;
    } 
    
    [HttpGet(template : "CarFilter")]
    public IEnumerable<CarView> getList(CarFilter dto) 
    {
        var all = _service.Getfilter(dto);
        var cars = _map.Map<List<CarView>>(all);
        return cars;
    } 

    [HttpGet("{id}")]
    public async Task<ActionResult<CarView>> Get(Guid id) 
    {
        var getCar = await _service.GetCarById(id);
        var car = _map.Map<CarView>(getCar);
        return Ok(car);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCar(CarDTO carDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Car car = _map.Map<Car>(carDTO);
        bool isExist = await _service.IsExist(carDTO.CarId);
        if(isExist)
            return BadRequest("The car number is unique !");
        await _service.AddCar(car);
        return Ok(car);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCar(Guid id , CarDTO carDTO)
    {
        if (id != carDTO.CarId)
            return BadRequest();
        Car car = _map.Map<Car>(carDTO);
        await _service.UpdateCar(car);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        var success = await _service.DeleteCar(id);
        if(!success) return NotFound();
        return NoContent();
    }
}