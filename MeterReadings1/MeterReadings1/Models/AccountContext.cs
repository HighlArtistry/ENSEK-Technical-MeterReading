using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadings1.Models
{
    public class AccountContext : DbContext
    {
        // This Context object allows querying and setting of data, required by EntityFrameworkCore
        public AccountContext(DbContextOptions<AccountContext> options)
            :base(options)
        {
            Database.EnsureCreated(); // Make sure db is created when using dbContext
        }
        public DbSet<Account> Accounts { get; set; } // Collection of accounts
    }
}
