using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Table
    {
        public Table()
        {
            Customers = new HashSet<Customer>();
        }

        public int TableId { get; set; }
        public string TableName { get; set; } = null!;
        public string TableQrcode { get; set; } = null!;

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
