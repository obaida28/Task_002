namespace Infrastructure.Data;
public class DriverRepository : GenericRepository<Driver> , IDriverRepository
{
    public DriverRepository(ApplicationDbContext context) : base(context) {}
    public async Task<bool> IsAvailableAsync(Guid id)
    {
        return await _context.Drivers.AnyAsync(d => d.Id == id && d.IsAvailable);
    }
    public async Task<Guid> GetSubtituteIdAsync(Guid id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        return (Guid)driver.SubstituteId;
    }
}