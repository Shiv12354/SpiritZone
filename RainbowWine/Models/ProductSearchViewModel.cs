using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class ProductSearchViewModel
    {
        public int ShopId { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int[] CategoryIds { get; set; }
        public string ProductName { get; set; }
        public float PriceStart { get; set; }
        public float PriceEnd { get; set; }
        public string SearchText { get; set; }
        public int Index { get; set; }
        public int Size { get; set; }
        public bool IsAscending { get; set; }
    }
}