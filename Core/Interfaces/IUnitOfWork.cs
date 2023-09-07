namespace Core.Interfaces;
public interface IUnitOfWork : IDisposable
{
    ICarRepository Cars { get; }
    ICustomerRepository Customers { get; }
    IDriverRepository Drivers { get; }
    IRentalRepository Rentals { get; }
    IUserRepository Users { get; }
    Task<int> SaveAsync();
}