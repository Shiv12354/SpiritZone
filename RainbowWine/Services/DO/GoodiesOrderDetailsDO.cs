using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class GoodiesOrderDetailsDO
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public string GName { get; set; }
    }
}