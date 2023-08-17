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
            return ApiNotFoundResponse.NOTresponse("This car id is invalid");
        // if(!entityCar.IsAvailable)
        //     return ApiBadRequestResponse.BADresponse("This car is not available !");
        var entityCustomer = await _unitOfWork.Customers.GetByIdAsync(input.CustomerId);
        if(entityCustomer == null)
            return ApiNotFoundResponse.NOTresponse("This customer id is invalid");
        bool IsDriverPassed = input.DriverId != null;
        Driver? entityDriver = null;     
        if(IsDriverPassed)
        {
            entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)input.DriverId);
            if(entityDriver == null)
                return ApiNotFoundResponse.NOTresponse("This driver id is invalid");
        }
        if(input.DailyRate == 0)
            input.DailyRate = entityCar.DailyRate;
        input.StartDate = input.StartDate.Date;
        input.EndDate = input.EndDate.Date;

        var queryRental = _unitOfWork.Rentals.GetQueryable();
        var isNotAvailableCar = await queryRental.AnyAsync
            (r => r.CarId == input.CarId &&
                (
                    (input.StartDate.Date >= r.StartDate && input.StartDate.Date <= r.EndDate) ||
                    (input.EndDate.Date >= r.StartDate && input.EndDate.Date <= r.EndDate)
                )
            );
        if(isNotAvailableCar)
            return ApiBadRequestResponse.BADresponse("This car is not available between this dates !");
        var isNotAvailableDriver = await queryRental.AnyAsync
            (r => 
                input.DriverId != null && r.DriverId == input.DriverId && 
                (
                    (input.StartDate.Date >= r.StartDate && input.StartDate.Date <= r.EndDate) ||
                    (input.EndDate.Date >= r.StartDate && input.EndDate.Date <= r.EndDate)
                )
            );

            if(input.DriverId != null && isNotAvailableDriver)
            {
                if(entityDriver.SubstituteId == null)
                    return ApiBadRequestResponse.BADresponse("This driver is not available between this dates !");
                entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
                var isNotAvailableSubstituteDriver = await queryRental.AnyAsync
                    (r =>  r.DriverId == entityDriver.SubstituteId && 
                        (
                            (input.StartDate.Date >= r.StartDate && input.StartDate.Date <= r.EndDate) ||
                            (input.EndDate.Date >= r.StartDate && input.EndDate.Date <= r.EndDate)
                        )
                    );
                if(isNotAvailableSubstituteDriver)
                    return ApiBadRequestResponse.BADresponse("This driver is not available between this dates !");
                else
                    input.DriverId = entityDriver.Id;   
            }

        
        var entity = _map.Map<Rental>(input);
        //TODO : Begin Transaction
        await _unitOfWork.Rentals.AddAsync(entity);
        if(entity.IsActive)
        {
            entityCar.IsAvailable = false;
            entity.State = "Active";
        }
        _unitOfWork.Cars.Update(entityCar);//Question
        if(IsDriverPassed && entity.IsActive)
        {
            entityDriver.IsAvailable = false;
            _unitOfWork.Drivers.Update(entityDriver);//Question
        }
        var result = await _unitOfWork.SaveAsync();
        //TODO : End Transaction
        entity.Car = entityCar;
        entity.Customer = entityCustomer;
        entity.Driver = entityDriver;
        var res = _map.Map<RentalDTO>(entity);
        return ApiResponse.response(result , res);
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<ApiResponse> GetListAsync(RentalRequestDTO input) 
    {
        var queryCar = _unitOfWork.Cars.GetQueryable();
        var queryCustomer = _unitOfWork.Customers.GetQueryable();
        var queryDriver = _unitOfWork.Drivers.GetQueryable();
        var queryRental = _unitOfWork.Rentals.GetQueryable();
        var searchQuery = queryRental;

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
            
            queryRental = queryRental.Where(r => 
                r.State.ToLower().Contains(input.SearchingValue));
        }
        searchQuery = searchQuery.Where
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
        return ApiOkResponse.OKresponse(dtoResult);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id , RentalUpdateDTO input)
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        if (id != input.Id)
            return ApiBadRequestResponse.BADresponse("Object id is not compatible with the pass id");
        
        var entityRental = await _unitOfWork.Rentals.GetByIdAsync(id);
        var queryRental = _unitOfWork.Rentals.GetQueryable();

        if(!entityRental.IsActive)
            return ApiBadRequestResponse.BADresponse("This rental is not active !");
        bool changeCar = entityRental.CarId != input.CarId;
        bool changeCustomer = entityRental.CustomerId != input.CustomerId;
        bool changeDriver = entityRental.DriverId != input.DriverId;

        var entityCar = await _unitOfWork.Cars.GetByIdAsync(input.CarId);
        if(changeCar)
        {
            if(entityCar == null)
                return ApiNotFoundResponse.NOTresponse("This car id is invalid");
            
             
            var isNotAvailableCar = await queryRental.AnyAsync
            (r => r.CarId == input.CarId &&
                (
                    (input.StartDate.Date >= r.StartDate && input.StartDate.Date <= r.EndDate) ||
                    (input.EndDate.Date >= r.StartDate && input.EndDate.Date <= r.EndDate)
                )
            );
        if(isNotAvailableCar)
            return ApiBadRequestResponse.BADresponse("This car is not available between this dates !");

            // if(!entityCar.IsAvailable)
            //     return ApiBadRequestResponse.BADresponse("This car is not available !");
        }
        
        if(changeCustomer)
        {
            var isCustomerExist = await _unitOfWork.Customers.IsExistAsync(input.CustomerId);
            if(!isCustomerExist)
                return ApiNotFoundResponse.NOTresponse("This customer id is invalid");
        }

        Driver? entityDriver = null;
        if(input.DriverId != null && changeDriver)
        {
            entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)input.DriverId);
            if(entityDriver == null)
                return ApiNotFoundResponse.NOTresponse("This driver id is invalid");
            // if(!entityDriver.IsAvailable)
            // {
            //     if(entityDriver.SubstituteId == null)
            //         return ApiNotFoundResponse.NOTresponse("This driver is not available !");
            //     entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
            //     if(!entityDriver.IsAvailable)
            //         return ApiBadRequestResponse.BADresponse("This driver is not available !");
            //     else
            //         input.DriverId = entityDriver.Id;   
            // }
            var isNotAvailableDriver = await queryRental.AnyAsync
            (r => r.DriverId == input.DriverId && 
                (
                    (input.StartDate.Date >= r.StartDate && input.StartDate.Date <= r.EndDate) ||
                    (input.EndDate.Date >= r.StartDate && input.EndDate.Date <= r.EndDate)
                )
            );

            if(isNotAvailableDriver)
            {
                if(entityDriver.SubstituteId == null)
                    return ApiBadRequestResponse.BADresponse("This driver is not available between this dates !");
                entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
                var isNotAvailableSubstituteDriver = await queryRental.AnyAsync
                    (r =>  r.DriverId == entityDriver.SubstituteId && 
                        (
                            (input.StartDate.Date >= r.StartDate && input.StartDate.Date <= r.EndDate) ||
                            (input.EndDate.Date >= r.StartDate && input.EndDate.Date <= r.EndDate)
                        )
                    );
                if(isNotAvailableSubstituteDriver)
                    return ApiBadRequestResponse.BADresponse("This driver is not available between this dates !");
                else
                    input.DriverId = entityDriver.Id;   
            }
        }
        var oldCarId = entityRental.CarId;
        var startDate = entityRental.StartDate;
        _map.Map(input, entityRental);
        entityRental.StartDate = startDate.Date;
        if(changeCar)
        {
            var oldEntityCar = await _unitOfWork.Cars.GetByIdAsync(oldCarId);
            if(entityRental.IsActive)
            {
                oldEntityCar.IsAvailable = true;
                entityCar.IsAvailable = false;
            }
        }
        if(input.FinishRental)
        {
            entityCar.IsAvailable = true;
            if(entityDriver != null) entityDriver.IsAvailable = true;
            entityRental.EndDate = DateTime.Now.Date;
        }
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.response(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        var entity = await _unitOfWork.Rentals.GetByIdAsync(id);
        if(entity == null)
            return ApiNotFoundResponse.NOTresponse("This id is invalid");
        var entityCar = await _unitOfWork.Cars.GetByIdAsync(entity.CarId);
        entityCar.IsAvailable = true;
        if(entity.DriverId != null)
        {
            var entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entity.DriverId);
            entityDriver.IsAvailable = true;
        }
        _unitOfWork.Rentals.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.response(result);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
            return ApiBadRequestResponse.BADresponse("Id is Required");
        var getOne = await _unitOfWork.Rentals.GetByIdAsync(id);
        var result = _map.Map<RentalDTO>(getOne);
        return ApiOkResponse.OKresponse(result);
    }
}