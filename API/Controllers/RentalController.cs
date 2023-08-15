using API.CustomFilters;
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
[ApiValidationFilterAttribute]
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
            return new ApiNotFoundResponse("This customer id is invalid");
        bool IsDriverPassed = input.DriverId != null;
        Driver? entityDriver = null;     
        if(IsDriverPassed)
        {
            entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)input.DriverId);
            if(entityDriver == null)
                return new ApiNotFoundResponse("This driver id is invalid");
            if(!entityDriver.IsAvailable)
            {
                if(entityDriver.SubstituteId == null)
                    return new ApiNotFoundResponse("This driver is not available !");
                entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
                if(!entityDriver.IsAvailable)
                    return new ApiBadRequestResponse("This driver is not available !");
                else
                    input.DriverId = entityDriver.Id;   
            }
        }
        if(input.DailyRate == 0)
            input.DailyRate = entityCar.DailyRate;
        var entity = _map.Map<Rental>(input);
        //TODO : Begin Transaction
        await _unitOfWork.Rentals.AddAsync(entity);
        entityCar.IsAvailable = false;
        _unitOfWork.Cars.Update(entityCar);
        if(IsDriverPassed)
        {
            entityDriver.IsAvailable = false;
            _unitOfWork.Drivers.Update(entityDriver);
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

    [HttpPut("{id}")]
    public async Task<object> UpdateAsync(Guid id , RentalUpdateDTO input)
    {
        if (id == Guid.Empty)
            return new ApiBadRequestResponse("Id is Required");
        if (id != input.Id)
            return new ApiBadRequestResponse("Object id is not compatible with the pass id");
        
        var entityRental = await _unitOfWork.Rentals.GetByIdAsync(id);
        if(!entityRental.IsActive)
            return new ApiBadRequestResponse("This rental is not active !");
        bool changeCar = entityRental.CarId != input.CarId;
        bool changeCustomer = entityRental.CustomerId != input.CustomerId;
        bool changeDriver = entityRental.DriverId != input.DriverId;

        var entityCar = await _unitOfWork.Cars.GetByIdAsync(input.CarId);
        if(changeCar)
        {
            if(entityCar == null)
                return new ApiNotFoundResponse("This car id is invalid");
            if(!entityCar.IsAvailable)
                return new ApiBadRequestResponse("This car is not available !");
        }
        
        if(changeCustomer)
        {
            var isCustomerExist = await _unitOfWork.Customers.IsExistAsync(input.CustomerId);
            if(!isCustomerExist)
                return new ApiNotFoundResponse("This customer id is invalid");
        }

        Driver? entityDriver = null;
        if(input.DriverId != null && changeDriver)
        {
            entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)input.DriverId);
            if(entityDriver == null)
                return new ApiNotFoundResponse("This driver id is invalid");
            if(!entityDriver.IsAvailable)
            {
                if(entityDriver.SubstituteId == null)
                    return new ApiNotFoundResponse("This driver is not available !");
                entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
                if(!entityDriver.IsAvailable)
                    return new ApiBadRequestResponse("This driver is not available !");
                else
                    input.DriverId = entityDriver.Id;   
            }
        }
        var oldCarId = entityRental.CarId;
        //entityRental = _map.Map<Rental>(input);
        
        // entityRental.StartDate = oldEntityRental.StartDate;
        
        // TODO : Begin Transaction
        entityRental.CarId = input.CarId;
        entityRental.CustomerId = input.CustomerId;
        entityRental.DailyRate = input.DailyRate;
        entityRental.DriverId = input.DriverId;
        entityRental.EndDate = input.EndDate;
        if(changeCar)
        {
            var oldEntityCar = await _unitOfWork.Cars.GetByIdAsync(oldCarId);
            oldEntityCar.IsAvailable = true;
            entityCar.IsAvailable = false;
        }
        if(input.FinishRental)
        {
            entityCar.IsAvailable = true;
            if(entityDriver != null) entityDriver.IsAvailable = true;
            entityRental.EndDate = DateTime.Now;
        }
        var result = await _unitOfWork.SaveAsync();
        // TODO : End Transaction
        var dtoResult = _map.Map<RentalDTO>(entityRental);
        return 
        result == 0 ? new ApiBadRequestResponse("Bad Request") : 
        new ApiOkResponse(dtoResult);
        // new {
        //     a = entityRental ,
        //     b = oldEntityRental
        // };
        
    }
    
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return new ApiBadRequestResponse("Id is Required");
        var entity = await _unitOfWork.Rentals.GetByIdAsync(id);
        if(entity == null)
            return new ApiNotFoundResponse("This id is invalid");
        var entityCar = await _unitOfWork.Cars.GetByIdAsync(entity.CarId);
        entityCar.IsAvailable = true;
        if(entity.DriverId != null)
        {
            var entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entity.DriverId);
            entityDriver.IsAvailable = true;
        }
        _unitOfWork.Rentals.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return result == 0 ? new ApiBadRequestResponse( "Bad Request") : new ApiOkResponse();
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            return new ApiBadRequestResponse("Id is Required");
        var getOne = await _unitOfWork.Rentals.GetByIdAsync(id);
        var result = _map.Map<RentalDTO>(getOne);
        return new ApiOkResponse(result);
    }
}