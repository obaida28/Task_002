using Core.Entites;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data;
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context ;
    public GenericRepository(ApplicationDbContext context) => _context = context;
    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        //await _context.SaveChangesAsync();
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        //await _context.SaveChangesAsync();
    }

    public IQueryable<T> GetQueryable()
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
        //await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        return await _context.Set<T>().AnyAsync(e => e.Id == id);
    }
}