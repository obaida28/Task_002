namespace API.DTOs;
public class CarDTO
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public string Type { get; set; }
    public decimal EngineCapacity { get; set; }
    public string Color { get; set; }
    public int DailyRate { get; set; }
}