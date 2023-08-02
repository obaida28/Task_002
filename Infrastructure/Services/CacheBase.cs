using Core.Entites;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;
public class GenericCache<T> : IGenericCache<T> where T : BaseEntity
{
    private readonly IMemoryCache _cache;
    private readonly IGenericRepository<T> _repository;
    private readonly string carCacheName = "cars";
    public GenericCache(IMemoryCache cache , IGenericRepository<T> repository) 
    {
        _cache = cache;
        _repository = repository;
    }
    public async Task AddToCache(T entity)
    {
        var getData = await GetAllFromCache();
        var listData = getData.ToList();
        if (!listData.IsEmpty())
        {
            listData.Add(entity);
            SetCache(listData);
        }
    }
    public async Task UpdateCache(T entity)
    {
        var getData = await GetAllFromCache();
        var listData = getData.ToList();
        if (!listData.IsEmpty())
        {
            var oldEntity = listData.Find(c => c.Id == entity.Id);
            listData.Remove(oldEntity);
            listData.Add(entity);
            SetCache(listData);
        }
    }
    public async Task DeleteFromCache(Guid id)
    {
        var getData = await GetAllFromCache();
        var listData = getData.ToList();
        if (!listData.IsEmpty())
        {
            var oldCar = listData.Find(c => c.Id == id);
            listData.Remove(oldCar);
            SetCache(listData);
        }
    }
    public async Task<IEnumerable<T>> GetAllFromCache()
    {
        if (_cache.TryGetValue(carCacheName, out IEnumerable<T> data)) return data;
        var dataFromStore = await _repository.GetAll();
        SetCache(dataFromStore);
        return dataFromStore;
    }
    public void SetCache(List<T> data) => _cache.Set(carCacheName, data, TimeSpan.FromHours(1));
}