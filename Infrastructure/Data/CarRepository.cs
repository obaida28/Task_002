using Core.Entites;
using Core.Interfaces;
namespace Infrastructure.Data;
public class CarRepository : GenericRepository<Car> , ICarRepository
{
    private readonly ApplicationDbContext _context;
    public CarRepository(ApplicationDbContext context) : base(context) {}
}