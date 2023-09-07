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

    public static IEnumerable<TInput> ApplyPaging<TInput>(
    this IEnumerable<TInput> query, RequestDTO<TInput> input) 
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
    public static PagingResult<TEntity> GetResult<TEntity>(
    this IEnumerable<TEntity> data , RequestDTO<TEntity> input, int totalCount) 
    where TEntity : BaseEntity
    {
        var result = data.ToList();
        return new PagingResult<TEntity>
        {
          RowsPerPage = input.RowsPerPage,
          CurrentPage = Math.Max(1,input.CurrentPage),
          TotalPages = Math.Max(1,totalCount / input.RowsPerPage),
          TotalRows = totalCount,
          Results = result
        };
    }
    public static IQueryable<Car> Searching(this IQueryable<Car> cars , string searchingValue) 
    {
        bool withSearching = !string.IsNullOrEmpty(searchingValue);
        if(withSearching) 
        {
            bool withDecimal = decimal.TryParse(searchingValue, out decimal decimalValue);
            bool withInt = int.TryParse(searchingValue, out int intValue);
            cars = cars.Where(c => 
                c.Type.ToLower().Contains(searchingValue) || 
                c.Color.ToLower().Contains(searchingValue) || 
                c.Number.ToLower().Contains(searchingValue) ||
                (withDecimal && c.EngineCapacity == decimalValue) || (withInt && c.DailyRate == intValue));
        }
        return cars;
    }
    public static IQueryable<Car> Sorting(this IQueryable<Car> cars , string orderByData) 
    {
        bool withSorting = !string.IsNullOrEmpty(orderByData);
        if(withSorting) 
        {
            string dataOrder = orderByData.ToLower();
            string[] orderResult = dataOrder.Split(" ");
            bool IsDesc = orderResult.Last() == "desc";
            cars = orderResult.First() switch
            {
                "type" => !IsDesc ? cars.OrderBy(c => c.Type) : cars.OrderByDescending(c => c.Type),
                "color" => !IsDesc ? cars.OrderBy(c => c.Color) : cars.OrderByDescending(c => c.Color),
                "enginecapacity" => !IsDesc ? cars.OrderBy(c => c.EngineCapacity) : cars.OrderByDescending(c => c.EngineCapacity),
                "dailyrate" => !IsDesc ? cars.OrderBy(c => c.DailyRate) : cars.OrderByDescending(c => c.DailyRate),
                _ => !IsDesc ? cars.OrderBy(c => c.Number) : cars.OrderByDescending(c => c.Number),
            };
        }
        return cars;
    }
    

}