namespace Core.Entites;
public class Driver : BaseEntity
{
    public string Name { get; set; }
    public Guid? SubstituteId { get; set; }
    public virtual Driver Substitute { get; set; }
    public bool IsAvailable { get; set; } = true;
    public virtual ICollection<Rental> Rentals { get; set; }
    public Driver() : base() {}
}