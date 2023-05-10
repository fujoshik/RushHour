using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Domain.Enums;

namespace RushHour.Data
{
    public class RushHourDbContext : DbContext
    {
        public RushHourDbContext()
            : base() { }
        public RushHourDbContext(DbContextOptions<RushHourDbContext> options) 
            : base(options) { }

        public DbSet<Provider> Providers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityEmployee> ActivityEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = Guid.NewGuid(), Email = "admin", FullName = "John Doe", Password = HashAdminPassword(), Role = Role.Admin });

            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Employees)
                .WithOne(e => e.Provider)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Activities)
                .WithOne(a => a.Provider)
                .HasForeignKey(a => a.ProviderId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Activities)
                .WithMany(a => a.Employees)
                .UsingEntity<ActivityEmployee>();

            modelBuilder.Entity<ActivityEmployee>()
                .HasKey(a => new { a.ActivityId, a.EmployeeId });
        }

        private string HashAdminPassword()
        {
            return BCrypt.Net.BCrypt.HashPassword("Password123+");
        }
    }
}
