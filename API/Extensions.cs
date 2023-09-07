namespace API.Extensions;
public static class Extensions
{
    public static bool WithSearching<T>(this RequestDTO<T> input) 
    where T : BaseEntity => !string.IsNullOrEmpty(input.SearchingValue);

    public static bool WithSorting<T>(this RequestDTO<T> input) 
    where T : BaseEntity => !string.IsNullOrEmpty(input.OrderByData);

    public static string GetColumnOrder<T>(this RequestDTO<T> input) 
    where T : BaseEntity => input.OrderByData.ToLower().Split(" ").First();

    public static bool IsOrderDesc<T>(this RequestDTO<T> input) 
    where T : BaseEntity => input.OrderByData.ToLower().Split(" ").Last() == "desc";

    public static string GetErrors(this IdentityResult result) => 
        string.Join(",", result.Errors.Select(o => o.Description).ToArray());
}