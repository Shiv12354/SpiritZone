using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spInventoryTrack_Sel_Grouped
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> PackingSize { get; set; }//PackageSize
        public string Size { get; set; }//Capacity
        public Nullable<double> Price { get; set; }
        public Nullable<int> QtySold { get; set; }
        public Nullable<int> QtyErrorSale { get; set; }
        public Nullable<int> QtyReturn { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
    }
}