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
public class CustomerController : ControllerBase //RootController<Car,ICarRepository>
{
    // private readonly ICarService _service;
    private readonly IGenericRepository<Customer> _repository;
    protected readonly IMapper _map;
    public CustomerController(IGenericRepository<Customer> repository , IMapper map) //: base(repository)
    {
        _repository = repository;
        _map = map;
    } 
       
    [HttpGet(template : "All")]
    public async Task<IActionResult> GetListAsync(CustomerRequestDTO input) 
    {
        var query = _repository.GetAll().Where($"{input.SearchingColumn} = @0", input.SearchingValue);
        var getCustomers = await query.GetPagedResult(input.CurrentPage, input.RowsPerPage, input.OrderByData, false);
        var customerList = _map.Map<List<CustomerDTO>>(getCustomers);
        return Ok(customerList);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(Guid id) 
    {
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<CustomerDTO>(getOne);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CustomerCreateDTO customerDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Customer customer = _map.Map<Customer>(customerDTO);
        _repository.Add(customer);
        return Ok(customer);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, CustomerUpdateDTO customerDTO)
    {
        if (id != customerDTO.CustomerId)
            return BadRequest();
        Customer customer = _map.Map<Customer>(customerDTO);
        _repository.Update(customer);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var customer = await _repository.DeleteAsync(id);
        if(customer == null) return NotFound();
        return NoContent();
    }
}