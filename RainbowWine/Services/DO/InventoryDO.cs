using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class InventoryDO
    {
        public int ProductId { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public string BarcodeID { get; set; }
        public int ShopID { get; set; }
        public int ChangeSource { get; set; }
        public string UserId { get; set; }
        public int Type { get; set; }
    }
}