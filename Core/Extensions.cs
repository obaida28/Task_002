namespace Core.Extensions;
public static class Extensions
{
    public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("Items are null");
            return items.Contains(item);
        }
    // public static bool In(this string father , string son) => father.ToLower().Contains(son.ToLower());
    public static bool DateBetween(this DateTime date , DateTime start , DateTime end) => 
      start.Date >= date.Date && end.Date <= date.Date;
    public static bool DateTimeBetween(this DateTime date , DateTime start , DateTime end) => 
      start >= date && end <= date;
}