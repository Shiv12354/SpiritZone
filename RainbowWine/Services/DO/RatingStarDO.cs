using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
   
    public class RatingStarttDO
    {
        public int Star { get; set; }
        public int TotalCount { get; set; }
    }

    public class ProductDetailExtDO
    {
        public int ProductRefID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductCategory { get; set; }
        public int Rating { get; set; }
        public int Price { get; set; }
        public int ProductRating { get; set; }
    }
}