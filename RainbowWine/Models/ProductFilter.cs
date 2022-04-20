using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace RainbowWine.Models
{
    public class ProductFilter
    {
        public List<PriceRange> Prices { get; set; }
        public int[] CategoryId { get; set; }
        public int[] BrandId { get; set; }
        public string[] Volume { get; set; }
        public string[] Region { get; set; }
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public bool IsAscending { get; set; }
    }
    public class PriceRange
    {
        public string PriceStart { get; set; }
        public string PriceEnd { get; set; }
    }

    
}