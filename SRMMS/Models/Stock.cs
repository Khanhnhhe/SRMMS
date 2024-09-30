using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Stock
    {
        public Stock()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int StockId { get; set; }
        public int ProId { get; set; }
        public int Quantity { get; set; }
        public bool StockStatus { get; set; }

        public virtual Menu Pro { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
