﻿using System;
namespace SRMMS.DTOs
{
	public class UpdateComboProductDTO
	{
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public IFormFile ComboImg { get; set; }
        public decimal ComboMoney { get; set; }
        public bool ComboStatus { get; set; }
        public List<string> ProductNames { get; set; } = new List<string>();
    }
}
