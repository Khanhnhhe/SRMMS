using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Category
    {
        public Category()
        {
            Menus = new HashSet<Menu>();
        }

        public int CatId { get; set; }
        public string CatName { get; set; } = null!;

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
