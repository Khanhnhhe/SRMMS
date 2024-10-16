using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class BookingTable
    {
        public int BookId { get; set; }
        public int? TableId { get; set; }
        public int? AccId { get; set; }
        public DateTime? TimeBooking { get; set; }
        public DateTime? TimeOut { get; set; }

        public virtual Account? Acc { get; set; }
        public virtual Table? Table { get; set; }
    }
}
