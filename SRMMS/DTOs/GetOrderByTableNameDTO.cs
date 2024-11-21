using System;
namespace SRMMS.DTOs
{
	public class GetOrderByTableNameDTO
	{
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalMoney { get; set; }
        public bool? Status { get; set; }
        public List<GetProductDTO> Products { get; set; }
        public List<GetComboDTO> Combos { get; set; }
    }
}

