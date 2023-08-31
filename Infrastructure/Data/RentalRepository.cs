namespace Infrastructure.Data;
public class RentalRepository : GenericRepository<Rental> , IRentalRepository
{
    public RentalRepository(ApplicationDbContext context) : base(context) {}
    public async Task<bool> IsCarWasRentedAsync(string carNumber)
    {
        return await _context.Rentals.AnyAsync(c => c.Car.Number == carNumber);
    }
}