using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UnitOfWork(ApplicationDbContext context , 
        UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager) 
    {
        _context = context;
        Cars = new CarRepository(_context);
        Customers = new CustomerRepository(_context);
        Drivers = new DriverRepository(_context);
        Rentals = new RentalRepository(_context);
        Users = new UserRepository(userManager, roleManager);
    }
    public ICarRepository Cars { get; }
    public ICustomerRepository Customers { get; }
    public IDriverRepository Drivers { get; }
    public IRentalRepository Rentals { get; }
    public IUserRepository Users { get; }

    public async void Dispose() => await _context.DisposeAsync();

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
}