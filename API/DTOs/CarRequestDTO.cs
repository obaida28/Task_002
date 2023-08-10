using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class CarRequestDTO : RequestDTO<Car>
{    
    public override IQueryable<Car> ApplySearching(IQueryable<Car> query)
    {
        return SearchingColumn switch
        {
            "Id" => query.Where(c => c.Id == new Guid(SearchingValue)),
            "Type" => query.Where(c => c.Type.Contains(SearchingValue)),
            "Color" => query.Where(c => c.Color.Contains(SearchingValue)),
            "EngineCapacity" => query.Where(c => c.EngineCapacity == Convert.ToDecimal(SearchingValue)),
            "DailyRate" => query.Where(c => c.DailyRate == Convert.ToInt32(SearchingValue)),
            _ => query.Where(c => c.CarNumber.Contains(SearchingValue)),
        };
    }
    public override IQueryable<Car> ApplySorting(IQueryable<Car> query)
    {
        return OrderByData switch
        {
            "Id" => ASC ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id),
            "Type" => ASC ? query.OrderBy(c => c.Type) : query.OrderByDescending(c => c.Type),
            "Color" => ASC ? query.OrderBy(c => c.Color) : query.OrderByDescending(c => c.Color),
            "EngineCapacity" => ASC ? query.OrderBy(c => c.EngineCapacity) : query.OrderByDescending(c => c.EngineCapacity),
            "DailyRate" => ASC ? query.OrderBy(c => c.DailyRate) : query.OrderByDescending(c => c.DailyRate),
            _ => ASC ? query.OrderBy(c => c.CarNumber) : query.OrderByDescending(c => c.CarNumber),
        };
    }
}