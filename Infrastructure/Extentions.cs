namespace Infrastructure;
public static class Extentions
{
    public static bool IsEmpty<T>(this IEnumerable<T> data) => data == null || !data.Any();
}