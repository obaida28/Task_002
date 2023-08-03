namespace Core.Entites;
public class Customer : BaseEntity
{
    public string CustomerName { get; set; }
    public virtual ICollection<Rental> Rentals { get; set; }
    public Customer() : base() {}
}