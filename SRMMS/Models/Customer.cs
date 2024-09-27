﻿using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Accounts = new HashSet<Account>();
        }

        public int CusId { get; set; }
        public string CusFullname { get; set; } = null!;
        public int CusPhone { get; set; }
        public int CusTableId { get; set; }

        public virtual Table CusTable { get; set; } = null!;
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
