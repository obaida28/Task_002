using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;

namespace API.Mapper;
public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap < CustomerCreateDTO, Customer > ()
        .ForMember(dest => dest.CustomerName , opt => opt.MapFrom(src => src.Name));

        CreateMap < CustomerUpdateDTO, Customer > ()
         .ForMember(dest => dest.CustomerName , opt => opt.MapFrom(src => src.Name));

        CreateMap<Customer, CustomerDTO>()
        .ForMember(dest => dest.Name , opt => opt.MapFrom(src => src.CustomerName));
        
        CreateMap<PagingResult<Customer> , PagingResult<CustomerDTO>>();
    }
}