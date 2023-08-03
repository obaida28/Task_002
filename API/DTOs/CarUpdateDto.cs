namespace API.DTOs;
public class CarUpdateDto
{
    public Guid CarId { get; set; }
    public string Type { get; set; }
    public decimal EngineCapacity { get; set; }
    public string Color { get; set; }
    public int DailyRate { get; set; }
    public Guid? DriverId { get; set; }
    public Guid? CustomerId { get; set; }
}