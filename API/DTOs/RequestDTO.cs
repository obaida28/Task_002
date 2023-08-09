using Core.Entites;
namespace API.DTOs;

public abstract class RequestDTO<T> where T : BaseEntity
{
    public int CurrentPage { get; set; } 

    public int RowsPerPage { get; set; }

    public string OrderByData { get; set; }

    public bool ASC { get; set; } = true;
    
    public string SearchingColumn { get; set; }
  
    public string SearchingValue { get; set; }
    
    public abstract IQueryable<T> ApplySearching(IQueryable<T> query);

    public abstract IQueryable<T> ApplySorting(IQueryable<T> query);
}