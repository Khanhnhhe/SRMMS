using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Table
    {
        public Table()
        {
            Orders = new HashSet<Order>();
        }

        public int TableId { get; set; }
        public string? TableName { get; set; }
        public int? StatusId { get; set; }
        public int? BookingId { get; set; }
        public int? TableOfPeople { get; set; }

        public virtual Booking? Booking { get; set; }
        public virtual StatusTable? StatusTable { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
