using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class CustomerRequestDTO : RequestDTO<Customer>
{    
    public override IQueryable<Customer> ApplySearching(IQueryable<Customer> query)
    {
        switch(SearchingColumn)
        {
            case "Id" : 
                return query.Where(d => d.Id == new Guid(SearchingValue));
            default :
                return query.Where(d => d.CustomerName.Contains(SearchingValue));
        }
    }
    public override IQueryable<Customer> ApplySorting(IQueryable<Customer> query)
    {
        switch(OrderByData)
        {
            case "Id" : 
                return ASC ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id);
            default :
                return ASC ? query.OrderBy(c => c.CustomerName) : query.OrderByDescending(c => c.CustomerName);
        }
    }
}