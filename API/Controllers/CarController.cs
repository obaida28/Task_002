namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
[Authorize]
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
        query = query.Searching(input.SearchingValue);
        int countFilterd = await query.CountAsync();
        query = query.Sorting(input.OrderByData);
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