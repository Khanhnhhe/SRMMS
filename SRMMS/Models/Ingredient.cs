using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            Products = new HashSet<Product>();
        }

        public int IngId { get; set; }
        public string IngName { get; set; } = null!;
        public string? Discription { get; set; }
        public int CateId { get; set; }
        public int IngQuantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime IngInsertDate { get; set; }
        public decimal IngPrice { get; set; }

        public virtual IngCategory Cate { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; }
    }
}
