namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
public class CarController : ControllerBase 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    public CarController(IUnitOfWork unitOfWork , IMapper map)
    {
        _unitOfWork = unitOfWork;
        _map = map;
    }
    
    [HttpGet(template: "GetListAsync")]
    public async Task<ApiResponse> GetListAsync([FromQuery]CarRequestDTO input) 
    {
        var query = _unitOfWork.Cars.GetQueryable();

        bool withSearching = !string.IsNullOrEmpty(input.SearchingValue);
        if(withSearching) 
        {
            bool withDecimal = decimal.TryParse(input.SearchingValue, out decimal decimalValue);
            bool withInt = int.TryParse(input.SearchingValue, out int intValue);
             query = query.Where(c => 
                c.Type.ToLower().Contains(input.SearchingValue) || 
                c.Color.ToLower().Contains(input.SearchingValue) || 
                c.Number.ToLower().Contains(input.SearchingValue) ||
                (withDecimal && c.EngineCapacity == decimalValue) || (withInt && c.DailyRate == intValue));
        }
           
        int countFilterd = await query.CountAsync();

        bool withSorting = !string.IsNullOrEmpty(input.OrderByData);
        if(withSorting) 
        {
            string dataOrder = input.OrderByData.ToLower();
            string[] orderResult = dataOrder.Split(" ");
            bool IsDesc = orderResult.Last() == "desc";
            query = orderResult.First() switch
            {
                "type" => !IsDesc ? query.OrderBy(c => c.Type) : query.OrderByDescending(c => c.Type),
                "color" => !IsDesc ? query.OrderBy(c => c.Color) : query.OrderByDescending(c => c.Color),
                "enginecapacity" => !IsDesc ? query.OrderBy(c => c.EngineCapacity) : query.OrderByDescending(c => c.EngineCapacity),
                "dailyrate" => !IsDesc ? query.OrderBy(c => c.DailyRate) : query.OrderByDescending(c => c.DailyRate),
                _ => !IsDesc ? query.OrderBy(c => c.Number) : query.OrderByDescending(c => c.Number),
            };
        }
        query = query.ApplyPaging(input);
        var entityResult = await query.GetResultAsync(input , countFilterd);
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
        var getOne = await _unitOfWork.Cars.GetByIdAsync(id);
        var result = _map.Map<CarDTO>(getOne);
        return ApiResponse.OK(result);
    }
    
    [HttpPost]
    public async Task<ApiResponse> CreateAsync(CarCreateDto input)
    {
        bool isExist = await _unitOfWork.Cars.IsExistNumberAsync(input.Number);
        if(isExist)
        {
             return ApiResponse.BAD( "The car number is unique !");
        }
        var entity = _map.Map<Car>(input);
        await _unitOfWork.Cars.AddAsync(entity);
        var result = await _unitOfWork.SaveAsync();
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
        Car car = _map.Map<Car>(input);
        _unitOfWork.Cars.Update(car);
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
        return ApiResponse.Response(result);
    }
}