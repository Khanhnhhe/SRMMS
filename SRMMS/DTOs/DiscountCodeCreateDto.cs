namespace SRMMS.DTOs
{
    public class DiscountCodeCreateDto
    {
        public string? CodeDetail { get; set; }
        public double? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }

        public DiscountType DiscountType { get; set; }
    }

    public enum DiscountType
    {
        Percentage = 0,  
        Fixed = 1       
    }
}
