using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
    public async Task<PagingModel<Customer>> GetListAsync(CustomerRequestDTO input) 
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
    public async Task<CustomerDTO> GetAsync(Guid id) 
    {
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<CustomerDTO>(getOne);
        return result;
    }

    [HttpPost]
    public CustomerDTO Create(CustomerCreateDTO customerDTO)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        Customer customer = _map.Map<Customer>(customerDTO);
        _repository.Add(customer);
        var res = _map.Map<CustomerDTO>(customer);
        return res;
    }

    [HttpPut("{id}")]
    public void Update(Guid id, CustomerUpdateDTO customerDTO)
    {
        if (id != customerDTO.CustomerId)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Customer customer = _map.Map<Customer>(customerDTO);
        _repository.Update(customer);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new BadHttpRequestException("This id is invalid");
        _repository.Delete(entity);
    }
}