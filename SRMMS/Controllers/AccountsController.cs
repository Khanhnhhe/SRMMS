using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/Accounts")]
    [ApiController]
    public class CustomerLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SRMMSContext _context;

        public CustomerLoginController(IConfiguration configuration, SRMMSContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpGet("/api/account/list")]
        public async Task<ActionResult> SearchByAccount(int? id = null, string? accountName = "", string? phone = "", int pageNumber = 1, int pageSize = 10)
        {
            if (id.HasValue && id > 0)
            {
                var account = await _context.Accounts
                    .Where(a => a.AccId == id.Value)
                    .Select(a => new
                    {
                        a.AccId,
                        a.FullName,
                        a.Email,
                        a.Phone,
                        a.RoleId,
                        a.Status
                    })
                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }

                return Ok(account);
            }

            var employeeCount = await _context.Accounts.CountAsync(a => a.RoleId == 2 || a.RoleId == 3 || a.RoleId == 4);
            var customerCount = await _context.Accounts.CountAsync(a => a.RoleId == 5);

            var skip = (pageNumber - 1) * pageSize;
            var query = _context.Accounts.Include(a => a.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(accountName))
            {
                var trimmedAccountName = accountName.Trim();
                query = query.Where(a => a.FullName.Contains(trimmedAccountName));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var trimmedPhone = phone.Trim();
                query = query.Where(a => a.Phone.Contains(trimmedPhone));
            }

            var accounts = await query
                                 .Skip(skip)
                                 .Take(pageSize)
                                 .Select(a => new ListAccountDTO
                                 {
                                     AccountId = a.AccId,
                                     FullName = a.FullName ?? "",
                                     Email = a.Email ?? "",
                                     Phone = a.Phone ?? "",
                                     RoleName = a.Role.RoleName ?? "",
                                     Status = a.Status
                                 }).ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalEmployees = employeeCount,
                TotalCustomers = customerCount,
                Accounts = accounts
            });
        }
        [HttpGet("/api/account/getByID")]
        public async Task<ActionResult> GetAccountById(int id)
        {
            var account = await _context.Accounts
                .Where(a => a.AccId == id)
                .Select(a => new
                {
                    a.AccId,
                    a.FullName,
                    a.Email,
                    a.Phone,
                    a.RoleId,
                    a.Status,
                    RoleName = a.Role.RoleName
                })
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            return Ok(account);
        }



        [HttpPost("/api/account/create")]
        public async Task<IActionResult> CreateEmployeeAccount([FromBody] CreateEmployeeAccountDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Invalid account data.");
            }

            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == model.Email);
            if (existingAccount != null)
            {
                return Conflict("Email already exists.");
            }

            var account = new Account
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password, 
                Phone = model.Phone,
                RoleId = model.RoleId,
                Status = model.Status
            };


            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Created("Account created successfully.", account);
        }



        [HttpPut("/api/account/update/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDTO model)
        {
            if (model == null || id <= 0)
            {
                return BadRequest("Invalid account data.");
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            account.FullName = model.FullName ?? account.FullName;
            account.Email = model.Email ?? account.Email;
            account.Phone = model.Phone ?? account.Phone;
            account.RoleId = model.RoleId ?? account.RoleId;
            if (model.Status.HasValue)
            {
                account.Status = model.Status.Value;
            }

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Account updated successfully.", account });
        }




        [HttpDelete("/api/account/delete/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Accounts.FirstOrDefault(c => c.AccId == id);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

            _context.Accounts.Remove(customer);
            _context.SaveChanges();

            return Ok(new { message = "Customer deleted successfully." });
        }

        [HttpGet("/api/account/total")]
        public async Task<IActionResult> GetAccountTotals()
        {
            
            var employeeCount = await _context.Accounts
                .CountAsync(a => a.RoleId == 2 || a.RoleId == 3 || a.RoleId == 4);

            
            var customerCount = await _context.Accounts
                .CountAsync(a => a.RoleId == 5);

            return Ok(new
            {
                EmployeeCount = employeeCount,
                CustomerCount = customerCount
            });
        }

    }
}
