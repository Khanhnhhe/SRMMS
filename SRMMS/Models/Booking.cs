using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public DateTime? TimeBooking { get; set; }
        public int? NumberOfPeople { get; set; }
        public int? AccId { get; set; }
        public bool? Status { get; set; }

        public virtual Account? Acc { get; set; }
        public virtual Table BookingNavigation { get; set; } = null!;
    }
}
