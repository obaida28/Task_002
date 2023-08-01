using API.DTOs;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase //RootController<Car,ICarRepository>
{
    private readonly ICarRepository _repo;
    protected readonly IMapper _map;
    public CarController(ICarRepository repository , IMapper map) 
    {
        _repo = repository;
        _map = map;
    } 
    
    [HttpGet(template : "All")]
    public IEnumerable<CarView> Get() 
    {
        var allCars = _repo.GetAll();
        var cars = _map.Map<List<CarView>>(allCars);
        return cars;
    }
    
    // [HttpGet(template : "AllByCache")]
    // public IEnumerable<CarView> GetByCache() 
    // {
    //     var allCars = _repo.getCarsByCache();
    //     var cars = _map.Map<List<CarView>>(allCars);
    //     return cars;
    // } 
    
    // [HttpGet(template : "CarFilter")]
    // public IEnumerable<CarView> getList(CarFilter dto) 
    // {
    //     var all = _repo.getfilter(dto);
    //     var cars = _map.Map<List<CarView>>(all);
    //     return cars;
    // } 

    [HttpGet("{id}")]
    public async Task<ActionResult<CarView>> Get(Guid id) 
    {
        var getCar = await _repo.GetById(id);
        var car = _map.Map<CarView>(getCar);
        return Ok(car);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCar(CarDTO carDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Car car = _map.Map<Car>(carDTO);
        bool isExist = await _repo.IsExist(carDTO.CarId);
        if(isExist)
            return BadRequest("The car number is unique !");
        _repo.Add(car);
        return Ok(car);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCar(Guid id , CarDTO carDTO)
    {
        if (id != carDTO.CarId)
            return BadRequest();
        Car car = _map.Map<Car>(carDTO);
        _repo.Update(car);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        // var IsExist = await _repo.IsExist(id);
        // if (!IsExist)
        //     return NotFound();
        var entity = await _repo.Delete(id);
        if(entity == null) return NotFound();
        return NoContent();
    }
}