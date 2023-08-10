using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class CustomerRequestDTO : RequestDTO<Customer>
{    
    public override IQueryable<Customer> ApplySearching(IQueryable<Customer> query)
    {
        return SearchingColumn switch
        {
            "Id" => query.Where(d => d.Id == new Guid(SearchingValue)),
            _ => query.Where(d => d.CustomerName.Contains(SearchingValue)),
        };
    }
    public override IQueryable<Customer> ApplySorting(IQueryable<Customer> query)
    {
        return OrderByData switch
        {
            "Id" => ASC ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id),
            _ => ASC ? query.OrderBy(c => c.CustomerName) : query.OrderByDescending(c => c.CustomerName),
        };
    }
}