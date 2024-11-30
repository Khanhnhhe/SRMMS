namespace SRMMS.DTOs
{
    public class UpdateDiscountDTO
    {
       
        public string? CodeDetail { get; set; }
        public double? DiscountValue { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? Status { get; set; }
    }
}
