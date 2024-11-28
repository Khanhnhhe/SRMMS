namespace SRMMS.DTOs
{
    public class DiscountCodeDto
    {
        public int CodeId { get; set; }
        public string? CodeDetail { get; set; }
        public double? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 
        public bool? Status { get; set; }
    }
}
