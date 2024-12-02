using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Booking
    {
        public Booking()
        {
            Tables = new HashSet<Table>();
        }

        public int BookingId { get; set; }
        public DateTime? DayBooking { get; set; }
        public int? NumberOfPeople { get; set; }
        public string? Shift { get; set; }
        public TimeSpan? HourBooking { get; set; }
        public string? NameBooking { get; set; }
        public string? PhoneBooking { get; set; }
        public int? StatusId { get; set; }

        public virtual StatusBooking? Status { get; set; }
        public virtual ICollection<Table> Tables { get; set; }
    }
}
