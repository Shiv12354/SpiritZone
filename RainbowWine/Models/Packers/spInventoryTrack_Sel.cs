using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spInventoryTrack_Sel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> Size { get; set; }
        public string Capacity { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<int> QtyAvailAfter { get; set; }
        public Nullable<int> QtyAvailBefore { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}