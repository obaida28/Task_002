namespace Core.Interfaces;
public interface IUnitOfWork : IDisposable
{
    ICarRepository Cars { get; }
    ICustomerRepository Customers { get; }
    IDriverRepository Drivers { get; }
    Task<int> SaveAsync();
}