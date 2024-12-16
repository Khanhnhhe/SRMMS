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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Models;
using SRMMS.SMS;

namespace SRMMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly SRMMSContext _context;
        private readonly ITwilioService _twilioService;

        public TableController(SRMMSContext context, ITwilioService twilioService)
        {
            _context = context;
            _twilioService = twilioService;

        }

        [HttpPost("/api/table/create")]
        public async Task<IActionResult> CreateTable([FromBody] TableDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Table_Name))
            {
                return BadRequest(new { Message = "Dữ liệu bàn không hợp lệ." });
            }

            var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.TableName == model.Table_Name && t.ShiftTable == null);
            if (existingTable != null)
            {
                return BadRequest(new { Message = "Tên bàn đã tồn tại." });
            }

            if (model.TableOfPeople <= 0 || model.TableOfPeople > 20)
            {
                return BadRequest(new { Message = "Chỗ người tại bàn phải là số nguyên dương và lớn hơn 0 và không vượt quá 20 ." });
            }

            var tableLunchShift = new SRMMS.Models.Table
            {
                TableName = model.Table_Name,
                TableOfPeople = model.TableOfPeople,
                StatusId = 1,
                ShiftTable = "Lunch"
            };

            var tableDinnerShift = new SRMMS.Models.Table
            {
                TableName = model.Table_Name,
                TableOfPeople = model.TableOfPeople,
                StatusId = 1,
                ShiftTable = "Dinner"
            };

            _context.Tables.AddRange( tableLunchShift, tableDinnerShift);
            await _context.SaveChangesAsync();

            var result = new List<TableDTO>
    {
       
        new TableDTO { Table_Id = tableLunchShift.TableId, Table_Name = tableLunchShift.TableName, TableOfPeople = tableLunchShift.TableOfPeople, Shift = tableLunchShift.ShiftTable },
        new TableDTO { Table_Id = tableDinnerShift.TableId, Table_Name = tableDinnerShift.TableName, TableOfPeople = tableDinnerShift.TableOfPeople, Shift = tableDinnerShift.ShiftTable }
    };

            return Ok(new { message = "Bàn đã được tạo thành công.", data = result });
        }



        [HttpPut("/api/table/update/{id}")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] TableDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Table_Name))
            {
                return BadRequest("Tên bàn không được để trống.");
            }

            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == id);
            if (table == null)
            {
                return BadRequest("Id không tồn tại");
            }

            if (model.TableOfPeople <= 0)
            {
                return BadRequest("Chỗ người tại bàn phải là số nguyên dương và lớn hơn 0.");
            }

            table.TableName = model.Table_Name;
            table.TableOfPeople = model.TableOfPeople;
            table.StatusId = model.StatusId;


            _context.Tables.Update(table);
            await _context.SaveChangesAsync();

            var result = new TableDTO
            {
                Table_Id = table.TableId,
                Table_Name = table.TableName,
                TableOfPeople = table.TableOfPeople,
                StatusId = table.StatusId
            };

            return Ok("Cập nhật thành công ");
        }

        [HttpGet("/api/table/{id}")]
        public async Task<IActionResult> GetTableById(int id)
        {
            var table = await _context.Tables
                .Where(t => t.TableId == id)
                .Select(t => new ListTableDTO
                {
                    TableId = t.TableId,
                    TableName = t.TableName,
                    StatusName = t.StatusId != null
                        ? _context.StatusTables
                            .Where(s => s.StatusId == t.StatusId)
                            .Select(s => s.StatusName)
                            .FirstOrDefault()
                        : null,
                    BookingId = t.BookingId,
                    TableOfPeople = t.TableOfPeople
                })
                .FirstOrDefaultAsync();

            if (table == null)
            {
                return BadRequest("not found table");
            }

            return Ok(table);
        }



        //[HttpGet("/api/table/list")]
        //public async Task<IActionResult> GetTables(int? statusId = null, int? tableOfPeople = null, int pageNumber = 1, int pageSize = 10)
        //{
       
        //    var query = _context.Tables.AsQueryable();

        //    if (statusId.HasValue)
        //    {
        //        query = query.Where(t => t.StatusId == statusId.Value);
        //    }

        //    if (tableOfPeople.HasValue)
        //    {
        //        query = query.Where(t => t.TableOfPeople == tableOfPeople.Value);
        //    }

        //    var totalTables = await query.CountAsync();

        //    var skip = (pageNumber - 1) * pageSize;

        //    var tables = await query
        //        .Skip(skip)
        //        .Take(pageSize)
        //        .Select(t => new ListTableDTO
        //        {
        //            TableId = t.TableId,
        //            TableName = t.TableName,
        //            StatusName = t.StatusId != null
        //                ? _context.StatusTables
        //                    .Where(s => s.StatusId == t.StatusId)
        //                    .Select(s => s.StatusName)
        //                    .FirstOrDefault()
        //                : null,
        //            BookingId = t.BookingId,
        //            TableOfPeople = t.TableOfPeople
        //        })
        //        .ToListAsync();

        //    return Ok(new
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalTables = totalTables,
        //        Tables = tables
        //    });
        //}

        [HttpGet("/api/table/list")]
        public async Task<IActionResult> GetTables()
        {
            var tables = await _context.Tables
                .Select(t => new ListTableDTO
                {
                    TableId = t.TableId,
                    TableName = t.TableName,
                    StatusId = t.StatusId,
                    StatusName = t.StatusId != null
                        ? _context.StatusTables
                            .Where(s => s.StatusId == t.StatusId)
                            .Select(s => s.StatusName)
                            .FirstOrDefault()
                        : null,

                    BookingId = t.BookingId,
                    TableOfPeople = t.TableOfPeople,
                    Shift = t.ShiftTable
                })
                .ToListAsync();

            return Ok(tables);
        }

        [HttpGet("/api/table/list/lunch")]
        public async Task<IActionResult> GetLunchTables()
        {
            var lunchTables = await _context.Tables
                .Where(t => t.ShiftTable == "Lunch")
                .Select(t => new ListTableDTO
                {
                    TableId = t.TableId,
                    TableName = t.TableName,
                    StatusId = t.StatusId,
                    StatusName = t.StatusId != null
                        ? _context.StatusTables
                            .Where(s => s.StatusId == t.StatusId)
                            .Select(s => s.StatusName)
                            .FirstOrDefault()
                        : null,
                    BookingId = t.BookingId,
                    TableOfPeople = t.TableOfPeople,
                    Shift = t.ShiftTable
                })
                .ToListAsync();

            return Ok(lunchTables);
        }

        [HttpGet("/api/table/list/dinner")]
        public async Task<IActionResult> GetDinnerTables()
        {
            var dinnerTables = await _context.Tables
                .Where(t => t.ShiftTable == "Dinner")
                .Select(t => new ListTableDTO
                {
                    TableId = t.TableId,
                    TableName = t.TableName,
                    StatusId = t.StatusId,
                    StatusName = t.StatusId != null
                        ? _context.StatusTables
                            .Where(s => s.StatusId == t.StatusId)
                            .Select(s => s.StatusName)
                            .FirstOrDefault()
                        : null,
                    BookingId = t.BookingId,
                    TableOfPeople = t.TableOfPeople,
                    Shift = t.ShiftTable
                })
                .ToListAsync();

            return Ok(dinnerTables);
        }





        [HttpGet("/api/status/list")]
        public async Task<IActionResult> GetStatusList()
        {
            
            var statuses = await _context.StatusTables
                .Select(s => new
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName
                })
                .ToListAsync();

            
            return Ok(statuses);
        }

        [HttpPost("/api/table/SetBookingForTable")]
        public async Task<IActionResult> SetBookingForTable([FromBody] SetBookingForTableRequest request)
        {
            if (request == null || request.TableId <= 0 || request.BookingId <= 0)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            var table = await _context.Tables.FindAsync(request.TableId);
            if (table == null)
            {
                return BadRequest(new { message = "Không tìm thấy bàn" });
            }

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == request.BookingId);
            if (booking == null)
            {
                return BadRequest(new { message = "Không tìm thấy đơn đặt chỗ." });
            }

            string shift = booking.HourBooking.HasValue ? (booking.HourBooking.Value.Hours >= 10 && booking.HourBooking.Value.Hours < 14
                ? "Lunch" : booking.HourBooking.Value.Hours >= 16 && booking.HourBooking.Value.Hours <= 23
                ? "Dinner" : null) : null;

            if (shift == null || table.ShiftTable != shift)
            {
                return BadRequest(new { message = "Bàn không phù hợp với ca đặt chỗ." });
            }

            table.BookingId = request.BookingId;
            table.StatusId = 3;

            booking.StatusId = 2;

            string message = $"Xin chào {booking.NameBooking}, đơn đặt chỗ của bạn cho {booking.NumberOfPeople} người vào ngày {booking.DayBooking?.ToString("dd/MM/yyyy")} lúc {booking.HourBooking?.ToString(@"hh\:mm")} đã được chấp nhận. Cảm ơn bạn đã chọn dịch vụ của chúng tôi!";

            try
            {
                _context.Tables.Update(table);
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();

                await _twilioService.SendSmsAsync(booking.PhoneBooking, message);

                return Ok(new { message = "Cập nhật đặt bàn thành công và thông báo đã được gửi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật hoặc gửi thông báo.", error = ex.Message });
            }
        }


        [HttpDelete("/api/table/delete/{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == id);
            if (table == null)
            {
                return BadRequest("Không tìm thấy bàn");
            }

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return Ok("xóa thành công ");
        }

    }
}

