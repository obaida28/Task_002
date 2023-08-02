using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase //RootController<Car,ICarRepository>
{
    // private readonly ICarService _service;
    private readonly IGenericRepository<Car> _repository;
    protected readonly IMapper _map;
    public CarController(IGenericRepository<Car> repository , IMapper map) //: base(repository)
    {
        _repository = repository;
        _map = map;
    } 
    
    [HttpGet]
    public async Task<IActionResult> Paging(PagingModel<Car> inputData)
    {
        var query = _repository.GetAllBeforeExecute();
        var result = await query.GetPagedResult(inputData.CurrentPage, inputData.RowsPerPage, inputData.OrderByData, false);
        return (IActionResult)result;
    }
    
    [HttpGet(template : "All")]
    public async Task<IActionResult> ReadAll() 
    {
        var getCars = await _repository.GetAll();
        var carList = _map.Map<List<CarDTO>>(getCars);
        return Ok(carList);
        // var allCars = await base.Get();
        // var cars = _map.Map<List<CarView>>(allCars);
        // return (IActionResult)cars;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ReadById(Guid id) 
    {
        var getCar = await _repository.GetById(id);
        var car = _map.Map<CarDTO>(getCar);
        return Ok(car);
        // var car = await base.Get(id);
        // var carView = _map.Map<CarView>(car);
        // return (IActionResult)carView;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCar(CreateUpdateCarDto carDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Car car = _map.Map<Car>(carDTO);
        //bool isExist = await _service.IsExist(carDTO.CarId);
        bool isExist = await _repository.IsExist(carDTO.CarId);
        if(isExist)
            return BadRequest("The car number is unique !");
        _repository.Add(car);
        return Ok(car);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCar(Guid id, CreateUpdateCarDto carDTO)
    {
        if (id != carDTO.CarId)
            return BadRequest();
        Car car = _map.Map<Car>(carDTO);
        _repository.Update(car);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        var car = await _repository.Delete(id);
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