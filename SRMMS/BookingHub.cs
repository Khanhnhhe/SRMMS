using Microsoft.AspNetCore.SignalR;
using SRMMS.Models;

namespace SRMMS
{
    public sealed class BookingHub : Hub
    {
        public async Task SendBookingUpdate(List<Booking> bookings)
        {
            
            await Clients.All.SendAsync("ReceiveBookingUpdate", bookings);
        }
    }
}
