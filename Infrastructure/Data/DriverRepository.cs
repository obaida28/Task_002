using Core.Entites;
using Core.Interfaces;
namespace Infrastructure.Data;
public class DriverRepository : GenericRepository<Driver> , IDriverRepository
{
    public DriverRepository(ApplicationDbContext context) : base(context) {}
}