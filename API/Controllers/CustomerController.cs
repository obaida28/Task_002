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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    public CustomerController(IUnitOfWork unitOfWork , IMapper map)
    {
        _unitOfWork = unitOfWork;
        _map = map;
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<PagingResult<CustomerDTO>> GetListAsync(CustomerRequestDTO input)
    {
        if(input == null)
            throw new BadHttpRequestException("Requied input");
        
        var query = _unitOfWork.Customers.GetQueryable();

        bool withSearching = input.SearchingValue != null;
        if(withSearching) query = query.Where(c => 
            c.Name.ToLower().Contains(input.SearchingValue));
        
        int countFilterd = await query.CountAsync();

        bool withSorting = input.OrderByData != null;
        if(withSorting) query = input.ASC ? 
            query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
        
        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await query.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<CustomerDTO>>(entityResult);
        return dtoResult;
    }
    
    [HttpGet("{id}")]
    public async Task<CustomerDTO> GetAsync(Guid id) 
    {
        var getOne = await _unitOfWork.Customers.GetByIdAsync(id);
        var result = _map.Map<CustomerDTO>(getOne);
        return result;
    }

    [HttpPost]
    public async Task<CustomerDTO> CreateAsync(CustomerCreateDTO customerCreateDTO)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        Customer customer = _map.Map<Customer>(customerCreateDTO);
        await _unitOfWork.Customers.AddAsync(customer);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
        var res = _map.Map<CustomerDTO>(customer);
        return res;
    }
    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, CustomerUpdateDTO customerUpdateDTO)
    {
        if (id != customerUpdateDTO.Id)
            throw new BadHttpRequestException("Object id is not compatible with the pass id");
        Customer customer = _map.Map<Customer>(customerUpdateDTO);
        await _unitOfWork.Customers.UpdateAsync(customer);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
    }
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _unitOfWork.Customers.GetByIdAsync(id) ?? throw new BadHttpRequestException("This id is invalid");
        await _unitOfWork.Customers.DeleteAsync(entity);
        var result = await _unitOfWork.SaveAsync();
        if(result == 0) throw new BadHttpRequestException("bad request!");
    }
}