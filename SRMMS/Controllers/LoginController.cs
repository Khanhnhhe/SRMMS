using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            var user = _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefault(a => a.Email == model.Email);

            if (user == null)
            {
                return Unauthorized("User not found");
            }

            if (!VerifyPassword(model.Password, user.Password))
            {
                return Unauthorized("Invalid password");
            }

            // Generate token
            var token = GenerateJwtToken(user.Email, user.Role?.RoleName);

            return Ok(new
            {
                token = token,
                email = user.Email,
                roleName = user.Role?.RoleName,
                fullName = user.FullName
            });
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }

        private string GenerateJwtToken(string email, string roleName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roleName ?? string.Empty)
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

        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var user = _context.Accounts
                .FirstOrDefault(a => a.Email == model.Email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!VerifyPassword(model.OldPassword, user.Password))
            {
                return BadRequest("Old password is incorrect");
            }

            user.Password = model.NewPassword;

            _context.Accounts.Update(user);
            _context.SaveChanges();

            return Ok("Password changed successfully");
        }
    }
}
