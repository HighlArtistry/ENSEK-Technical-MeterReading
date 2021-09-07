using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using MeterReadings1.Models;
using MeterReadings1.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadings1.Controllers
{
    [Route("api/[controller]")] //Defines path that controller will ...
    //[ApiController]
    public class AccountsController : Controller//Base
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Account>> GetAccounts()
        {
            return await _accountRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccounts(int id) // ActionResult provides ability to return multiple types like NootFound or BadRequest
        {
            return await _accountRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Account>> PostAccounts([FromBody] Account account) //Asp.NET ModelBinding will convert Json Payload in body into an Account object
        {
            var newAccount = await _accountRepository.Create(account);

            return CreatedAtAction(nameof(GetAccounts), new { id = newAccount.AccountId }, newAccount);
        }

        [HttpPut]
        public async Task<ActionResult<Account>> PutAccounts(int id, [FromBody] Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest();
            }
            else
            {
                await _accountRepository.Update(account);

                return NoContent();
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> DeleteAccounts(int id)
        {
            var accountToDelete = await _accountRepository.Get(id);
            if (accountToDelete == null)
            {
                return NotFound();
            }
            else
            {
                await _accountRepository.Delete(id);

                return NoContent();
            }
        }
    }
}
