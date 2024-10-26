using System;
namespace SRMMS.DTOs
{
	public class ListComboProductDTO
	{
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public string ComboImg { get; set; }
        public decimal? ComboMoney { get; set; }
        public bool? ComboStatus { get; set; }
    }
}

