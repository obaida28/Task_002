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
            .HasForeignKey(r => r.DriverId);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.Substitute)
            .WithOne()
            .HasForeignKey<Driver>(d => d.SubstituteDriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Driver>().HasData(
            new Driver { DriverName = "driver1" } ,
            new Driver { DriverName = "driver2" } ,
            new Driver { DriverName = "driver3" }
        );
        modelBuilder.Entity<Customer>().HasData(
            new Customer { CustomerName = "Customer1" } ,
            new Customer { CustomerName = "Customer2" } ,
            new Customer { CustomerName = "Customer3" }
        );
    }
    public virtual DbSet<Car> Cars {get; set;}
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Driver> Drivers { get; set; }
    public virtual DbSet<Rental> Rentals { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
        optionsBuilder.UseLazyLoadingProxies();
}