using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [ApiController]
    [Route("api")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SRMMSContext _context;

        public LoginController(IConfiguration configuration, SRMMSContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO model)
        {

            var user = _context.Employees
                .Include(e => e.EmpRole)
                .FirstOrDefault(u => u.EmpEmail == model.EmpEmail);


            if (user == null)
            {
                return Unauthorized("User not found");
            }


            if (!VerifyPassword(model.EmpPassword, user.EmpPassword))
            {
                return Unauthorized("Invalid password");
            }

            // Create token 
            var token = GenerateJwtToken(user.EmpEmail, user.EmpRole.RoleName);

            return Ok(new
            {
                token = token,
                email = user.EmpEmail,
                roleName = user.EmpRole.RoleName,
                EmpName = user.EmpFirstName, EmpLastName = user.EmpLastName
            });
        }

        // check passwork
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {

            return enteredPassword == storedPassword;
        }


        private string GenerateJwtToken(string username, string roleName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roleName)
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

        [HttpPost("Change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var user = _context.Employees
                .FirstOrDefault(u => u.EmpEmail == model.EmpEmail);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!VerifyPassword(model.OldPassword, user.EmpPassword))
            {
                return BadRequest("Old password is incorrect");
            }

            user.EmpPassword = model.NewPassword;

            _context.Employees.Update(user);
            _context.SaveChanges();

            return Ok("Password changed successfully");
        }
    }
}
