using Core.Entites;

namespace Core.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    void Add(T entity);
    void Update(T entity);
    Task<T> Delete(Guid id);
    Task<T> GetById(Guid id);
    Task<List<T>> GetAll();
    Task<bool> IsExist(Guid id);
}