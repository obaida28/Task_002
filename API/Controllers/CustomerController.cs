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
    public async Task<PagingResult<CustomerDTO>> GetListAsync(CustomerRequestDTO input)
    {
        if(input == null)
            throw new BadHttpRequestException("Requied input");
        var query = _repository.GetQueryable();
        bool withSearching = input.SearchingColumn != null && input.SearchingValue != null;
        if(withSearching) query = input.ApplySearching(query);
        int countFilterd = query.Count();
        bool withSorting = input.OrderByData != null;
        if(withSorting) query = input.ApplySorting(query);
        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);
        var entityResult = await query.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var customerResult = entityResult.Results.Select(c =>
            new CustomerDTO { Name = c.CustomerName });
        var dtoResult = new PagingResult<CustomerDTO>
        {
            CurrentPage = entityResult.CurrentPage ,
            RowsPerPage = entityResult.RowsPerPage,
            TotalPages = entityResult.TotalPages,
            TotalRows = entityResult.TotalRows ,
            Results = customerResult
        };
        return dtoResult;
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