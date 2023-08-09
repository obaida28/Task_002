using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class DriverRequestDTO : PagingModel<Driver>
{
    public string? SearchingColumn { get; set; }
  
    public string? SearchingValue { get; set; }
}