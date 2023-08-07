using API.DTOs;
using AutoMapper;
using Core.Entites;

namespace API.Mapper;
public class DriverProfile : Profile
{
    public DriverProfile()
    {
        CreateMap < DriverCreateDTO, Driver > ();
        CreateMap < DriverUpdateDTO, Driver > ()
         .ForMember(dest => dest.Id , opt => opt.MapFrom(src => src.DriverId));
        CreateMap < Driver, DriverDTO>();
    }
}