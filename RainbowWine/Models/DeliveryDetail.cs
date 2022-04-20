using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class DeliveryDetail
    {
        public RUser User { get; set; }
        public IList<Order> Orders { get; set; }
    }
}