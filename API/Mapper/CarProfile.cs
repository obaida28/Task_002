using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;

namespace API.Mapper;
public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap < CarCreateDto, Car > ()
            .ForMember(dest => dest.CarNumber  ,opt => opt.MapFrom(src => src.Number));

        CreateMap < CarUpdateDto, Car > ()
            .ForMember(dest => dest.CarNumber , opt => opt.MapFrom(src => src.Number));

        CreateMap<Car, CarDTO> ()
            .ForMember(dest => dest.Number  ,opt => opt.MapFrom(src => src.CarNumber));

        CreateMap<PagingResult<Car> , PagingResult<CarDTO>>();
    }
}