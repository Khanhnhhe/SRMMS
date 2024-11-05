using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class StatusTable
    {
        public int StatusId { get; set; }
        public string? StatusName { get; set; }

        public virtual Table Status { get; set; } = null!;
    }
}
