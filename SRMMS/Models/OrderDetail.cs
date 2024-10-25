using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProId { get; set; }
        public int? Quantiity { get; set; }
        public decimal? Price { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Pro { get; set; }
    }
}
