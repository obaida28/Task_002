﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230814100310_edit_f.k_required")]
    partial class edit_fk_required
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Core.Entites.Car", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DailyRate")
                        .HasColumnType("int");

                    b.Property<decimal>("EngineCapacity")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("Core.Entites.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Customers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("87ef817e-7604-42d4-b6a0-1fe7711a6f0e"),
                            Name = "Customer1"
                        },
                        new
                        {
                            Id = new Guid("7806670b-dac7-4051-97e2-61e0a043b8a6"),
                            Name = "Customer2"
                        },
                        new
                        {
                            Id = new Guid("5801e7c9-2daa-4f91-9158-e2bbcda0bd0e"),
                            Name = "Customer3"
                        });
                });

            modelBuilder.Entity("Core.Entites.Driver", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SubstituteId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SubstituteId")
                        .IsUnique()
                        .HasFilter("[SubstituteId] IS NOT NULL");

                    b.ToTable("Drivers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("20b4da7b-2dfb-4758-8d1d-b18cfaf9a5b7"),
                            IsAvailable = true,
                            Name = "driver1"
                        },
                        new
                        {
                            Id = new Guid("ff11e306-41fe-4290-bffc-d8fc5fa05b84"),
                            IsAvailable = true,
                            Name = "driver2"
                        },
                        new
                        {
                            Id = new Guid("6ad5c888-778c-444d-9692-3768d0de7bb5"),
                            IsAvailable = true,
                            Name = "driver3"
                        });
                });

            modelBuilder.Entity("Core.Entites.Rental", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DailyRate")
                        .HasColumnType("int");

                    b.Property<Guid?>("DriverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("DriverId");

                    b.ToTable("Rentals");
                });

            modelBuilder.Entity("Core.Entites.Driver", b =>
                {
                    b.HasOne("Core.Entites.Driver", "Substitute")
                        .WithOne()
                        .HasForeignKey("Core.Entites.Driver", "SubstituteId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Substitute");
                });

            modelBuilder.Entity("Core.Entites.Rental", b =>
                {
                    b.HasOne("Core.Entites.Car", "Car")
                        .WithMany("Rentals")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Entites.Customer", "Customer")
                        .WithMany("Rentals")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Entites.Driver", "Driver")
                        .WithMany("Rentals")
                        .HasForeignKey("DriverId");

                    b.Navigation("Car");

                    b.Navigation("Customer");

                    b.Navigation("Driver");
                });

            modelBuilder.Entity("Core.Entites.Car", b =>
                {
                    b.Navigation("Rentals");
                });

            modelBuilder.Entity("Core.Entites.Customer", b =>
                {
                    b.Navigation("Rentals");
                });

            modelBuilder.Entity("Core.Entites.Driver", b =>
                {
                    b.Navigation("Rentals");
                });
#pragma warning restore 612, 618
        }
    }
}