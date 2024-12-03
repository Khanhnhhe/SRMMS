using System;
namespace SRMMS.DTOs
{
	public class GetOrderByOrderIdDTO
	{
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public double TotalMoney { get; set; }
        public int? Status { get; set; }
        public int TableId { get; set; }
        public String? TableName { get; set; }
        public List<GetProductDTO> Products { get; set; }
        public List<GetComboDTO> Combos { get; set; }
        public List<GetAccountDTO> Customers { get; set; } 
        public int? DiscountId { get; set; }
        public double? DiscountValue { get; set; }
        public List<int> PointIds { get; set; } 
        public List<double?> PointNumbers { get; set; } 
    }
}

