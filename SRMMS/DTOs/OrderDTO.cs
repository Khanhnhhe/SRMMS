using System;
using SRMMS.Models;

namespace SRMMS.DTOs
{
	public class OrderDTO
	{
        public int OrderId { get; set; }
        public int? TableId { get; set; }
        public int? AccId { get; set; }
        public string TableName { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalMoney { get; set; }
        public bool? Status { get; set; }
        public int? CodeId { get; set; }
        public int? PointId { get; set; }
        public List<ComboDetailDTO> ComboDetails { get; set; }  
        public List<ProductDetailOrderDTO> ProductDetails { get; set; }
        
    }
}


