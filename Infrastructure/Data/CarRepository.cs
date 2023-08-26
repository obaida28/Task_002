namespace Infrastructure.Data;
public class CarRepository : GenericRepository<Car> , ICarRepository
{
    public CarRepository(ApplicationDbContext context) : base(context) {}
    public async Task<bool> IsExistNumberAsync(string carNumber)
    {
        return await _context.Cars.AnyAsync(c => c.Number == carNumber);
    }
}