namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
[Authorize]
public class CarInMemoryController : ControllerBase 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    private readonly IMemoryCache _cache;
    public CarInMemoryController(IUnitOfWork unitOfWork , IMapper map , IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _map = map;
        _cache = cache;
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public IQueryable<Car> Searching(IQueryable<Car> cars , string searchingValue) 
    {
        bool withSearching = !string.IsNullOrEmpty(searchingValue);
        if(withSearching) 
        {
            bool withDecimal = decimal.TryParse(searchingValue, out decimal decimalValue);
            bool withInt = int.TryParse(searchingValue, out int intValue);
            cars = cars.Where(c => 
                c.Type.ToLower().Contains(searchingValue) || 
                c.Color.ToLower().Contains(searchingValue) || 
                c.Number.ToLower().Contains(searchingValue) ||
                (withDecimal && c.EngineCapacity == decimalValue) || (withInt && c.DailyRate == intValue));
        }
        return cars;
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public IQueryable<Car> Sorting(IQueryable<Car> cars , string orderByData) 
    {
        bool withSorting = !string.IsNullOrEmpty(orderByData);
        if(withSorting) 
        {
            string dataOrder = orderByData.ToLower();
            string[] orderResult = dataOrder.Split(" ");
            bool IsDesc = orderResult.Last() == "desc";
            cars = orderResult.First() switch
            {
                "type" => !IsDesc ? cars.OrderBy(c => c.Type) : cars.OrderByDescending(c => c.Type),
                "color" => !IsDesc ? cars.OrderBy(c => c.Color) : cars.OrderByDescending(c => c.Color),
                "enginecapacity" => !IsDesc ? cars.OrderBy(c => c.EngineCapacity) : cars.OrderByDescending(c => c.EngineCapacity),
                "dailyrate" => !IsDesc ? cars.OrderBy(c => c.DailyRate) : cars.OrderByDescending(c => c.DailyRate),
                _ => !IsDesc ? cars.OrderBy(c => c.Number) : cars.OrderByDescending(c => c.Number),
            };
        }
        return cars;
    }
    
    
    [HttpGet(template: "GetListAsync")]
    public async Task<ApiResponse> GetListAsync([FromQuery]CarRequestDTO input) 
    {
        IQueryable<Car> cars;
        if (_cache.TryGetValue("cars", out IEnumerable<Car> _cars)) 
        {
            cars = _cars as IQueryable<Car>;
        }
        else
        {
            cars = _unitOfWork.Cars.GetQueryable();
        }
        cars = Searching(cars,input.SearchingValue);
        int countFilterd = await cars.CountAsync();
        cars = Sorting(cars,input.OrderByData);
        cars = cars.ApplyPaging(input);
        var entityResult = await cars.GetResultAsync(input , countFilterd);
        var dtoResult = _map.Map<PagingResult<CarDTO>>(entityResult);
        return ApiResponse.OK(dtoResult);
    }
    
    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        Car getOne;
        if (_cache.TryGetValue("cars", out IEnumerable<Car> _cars)) 
        {
            var list = _cars.ToList();
            getOne = list.Find(c => c.Id == id);
        }
        else
        {
            getOne = await _unitOfWork.Cars.GetByIdAsync(id);
        }
        var result = _map.Map<CarDTO>(getOne);
        return ApiResponse.OK(result);
    }
    
    [HttpPost]
    public async Task<ApiResponse> CreateAsync(CarCreateDto input)
    {
        bool isExist;
        List<Car>? list = null;
        bool hasCache = _cache.TryGetValue("cars", out IEnumerable<Car> _cars);
        if (hasCache)
        {
            list = _cars.ToList();
            isExist = list.Any(c => c.Number == input.Number);
        }
        else
        {
            isExist = await _unitOfWork.Cars.IsExistNumberAsync(input.Number);
        }
        if(isExist)
        {
            return ApiResponse.BAD( "The car number is unique !");
        }
        var entity = _map.Map<Car>(input);
        await _unitOfWork.Cars.AddAsync(entity);
        var result = await _unitOfWork.SaveAsync();
        if (hasCache) 
        {
            list.Add(entity);
            _cache.Set("cars", list, TimeSpan.FromDays(1));
        }
        var dto = _map.Map<CarDTO>(entity);
        return ApiResponse.Response(result , dto);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id, CarUpdateDto input)
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        if (id != input.Id)
        {
            return ApiResponse.BAD("Object id is not compatible with the pass id");
        }
        bool isExist;
        List<Car>? list = null;
        bool hasCache = _cache.TryGetValue("cars", out IEnumerable<Car> _cars);
        if (hasCache)
        {
            list = _cars.ToList();
            isExist = list.Any(c => c.Number == input.Number);
        }
        else
        {
            isExist = await _unitOfWork.Cars.IsExistNumberAsync(input.Number);
        }
        if(isExist)
        {
            return ApiResponse.BAD( "The car number is unique !");
        }
        Car car = _map.Map<Car>(input);
        _unitOfWork.Cars.Update(car);        
        var result = await _unitOfWork.SaveAsync();
        if (hasCache) 
        {
            list.Remove(car);
            list.Add(car);
            _cache.Set("cars", list, TimeSpan.FromDays(1));
        }
        return ApiResponse.Response(result);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        var entity = await _unitOfWork.Cars.GetByIdAsync(id);
        if(entity == null)
        {
            return ApiResponse.NOT("This id is invalid");
        }
        bool WasRented = await _unitOfWork.Rentals.IsCarWasRentedAsync(entity.Number);
        if(WasRented)
        {
            return ApiResponse.BAD("This car was rented");
        }
        _unitOfWork.Cars.Delete(entity);
        var result = await _unitOfWork.SaveAsync();

        if (_cache.TryGetValue("cars", out IEnumerable<Car> cars)) 
        {
            var list = cars.ToList();
            list.Remove(entity);
            _cache.Set("cars", list, TimeSpan.FromDays(1));
        }

        return ApiResponse.Response(result);
    }
}