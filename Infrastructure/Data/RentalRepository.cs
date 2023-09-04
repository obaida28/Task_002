using Core.Extensions;

namespace Infrastructure.Data;
public class RentalRepository : GenericRepository<Rental> , IRentalRepository
{
    public RentalRepository(ApplicationDbContext context) : base(context) {}
    public async Task<bool> IsCarWasRentedAsync(string carNumber)
    {
        return await _context.Rentals.AnyAsync(c => c.Car.Number == carNumber);
    }
    public async Task<bool> IsDriverAvailableBetweenDatesAsync
        (Guid driverId , DateTime startDate, DateTime endDate )
    {
        var query = _context.Rentals.Include(r => r.Driver);
        var isNotAvailableDriver = await query.AnyAsync
        (r => r.State.In("Created","Active") && r.DriverId == driverId &&  ( 
            startDate.DateBetween(r.StartDate,r.EndDate) || endDate.DateBetween(r.StartDate,r.EndDate))
        );
        return !isNotAvailableDriver; 
    }
       public async Task<bool> IsCarAvailableBetweenDatesAsync(Guid carId ,DateTime startDate, DateTime endDate)
    {
        var query = _context.Rentals.Include(r => r.Car);
        var isNotAvailableCar = await query.AnyAsync
        (r => r.State.In("Created","Active") && r.CarId == carId &&  ( 
            startDate.DateBetween(r.StartDate,r.EndDate) || endDate.DateBetween(r.StartDate,r.EndDate))
        );
        return !isNotAvailableCar; 
    }
}