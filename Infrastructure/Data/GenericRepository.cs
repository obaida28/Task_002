using Core.Entites;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context ;
    public GenericRepository(ApplicationDbContext context) => _context = context;
    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public async Task<T> Delete(Guid id)
    {
        var entity = await GetById(id);
        if(entity == null) return entity;
        _context.Set<T>().Remove(entity);
        return entity;
    }

    public async Task<List<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public DbSet<T> GetAllBeforeExecute() =>  _context.Set<T>();

    public async Task<T> GetById(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<bool> IsExist(Guid id)
    {
        return GetById(id) != null;
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}