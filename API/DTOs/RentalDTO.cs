namespace API.DTOs;
public class RentalDTO
{
    public string CarNumber { get; set; }
    public string CarType { get; set; }
    public string CustomerName { get; set; }
    public string? DriverName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DailyRate { get; set; }
}