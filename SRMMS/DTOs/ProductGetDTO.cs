namespace SRMMS.DTOs
{
    public class ProductGetDTO
    {
        public int ProId { get; set; }
        public string ProName { get; set; } = null!;
        public string ProDescription { get; set; } = null!;
        public decimal ProPrice { get; set; }
        public string CatName { get; set; } = null!;

        // Thêm các thuộc tính khác từ ProductCreateDTO
        public string ProWarning { get; set; } = null!;
        public string ProImg { get; set; } = null!;
        public string ProCalories { get; set; } = null!;
        public int ProCookingTime { get; set; }
        public bool ProStatus { get; set; }
    }
}
