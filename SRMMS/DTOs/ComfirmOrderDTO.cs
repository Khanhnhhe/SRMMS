using System;
namespace SRMMS.DTOs
{
	public class ComfirmOrderDTO
	{
        public decimal TotalMoney { get; set; } 
        public List<ComboDetailDTO> ComboDetails { get; set; } 
        public List<ProductDetailOrderDTO> ProductDetails { get; set; } 
    }
}

