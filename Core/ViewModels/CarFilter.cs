namespace Core.ViewModels;
public class CarFilter
{
    public bool WithPaging { get; set; }
    public int? pageNumber { get; set; }
    public int? pageSize { get; set; }
    public bool WithSorting { get; set; }
    public string? colNameSort { get; set; }
    public bool? Desc { get; set; }
    public bool WithSearching { get; set; }
    public string? colNameSearch { get; set; }
    public string? valueSearch { get; set; }
}