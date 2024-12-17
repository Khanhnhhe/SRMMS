using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Hubs;
using SRMMS.Models;
using SRMMS.SMS;
using System.Net.NetworkInformation;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly SRMMSContext _context;
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ITwilioService _twilioService;

        public BookingController(SRMMSContext context, IHubContext<BookingHub> hubContext, ITwilioService twilioService)
        {
            _context = context;
            _hubContext = hubContext;
            _twilioService = twilioService;
        }



        [HttpPost("/api/booking/Create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest(new { Message = "Dữ liệu đặt bàn không hợp lệ." });
            }

            DateTime now = DateTime.Now;

            TimeSpan? hourBooking = null;
            if (!string.IsNullOrEmpty(bookingDto.HourBooking))
            {
                if (!TimeSpan.TryParse(bookingDto.HourBooking, out TimeSpan parsedHourBooking))
                {
                    return BadRequest(new { Message = "Giờ đặt bàn không hợp lệ." });
                }
                hourBooking = parsedHourBooking;
            }

            if (bookingDto.NumberOfPeople <= 0)
            {
                return BadRequest(new { Message = "Số người đặt bàn phải là số nguyên dương và lớn hơn 0." });
            }

            if (bookingDto.DayBooking < now.Date ||
                             (bookingDto.DayBooking == now.Date && hourBooking.HasValue && hourBooking <= now.TimeOfDay))
            {
                return BadRequest(new { Message = "Ngày và giờ đặt bàn không hợp lệ. Vui lòng chọn ngày và giờ sau mốc thời gian hiện tại ." });
            }

            string? phoneBooking = bookingDto.PhoneBooking;
            if (string.IsNullOrEmpty(phoneBooking) || !IsValidPhoneNumber(phoneBooking))
            {
                return BadRequest(new { Message = "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại hợp lệ." });
            }

            string? nameBooking = bookingDto.NameBooking;
            if (string.IsNullOrEmpty(nameBooking))
            {
                return BadRequest(new { Message = "Vui lòng cung cấp tên của bạn." });
            }


            var booking = new Booking
            {
                DayBooking = bookingDto.DayBooking,
                HourBooking = hourBooking,
                NumberOfPeople = bookingDto.NumberOfPeople,
                NameBooking = nameBooking,
                PhoneBooking = phoneBooking,
                StatusId = 1,
                Shift = GetShift(hourBooking),
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var bookings = await _context.Bookings.ToListAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

            return CreatedAtAction(nameof(CreateBooking), new { id = booking.BookingId }, booking);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10,11}$");
        }

        [HttpGet("/api/booking/getById/{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Where(b => b.BookingId == id)
                .Select(b => new
                {
                    b.BookingId,
                    DayBooking = b.DayBooking,
                    HourBooking = b.HourBooking,
                    b.NumberOfPeople,
                    Shift = GetShift(b.HourBooking),
                    NameBooking = b.NameBooking,
                    PhoneBooking = b.PhoneBooking,
                    b.Status
                })
                .FirstOrDefaultAsync();

            var result = new
            {
                booking?.BookingId,
                booking?.DayBooking,
                HourBooking = booking?.HourBooking?.ToString(@"hh\:mm\:ss"),
                booking?.Shift,
                booking?.NumberOfPeople,
                NameBooking = booking?.NameBooking,
                PhoneBooking = booking?.PhoneBooking,
                booking?.Status
            };

            return Ok(result);
        }


        [HttpGet("/api/booking/getList")]
        public async Task<ActionResult<IEnumerable<Booking>>> SearchBookings(
        string? nameBooking = "",
        DateTime? bookingDate = null,
        int? statusId = null,
        int pageNumber = 1,
        int pageSize = 10)
        {
            var query = _context.Bookings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameBooking))
            {
                var trimmedNameBooking = nameBooking.Trim();
                query = query.Where(b =>
                    !string.IsNullOrEmpty(b.NameBooking) && b.NameBooking.Contains(trimmedNameBooking));
            }

            if (bookingDate.HasValue)
            {
                query = query.Where(b => b.DayBooking.HasValue && b.DayBooking.Value.Date == bookingDate.Value.Date);
            }

            if (statusId.HasValue)
            {
                query = query.Where(b => b.StatusId == statusId.Value);
            }

            var totalBookings = await query.CountAsync();

            var skip = (pageNumber - 1) * pageSize;

            var bookings = await query
                .OrderByDescending(b => b.BookingId)
                .Skip(skip)
                .Take(pageSize)
                .Join(_context.StatusBookings,
                      booking => booking.StatusId,
                      status => status.StatusId,
                      (booking, status) => new
                      {
                          booking.BookingId,
                          booking.DayBooking,
                          booking.HourBooking,
                          booking.NumberOfPeople,
                          booking.NameBooking,
                          booking.PhoneBooking,
                          Shift = BookingController.GetShift(booking.HourBooking),
                          booking.StatusId,
                          StatusName = status.StatusName,
                          Tables = _context.Tables
                           .Where(t => t.BookingId == booking.BookingId)
                           .Select(t => t.TableName)
                           .ToList()
                      })
                .ToListAsync();

            var result = bookings.Select(b => new
            {
                b.BookingId,
                b.DayBooking,
                HourBooking = b.HourBooking?.ToString(@"hh\:mm\:ss"),
                b.NumberOfPeople,
                b.NameBooking,
                b.PhoneBooking,
                b.Shift,
                TableNames = b.Tables,
                StatusId = b.StatusId,
                b.StatusName
            }).ToList();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalBookings = totalBookings,
                Bookings = result
            });
        }




        public static string GetShift(TimeSpan? hourBooking)
        {
            if (!hourBooking.HasValue)
                return "Không phải ca làm của nhà hàng";

            var hour = hourBooking.Value.Hours;


            if (hour >= 10 && hour < 14)
            {
                return "Ca Trưa";
            }
            else if (hour >= 16 && hour <= 23)
            {
                return "Ca Tối";
            }

            return "Ca Khác";
        }

        [HttpPut("/api/booking/updateStatus/{id}")]
        public async Task<IActionResult> UpdateStatusBooking(int id, [FromBody] BookingStatusDTO model)
        {
            if (model == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null)
            {
                return BadRequest("Không tìm thấy đơn đặt chỗ.");
            }


            if (model.StatusId == 2)
            {
                booking.StatusId = 2; 
            }
            else if (model.StatusId == 3)
            {
                booking.StatusId = 3; 
            }
            else
            {
                return BadRequest("Trạng thái không hợp lệ.");
            }

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();


            string message;
            if (booking.StatusId == 2) 
            {
                message = $"Xin chào {booking.NameBooking}, đơn đặt chỗ của bạn cho {booking.NumberOfPeople} người vào ngày {booking.DayBooking?.ToString("dd/MM/yyyy")} lúc {booking.HourBooking?.ToString(@"hh\:mm")} đã được chấp nhận. Cảm ơn bạn đã chọn dịch vụ của chúng tôi!";
            }
            else if (booking.StatusId == 3) 
            {
                message = $"Xin chào {booking.NameBooking}, rất tiếc đơn đặt chỗ của bạn cho ngày {booking.DayBooking?.ToString("dd/MM/yyyy")} vào lúc {booking.HourBooking?.ToString(@"hh\:mm")} không được chấp nhận. Vui lòng liên hệ với chúng tôi để biết thêm chi tiết.";
            }
            else
            {
                return BadRequest("Trạng thái không hợp lệ.");
            }

            try
            {
                await _twilioService.SendSmsAsync(booking.PhoneBooking, message);
                return Ok("Cập nhật trạng thái và gửi thông báo thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi SMS: {ex.Message}");
                return StatusCode(500, "Cập nhật trạng thái thành công, nhưng không gửi được thông báo SMS.");
            }
        }





        [HttpPut("/api/booking/update/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDTO bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest(new { Message = "Dữ liệu đặt chỗ không hợp lệ." });
            }

            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return BadRequest(new { Message = "Không tìm thấy đặt chỗ." });
            }

            DateTime now = DateTime.Now;


            if (bookingDto.DayBooking.HasValue &&
                (bookingDto.DayBooking.Value < now.Date ||
                (bookingDto.DayBooking.Value == now.Date &&
                !string.IsNullOrEmpty(bookingDto.HourBooking) &&
                TimeSpan.TryParse(bookingDto.HourBooking, out TimeSpan parsedHourBooking) &&
                parsedHourBooking <= now.TimeOfDay)))
            {
                return BadRequest(new { Message = "Ngày và giờ đặt chỗ không hợp lệ. Vui lòng chọn ngày và giờ sau thời gian hiện tại." });
            }

            if (bookingDto.DayBooking.HasValue)
            {
                existingBooking.DayBooking = bookingDto.DayBooking.Value;
            }

            if (!string.IsNullOrEmpty(bookingDto.HourBooking))
            {
                if (TimeSpan.TryParse(bookingDto.HourBooking, out TimeSpan parsedTime))
                {
                    existingBooking.HourBooking = parsedTime;

                    int hour = parsedTime.Hours;
                    if (hour >= 10 && hour <= 14)
                    {
                        existingBooking.Shift = "Ca Trưa";
                    }
                    else if (hour >= 16 && hour <= 23)
                    {
                        existingBooking.Shift = "Ca Tối";
                    }
                    else
                    {
                        existingBooking.Shift = "Khác";
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Giờ đặt chỗ không hợp lệ." });
                }
            }

            existingBooking.NumberOfPeople = bookingDto.NumberOfPeople ?? existingBooking.NumberOfPeople;
            if (bookingDto.StatusId.HasValue)
            {
                var status = await _context.StatusBookings.FindAsync(bookingDto.StatusId.Value);
                if (status == null)
                {
                    return BadRequest(new { Message = "Trạng thái không hợp lệ." });
                }
                existingBooking.StatusId = bookingDto.StatusId.Value;
            }

            _context.Bookings.Update(existingBooking);
            await _context.SaveChangesAsync();
            var bookings = await _context.Bookings
                .Include(b => b.Status) 
                .Select(b => new
                {
                    b.BookingId,
                    b.DayBooking,
                    HourBooking = b.HourBooking.Value.ToString(@"hh\:mm"),
                    b.Shift,
                    b.NumberOfPeople,
                    StatusName = b.Status.StatusName
                }).ToListAsync();

            return Ok(bookings); 
        }



        [HttpGet("/api/booking/statusList")]
        public async Task<ActionResult<IEnumerable<StatusBooking>>> GetStatusList()
        {
            var statusList = await _context.StatusBookings.ToListAsync(); 

            if (statusList == null || !statusList.Any())
            {
                return NotFound("Không tìm thấy trạng thái.");
            }

            return Ok(statusList);
        }



        [HttpDelete("/api/booking/delete/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return BadRequest("Không tìm thấy đơn đặt bàn.");
            }

            _context.Bookings.Remove(existingBooking);
            await _context.SaveChangesAsync();

            var bookings = await _context.Bookings.ToListAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

            return NoContent();
        }

    }
}
