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
        public bool? Status { get; set; }
        public int? BookingId { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
