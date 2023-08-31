namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
public class DriverController : ControllerBase 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    public DriverController(IUnitOfWork unitOfWork , IMapper map)
    {
        _unitOfWork = unitOfWork;
        _map = map;
    }

    [HttpGet(template: "GetListAsync")]
    public async Task<ApiResponse> GetListAsync([FromQuery]DriverRequestDTO input)
    {
        var query = _unitOfWork.Drivers.GetQueryable();

        bool withSearching = !string.IsNullOrEmpty(input.SearchingValue);
        if(withSearching) 
        {
            query = query.Where(d => d.Name.ToLower().Contains(input.SearchingValue));
        }

        int countFilterd = await query.CountAsync();

        bool withSorting = !string.IsNullOrEmpty(input.OrderByData);
        if(withSorting) 
        {
            string dataOrder = input.OrderByData.ToLower();
            string[] orderResult = dataOrder.Split(" ");
            bool IsDesc = orderResult.Last() == "desc";
            query = !IsDesc ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name);
        }
        query = query.ApplyPaging(input);
        var entityResult = await query.GetResultAsync(input , countFilterd);
        var dtoResult = _map.Map<PagingResult<DriverDTO>>(entityResult);
        return ApiResponse.OK(dtoResult);
    }
    
    [HttpGet("{id}")]
    public async Task<ApiResponse> GetAsync(Guid id) 
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        var getOne = await _unitOfWork.Drivers.GetByIdAsync(id);
        var result = _map.Map<DriverDTO>(getOne);
        return ApiResponse.OK(result);
    }

    [HttpPost]
    public async Task<ApiResponse> CreateAsync(DriverCreateDTO input)
    {
        Driver driver = _map.Map<Driver>(input);
        await _unitOfWork.Drivers.AddAsync(driver);
        var result = await _unitOfWork.SaveAsync();
        var dto = _map.Map<DriverDTO>(driver);
        return ApiResponse.Response(result , dto);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAsync(Guid id, DriverUpdateDTO input)
    {
        if (id == Guid.Empty)
        {
            return ApiResponse.BAD("Id is Required");
        }
        if (id != input.Id)
        {
            return ApiResponse.BAD("Object id is not compatible with the pass id");
        }
        Driver driver = _map.Map<Driver>(input);
        _unitOfWork.Drivers.Update(driver);
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
        var entity = await _unitOfWork.Drivers.GetByIdAsync(id);
        if(entity == null)
        {
            return ApiResponse.NOT("This id is invalid");
        }
        _unitOfWork.Drivers.Delete(entity);
        var result = await _unitOfWork.SaveAsync();
        return ApiResponse.Response(result);
    }
}