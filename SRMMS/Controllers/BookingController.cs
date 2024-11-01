using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            if (booking == null)
            {
                return BadRequest();
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            
            var bookings = await _context.Bookings.ToListAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

            return CreatedAtAction(nameof(CreateBooking), new { id = booking.BookingId }, booking);
        }

    }
}
