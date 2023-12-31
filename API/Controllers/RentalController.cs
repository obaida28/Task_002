using API.Extensions;
using Core.Extensions;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
public class RentalController : ControllerBase 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    public RentalController(IUnitOfWork unitOfWork , IMapper map)
    {
        _unitOfWork = unitOfWork;
        _map = map;
    }  
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ApiResponse> CheckDriver(Guid driverId ,DateTime startDate, DateTime endDate)
    {
        List<Guid> lists = new();
        while (!lists.Contains(driverId))
        {
            var entityDriver = await _unitOfWork.Drivers.GetByIdAsync(driverId);
            if(entityDriver == null)
            {
                return ApiResponse.NOT("This driver id is invalid");
            }
            bool available = await _unitOfWork.Rentals.IsDriverAvailableBetweenDatesAsync
                (driverId , startDate , endDate);
            if(entityDriver.IsAvailable && available)
            {
                return ApiResponse.OK(entityDriver);
            }
            lists.Add(driverId);
            var _driverId = entityDriver.SubstituteId;
            if(!_driverId.HasValue)
            {
                return ApiResponse.NOT("This driver is not available");
            }
            driverId = (Guid)_driverId;
        }
        return ApiResponse.BAD("This driver is not available");
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ApiResponse> CheckCar(Guid carId , DateTime startDate, DateTime endDate )
    {
        var entityCar = await _unitOfWork.Cars.GetByIdAsync(carId);
        if(entityCar == null)
        {
            return ApiResponse.NOT("This car id is invalid");
        }
        if(!entityCar.IsAvailable)
        {
            return ApiResponse.BAD("This car is not available !");
        }
        var isAvailableCar = await _unitOfWork.Rentals.IsCarAvailableBetweenDatesAsync(carId ,startDate , endDate);
        if(isAvailableCar)
        {
            return ApiResponse.BAD("This car is not available between this dates !");
        }
        return ApiResponse.OK(entityCar);
    }
    
    [HttpPost]
    public async Task<ApiResponse> CreateAsync(RentalCreateDto input)
    {
        input.StartDate = input.StartDate.Date;
        input.EndDate = input.EndDate.Date.AddDays(1);
        //Car Check
        var carCheck = await CheckCar(input.CarId,input.StartDate,input.EndDate);
        if(carCheck.StatusCode != 200) 
        {
            return carCheck;
        }
        Car entityCar = ApiResponse.GetResult(carCheck) as Car;
        if(input.DailyRate == 0) 
        {
            input.DailyRate = entityCar.DailyRate;
        }
        //Customer Check
        var entityCustomer = await _unitOfWork.Customers.GetByIdAsync(input.CustomerId);
        if(entityCustomer == null)
        {
            return ApiResponse.NOT("This customer id is invalid");
        }
        //Driver check
        bool IsDriverPassed = input.DriverId.HasValue;
        Driver? entityDriver = null;     
        if(IsDriverPassed)
        {
            var driverCheck = await CheckDriver((Guid)input.DriverId,input.StartDate,input.EndDate);
            if(driverCheck.StatusCode != 200) return driverCheck;
            entityDriver = ApiResponse.GetResult(driverCheck) as Driver;
            input.DriverId = entityDriver.Id;
        }  
        //End Check
        var entity = _map.Map<Rental>(input);
        //TODO : Begin Transaction
        await _unitOfWork.Rentals.AddAsync(entity);
        if(entity.IsActive)
        {
            entityCar.IsAvailable = false;
            entity.State = "Active";
            if(IsDriverPassed)
            {
                entityDriver.IsAvailable = false;
            }
            _unitOfWork.Cars.Update(entityCar);
            _unitOfWork.Drivers.Update(entityDriver);
        }
        var saveResult = await _unitOfWork.SaveAsync();
        //TODO : End Transaction
        entity.Car = entityCar;
        entity.Customer = entityCustomer;
        entity.Driver = entityDriver;
        var res = _map.Map<RentalDTO>(entity);
        return ApiResponse.Response(saveResult , res);
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<ApiResponse> GetListAsync([FromQuery]RentalRequestDTO input) 
    {
       IQueryable<Rental> query = _unitOfWork.Rentals.GetQueryable()
            .Include(r => r.Car).Include(r => r.Customer).Include(r => r.Driver);
        bool withSearching = input.WithSearching();
        if(withSearching) 
        {
            bool withDecimal = decimal.TryParse(input.SearchingValue, out decimal decimalValue);
            bool withInt = int.TryParse(input.SearchingValue, out int intValue);
             query = query.Where(r => 
                r.Car.Type.In(input.SearchingValue) || 
                r.Car.Color.In(input.SearchingValue) || 
                r.Car.Number.In(input.SearchingValue) ||
                (withDecimal && r.Car.EngineCapacity == decimalValue) ||
                r.Customer.Name.In(input.SearchingValue) ||
                r.Driver.Name.In(input.SearchingValue) ||
                r.State.In(input.SearchingValue) || 
                (withInt && r.DailyRate == intValue) ||
                input.SearchDate.DateBetween(r.StartDate,r.EndDate));
        }
        int countFilterd = await query.CountAsync();
        bool withSorting = input.WithSorting();
        if(withSorting) 
        {       
            bool IsDesc = input.IsOrderDesc();
            query = input.GetColumnOrder() switch
            {
                "type" => !IsDesc ? query.OrderBy(r => r.Car.Type) : query.OrderByDescending(r => r.Car.Type),
                "color" => !IsDesc ? query.OrderBy(r => r.Car.Color) : query.OrderByDescending(r => r.Car.Color),
                "engineeapacity" => !IsDesc ? query.OrderBy(r => r.Car.EngineCapacity) : query.OrderByDescending(r => r.Car.EngineCapacity),
                //"carnumber" => !IsDesc ? query.OrderBy(r => r.Car.Number) : query.OrderByDescending(r => r.Car.Number),
                "customercame" => !IsDesc ? query.OrderBy(r => r.Customer.Name) : query.OrderByDescending(r => r.Customer.Name),
                "drivername" => !IsDesc ? query.OrderBy(r => r.Driver.Name) : query.OrderByDescending(r => r.Driver.Name),
                "dailyrate" => !IsDesc ? query.OrderBy(r => r.DailyRate) : query.OrderByDescending(r => r.DailyRate),
                "state" => !IsDesc ? query.OrderBy(r => r.State) : query.OrderByDescending(r => r.State),
                "startdate" => !IsDesc ? query.OrderBy(r => r.StartDate) : query.OrderByDescending(r => r.StartDate),
                "enddate" => !IsDesc ? query.OrderBy(r => r.EndDate) : query.OrderByDescending(r => r.EndDate),
                _ => !IsDesc ? query.OrderBy(r => r.Car.Number) : query.OrderByDescending(r => r.Car.Number),
            };
        }
        query = query.ApplyPaging(input);
        var entityResult = await query.GetResultAsync(input , countFilterd);
        var dtoResult = _map.Map<PagingResult<RentalDTO>>(entityResult);
        return ApiResponse.OK(dtoResult);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id , RentalUpdateDTO input)
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        if (id != input.Id)
        {
            return ApiResponse.BAD("Object id is not compatible with the pass id");
        }
        input.StartDate = input.StartDate.Date;
        input.EndDate = input.EndDate.Date.AddDays(1);
        var entityRental = await _unitOfWork.Rentals.GetByIdAsync(id);
        // if(!entityRental.IsActive)
        //     return ApiResponse.BAD("This rental is not active !");
        bool changeCar = entityRental.CarId != input.CarId;
        bool changeCustomer = entityRental.CustomerId != input.CustomerId;
        bool changeDriver = entityRental.DriverId != input.DriverId;

        Car? entityCar = null;     
        if(changeCar)
        {
            //Car Check
            var carCheck = await CheckCar(input.CarId,input.StartDate,input.EndDate);
            if(carCheck.StatusCode != 200) 
            {
                return carCheck;
            }
            entityCar = ApiResponse.GetResult(carCheck) as Car;
        }
        
        if(changeCustomer)
        {
            var isCustomerExist = await _unitOfWork.Customers.IsExistAsync(input.CustomerId);
            if(!isCustomerExist)
            {
                return ApiResponse.NOT("This customer id is invalid");
            }
        }

        bool IsDriverPassed = input.DriverId.HasValue;
        Driver? entityDriver = null;     
        if(IsDriverPassed && changeDriver)
        {
            var driverCheck = await CheckDriver((Guid)input.DriverId,input.StartDate,input.EndDate);
            if(driverCheck.StatusCode != 200) 
            {
                return driverCheck;
            }
            entityDriver = ApiResponse.GetResult(driverCheck) as Driver;
            input.DriverId = entityDriver.Id;
        }
        var oldCarId = entityRental.CarId;
        var oldDriverId = entityRental.DriverId;
        _map.Map(input, entityRental);
        entityRental.EndDate = entityRental.EndDate.Date.AddDays(1);
        // entityRental.StartDate = startDate.Date;
        //entityRental.EndDate = startDate.Date;
        if(entityRental.IsActive)
        {
            if(changeCar)
            {
                var oldEntityCar = await _unitOfWork.Cars.GetByIdAsync(oldCarId);
                oldEntityCar.IsAvailable = true;
                entityCar.IsAvailable = false;
            }
            entityRental.State = "Active";
            if(IsDriverPassed && changeDriver) 
            {
                var oldEntityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)oldDriverId);
                oldEntityDriver.IsAvailable = true;
                entityDriver.IsAvailable = false;
            }
            _unitOfWork.Cars.Update(entityCar);
            _unitOfWork.Drivers.Update(entityDriver);
        }
        // if(input.FinishRental)
        // {
        //     entityCar.IsAvailable = true;
        //     if(entityDriver != null) entityDriver.IsAvailable = true;
        //     entityRental.EndDate = DateTime.Now.Date;
        // }
        _unitOfWork.Rentals.Update(entityRental);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.Response(result);
    }
    
    [HttpPut("ChangeState/{id}")]
    public async Task<ApiResponse> ChangeStateAsync(Guid id , string newState)
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        var entityRental = await _unitOfWork.Rentals.GetByIdAsync(id);
        if (entityRental == null)
        {
            return ApiResponse.NOT("This Id is invalid");
        }
        if(entityRental.State == "Finished" || entityRental.State == "Canceled")
        {
            return ApiResponse.BAD("This rental is not editable !");
        }
        if(newState == "Finished" && !entityRental.IsActive)
        {
            return ApiResponse.BAD("This rental is not active !");
        }
        if(newState == "Canceled" || newState == "Finished")
        {
            var entityCar = await _unitOfWork.Cars.GetByIdAsync(entityRental.CarId);
            entityCar.IsAvailable = true;
            Driver? entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityRental.DriverId);
            if(entityDriver == null)
            {
                entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entityDriver.SubstituteId);
            }
            if(entityDriver != null) 
            {
                entityDriver.IsAvailable = true;
            }
        }
        entityRental.State = newState;
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.Response(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        var entity = await _unitOfWork.Rentals.GetByIdAsync(id);
        if(entity == null)
        {
            return ApiResponse.NOT("This id is invalid");
        }
        var entityCar = await _unitOfWork.Cars.GetByIdAsync(entity.CarId);
        entityCar.IsAvailable = true;
        if(entity.DriverId.HasValue)
        {
            var entityDriver = await _unitOfWork.Drivers.GetByIdAsync((Guid)entity.DriverId);
            entityDriver.IsAvailable = true;
        }
        _unitOfWork.Rentals.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.Response(result);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        var getOne = await _unitOfWork.Rentals.GetByIdAsync(id);
        var result = _map.Map<RentalDTO>(getOne);
        return ApiResponse.OK(result);
    }
}