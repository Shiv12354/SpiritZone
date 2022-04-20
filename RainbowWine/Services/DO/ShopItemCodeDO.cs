using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ShopItemCodeDO
    {
        public string ShopCode { get; set; }
        public string ShopItemCode { get; set; }
        public int Stock { get; set; }
        public decimal Rate { get; set; }
        public string UniversalItemCode { get; set; }
        public string TimeStamp { get; set; }
        public string ItemName { get; set; }
        public string Packing { get; set; }
        public string ML { get; set; }
    }
}