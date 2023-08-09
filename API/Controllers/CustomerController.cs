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
public class CustomerController : ControllerBase 
{
    private readonly ICustomerRepository _repository; 
    private readonly IMapper _map;
    public CustomerController(ICustomerRepository repository , IMapper map)
    {
        _repository = repository;
        _map = map;
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<PagingModel<Customer>> GetListAsync(CustomerRequestDTO input)
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
    public async Task<CustomerDTO> GetAsync(Guid id) 
    {
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<CustomerDTO>(getOne);
        return result;
    }

    [HttpPost]
    public async Task<CustomerDTO> CreateAsync(CustomerCreateDTO customerCreateDTO)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        Customer customer = _map.Map<Customer>(customerCreateDTO);
        await _repository.AddAsync(customer);
        var res = _map.Map<CustomerDTO>(customer);
        return res;
    }
    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, CustomerUpdateDTO customerUpdateDTO)
    {
        if (id != customerUpdateDTO.Id)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Customer customer = _map.Map<Customer>(customerUpdateDTO);
        await _repository.UpdateAsync(customer);
    }
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new BadHttpRequestException("This id is invalid");
        await _repository.DeleteAsync(entity);
    }
}