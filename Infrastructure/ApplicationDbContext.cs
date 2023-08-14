using Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class ApplicationDbContext : DbContext 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Rental>()
        //     .HasKey(r => new { r.CarNumber , r.CustomerId });

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Driver)
            .WithMany(d => d.Rentals)
            .IsRequired(false)
            .HasForeignKey(r => r.DriverId);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.Substitute)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey<Driver>(d => d.SubstituteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Driver>().HasData(
            new Driver { Name = "driver1" } ,
            new Driver { Name = "driver2" } ,
            new Driver { Name = "driver3" }
        );
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Name = "Customer1" } ,
            new Customer { Name = "Customer2" } ,
            new Customer { Name = "Customer3" }
        );
    }
    public virtual DbSet<Car> Cars {get; set;}
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Driver> Drivers { get; set; }
    public virtual DbSet<Rental> Rentals { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
        optionsBuilder.UseLazyLoadingProxies();
}