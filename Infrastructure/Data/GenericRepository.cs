using Core.Entites;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data;
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context ;
    public GenericRepository(ApplicationDbContext context) => _context = context;
    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public async Task<T> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if(entity == null) return entity;
        _context.Set<T>().Remove(entity);
        return entity;
    }

    public IQueryable<T> GetAll()
    {
        return _context.Set<T>().AsQueryable();
    }

    //public DbSet<T> GetAllBeforeExecute() =>  _context.Set<T>();

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        return await _context.Set<T>().AnyAsync(e => e.Id == id);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}