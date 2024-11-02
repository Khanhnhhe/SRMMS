using System;
namespace SRMMS.DTOs
{
	public class ProductDetailDTO
	{
        public int? ProductId { get; set; }
        public string? ProductName { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public string? Calories { get; set; } = null!;
    }
}

