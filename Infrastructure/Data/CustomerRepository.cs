using Core.Entites;
using Core.Interfaces;
namespace Infrastructure.Data;
public class CustomerRepository : GenericRepository<Customer> , ICustomerRepository
{
    private readonly ApplicationDbContext _context;
    public CustomerRepository(ApplicationDbContext context) : base(context) {}
}