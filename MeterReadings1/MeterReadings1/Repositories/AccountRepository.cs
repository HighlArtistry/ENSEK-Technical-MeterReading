using MeterReadings1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadings1.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        //This repository will query the db using the AccountContext

        private readonly AccountContext _context;

        public AccountRepository(AccountContext context)
        {
            _context = context;
        }

        public async Task<Account> Create(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task Delete(int id)
        {
            var accountToDelete = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> Get()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<Account> Get(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task Update(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
