namespace API.Mapper;
public class DriverProfile : Profile
{
    public DriverProfile()
    {
        CreateMap < DriverCreateDTO, Driver > ();
        // .ForMember(dest => dest.DriverName , opt => opt.MapFrom(src => src.Name));

        CreateMap < DriverUpdateDTO, Driver > ();
        //  .ForMember(dest => dest.DriverName , opt => opt.MapFrom(src => src.Name));

        CreateMap < Driver, DriverDTO>();
        // .ForMember(dest => dest.Name , opt => opt.MapFrom(src => src.DriverName));
        
        CreateMap<PagingResult<Driver> , PagingResult<DriverDTO>>();
    }
}