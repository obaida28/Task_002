namespace API.Mapper;
public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap < CarCreateDto, Car > ();
            // .ForMember(dest => dest.Number  ,opt => opt.MapFrom(src => src.Number));

        CreateMap < CarUpdateDto, Car > ();
            // .ForMember(dest => dest.Number , opt => opt.MapFrom(src => src.Number));

        CreateMap<Car, CarDTO> ();
            // .ForMember(dest => dest.Number  ,opt => opt.MapFrom(src => src.Number));

        CreateMap<PagingResult<Car> , PagingResult<CarDTO>>();
    }
}