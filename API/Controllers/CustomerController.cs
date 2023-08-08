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
public class CustomerController : BaseController<Customer,ICustomerRepository>
{
    public CustomerController(ICustomerRepository repository , IMapper map) : base(repository,map){}

    [HttpGet(template: "All")]
    public async Task<PagingModel<Customer>> GetListAsync(CustomerRequestDTO input) => 
        await GetAllAsync(input);

    [HttpGet("{id}")]
    public async Task<CustomerDTO> GetAsync(Guid id) => 
        await base.GetByIdAsync<CustomerDTO>(id);

    [HttpPost]
    public async Task<CustomerDTO> CreateAsync(CustomerCreateDTO customerCreateDTO) =>
         await base.PostAsync<CustomerCreateDTO,CustomerDTO>(customerCreateDTO);

    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, CustomerUpdateDTO customerUpdateDTO) => 
        await base.PutAsync(id , customerUpdateDTO , id == customerUpdateDTO.CustomerId);
}