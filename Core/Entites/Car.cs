namespace Core.Entites;
public class Car : BaseEntity
{
    public string Number { get; set; }
    public string Type { get; set; }
    public decimal EngineCapacity { get; set; }
    public string Color { get; set; }
    public int DailyRate { get; set; }
    public bool IsAvailable { get; set; } = true;
    public virtual ICollection<Rental> Rentals { get; set; }
    public Car() : base() {}
}