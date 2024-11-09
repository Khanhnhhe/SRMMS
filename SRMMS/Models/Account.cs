using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Account
    {
        public Account()
        {
            Bookings = new HashSet<Booking>();
            Feedbacks = new HashSet<Feedback>();
            PointLists = new HashSet<PointList>();
        }

        public int AccId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }
        public bool? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<PointList> PointLists { get; set; }
    }
}
