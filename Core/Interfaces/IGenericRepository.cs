using Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace Core.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> GetByIdAsync(Guid id);
    IQueryable<T> GetQueryable();
}