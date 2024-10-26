using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Combo
    {
        public int ComboId { get; set; }
        public string? ComboName { get; set; }
        public string? ComboDiscription { get; set; }
        public string? ComboImg { get; set; }
        public decimal? ComboMoney { get; set; }
        public bool? ComboStatus { get; set; }
        public virtual ICollection<ComboDetail> ComboDetails { get; set; } = new List<ComboDetail>();
    }
}
