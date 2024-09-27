using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class IngCategory
    {
        public IngCategory()
        {
            Ingredients = new HashSet<Ingredient>();
        }

        public int CateId { get; set; }
        public string CateName { get; set; } = null!;

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}
