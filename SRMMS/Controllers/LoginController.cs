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
using SRMMS.SMS;


namespace SRMMS.Controllers
{
    [ApiController]
    [Route("api")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SRMMSContext _context;
        private readonly ITwilioService _twilioService;

        public LoginController(IConfiguration configuration, SRMMSContext context , ITwilioService twilioService)
        {
            _configuration = configuration;
            _context = context;
            _twilioService = twilioService;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var existingUser = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already registered.");
            }

            try
            {
                string verificationCode = GenerateVerificationCode();

              
                await _twilioService.SendSmsAsync(model.PhoneNumber, $"Mã xác nhận của bạn là: {verificationCode}");

                
                var newUser = new Account
                {
                    Email = model.Email,
                    Phone = model.PhoneNumber,
                    Password = model.Password, 
                    FullName = model.FullName
                };

                _context.Accounts.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Mã xác nhận đã được gửi đến số điện thoại của bạn. Vui lòng kiểm tra và nhập mã xác nhận để hoàn tất đăng ký." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Tạo mã xác nhận 6 chữ số
        }



    }
}
