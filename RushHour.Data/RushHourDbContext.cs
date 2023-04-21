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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = Guid.NewGuid(), Email = "admin", FullName = "John Doe", Password = HashAdminPassword(), Role = Role.Admin });

            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Employees)
                .WithOne(e => e.Provider)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }

        private string HashAdminPassword()
        {
            return BCrypt.Net.BCrypt.HashPassword("Password123+");
        }
    }
}
