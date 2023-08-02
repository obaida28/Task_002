using Core.Entites;
using Core.Interfaces;
using Core.ViewModels;

namespace Infrastructure.Services;
public class CarService : ICarService
{
    private readonly ICarRepository _repository;
    private readonly ICarCache _cache;
    public CarService(ICarRepository repository , ICarCache cache)
    {
        _repository = repository;
        _cache = cache;
    }
    public async Task<List<Car>> GetAllCars() 
    {
        var data = await _repository.GetAll();
        return data;
    }
    public async Task<Car> GetCarById(Guid id) 
    {
        var data = await _repository.GetById(id);
        return data;
    }
    public async Task AddCar(Car car)
    {
        _repository.Add(car);
        await _cache.AddToCache(car);
    }
    public async Task<bool> IsExist(Guid id) 
    {
        var res = await _repository.IsExist(id);
        return res;
    } 
    public async Task UpdateCar(Car car)
    {
        _repository.Update(car);
        await _cache.UpdateCache(car);
    }
    public async Task DeleteCar(Guid id) 
    {
        await _repository.Delete(id);
        await _cache.DeleteFromCache(id);
    }     
    public List<Car> Getfilter(CarFilter dto)
    {
        IQueryable<Car> data = _repository.GetAllBeforeExecute();
        if(dto.WithPaging)
            data = Paging(data , dto.pageNumber , dto.pageSize);
        if(dto.WithSearching && dto.colNameSearch is not null && dto.valueSearch is not null)
            data = Searching(data , dto.colNameSearch , dto.valueSearch);
        if(dto.WithSorting && dto.colNameSort is not null)
            data = Sorting(data , dto.colNameSort , dto.Desc ?? false);
        return data.ToList();
    }
    private IQueryable<Car> Paging(IQueryable<Car> cars , int? pageNumber , int? pageSize)
    {
        int pgSize = (int)(pageNumber ?? 10);
        int pgNum = (int)(pageSize ?? 1);
        int skip = pgSize * (pgNum - 1);
        return cars.Skip(skip).Take(pgNum);   
    }
    private IQueryable<Car> Sorting(IQueryable<Car> cars , string colName , bool desc = false)
    {
        switch(colName)
        {
            case "Type" : 
                return desc ? cars.OrderByDescending(c => c.Type) : cars.OrderBy(c => c.Type);
            case "Color" : 
                return desc ? cars.OrderByDescending(c => c.Color) : cars.OrderBy(c => c.Color);
            case "EngineCapacity" : 
                return desc ? cars.OrderByDescending(c => c.EngineCapacity) : cars.OrderBy(c => c.EngineCapacity);
            case "DailyRate" : 
                return desc ? cars.OrderByDescending(c => c.DailyRate) : cars.OrderBy(c => c.DailyRate);
            default :
                return desc ? cars.OrderByDescending(c => c.CarNumber) : cars.OrderBy(c => c.CarNumber);
        }
    }
    private IQueryable<Car> Searching(IQueryable<Car> cars , string colName , string value)
    {
        switch(colName)
        {
            case "Type" : 
                return cars.Where(c => c.Type.Contains(value.ToString()));
            case "Color" : 
                return cars.Where(c => c.Color.Contains(value.ToString()));
            case "EngineCapacity" : 
                return cars.Where(c => c.EngineCapacity == Convert.ToDecimal(value));
            case "DailyRate" : 
                return cars.Where(c => c.DailyRate == Convert.ToInt32(value));
            default :
                return cars.Where(c => c.CarNumber.Contains(value.ToString()));
        }
    }
    public async Task<IEnumerable<Car>> GetCarsByCache() 
    {
        var data = await _cache.GetAllFromCache();
        return data;
    }
}