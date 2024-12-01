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
        [HttpGet("/api/account/list/customers")]
        public async Task<ActionResult> GetCustomers(string? accountName = "", string? phone = "", int pageNumber = 1, int pageSize = 10)
        {
            var customerCount = await _context.Accounts.CountAsync(a => a.RoleId == 5);

            var skip = (pageNumber - 1) * pageSize;
            var query = _context.Accounts
                                .Include(a => a.Role)
                                .Where(a => a.RoleId == 5)
                                .AsQueryable();

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
                                     Phone = a.Phone ?? "",
                                     Email = a.Email ?? "",
                                     RoleName = a.Role.RoleName ?? "",
                                     RoleId = a.RoleId,
                                     Status = a.Status,
                                     StartDate = a.StartDate,
                                     EndDate = a.EndDate
                                 }).ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCustomers = customerCount,
                Accounts = accounts
            });
        }


        [HttpGet("/api/account/list/employees")]
        public async Task<ActionResult> GetEmployees(string? accountName = "", string? phone = "", int? roleId = null, int pageNumber = 1, int pageSize = 10)
        {
            var employeeCount = await _context.Accounts.CountAsync(a => a.RoleId == 2 || a.RoleId == 3 || a.RoleId == 4);

            var skip = (pageNumber - 1) * pageSize;
            var query = _context.Accounts
                                .Include(a => a.Role)
                                .Where(a => a.RoleId == 2 || a.RoleId == 3 || a.RoleId == 4)
                                .AsQueryable();

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

            if (roleId.HasValue && (roleId == 2 || roleId == 3 || roleId == 4))
            {
                query = query.Where(a => a.RoleId == roleId.Value);
            }

            var accounts = await query
                                 .Skip(skip)
                                 .Take(pageSize)
                                 .Select(a => new ListAccountDTO
                                 {
                                     AccountId = a.AccId,
                                     FullName = a.FullName ?? "",
                                     Phone = a.Phone ?? "",
                                     Email = a.Email ?? "",
                                     RoleName = a.Role.RoleName ?? "",
                                     RoleId = a.RoleId,
                                     Status = a.Status,
                                     StartDate = a.StartDate,
                                     EndDate = a.EndDate
                                 }).ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalEmployees = employeeCount,
                Accounts = accounts
            });
        }




        [HttpGet("/api/account/getByID/{id}")]
        public async Task<ActionResult> GetAccountById(int id)
        {
            var account = await _context.Accounts
                .Where(a => a.AccId == id)
                .Include(a => a.PointLists)
                .Select(a => new
                {
                    a.AccId,
                    a.FullName,
                    a.Phone,
                    a.RoleId,
                    a.Status,
                    a.StartDate,
                    a.EndDate,
                    RoleName = a.Role.RoleName,
                    TotalPoints = a.RoleId == 5 ? a.PointLists.Sum(p => p.NumberPonit) : null 
                })
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return Ok(new { message = "Không tìm thấy tài khoản." });
            }

            if (account.RoleId == 5 && account.TotalPoints == null)
            {
                return Ok(new { Message = $"Tài khoản có ID {id} không phải là của khách hàng." });
            }

            return Ok(account);
        }




        [HttpPost("/api/account/create")]
        public async Task<IActionResult> CreateEmployeeAccount([FromBody] CreateEmployeeAccountDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Dữ liệu tài khoản không hợp lệ.");
            }

            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Phone == model.Phone);
            if (existingAccount != null)
            {
                return BadRequest("Số điện thoại đã tồn tại.");
            }

            var account = new Account
            {
                FullName = model.FullName,
                Password = model.Password, 
                Phone = model.Phone,
                RoleId = model.RoleId,
                Status = model.Status,
                StartDate = DateTime.Now
            };


            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Created("Tài khoản đã được tạo thành công.", account);
        }



        [HttpPut("/api/account/update/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDTO model)
        {
            if (model == null || id <= 0)
            {
                return BadRequest("Dữ liệu tài khoản không hợp lệ.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account.Status == true && model.Status == false)
            {
                account.EndDate = DateTime.Now; 
            }
            else if (account.Status == false && model.Status == true)
            {
                account.EndDate = null;
            }
            else if (account.Status == false && model.Status == true)
            {
                account.EndDate = null; 
            }
            account.FullName = model.FullName ?? account.FullName;
            account.Phone = model.Phone ?? account.Phone;
            account.RoleId = model.RoleId ?? account.RoleId;



            if (model.Status.HasValue)
            {
                account.Status = model.Status.Value;
            }

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Tài khoản đã được cập nhật thành công.",
                data = account
            });
        }


        [HttpDelete("/api/account/delete/{id}")]
        public IActionResult DeleteCustomer(int id)
        {

            var customer = _context.Accounts
                .Include(a => a.Feedbacks) 
                .Include(a => a.PointLists) 
                .FirstOrDefault(c => c.AccId == id);

            if (customer == null)
            {
                return BadRequest(new { message = "Customer not found." });
            }

            _context.Feedbacks.RemoveRange(customer.Feedbacks);

            _context.PointLists.RemoveRange(customer.PointLists);

            _context.Accounts.Remove(customer);

            _context.SaveChanges();

            return Ok(new { message = "Đã xóa tài khoản khách hàng cùng với các dữ liệu liên quan thành công." });
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
