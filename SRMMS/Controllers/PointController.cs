using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;
using System;
using System.Threading.Tasks;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public PointController(SRMMSContext context)
        {
            _context = context;
        }

        [HttpPost("SettingPoint")]
        public async Task<IActionResult> SettingPoint([FromBody] SettingPointDTO settingPointDto)
        {
            try
            {
                if (settingPointDto == null || settingPointDto.MoneyToPointRate <= 0 || settingPointDto.PointToMoneyRate <= 0)
                {
                    return BadRequest(new { Message = "Thông tin tỷ lệ quy đổi không hợp lệ." });
                }

                var existingSetting = await _context.ConversionPoints.FirstOrDefaultAsync();

                if (existingSetting != null)
                {
                    existingSetting.MoneyToPointRate = settingPointDto.MoneyToPointRate;
                    existingSetting.PointToMoneyRate = settingPointDto.PointToMoneyRate;

                    _context.ConversionPoints.Update(existingSetting);
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Cập nhật tỷ lệ quy đổi điểm thành công.", SettingId = existingSetting.SettingId });
                }
                else
                {
                    var newSetting = new ConversionPoint
                    {
                        MoneyToPointRate = settingPointDto.MoneyToPointRate,
                        PointToMoneyRate = settingPointDto.PointToMoneyRate
                    };

                    _context.ConversionPoints.Add(newSetting);
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Thiết lập tỷ lệ quy đổi điểm thành công.", SettingId = newSetting.SettingId });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi thiết lập tỷ lệ quy đổi.", Error = ex.Message });
            }
        }

        [HttpGet("GetConversionPoint")]
        public async Task<IActionResult> GetConversionPoint()
        {
            try
            {
                var conversionPoint = await _context.ConversionPoints.FirstOrDefaultAsync();

                if (conversionPoint == null)
                {
                    return NotFound(new { Message = "Chưa có thiết lập tỷ lệ quy đổi điểm." });
                }

                return Ok(new
                {
                    MoneyToPointRate = conversionPoint.MoneyToPointRate,
                    PointToMoneyRate = conversionPoint.PointToMoneyRate
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy tỷ lệ quy đổi.", Error = ex.Message });
            }
        }



    }
}

