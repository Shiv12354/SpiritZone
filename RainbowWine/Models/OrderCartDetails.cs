using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class OrderCartDetails
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public bool IsMixer { get; set; }
        public string OrderGroupId { get; set; }
        public int SupplierId { get; set; }
        
    }
}