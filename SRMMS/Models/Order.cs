using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? AccId { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? OrderStatusId { get; set; }
        public bool? Staus { get; set; }
        public int? CodeId { get; set; }

        public virtual Account? Acc { get; set; }
        public virtual DiscountCode? Code { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
