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
public class DriverController : ControllerBase //RootController<Car,ICarRepository>
{
    // private readonly ICarService _service;
    private readonly IGenericRepository<Driver> _repository;
    protected readonly IMapper _map;
    public DriverController(IGenericRepository<Driver> repository , IMapper map) //: base(repository)
    {
        _repository = repository;
        _map = map;
    } 
       
    [HttpGet(template : "All")]
    public async Task<IActionResult> GetListAsync(DriverRequestDTO input) 
    {
        var query = _repository.GetAll().Where($"{input.SearchingColumn} = @0", input.SearchingValue);
        var getDrivers = await query.GetPagedResult(input.CurrentPage, input.RowsPerPage, input.OrderByData, false);
        var driverList = _map.Map<List<DriverDTO>>(getDrivers);
        return Ok(driverList);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(Guid id) 
    {
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<DriverDTO>(getOne);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(DriverCreateDTO driverDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Driver driver = _map.Map<Driver>(driverDTO);
        _repository.Add(driver);
        return Ok(driver);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, DriverUpdateDTO driverDTO)
    {
        if (id != driverDTO.DriverId)
            return BadRequest();
        Driver driver = _map.Map<Driver>(driverDTO);
        _repository.Update(driver);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var driver = await _repository.DeleteAsync(id);
        if(driver == null) return NotFound();
        return NoContent();
    }
}