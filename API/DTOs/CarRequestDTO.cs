using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class CarRequestDTO : PagingModel<Car>
{
    public string? SearchingColumn { get; set; }
  
    public string? SearchingValue { get; set; }
}