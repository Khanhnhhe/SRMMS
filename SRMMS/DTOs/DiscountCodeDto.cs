namespace SRMMS.DTOs
{
    public class DiscountCodeDto
    {
        public int CodeId { get; set; }
        public string? CodeDetail { get; set; }
        public double? DiscountValue { get; set; }
        public String? StartDate { get; set; } 
        public String? EndDate { get; set; } 
        public bool? Status { get; set; }

        public int DiscountType { get; set; }


    }
}
