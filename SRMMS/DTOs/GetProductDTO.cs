using System;
namespace SRMMS.DTOs
{
	public class GetProductDTO
	{
        public int ProductId { get; set; }
        public string? ProName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}

