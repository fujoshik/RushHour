﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RushHour.Data;

#nullable disable

namespace RushHour.Data.Migrations
{
    [DbContext(typeof(RushHourDbContext))]
    [Migration("20230522125136_AddProviderWorkingDaysTable")]
    partial class AddProviderWorkingDaysTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RushHour.Data.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5830df6d-ca4d-4f80-9f0b-cdb2b4a206f1"),
                            Email = "admin",
                            FullName = "John Doe",
                            Password = "$2a$11$8/lJQu2hoJln8Ilp6/9hFeRLtNAPKonUDtYmwV6vXl.8UmiQZLsRi",
                            Role = 0
                        });
                });

            modelBuilder.Entity("RushHour.Data.Entities.Activity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("ProviderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("RushHour.Data.Entities.ActivityEmployee", b =>
                {
                    b.Property<Guid>("ActivityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ActivityId", "EmployeeId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("ActivityEmployees");
                });

            modelBuilder.Entity("RushHour.Data.Entities.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("RushHour.Data.Entities.AppointmentActivity", b =>
                {
                    b.Property<Guid>("AppointmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ActivityId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AppointmentId", "ActivityId");

                    b.HasIndex("ActivityId");

                    b.ToTable("AppointmentActivities");
                });

            modelBuilder.Entity("RushHour.Data.Entities.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("RushHour.Data.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("HireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProviderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("RatePerHour")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ProviderId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("RushHour.Data.Entities.Provider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BusinessDomain")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Website")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name", "BusinessDomain")
                        .IsUnique();

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("RushHour.Data.Entities.ProviderWorkingDays", b =>
                {
                    b.Property<Guid>("ProviderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DayOfTheWeek")
                        .HasColumnType("int");

                    b.HasKey("ProviderId", "DayOfTheWeek");

                    b.ToTable("ProviderWorkingDays");
                });

            modelBuilder.Entity("RushHour.Data.Entities.Activity", b =>
                {
                    b.HasOne("RushHour.Data.Entities.Provider", "Provider")
                        .WithMany("Activities")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("RushHour.Data.Entities.ActivityEmployee", b =>
                {
                    b.HasOne("RushHour.Data.Entities.Activity", null)
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RushHour.Data.Entities.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RushHour.Data.Entities.Appointment", b =>
                {
                    b.HasOne("RushHour.Data.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RushHour.Data.Entities.Employee", "Employee")
                        .WithOne()
                        .HasForeignKey("RushHour.Data.Entities.Appointment", "EmployeeId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("RushHour.Data.Entities.AppointmentActivity", b =>
                {
                    b.HasOne("RushHour.Data.Entities.Activity", null)
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RushHour.Data.Entities.Appointment", null)
                        .WithMany()
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RushHour.Data.Entities.Client", b =>
                {
                    b.HasOne("RushHour.Data.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("RushHour.Data.Entities.Employee", b =>
                {
                    b.HasOne("RushHour.Data.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RushHour.Data.Entities.Provider", "Provider")
                        .WithMany("Employees")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("RushHour.Data.Entities.Provider", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("Employees");
                });
#pragma warning restore 612, 618
        }
    }
}
