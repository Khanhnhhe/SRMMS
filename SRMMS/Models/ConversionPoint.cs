using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class ConversionPoint
    {
        public int SettingId { get; set; }
        public decimal? MoneyToPointRate { get; set; }
        public decimal? PointToMoneyRate { get; set; }
    }
}
