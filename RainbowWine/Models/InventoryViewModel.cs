using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class InventoryViewModel
    {
        public int InventoryId { get; set; }
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QtyAvailable { get; set; }
        public int PackingSize { get; set; }
        public double MRP { get; set; }
        public string ShopName { get; set; }
        public double Price { get; set; }
        public string Size { get; set; }
        public int ProductSizeId { get; set; }
    }
}