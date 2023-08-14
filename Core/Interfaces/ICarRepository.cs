using Core.Entites;

namespace Core.Interfaces;
public interface ICarRepository : IGenericRepository<Car>
{
    Task<bool> IsExistNumberAsync(string carNumber);
}