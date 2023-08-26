using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.ErrorResponse;
using API.ActionFilter;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
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
    public async Task<ApiResponse> GetListAsync(CustomerRequestDTO input)
    {
        if(input == null)
            throw new BadHttpRequestException("Requied input");
        
        var query = _unitOfWork.Customers.GetQueryable();

        bool withSearching = !string.IsNullOrEmpty(input.SearchingValue);
        if(withSearching) query = query.Where(c => c.Name.ToLower().Contains(input.SearchingValue));
        
        int countFilterd = await query.CountAsync();

        bool withSorting = !string.IsNullOrEmpty(input.OrderByData);
        if(withSorting) 
        {
            string dataOrder = input.OrderByData.ToLower();
            string[] orderResult = dataOrder.Split(" ");
            bool IsDesc = orderResult.Last() == "desc";
            query = !IsDesc ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
        }
        
        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        query = query.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await query.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<CustomerDTO>>(entityResult);
        return ApiOkResponse.OKresponse(dtoResult);
    }
    
    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        var getOne = await _unitOfWork.Customers.GetByIdAsync(id);
        var result = _map.Map<CustomerDTO>(getOne);
        return ApiOkResponse.OKresponse(result);
    }

    [HttpPost]
    public async Task<ApiResponse> CreateAsync(CustomerCreateDTO customerCreateDTO)
    {
        Customer customer = _map.Map<Customer>(customerCreateDTO);
        await _unitOfWork.Customers.AddAsync(customer);
        var result = await _unitOfWork.SaveAsync();
        var dto = _map.Map<CustomerDTO>(customer);
        return ApiResponse.response(result , dto);
    }
    
    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id, CustomerUpdateDTO customerUpdateDTO)
    {
        if (id != customerUpdateDTO.Id)
            return ApiBadRequestResponse.BADresponse("Object id is not compatible with the pass id");
        Customer customer = _map.Map<Customer>(customerUpdateDTO);
        _unitOfWork.Customers.Update(customer);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.response(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var entity = await _unitOfWork.Customers.GetByIdAsync(id);
        if(entity == null)
            return ApiNotFoundResponse.NOTresponse("This id is invalid");
        _unitOfWork.Customers.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.response(result);
    }
}