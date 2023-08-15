using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;

namespace API.Mapper;
public class RentalProfile : Profile
{
    public RentalProfile()
    {
       CreateMap<RentalCreateDto, Rental>();
            //.ForMember(dest => dest.IsValidPeriod, opt => opt.Ignore());

        CreateMap < RentalUpdateDTO, Rental > ()
            .ForMember(dest => dest.IsActive , opt => opt.Ignore());
            // .ForMember(dest => dest. , opt => opt.Ignore())


        CreateMap<Rental, RentalDTO> ()
            .ForMember(dest => dest.CarNumber  ,opt => opt.MapFrom(src => src.Car.Number))
            .ForMember(dest => dest.CarType  ,opt => opt.MapFrom(src => src.Car.Type))
            .ForMember(dest => dest.CustomerName  ,opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.DriverName  ,opt => opt.MapFrom(src => src.Driver.Name));

        CreateMap<PagingResult<Rental> , PagingResult<RentalDTO>>();
    }
}