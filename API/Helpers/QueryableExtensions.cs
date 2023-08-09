// using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;
public static class QueryableExtensions
{
//   public static IQueryable<TInput> ApplySearching<TInput>(
//     this IQueryable<TInput> query, string SearchingColumn , string SearchingValue) 
//     where TInput : class
//   {
//     var finalQuery = query.Where($"{SearchingColumn} = @0", SearchingValue);
//     return finalQuery;
//  }
  // public static IQueryable<TInput> ApplySorting<TInput>(
  //    this IQueryable<TInput> query, string orderBy, string sortOrder = "asc")
  //    where TInput : class
  //   {
  //       var sortOrderTerm = (sortOrder != "asc") ? " descending" : string.Empty;
  //       var finalQuery = string.IsNullOrWhiteSpace(orderBy) ? query : query.OrderBy(orderBy + sortOrderTerm);
  //       return finalQuery;
  //   }
    public static IQueryable<TInput> ApplyPaging<TInput>(
    this IQueryable<TInput> query, int currentPage, int rowsPerPage, bool getAllRecords = false) 
    where TInput : class
    {
      var itemsToSkip = (currentPage - 1) * rowsPerPage;
      var result = getAllRecords ? query : query.Skip(itemsToSkip).Take(rowsPerPage);
      return result;
    }
    
    public static async Task<PagingModel<TInput>> GetResultAsync<TInput>(
    this IQueryable<TInput> query, int currentPage, int rowsPerPage, int totalCount) 
    where TInput : class
    {
        var result = await query.ToListAsync();
        return new PagingModel<TInput>
        {
          RowsPerPage = rowsPerPage,
          CurrentPage = currentPage,
          TotalPages = totalCount / rowsPerPage,
          TotalRows = totalCount,
          Results = result
        };
    }
}