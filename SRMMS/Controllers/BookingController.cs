
﻿using Microsoft.AspNetCore.Mvc;
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
                return BadRequest("Dữ liệu đặt bàn không hợp lệ.");
            }

            TimeSpan? hourBooking = null;
            if (!string.IsNullOrEmpty(bookingDto.HourBooking))
            {
                hourBooking = TimeSpan.Parse(bookingDto.HourBooking);
            }

            string? nameBooking = bookingDto.NameBooking;
            string? phoneBooking = bookingDto.PhoneBooking;

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


//            _context.Bookings.Update(existingBooking);
//            await _context.SaveChangesAsync();

//            var bookings = await _context.Bookings.ToListAsync();
//            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

//            return Ok(existingBooking);
//        }




        [HttpDelete("/api/booking/delete/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound("Booking not found.");
            }


//            _context.Bookings.Remove(existingBooking);
//            await _context.SaveChangesAsync();

//            var bookings = await _context.Bookings.ToListAsync();
//            await _hubContext.Clients.All.SendAsync("ReceiveBookingUpdate", bookings);

//            return NoContent();
//        }

//    }
//}
