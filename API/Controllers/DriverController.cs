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
public class DriverController : BaseController<Driver,IDriverRepository>
{
    public DriverController(IDriverRepository repository , IMapper map) : base(repository,map){}

    [HttpGet(template: "All")]
    public async Task<PagingModel<Driver>> GetListAsync(DriverRequestDTO input) => 
        await GetAllAsync(input);

    [HttpGet("{id}")]
    public async Task<DriverDTO> GetAsync(Guid id) => 
        await base.GetByIdAsync<DriverDTO>(id);

    [HttpPost]
    public async Task<DriverDTO> CreateAsync(DriverCreateDTO driverCreateDTO) =>
         await base.PostAsync<DriverCreateDTO,DriverDTO>(driverCreateDTO);

    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, DriverUpdateDTO driverUpdateDTO) => 
        await base.PutAsync(id , driverUpdateDTO , id == driverUpdateDTO.DriverId);
}