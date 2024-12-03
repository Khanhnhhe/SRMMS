using System;
using SRMMS.Models;

namespace SRMMS.DTOs
{
	public class OrderDTO
	{
        public int OrderId { get; set; }
        public int? TableId { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? Status { get; set; }
        public List<ComboDetailDTO> ComboDetails { get; set; }  
        public List<ProductDetailOrderDTO> ProductDetails { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}


