using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProId { get; set; }
        public string? ProName { get; set; }
        public string? ProDiscription { get; set; }
        public decimal? ProPrice { get; set; }
        public int? CatId { get; set; }
        public string? ProImg { get; set; }
        public string? ProCalories { get; set; }
        public bool? ProStatus { get; set; }

        public virtual Category? Cat { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
