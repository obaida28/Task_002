using Core.Entites;

namespace Core.Interfaces;
public interface IDriverRepository : IGenericRepository<Driver>
{
    Task<bool> IsAvailableAsync(Guid id);
    Task<Guid> GetSubtituteIdAsync(Guid id);
}