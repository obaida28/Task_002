namespace Infrastructure.Data;
public class RentalRepository : GenericRepository<Rental> , IRentalRepository
{
    public RentalRepository(ApplicationDbContext context) : base(context) {}
}