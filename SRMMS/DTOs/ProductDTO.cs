using System;
using SRMMS.Models;

namespace SRMMS.DTOs
{
	public class ListProductDTO
	{

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal? Price { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Calories { get; set; } = null!;
        public int? CookingTime { get; set; }
        public bool? Status { get; set; }
    }
	
}

