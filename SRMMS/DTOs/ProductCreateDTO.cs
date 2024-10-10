namespace SRMMS.DTOs
{
    public class ProductCreateDTO
    {
        public string ProName { get; set; } = null!;
        public string ProDiscription { get; set; } = null!;
        public string ProWarning { get; set; } = null!;
        public decimal ProPrice { get; set; }
        public int CatId { get; set; }
        public string ProImg { get; set; } = null!;
        public string ProCalories { get; set; } = null!;
        public int ProCookingTime { get; set; }
        public bool ProStatus { get; set; }
    }

}
