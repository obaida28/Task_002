using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class DriverRequestDTO : PagingModel<Customer>
{
    public string? SearchingColumn { get; set; }
    public string? SearchingValue { get; set; }
}