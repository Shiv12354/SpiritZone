using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ProductCatViewDO
    {
        public float PriceStart { get; set; }
        public float PriceEnd { get; set; }
        public List<ProductCategoryDO> ProductCategoryDOs { get; set; }

    }
    public class ProductCategoryDO
    {
        public int ProductID { get; set; }
        public int ShopId { get; set; }
        public float Price { get; set; }
        public string ProductName { get; set; }
        public int ProductCategoryID { get; set; }
        public string ProductCategory { get; set; }
        public int ParentID { get; set; }
        public bool IsForFilter { get; set; }
       
    }

    public class ProductCategoryExtDO
    {
        public ProductCategoryExtDO()
        {
            this.SubProductCategory = new HashSet<ProductCategoryExtDO>();
        }
        public int ProductCategoryID { get; set; }
        public string ProductCategory { get; set; }
       
        public ICollection<ProductCategoryExtDO> SubProductCategory { get; set; }
    }
}