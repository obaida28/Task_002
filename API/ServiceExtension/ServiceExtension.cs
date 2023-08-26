namespace API.ServiceExtension;
public static class ServiceExtension
{
    public static IServiceCollection AddDIServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("LocalConnection"));
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();

        return services;
    }
}