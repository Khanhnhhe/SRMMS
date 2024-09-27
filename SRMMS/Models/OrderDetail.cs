using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int StockId { get; set; }
        public int Quantiity { get; set; }
        public double Price { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
    }
}
