using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class VW_FilterBarcode
    {
        public int ProductID { get; set; }
        public string BarcodeID { get; set; }
        public int ShopID { get; set; }
        public string Name { get; set; }
        public Nullable<double> Price { get; set; }
        public int Quantity { get; set; }
        public string Volume { get; set; }
        public int PackageSize { get; set; }
        public DateTime DateTime { get; set; }
    }
}