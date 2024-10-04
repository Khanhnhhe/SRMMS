using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Table
    {
        public Table()
        {
            Accounts = new HashSet<Account>();
        }

        public int TableId { get; set; }
        public string TableName { get; set; } = null!;
        public string TableQrcode { get; set; } = null!;

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
