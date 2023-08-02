using Core.Entites;
namespace Core.Interfaces;
public interface IGenericCache<T> where T : BaseEntity
{
    public void SetCache(List<T> data);
    public Task AddToCache(T entity);
    public Task UpdateCache(T entity);
    public Task DeleteFromCache(Guid id);
    public Task<IEnumerable<T>> GetAllFromCache();
}