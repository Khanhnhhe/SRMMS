using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Account
    {
        public Account()
        {
            Orders = new HashSet<Order>();
            PointLists = new HashSet<PointList>();
            Tables = new HashSet<Table>();
        }

        public int AccId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PointList> PointLists { get; set; }
        public virtual ICollection<Table> Tables { get; set; }
    }
}
