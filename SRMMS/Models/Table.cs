using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Table
    {
        public Table()
        {
            BookingTables = new HashSet<BookingTable>();
        }

        public int TableId { get; set; }
        public string TableName { get; set; } = null!;
        public string QrCode { get; set; } = null!;
        public bool Status { get; set; }

        public virtual ICollection<BookingTable> BookingTables { get; set; }
    }
}
