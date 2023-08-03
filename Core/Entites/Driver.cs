namespace Core.Entites;
public class Driver : BaseEntity
{
    public string DriverName { get; set; }
    public Guid? SubstituteDriverId { get; set; }
    public virtual Driver Substitute { get; set; }
    public bool IsAvailable { get; set; }
    public virtual ICollection<Rental> Rentals { get; set; }
    public Driver() : base() {}
}