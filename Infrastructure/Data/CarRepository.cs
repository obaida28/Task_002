using Core.Entites;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Data;
public class CarRepository : GenericRepository<Car> , ICarRepository
{
    private readonly ApplicationDbContext _context;
    public CarRepository(ApplicationDbContext context) : base(context) {}
    public async Task<bool> IsExistAsync(string carNumber)
    {
        return await _context.Cars.AnyAsync(c => c.CarNumber == carNumber);
    }
}