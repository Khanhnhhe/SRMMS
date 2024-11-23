using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _memoryCache;

        public LoginController(IConfiguration configuration, SRMMSContext context , ITwilioService twilioService , IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _context = context;
            _twilioService = twilioService;
            _memoryCache = memoryCache;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO model)
        {
            var user = _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefault(a => a.Phone == model.Phone);

            if (user == null)
            {
                return Unauthorized("User not found");
            }

            if (user.Status != true)
            {
                return Unauthorized("Account is inactive");
            }

            if (!VerifyPassword(model.Password, user.Password))
            {
                return Unauthorized("Invalid password");
            }

            // Generate token
            var token = GenerateJwtToken(user.Phone, user.Role?.RoleName);

            return Ok(new
            {
                token = token,
                phone = user.Phone,
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
                .FirstOrDefault(a => a.Phone == model.Phone);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!VerifyPassword(model.OldPassword, user.Password))
            {
                return BadRequest("Old password is incorrect");
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest("New password and confirmation do not match");
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

                _memoryCache.Set(model.PhoneNumber, verificationCode, TimeSpan.FromMinutes(5));


                await _twilioService.SendSmsAsync(model.PhoneNumber, $"Mã xác nhận của bạn là: {verificationCode}");

                
                var newUser = new Account
                {
                    Email = model.Email,
                    Phone = model.PhoneNumber,
                    Password = model.Password, 
                    FullName = model.FullName,
                    Status = false,
                    RoleId = 5,
                    StartDate = DateTime.UtcNow,
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

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDTO model)
        {
            if (!_memoryCache.TryGetValue(model.PhoneNumber, out string verificationCode))
            {
                verificationCode = GenerateVerificationCode();
                _memoryCache.Set(model.PhoneNumber, verificationCode, TimeSpan.FromMinutes(5));


                await _twilioService.SendSmsAsync(model.PhoneNumber, $"Mã xác nhận của bạn là: {verificationCode}");
            }
            else
            {
                return Ok("Mã xác nhận đã được gửi trước đó. Vui lòng kiểm tra điện thoại của bạn.");
            }

            return Ok("Mã xác nhận đã được gửi lại đến số điện thoại của bạn.");
        }




        [HttpPost("forgot-password")] // send opt 
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Phone == model.PhoneNumber);

            try
            {
                string verificationCode = GenerateVerificationCode();

                _memoryCache.Set(model.PhoneNumber, verificationCode, TimeSpan.FromMinutes(5));

                await _twilioService.SendSmsAsync(model.PhoneNumber, $"Mã xác nhận quên mật khẩu của bạn là: {verificationCode}");

                return Ok(new { Message = "Mã xác nhận đã được gửi đến số điện thoại của bạn. Vui lòng kiểm tra và nhập mã xác nhận." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }


        [HttpPost("reset-password")] // update password after comfirm otp 
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Phone == model.PhoneNumber);

            user.Password = model.NewPassword;

            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Mật khẩu đã được thay đổi thành công.");
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO model)
        {
            if (!_memoryCache.TryGetValue(model.PhoneNumber, out string cachedOtp) || cachedOtp != model.VerificationCode)
            {
                return BadRequest("Mã OTP không chính xác hoặc đã hết hạn.");
            }

            var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Phone == model.PhoneNumber);
            //if (user == null)
            //{
            //    return NotFound("User not found");
            //}

            user.Status = true;
            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            _memoryCache.Remove(model.PhoneNumber);

            return Ok("Xác thực thành công! Bạn có thể đăng nhập.");
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Tạo mã xác nhận 6 chữ số
        }



    }
}
