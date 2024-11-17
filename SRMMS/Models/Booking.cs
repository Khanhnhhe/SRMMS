using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public DateTime? DayBooking { get; set; }
        public int? NumberOfPeople { get; set; }
        public int? AccId { get; set; }
        public bool? Status { get; set; }
        public string? Shift { get; set; }
        public TimeSpan? HourBooking { get; set; }

        public virtual Account? Acc { get; set; }
        public virtual Table BookingNavigation { get; set; } = null!;
    }
}
