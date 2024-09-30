using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Accounts = new HashSet<Account>();
            Posts = new HashSet<Post>();
        }

        public int EmpId { get; set; }
        public string EmpFirstName { get; set; } = null!;
        public string EmpLastName { get; set; } = null!;
        public DateTime EmpDob { get; set; }
        public bool EmpGender { get; set; }
        public int EmpPhoneNumber { get; set; }
        public string EmpEmail { get; set; } = null!;
        public string EmpPassword { get; set; } = null!;
        public string EmpAdress { get; set; } = null!;
        public string EmpDistrict { get; set; } = null!;
        public string EmpCity { get; set; } = null!;
        public string EmpWard { get; set; } = null!;
        public DateTime EmpStartDate { get; set; }
        public DateTime EmpEndDate { get; set; }
        public int EmpSalaryId { get; set; }
        public int EmpRoleId { get; set; }
        public bool EmpStatus { get; set; }

        public virtual Role EmpRole { get; set; } = null!;
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
