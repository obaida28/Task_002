var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddMemoryCache();
// builder.Services.AddScoped<ICarRepository, CarRepository>();
// builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
// builder.Services.AddScoped<IDriverRepository, DriverRepository>();
// builder.Services.AddScoped<ICarCache, CarCache>();
// builder.Services.AddScoped<ICarService, CarService>();
// builder.Services.AddScoped(typeof(IGenericCache<>), typeof(GenericCache<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});
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
builder.Services.AddSwaggerGen(
    c => 
    {
        c.SwaggerDoc("v1",new OpenApiInfo
        {
            Title = "Rental" , Version = "v1"
        });
        c.AddSecurityDefinition("Bearer" , new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please insert token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
            }
        });
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseExceptionHandler("/error/500");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
// app.UseMiddleware<ErrorWrappingMiddleware>();
app.MapControllers();

app.Run();
