using System;
namespace SRMMS.DTOs
{
	public class ComboProductDetailDTO
	{
        public int ComboId { get; set; }
        public string ComboName { get; set; }
        public string ComboDescription { get; set; }
        public List<ProductDetailDTO> Products { get; set; }
    }
}

