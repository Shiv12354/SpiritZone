using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spInventory_Shop_Sel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int QtyAvailable { get; set; }
        public Nullable<double> MRP { get; set; }
        public Nullable<int> Size { get; set; }
        public string Capacity { get; set; }
        public System.DateTime LastModified { get; set; }
        public string ProductName1 { get; set; }
        public Nullable<bool> IsPremium { get; set; }
    }
}