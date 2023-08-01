namespace API.DTOs;
public class CarView
{
    public string CarNumber { get; set; }
    public string Type { get; set; }
    public decimal EngineCapacity { get; set; }
    public string Color { get; set; }
    public int DailyRate { get; set; }
    public string DriverName { get; set; }
    public string CustomerName { get; set; }
}
// public record CarView(string CarNumber , string Type , decimal EngineCapacity ,
//  string Color , int DailyRate , List<string> DriverName ,  List<string> CustomerName);