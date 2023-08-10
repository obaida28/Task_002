using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class DriverRequestDTO : RequestDTO<Driver>
{    
    public override IQueryable<Driver> ApplySearching(IQueryable<Driver> query)
    {
        return SearchingColumn switch
        {
            "Id" => query.Where(d => d.Id == new Guid(SearchingValue)),
            _ => query.Where(d => d.DriverName.Contains(SearchingValue)),
        };
    }
    public override IQueryable<Driver> ApplySorting(IQueryable<Driver> query)
    {
        return OrderByData switch
        {
            "Id" => ASC ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id),
            _ => ASC ? query.OrderBy(c => c.DriverName) : query.OrderByDescending(c => c.DriverName),
        };
    }
}