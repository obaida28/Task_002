using Core.Entites;
using Core.Interfaces;
namespace Infrastructure.Data;
public class DriverRepository : GenericRepository<Driver> , IDriverRepository
{
    private readonly ApplicationDbContext _context;
    public DriverRepository(ApplicationDbContext context) : base(context) {}
}