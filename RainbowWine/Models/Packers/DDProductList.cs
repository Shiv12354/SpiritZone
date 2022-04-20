using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class DDProductList
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string BarcodeId { get; set; }
        public Nullable<double> Price { get; set; }
    }
}