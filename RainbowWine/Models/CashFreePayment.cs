using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class CashFreePayment
    {
        public string InputParam { get; set; }
        public string VenderOutput { get; set; }
        public int OrderId { get; set; }
    }
}