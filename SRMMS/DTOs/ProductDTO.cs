using System;
using SRMMS.Models;

namespace SRMMS.DTOs
{
	public class ListProductDTO
	{
		

        public int ProId { get; set; }
        public string ProName { get; set; } = null!;
        public string ProDescription { get; set; } = null!;
        public decimal? ProPrice { get; set; }
        public string CatName { get; set; }
        public IFormFile ProImg { get; set; } 
        public string? ProCalories { get; set; } = null!;
        public int? ProCookingTime { get; set; } 
        public bool? ProStatus { get; set; }
    }
	
}

