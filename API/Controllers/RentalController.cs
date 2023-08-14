using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using System.Linq.Dynamic.Core;
using System.Net;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RentalController : ControllerBase 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    public RentalController(IUnitOfWork unitOfWork , IMapper map)
    {
        _unitOfWork = unitOfWork;
        _map = map;
    }
    
    [HttpPost]
    public async Task<RentalDTO> CreateAsync(RentalCreateDto input)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
        var entityCar = await _unitOfWork.Cars.GetByIdAsync(input.CarId) ?? 
            throw new BadHttpRequestException("This car id is invalid");
        if(!entityCar.IsAvailable)
            throw new BadHttpRequestException("This car is not available !");
        var entityCustomer = await _unitOfWork.Customers.GetByIdAsync(input.CustomerId) ?? 
            throw new BadHttpRequestException("This customer id is invalid");
        bool IsDriverPassed = input.DriverId != null;
        Driver? entityDriver = null;     
        if(IsDriverPassed)
        {
            entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)input.DriverId) ?? 
                throw new BadHttpRequestException("This driver id is invalid"); 
            if(!entityDriver.IsAvailable)
            {
                if(entityDriver.SubstituteId != null)
                {
                    entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
                    if(!entityDriver.IsAvailable)
                        throw new BadHttpRequestException("This driver is not available !");
                    else
                        input.DriverId = entityDriver.Id;
                }
                else
                    throw new BadHttpRequestException("This driver is not available !");
            }
        }
        if(!input.IsValidPeriod) 
            throw new BadHttpRequestException("End Date must be greater than or equal to Start Date.");
        if(!input.InFutureDate) 
            throw new BadHttpRequestException("Start Date and End Date must be a future date.");
        if(input.DailyRate == 0)
            input.DailyRate = entityCar.DailyRate;
        var entity = _map.Map<Rental>(input);
        //TODO : Begin Transaction
        await _unitOfWork.Rentals.AddAsync(entity);
        entityCar.IsAvailable = false;
        await _unitOfWork.Cars.UpdateAsync(entityCar);
        if(IsDriverPassed)
        {
            entityDriver.IsAvailable = false;
            await _unitOfWork.Drivers.UpdateAsync(entityDriver);
        }
        var result = await _unitOfWork.SaveAsync();
        //TODO : End Transaction
        if(result == 0) throw new BadHttpRequestException("bad request!");
        entity.Car = entityCar;
        entity.Customer = entityCustomer;
        entity.Driver = entityDriver;
        var res = _map.Map<RentalDTO>(entity);
        return res;
    }
}