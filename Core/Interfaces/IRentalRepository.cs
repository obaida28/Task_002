namespace Core.Interfaces;
public interface IRentalRepository : IGenericRepository<Rental>
{
    Task<bool> IsCarWasRentedAsync(string carNumber) ;
    Task<bool> IsDriverAvailableBetweenDatesAsync (Guid driverId , DateTime startDate, DateTime endDate ); 
    Task<bool> IsCarAvailableBetweenDatesAsync(Guid carId ,DateTime startDate, DateTime endDate);
}