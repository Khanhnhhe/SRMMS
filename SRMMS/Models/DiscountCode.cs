using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class DiscountCode
    {
        public int CodeId { get; set; }
        public int? OrderId { get; set; }
        public string? CodeDetail { get; set; }
        public double? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }

        public virtual Order? Order { get; set; }
    }
}
