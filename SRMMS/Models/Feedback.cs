using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Feedback
    {
        public int ProId { get; set; }
        public string Feedback1 { get; set; } = null!;
        public int Ratestar { get; set; }
        public int AccId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Account Acc { get; set; } = null!;
        public virtual Product Pro { get; set; } = null!;
    }
}
