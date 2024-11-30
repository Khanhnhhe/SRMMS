using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Hubs;
using SRMMS.Models;
using SRMMS.SMS;

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
                return BadRequest("Dữ liệu đặt bàn không hợp lệ.");
            }

            DateTime now = DateTime.Now;

            TimeSpan? hourBooking = null;
            if (!string.IsNullOrEmpty(bookingDto.HourBooking))
            {
                if (!TimeSpan.TryParse(bookingDto.HourBooking, out TimeSpan parsedHourBooking))
                {
                    return BadRequest("Giờ đặt bàn không hợp lệ.");
                }
                hourBooking = parsedHourBooking;
            }

            if (bookingDto.NumberOfPeople <= 0)
            {
                return BadRequest("Số người đặt bàn phải là số nguyên dương và lớn hơn 0.");
            }

            if (bookingDto.DayBooking < now.Date ||
                             (bookingDto.DayBooking == now.Date && hourBooking.HasValue && hourBooking <= now.TimeOfDay))
            {
                return BadRequest("Ngày và giờ đặt bàn không hợp lệ. Vui lòng chọn ngày và giờ sau mốc thời gian hiện tại .");
            }

            string? phoneBooking = bookingDto.PhoneBooking;
            if (string.IsNullOrEmpty(phoneBooking) || !IsValidPhoneNumber(phoneBooking))
            {
                return BadRequest("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại hợp lệ.");
            }

            string? nameBooking = bookingDto.NameBooking;
            if (string.IsNullOrEmpty(nameBooking))
            {
                return BadRequest("Vui lòng cung cấp tên của bạn.");
            }

            

            if (string.IsNullOrEmpty(nameBooking) || string.IsNullOrEmpty(phoneBooking))
            {
                return BadRequest("Vui lòng cung cấp tên và số điện thoại của khách.");
            }

            var booking = new Booking
            {
                DayBooking = bookingDto.DayBooking,
                HourBooking = hourBooking,
                NumberOfPeople = bookingDto.NumberOfPeople,
                NameBooking = nameBooking,
                PhoneBooking = phoneBooking,
                Status = true,
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
        bool? status = null,
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

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            var totalBookings = await query.CountAsync();

            var skip = (pageNumber - 1) * pageSize;

            var bookings = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(b => new
                {
                    b.BookingId,
                    DayBooking = b.DayBooking,
                    HourBooking = b.HourBooking,
                    b.NumberOfPeople,
                    b.NameBooking,
                    b.PhoneBooking,
                    Shift = BookingController.GetShift(b.HourBooking),
                    b.Status
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
                b.Status
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
                return "Unknown";

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
                return NotFound("Không tìm thấy đơn đặt chỗ.");
            }

            
            booking.Status = model.Status;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            
            string message = model.Status
                ? $"Xin chào {booking.NameBooking}, đơn đặt chỗ của bạn cho {booking.NumberOfPeople} người vào ngày {booking.DayBooking?.ToString("dd/MM/yyyy")} lúc {booking.HourBooking?.ToString(@"hh\:mm")} đã được chấp nhận. Cảm ơn bạn đã chọn dịch vụ của chúng tôi!"
                : $"Xin chào {booking.NameBooking}, rất tiếc đơn đặt chỗ của bạn cho ngày {booking.DayBooking?.ToString("dd/MM/yyyy")} vào lúc {booking.HourBooking?.ToString(@"hh\:mm")} không được chấp nhận. Vui lòng liên hệ với chúng tôi để biết thêm chi tiết.";

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
                return BadRequest("Invalid booking data.");
            }

            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound("Booking not found.");
            }

            existingBooking.DayBooking = bookingDto.DayBooking ?? existingBooking.DayBooking;

            if (!string.IsNullOrEmpty(bookingDto.HourBooking))
            {
                existingBooking.HourBooking = TimeSpan.Parse(bookingDto.HourBooking);

                int hour = existingBooking.HourBooking.Value.Hours;

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

            existingBooking.NumberOfPeople = bookingDto.NumberOfPeople ?? existingBooking.NumberOfPeople;
            existingBooking.Status = bookingDto.Status ?? existingBooking.Status;

            _context.Bookings.Update(existingBooking);
            await _context.SaveChangesAsync();

            var bookings = await _context.Bookings.ToListAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

            return Ok(existingBooking);
        }



        [HttpDelete("/api/booking/delete/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound("Booking not found.");
            }

            _context.Bookings.Remove(existingBooking);
            await _context.SaveChangesAsync();

            var bookings = await _context.Bookings.ToListAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

            return NoContent();
        }

    }
}
