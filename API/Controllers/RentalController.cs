using API.DTOs;
using API.ErrorResponse;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
    public async Task<ApiResponse> CreateAsync(RentalCreateDto input)
    {
        var entityCar = await _unitOfWork.Cars.GetByIdAsync(input.CarId);
        if(entityCar == null)
            return new ApiNotFoundResponse("This car id is invalid");
        if(!entityCar.IsAvailable)
            return new ApiBadRequestResponse("This car is not available !");
        var entityCustomer = await _unitOfWork.Customers.GetByIdAsync(input.CustomerId);
        if(entityCustomer == null)
            return new ApiBadRequestResponse("This customer id is invalid");
        bool IsDriverPassed = input.DriverId != null;
        Driver? entityDriver = null;     
        if(IsDriverPassed)
        {
            entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)input.DriverId);
            if(entityDriver == null)
                return new ApiBadRequestResponse("This driver id is invalid");
            if(!entityDriver.IsAvailable)
            {
                if(entityDriver.SubstituteId != null)
                {
                    entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
                    if(!entityDriver.IsAvailable)
                        return new ApiBadRequestResponse("This driver is not available !");
                    else
                        input.DriverId = entityDriver.Id;
                }
                else
                    return new ApiNotFoundResponse("This driver is not available !");
            }
        }
        if(!input.IsValidPeriod) 
            return new ApiBadRequestResponse("End Date must be greater than or equal to Start Date.");
        if(!input.InFutureDate) 
            return new ApiBadRequestResponse("Start Date and End Date must be a future date.");
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
        if(result == 0) return new ApiBadRequestResponse("bad request!");
        entity.Car = entityCar;
        entity.Customer = entityCustomer;
        entity.Driver = entityDriver;
        var res = _map.Map<RentalDTO>(entity);
        return new ApiOkResponse(res);
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<ApiResponse> GetListAsync(RentalRequestDTO input) 
    {
        var queryCar = _unitOfWork.Cars.GetQueryable();
        var queryCustomer = _unitOfWork.Customers.GetQueryable();
        var queryDriver = _unitOfWork.Drivers.GetQueryable();
        var queryRental = _unitOfWork.Rentals.GetQueryable();

        bool withSearching = input.SearchingValue != null;
        if(withSearching) 
        {
            bool withDecimal = decimal.TryParse(input.SearchingValue, out decimal decimalValue);
            bool withInt = int.TryParse(input.SearchingValue, out int intValue);
             queryCar = queryCar.Where(c => 
                c.Type.ToLower().Contains(input.SearchingValue) || 
                c.Color.ToLower().Contains(input.SearchingValue) || 
                c.Number.ToLower().Contains(input.SearchingValue) ||
                (withDecimal && c.EngineCapacity == decimalValue) || (withInt && c.DailyRate == intValue));

            queryCustomer = queryCustomer.Where(c => 
                c.Name.ToLower().Contains(input.SearchingValue));

            queryDriver = queryDriver.Where(c => 
                c.Name.ToLower().Contains(input.SearchingValue));
        }
        queryRental = queryRental.Where
            (r => 
                queryCar.Any(c => c.Id == r.CarId) ||
                queryCustomer.Any(c => c.Id == r.CustomerId) || 
                (r.DriverId.HasValue &&  queryDriver.Any(d => d.Id == r.DriverId.Value))
            );

        int countFilterd = await queryRental.CountAsync();

        bool withSorting = input.OrderByData != null;
        if(withSorting) 
        {
            queryRental = input.OrderByData switch
            {
                "Type" => input.ASC ? queryRental.OrderBy(c => c.Car.Type) : queryRental.OrderByDescending(c => c.Car.Type),
                "Color" => input.ASC ? queryRental.OrderBy(c => c.Car.Color) : queryRental.OrderByDescending(c => c.Car.Color),
                "EngineCapacity" => input.ASC ? queryRental.OrderBy(c => c.Car.EngineCapacity) : queryRental.OrderByDescending(c => c.Car.EngineCapacity),
                "DailyRate" => input.ASC ? queryRental.OrderBy(c => c.DailyRate) : queryRental.OrderByDescending(c => c.DailyRate),
                "CarNumber" => input.ASC ? queryRental.OrderBy(c => c.Car.Number) : queryRental.OrderByDescending(c => c.Car.Number),
                "CustomerName" => input.ASC ? queryRental.OrderBy(c => c.Customer.Name) : queryRental.OrderByDescending(c => c.Customer.Name),
                "DriverName" => input.ASC ? queryRental.OrderBy(c => c.Driver.Name) : queryRental.OrderByDescending(c => c.Driver.Name),
                _ => input.ASC ? queryRental.OrderBy(c => c.Car.Number) : queryRental.OrderByDescending(c => c.Car.Number),
            };
        }

        bool withPaging = input.CurrentPage != 0 && input.RowsPerPage != 0;
        queryRental = queryRental.ApplyPaging(input.CurrentPage, input.RowsPerPage , withPaging);

        var entityResult = await queryRental.GetResultAsync(withPaging , input.CurrentPage, input.RowsPerPage , countFilterd);
        var dtoResult = _map.Map<PagingResult<RentalDTO>>(entityResult);
        return new ApiOkResponse(dtoResult);
    }
}