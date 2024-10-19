﻿namespace SRMMS.DTOs
{
    public class addProductDTO
    {
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal? Price { get; set; }
        public int? Category  { get; set; } 
        public IFormFile Image { get; set; } 
        public string Calories { get; set; } = null!;
        public int? CookingTime { get; set; } 
        public bool? Status { get; set; }
    }
}
