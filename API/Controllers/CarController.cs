using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;

namespace API.Controllers;
public class CarController : BaseController<Car,ICarRepository> 
{
    public CarController(ICarRepository repository , IMapper map) : base(repository , map) {}
    
    [HttpGet(template: "All")]
    public async Task<PagingModel<Car>> GetListAsync(CarRequestDTO input) => 
        await GetAllAsync(input);

    [HttpGet("{id}")]
    public async Task<CarDTO> GetAsync(Guid id) => 
        await base.GetByIdAsync<CarDTO>(id);
    
    [HttpPost]
    public async Task<CarDTO> CreateAsync(CarCreateDto carCreateDto)
    {
        bool isExist = await _repository.IsExistAsync(carCreateDto.CarNumber);
        if(isExist) throw new BadHttpRequestException("The car number is unique !");
        return await base.PostAsync<CarCreateDto,CarDTO>(carCreateDto);
    }

        [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, CustomerUpdateDTO customerUpdateDTO) => 
        await base.PutAsync(id , customerUpdateDTO , id == customerUpdateDTO.CustomerId);
}