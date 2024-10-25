using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class PointList
    {
        public int PointId { get; set; }
        public int? AccId { get; set; }
        public double? NumberPonit { get; set; }

        public virtual Account? Acc { get; set; }
    }
}
