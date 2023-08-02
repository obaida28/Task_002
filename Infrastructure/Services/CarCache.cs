using Core.Entites;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;
public class CarCache : GenericCache<Car> , ICarCache
{
    public CarCache(IMemoryCache cache , ICarRepository repository) : base(cache , repository) {}
}