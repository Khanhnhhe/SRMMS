namespace SRMMS.DTOs
{
    public class ProductUpdateDTO
    {
        public string ProName { get; set; } = null!;
        public string ProDiscription { get; set; } = null!;
        public string ProWarning { get; set; } = null!;
        public decimal ProPrice { get; set; }
        public int CatId { get; set; }  // Thêm CatId vào DTO
        public string ProImg { get; set; } = null!;
        public string ProCalories { get; set; } = null!;
        public int ProCookingTime { get; set; }
        public bool ProStatus { get; set; }
    }
}
