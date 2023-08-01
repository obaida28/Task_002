namespace Core.Entites;
public class Customer
{
    public string CustomerName { get; set; }
    public virtual ICollection<Rental> Rentals { get; set; }
    public Customer() : base() {}
}