using MeterReadings1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadings1.Repositories
{
    public interface IAccountRepository
    {
        // Retrieve All Accounts
        Task<IEnumerable<Account>> Get();
        // Retrieve a single Account
        Task<Account> Get(int id);
        // Create a new Account
        Task<Account> Create(Account account);
        // Update a Account
        Task Update(Account account);
        // Delete a Account
        Task Delete(int id);
    }
}
