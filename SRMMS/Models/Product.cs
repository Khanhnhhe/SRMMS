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
        public string ProName { get; set; } = null!;
        public string ProDiscription { get; set; } = null!;
        public decimal ProPrice { get; set; }
        public int CatId { get; set; }
        public string ProImg { get; set; } = null!;
        public string ProCalories { get; set; } = null!;
        public int ProCookingTime { get; set; }
        public bool ProStatus { get; set; }

        public virtual Category Cat { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
