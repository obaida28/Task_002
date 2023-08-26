namespace Infrastructure.Data.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public UnitOfWork(ApplicationDbContext context) 
    {
        _context = context;
        Cars = new CarRepository(_context);
        Customers = new CustomerRepository(_context);
        Drivers = new DriverRepository(_context);
        Rentals = new RentalRepository(_context);
    }
    public ICarRepository Cars { get; }
    public ICustomerRepository Customers { get; }
    public IDriverRepository Drivers { get; }
    public IRentalRepository Rentals { get; }

    public async void Dispose() => await _context.DisposeAsync();

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
}