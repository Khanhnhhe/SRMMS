using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Account
    {
        public Account()
        {
            PointLists = new HashSet<PointList>();
        }

        public int AccId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }
        public bool? Status { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<PointList> PointLists { get; set; }
    }
}
