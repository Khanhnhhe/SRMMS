using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
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
            var booking = new Booking
            {
                TimeBooking = bookingDto.TimeBooking,
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
                query = query.Where(b => b.TimeBooking.Value.Date == bookingDate.Value.Date);
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
                    b.TimeBooking,
                    b.NumberOfPeople,
                    AccountName = b.Acc.FullName,
                    b.Status
                }).ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalBookings = totalBookings,
                Bookings = bookings
            });
        }

    }
}
