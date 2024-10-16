using System;
using SRMMS.Models;

namespace SRMMS.DTOs
{
	public class ListProductDTO
	{
		

        public int ProId { get; set; }
        public string ProName { get; set; } = null!;
        public string ProDiscription { get; set; } = null!;
        public string ProWarning { get; set; } = null!;
        public decimal ProPrice { get; set; }
        public string CatName { get; set; }
        public string ProImg { get; set; } = null!;
        public string ProCalories { get; set; } = null!;
        public string ProCookingTime { get; set; } = null!;
        public bool ProStatus { get; set; }
    }
	
}

