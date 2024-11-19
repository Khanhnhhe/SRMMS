using System;
namespace SRMMS.DTOs
{
	public class OrderDTO
	{
        public int OrderId { get; set; }
        public int? TableId { get; set; }
        public string TableName { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalMoney { get; set; }
        public bool? Status { get; set; }
        public int? CodeId { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}

