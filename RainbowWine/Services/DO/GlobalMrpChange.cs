using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class GlobalMrpChange
    {
        public int ProductId  { get; set; }
        public string ProductName { get; set; }
        public double MRP { get; set; }
        public int ShopId { get; set; }
    }
}