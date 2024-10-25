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
    [ApiController]
    [Route("api/customer")]
    public class CustomerLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SRMMSContext _context;

        public CustomerLoginController(IConfiguration configuration, SRMMSContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("searchAccountName")]
        public async Task<ActionResult<IEnumerable<ListAccountDTO>>> SearchByAccountName(string? accountName = "", int pageNumber = 1, int pageSize = 10)
        {
            var skip = (pageNumber - 1) * pageSize;

            
            var query = _context.Accounts.Include(a => a.Role).AsQueryable();

         
            if (!string.IsNullOrWhiteSpace(accountName))
            {
                query = query.Where(a => a.FullName.Contains(accountName));
            }

            var accounts = await query
                                 .Skip(skip)
                                 .Take(pageSize)
                                 .Select(a => new ListAccountDTO
                                 {
                                     AccountId = a.AccId,
                                     FullName = a.FullName,
                                     Email = a.Email,
                                     Phone = a.Phone,
                                     RoleName = a.Role.RoleName
                                 }).ToListAsync();

            if (accounts == null || !accounts.Any())
            {
                return NotFound("No accounts found.");
            }

            return Ok(accounts);
        }

        [HttpPost("createEmployeeAccount")]
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
                RoleId = model.RoleId
            };


            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Created("Account created successfully.", account);
        }







        [HttpGet("list")]
        public IActionResult GetCustomerList()
        {
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var customers = _context.Accounts.ToList();
            return Ok(customers);
        }

        [HttpDelete("delete/{id}")]
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
    }
}
