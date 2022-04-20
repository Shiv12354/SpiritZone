using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class ProductAdminSize
    {
        public int ProductSizeID { get; set; }
        public string Capacity { get; set; }
        public string SizeName { get; set; }
        public IList<ProductAdminSize> ProductAdminSizeList { get; set; }
    }
    public class ProductCategories
    {
        public string Category { get; set; }
        public IList<ProductCategories> ProductCategoriesList { get; set; }
    }
}