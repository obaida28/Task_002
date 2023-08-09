using API.DTOs;
using AutoMapper;
using Core.Entites;

namespace API.Mapper;
public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap < CustomerCreateDTO, Customer > ();
        CreateMap < CustomerUpdateDTO, Customer > ();
        //  .ForMember(dest => dest.Id , opt => opt.MapFrom(src => src.CustomerId));
        CreateMap<Customer, CustomerDTO>();
    }
}