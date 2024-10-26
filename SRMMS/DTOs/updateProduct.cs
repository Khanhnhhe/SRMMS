using System;
namespace SRMMS.DTOs
{
	public class updateProduct
	{
        public string? ProductName { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public decimal? Price { get; set; }
        public int? Category { get; set; }
        public IFormFile? Image { get; set; } = null!;
        public string? Calories { get; set; } = null!;
        public bool? Status { get; set; }
    }
}

