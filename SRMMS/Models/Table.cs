using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Table
    {
        public int TableId { get; set; }
        public string? TableName { get; set; }
        public string? QrCode { get; set; }
        public int? AccId { get; set; }
        public DateTime? TimeBooking { get; set; }
        public bool? Status { get; set; }

        public virtual Account? Acc { get; set; }
    }
}
