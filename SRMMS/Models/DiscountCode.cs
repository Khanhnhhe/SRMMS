using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class DiscountCode
    {
        public DiscountCode()
        {
            Orders = new HashSet<Order>();
        }

        public int CodeId { get; set; }
        public string? CodeDetail { get; set; }
        public double? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
