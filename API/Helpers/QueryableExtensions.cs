using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;
public static class QueryableExtensions
{
    public async static Task<PagingModel<TInput>> GetPagedResult<TInput>(this IQueryable<TInput> query, int currentPage, int rowsPerPage, string orderBy, bool getAllRecords, string sortOrder = "asc")

   where TInput : class

 {
   var sortOrderTerm = (sortOrder != "asc") ? " descending" : string.Empty ;
   var finalQuery = String.IsNullOrWhiteSpace(orderBy) ? query : query.OrderBy(orderBy + sortOrderTerm);

   var itemsToSkip = (currentPage - 1) * rowsPerPage;
   var totalCount = finalQuery.Count();
   var result = getAllRecords ? await finalQuery.ToListAsync() : await finalQuery.Skip(itemsToSkip).Take(rowsPerPage).ToListAsync();


   return new PagingModel<TInput>

   {
     RowsPerPage = rowsPerPage,
     CurrentPage = currentPage,
     TotalPages = (totalCount / rowsPerPage),
     TotalRows = totalCount,
     Results = result
   };

 }
}