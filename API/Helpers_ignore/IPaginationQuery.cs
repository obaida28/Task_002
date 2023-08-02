namespace API.Helpers;
public interface IPaginationQuery
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
}