namespace SRMMS.DTOs
{
    public class UpdateDiscountDTO
    {
       
        public string? CodeDetail { get; set; }
        public double? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }

        public DiscountType DiscountType { get; set; }
    }

    
}
