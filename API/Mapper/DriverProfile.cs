using API.DTOs;
using AutoMapper;
using Core.Entites;

namespace API.Mapper;
public class DriverProfile : Profile
{
    public DriverProfile()
    {
        CreateMap < DriverCreateDTO, Driver > ();
        CreateMap < DriverUpdateDTO, Driver > ();
        CreateMap < Driver, DriverDTO>();
    }
}