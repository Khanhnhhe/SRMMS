using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Menu
    {
        public Menu()
        {
            Stocks = new HashSet<Stock>();
        }

        public int ProId { get; set; }
        public string ProName { get; set; } = null!;
        public string ProDiscription { get; set; } = null!;
        public string ProWarning { get; set; } = null!;
        public decimal ProPrice { get; set; }
        public int CatId { get; set; }
        public int? IngId { get; set; }
        public string ProImg { get; set; } = null!;
        public string ProCalories { get; set; } = null!;
        public int ProCookingTime { get; set; }
        public bool ProStatus { get; set; }

        public virtual Category Cat { get; set; } = null!;
        public virtual Ingredient? Ing { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
