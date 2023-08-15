using Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace Core.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T> GetByIdAsync(Guid id);
    IQueryable<T> GetQueryable();
    Task<bool> IsExistAsync(Guid id);
}