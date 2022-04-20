using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class CartInput
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Double UnitPrice { get; set; }
        public string ProductImage { get; set; }
        public int Qty { get; set; }
        public bool IsMixer { get; set; }
        public bool IsReserve { get; set; }
        public string MixerType { get; set; }
        public int MixerRefId { get; set; }


    }
}