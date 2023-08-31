namespace Core.Interfaces;
public interface IRentalRepository : IGenericRepository<Rental>
{
    Task<bool> IsCarWasRentedAsync(string carNumber) ;
}