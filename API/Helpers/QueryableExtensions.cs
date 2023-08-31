namespace API.Helpers;
public static class QueryableExtensions
{
    public static IQueryable<TInput> ApplyPaging<TInput>(
    this IQueryable<TInput> query, RequestDTO<TInput> input) 
    where TInput : BaseEntity
    {
      input.RowsPerPage = input.RowsPerPage == 0 ? 1 : input.RowsPerPage;
      input.CurrentPage = input.CurrentPage == 0 ? 1 : input.CurrentPage;
      var itemsToSkip = (input.CurrentPage - 1) * input.RowsPerPage ;
      var result = query.Skip(itemsToSkip).Take(input.RowsPerPage) ;
      return result;
    }

    public static async Task<PagingResult<TEntity>> GetResultAsync<TEntity>(
    this IQueryable<TEntity> query , RequestDTO<TEntity> input, int totalCount) 
    where TEntity : BaseEntity
    {
        var result = await query.ToListAsync();
        return new PagingResult<TEntity>
        {
          RowsPerPage = input.RowsPerPage,
          CurrentPage = Math.Max(1,input.CurrentPage),
          TotalPages = Math.Max(1,totalCount / input.RowsPerPage),
          TotalRows = totalCount,
          Results = result
        };
    }
}