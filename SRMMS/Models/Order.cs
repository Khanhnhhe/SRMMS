using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            PointLists = new HashSet<PointList>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? TableId { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? CodeId { get; set; }
        public int? StatusId { get; set; }

        public virtual DiscountCode? Code { get; set; }
        public virtual StatusOrder? Status { get; set; }
        public virtual Table? Table { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<PointList> PointLists { get; set; }
    }
}
