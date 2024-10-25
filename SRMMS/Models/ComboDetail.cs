using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Đảm bảo nhập đúng namespace
using System.ComponentModel.DataAnnotations.Schema;

namespace SRMMS.Models
{
    public partial class ComboDetail
    {
        [Key] 
        public int ComboId { get; set; }

        [Key] 
        public int ProId { get; set; }

       
        public virtual Combo Combo { get; set; } = null!;
        public virtual Product Pro { get; set; } = null!;
    }
}
