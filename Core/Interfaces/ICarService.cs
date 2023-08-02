using Core.Entites;
using Core.ViewModels;

namespace Core.Interfaces;
public interface ICarService
{
    Task AddCar(Car car);
    Task<bool> IsExist(Guid id);
    Task UpdateCar(Car car);
    Task DeleteCar(Guid id);
    Task<List<Car>> GetAllCars();
    Task<Car> GetCarById(Guid id);
    List<Car> Getfilter(CarFilter dto);
    Task<IEnumerable<Car>> GetCarsByCache();
}