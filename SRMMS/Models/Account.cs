using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Account
    {
        public Account()
        {
            Orders = new HashSet<Order>();
        }

        public int AccId { get; set; }
        public int CusId { get; set; }
        public int EmpId { get; set; }
        public int? TableId { get; set; }

        public virtual Customer Cus { get; set; } = null!;
        public virtual Employee Emp { get; set; } = null!;
        public virtual Table? Table { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
