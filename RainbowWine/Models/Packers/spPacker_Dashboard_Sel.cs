using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spPacker_Dashboard_Sel
    {
        public int Unpacked { get; set; }
        public int Packed { get; set; }
        public int Issue { get; set; }
        public int OutForDelivery { get; set; }
        public int BackToStore { get; set; }
        public int Delivered { get; set; }
        public int CancelOrder { get; set; }
    }
}