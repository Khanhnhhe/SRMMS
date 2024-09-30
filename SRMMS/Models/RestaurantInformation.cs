using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class RestaurantInformation
    {
        public string ResName { get; set; } = null!;
        public string ResAdress { get; set; } = null!;
        public string ResFacebook { get; set; } = null!;
        public string ResEmail { get; set; } = null!;
        public int ResPhone { get; set; }
    }
}
