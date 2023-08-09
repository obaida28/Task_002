using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class CarRequestDTO : RequestDTO<Car>
{    
    public override IQueryable<Car> ApplySearching(IQueryable<Car> query)
    {
        switch(SearchingColumn)
        {
            case "Id" : 
                return query.Where(c => c.Id == new Guid(SearchingValue));
            case "Type" : 
                return query.Where(c => c.Type.Contains(SearchingValue));
            case "Color" : 
                return query.Where(c => c.Color.Contains(SearchingValue));
            case "EngineCapacity" : 
                return query.Where(c => c.EngineCapacity == Convert.ToDecimal(SearchingValue));
            case "DailyRate" : 
                return query.Where(c => c.DailyRate == Convert.ToInt32(SearchingValue));
            default :
                return query.Where(c => c.CarNumber.Contains(SearchingValue));
        }
    }
    public override IQueryable<Car> ApplySorting(IQueryable<Car> query)
    {
        switch(OrderByData)
        {
            case "Id" : 
                return ASC ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id);
            case "Type" : 
                return ASC ? query.OrderBy(c => c.Type) : query.OrderByDescending(c => c.Type);
            case "Color" : 
                return ASC ? query.OrderBy(c => c.Color) : query.OrderByDescending(c => c.Color);
            case "EngineCapacity" : 
                return ASC ? query.OrderBy(c => c.EngineCapacity) : query.OrderByDescending(c => c.EngineCapacity);
            case "DailyRate" : 
                return ASC ? query.OrderBy(c => c.DailyRate) : query.OrderByDescending(c => c.DailyRate);
            default :
                return ASC ? query.OrderBy(c => c.CarNumber) : query.OrderByDescending(c => c.CarNumber);
        }
    }
}