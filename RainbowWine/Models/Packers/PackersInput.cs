using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class PackersInput
    {
        public int ID { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public string BarcodeID { get; set; }
        public int ShopID { get; set; }
        public int ChangeSource { get; set; }
        public string User { get; set; }
    }
}