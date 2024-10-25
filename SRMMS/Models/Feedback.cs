using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public string? Feedback1 { get; set; }
        public int? RateStar { get; set; }
        public int? AccId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Account? Acc { get; set; }
    }
}
