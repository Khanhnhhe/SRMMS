using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class StatusBooking
    {
        public StatusBooking()
        {
            Bookings = new HashSet<Booking>();
        }

        public int StatusId { get; set; }
        public string? StatusName { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
