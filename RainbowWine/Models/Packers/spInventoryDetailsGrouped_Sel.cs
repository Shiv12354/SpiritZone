using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spInventoryDetailsGrouped_Sel
    {
        public int ProductID { get; set; }
        public string AppProductName { get; set; }
        public string Size { get; set; }//PackageSize
        public Nullable<int> PackingSize { get; set; }//PackageSize      
        public Nullable<double> Price { get; set; }
        public Nullable<int> SoldOnline { get; set; }
        public Nullable<int> SoldCounter { get; set; }
        public Nullable<int> TotalSold { get; set; }

        public Nullable<double> TotalAmount { get; set; }
        public Nullable<int> QtyAvailable { get; set; }
        public Nullable<int> QtyErrorSale { get; set; }
        public Nullable<int> QtyReturn { get; set; }
        public Nullable<int> ResQty { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
    }
}