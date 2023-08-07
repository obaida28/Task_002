using API.DTOs;
using AutoMapper;
using Core.Entites;

namespace API.Mapper;
public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap < CarCreateDto, Car > ()
        .ForMember(
            dest => dest.Rentals  ,opt => opt.MapFrom(src => 
                new List<Rental> { 
                    new Rental { CustomerId = (Guid)src.CustomerId , 
                    DriverId = src.DriverId } }
            ));

        CreateMap < CarUpdateDto, Car > ()
        .ForMember(dest => dest.Id , opt => opt.MapFrom(src => src.CarId));
        // .ForMember(dest => dest.CarNumber , opt => opt.MapFrom(src => 
        // )
        // .ForMember(
        //     dest => dest.Rentals  ,opt => opt.MapFrom(src => 
        //         new List<Rental> { 
        //             new Rental { CustomerId = (Guid)src.CustomerId , 
        //             DriverId = src.DriverId } }
        //     ));

         CreateMap<Car, CarDTO>()
            .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => getDrivers(src)))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => getCustomers(src)));
    }
    private IEnumerable<string> getDrivers(Car list)
    {
        if(list.Rentals.Any())
            return list.Rentals.Where(r => r.Driver.IsAvailable).Select(a => a.Driver.DriverName);
        IEnumerable<string> res = new[] { "No Driver" };
        return res;
    }
    private IEnumerable<string> getCustomers(Car list)
    {
        if(list.Rentals.Any())
            return list.Rentals.Select(a => a.Customer.CustomerName);
        IEnumerable<string> res = new[] { "No Customer" };
        return res;
    }
}