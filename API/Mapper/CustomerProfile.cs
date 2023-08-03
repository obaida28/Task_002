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
        CreateMap<Customer, CustomerDTO>();
    }
}