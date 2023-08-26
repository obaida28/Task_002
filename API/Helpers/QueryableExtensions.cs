namespace API.Helpers;
public static class QueryableExtensions
{
    public static IQueryable<TInput> ApplyPaging<TInput>(
    this IQueryable<TInput> query, int currentPage, int rowsPerPage, bool WithPaging) 
    where TInput : class
    {
      var itemsToSkip = (currentPage - 1) * rowsPerPage ;
      var result = WithPaging ? query.Skip(itemsToSkip).Take(rowsPerPage) : query ;
      return result;
    }

    public static async Task<PagingResult<TEntity>> GetResultAsync<TEntity>(
    this IQueryable<TEntity> query, bool WithPaging , int currentPage, int rowsPerPage, int totalCount) 
    where TEntity : class
    {
        var result = await query.ToListAsync();
        if(!WithPaging) return new PagingResult<TEntity> { Results = result };
        return new PagingResult<TEntity>
        {
          RowsPerPage = rowsPerPage,
          CurrentPage = Math.Max(1,currentPage),
          TotalPages = Math.Max(1,totalCount / rowsPerPage),
          TotalRows = totalCount,
          Results = result
        };
    }
}