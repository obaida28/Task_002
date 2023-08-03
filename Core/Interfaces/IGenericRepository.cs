using Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace Core.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    void Add(T entity);
    void Update(T entity);
    Task<T> DeleteAsync(Guid id);
    Task<T> GetByIdAsync(Guid id);
    IQueryable<T> GetAll();
    // DbSet<T> GetAllBeforeExecute();
    Task<bool> IsExistAsync(Guid id);
}