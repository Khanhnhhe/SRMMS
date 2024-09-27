using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class ComboDetail
    {
        public int ComboId { get; set; }
        public int ProId { get; set; }

        public virtual Combo Combo { get; set; } = null!;
        public virtual Menu Pro { get; set; } = null!;
    }
}
