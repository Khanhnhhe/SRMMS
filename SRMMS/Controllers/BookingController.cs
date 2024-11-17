using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Hubs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly SRMMSContext _context;
        private readonly IHubContext<BookingHub> _hubContext;

        public BookingController(SRMMSContext context, IHubContext<BookingHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("/api/booking/Create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest("Invalid booking data.");
            }

            TimeSpan? hourBooking = null;
            if (!string.IsNullOrEmpty(bookingDto.HourBooking))
            {
                hourBooking = TimeSpan.Parse(bookingDto.HourBooking);  
            }

            var booking = new Booking
            {
                DayBooking = bookingDto.DayBooking,
                HourBooking = hourBooking,
                NumberOfPeople = bookingDto.NumberOfPeople,
                AccId = bookingDto.AccId,
                Status = true
            };


            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            var bookings = await _context.Bookings.ToListAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

            return CreatedAtAction(nameof(CreateBooking), new { id = booking.BookingId }, booking);
        }

        [HttpGet("/api/booking/getById/{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Acc)
                .Where(b => b.BookingId == id)
                .Select(b => new
                {
                    b.BookingId,
                    DayBooking = b.DayBooking,
                    HourBooking = b.HourBooking,
                    b.NumberOfPeople,
                    AccountName = b.Acc.FullName,
                    Phone = b.Acc.Phone,
                    b.Status
                })
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var result = new
            {
                booking.BookingId,
                booking.DayBooking,
                HourBooking = booking.HourBooking?.ToString(@"hh\:mm\:ss"),
                booking.NumberOfPeople,
                booking.AccountName,
                booking.Phone,
                booking.Status
            };

            return Ok(booking);
        }


        [HttpGet("/api/booking/getList")]
        public async Task<ActionResult<IEnumerable<Booking>>> SearchBookings(string? accountName = "", DateTime? bookingDate = null, bool? status = null, int pageNumber = 1, int pageSize = 10)
        {

            var totalBookings = await _context.Bookings.CountAsync();
            var skip = (pageNumber - 1) * pageSize;


            var query = _context.Bookings.Include(b => b.Acc).AsQueryable();

            if (!string.IsNullOrWhiteSpace(accountName))
            {
                var trimmedAccountName = accountName.Trim();
                query = query.Where(b => b.Acc.FullName.Contains(trimmedAccountName));
            }


            if (bookingDate.HasValue)
            {
                query = query.Where(b => b.DayBooking.HasValue && b.DayBooking.Value.Date == bookingDate.Value.Date);
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            var bookings = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(b => new
                {
                    b.BookingId,
                    DayBooking = b.DayBooking,
                    HourBooking = b.HourBooking,
                    b.NumberOfPeople,
                    AccountName = b.Acc.FullName,
                    Phone = b.Acc.Phone,
                    b.Shift,
                    b.Status
                }).ToListAsync();

            var result = bookings.Select(b => new
            {
                b.BookingId,
                b.DayBooking,
                HourBooking = b.HourBooking?.ToString(@"hh\:mm\:ss"),
                b.NumberOfPeople,
                b.AccountName,
                b.Phone,
                b.Shift,
                b.Status
            }).ToList();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalBookings = totalBookings,
                Bookings = bookings
            });
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
            }

            
            existingBooking.NumberOfPeople = bookingDto.NumberOfPeople ?? existingBooking.NumberOfPeople;
            existingBooking.Status = bookingDto.Status ?? existingBooking.Status;
            existingBooking.Shift = bookingDto.Shift ?? existingBooking.Shift;

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
