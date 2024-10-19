using System;
namespace SRMMS.DTOs
{
	public class ProductCategoriesDTO
	{
        public int CatId { get; set; }
        public string CatName { get; set; } = null!;
        public string? Description { get; set; }

    }
}

