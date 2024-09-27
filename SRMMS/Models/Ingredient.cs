using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            Menus = new HashSet<Menu>();
        }

        public int IngId { get; set; }
        public string IngName { get; set; } = null!;
        public string Discription { get; set; } = null!;
        public int CateId { get; set; }
        public int IngQuantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime IngInsertDate { get; set; }
        public decimal IngPrice { get; set; }

        public virtual IngCategory Cate { get; set; } = null!;
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
