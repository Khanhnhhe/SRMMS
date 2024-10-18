using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [ApiController]
    [Route("api")]
    public class CustomerLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SRMMSContext _context;

        public CustomerLoginController(IConfiguration configuration, SRMMSContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("loginCustomer")]
        public IActionResult LoginCustomer([FromBody] CustomerLoginDTO model)
        {

            var customer = _context.Customers
                .FirstOrDefault(c => c.CusPhone == model.CusPhone);

            if (customer == null)
            {
                return Unauthorized("Customer not found");
            }


            if (!VerifyPassword(model.CusPassword, customer.CusPassword))
            {
                return Unauthorized("Invalid password");
            }

            var token = GenerateJwtToken(customer.CusPhone.ToString());

            return Ok(new
            {
                token = token,
                phone = customer.CusPhone,
                fullname = customer.CusFullname
            });
        }


        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
 
            return enteredPassword == storedPassword;
        }

        private string GenerateJwtToken(string customerPhone)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customerPhone),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("changeCustomerPassword")]
        public IActionResult ChangeCustomerPassword([FromBody] ChangeCustomerPasswordDTO model)
        {

            var customer = _context.Customers
                .FirstOrDefault(c => c.CusPhone == model.CusPhone);

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            if (!VerifyPassword(model.OldPassword, customer.CusPassword))
            {
                return BadRequest("Old password is incorrect");
            }

            customer.CusPassword = model.NewPassword;

            _context.Customers.Update(customer);
            _context.SaveChanges();

            return Ok("Password changed successfully");
        }
    


    [HttpGet("listCustomer")]
        public IActionResult GetCustomerList()
        {
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }

        [HttpDelete("deleteCustomer/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CusId == id);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return Ok(new { message = "Customer deleted successfully." });
        }
    }
}
