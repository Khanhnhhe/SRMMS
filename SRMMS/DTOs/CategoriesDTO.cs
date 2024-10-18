using System;
namespace SRMMS.DTOs
{
	public class CategoriesDTO
	{
        public int CatId { get; set; }
        public string CatName { get; set; } = null!;
        public string? Description { get; set; }
    }
}

