using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class CarRequestDTO : PagingModel<Car>
{
    // public string? SearchingColumn { get; set; }
    // public string? SearchingValue { get; set; }
}
// public record CarView(string CarNumber , string Type , decimal EngineCapacity ,
//  string Color , int DailyRate , List<string> DriverName ,  List<string> CustomerName);