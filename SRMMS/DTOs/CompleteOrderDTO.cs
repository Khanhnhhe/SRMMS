using System;
namespace SRMMS.DTOs
{
	public class CompleteOrderDTO
	{
		public int? discountId { get; set; }
	    public decimal? totalMoney { get; set; }
        public int? accId { get; set; }

        public int? usedPoints { get; set; }
    }
}

