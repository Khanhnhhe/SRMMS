namespace SRMMS.DTOs
{
    public class addProductDTO
    {
        public string ProductName { get; set; } 
        public string Description { get; set; } 
        public decimal? Price { get; set; }
        public int? Category  { get; set; }
        public IFormFile Image { get; set; }
        public string Calories { get; set; } 
        public int? CookingTime { get; set; } 
        public bool? Status { get; set; }
    }
}
