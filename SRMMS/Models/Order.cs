using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Order
    {
        public Order()
        {
            DiscountCodes = new HashSet<DiscountCode>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int AccId { get; set; }
        public decimal TotalMoney { get; set; }
        public int OrderStatusId { get; set; }

        public virtual Account Acc { get; set; } = null!;
        public virtual Status OrderStatus { get; set; } = null!;
        public virtual ICollection<DiscountCode> DiscountCodes { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
