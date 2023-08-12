using System.ComponentModel.DataAnnotations;
using Core.Entites;
namespace API.DTOs;

public class RequestDTO<T> where T : BaseEntity
{
    [Range(0, int.MaxValue, ErrorMessage = "Current page must be a non-negative value.")]
    public int CurrentPage { get; set; } 

    [Range(0, int.MaxValue, ErrorMessage = "Rows per page must be a non-negative value.")]
    public int RowsPerPage { get; set; }

    public string OrderByData { get; set; }

    public bool ASC { get; set; } = true;
    
    public string SearchingValue { get; set; }
}