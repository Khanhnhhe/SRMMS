namespace SRMMS.DTOs
{
    public class addProductDTO
    {
        public string ProName { get; set; } = null!;
        public string ProDiscription { get; set; } = null!;
        public string ProWarning { get; set; } = null!;
        public decimal ProPrice { get; set; }
        public int cateId  { get; set; } 
        public IFormFile Image { get; set; } = null!; 
        public string ProCalories { get; set; } = null!;
        public string ProCookingTime { get; set; } = null!;
        public bool ProStatus { get; set; }
    }
}
