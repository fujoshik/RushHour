using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Data
{
    public class RushHourDbContext : DbContext
    {
        public RushHourDbContext()
            : base() { }

        public RushHourDbContext(DbContextOptions<RushHourDbContext> options) 
            : base(options) { }

        public DbSet<Provider> Providers { get; set; }
    }
}
