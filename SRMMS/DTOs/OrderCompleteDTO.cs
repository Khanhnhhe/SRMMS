using System;
namespace SRMMS.DTOs
{
	public class OrderCompleteDTO
	{
        public int OrderId { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? TableId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public int? DiscountId { get; set; }
        public double? DiscountValue { get; set; }
    }
}

