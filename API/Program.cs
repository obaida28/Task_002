using System.Text.Json.Serialization;
using Core.Interfaces;
using Infrastructure.Data;
using API.ServiceExtension;
using Microsoft.AspNetCore.Mvc;
// using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddMemoryCache();
// builder.Services.AddScoped<ICarRepository, CarRepository>();
// builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
// builder.Services.AddScoped<IDriverRepository, DriverRepository>();
// builder.Services.AddScoped<ICarCache, CarCache>();
// builder.Services.AddScoped<ICarService, CarService>();
// builder.Services.AddScoped(typeof(IGenericCache<>), typeof(GenericCache<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(Program));
// builder.Services.Configure<ApiBehaviorOptions>(options => {    
// options.SuppressModelStateInvalidFilter = true;  });
builder.Services.AddDIServices(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// إيقاف الرد التلقائي 400 للـ ModalState
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler("/error/500");
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ErrorWrappingMiddleware>();
app.MapControllers();

app.Run();
