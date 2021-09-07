using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadings1.Models
{
    public class MeterReadingContext : DbContext
    {
        // This Context object allows querying and setting of data, required by EntityFrameworkCore
        public MeterReadingContext(DbContextOptions<MeterReadingContext> options)
            :base(options)
        {
            Database.EnsureCreated(); // Make sure db is created when using dbContext
        }
        public DbSet<MeterReading> MeterReadings { get; set; } // Collection of accounts
    }
}
