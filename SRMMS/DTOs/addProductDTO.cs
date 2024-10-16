﻿namespace SRMMS.DTOs
{
    public class addProductDTO
    {
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Warning { get; set; } = null!;
        public decimal Price { get; set; }
        public int Category { get; set; } 
        public string Image { get; set; } = null!;
        public string Calories { get; set; } = null!;
        public string CookingTime { get; set; } = null!;
        public bool Status { get; set; }
    }
}

