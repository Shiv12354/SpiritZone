using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class FiltersDO
    {
        public List<PricerangeExtDO> PricerangeExtDO { get; set; }
        public List<ProductCategoryExtDO> ProductCategoryExtDO { get; set; }
        public List<ProductBrandExtDO> ProductBrandExtDO { get; set; }
        public List<ProductSizeExtDO> ProductSizeExtDO { get; set; }
        public List<RegionExtDO> RegionExtDO { get; set; }
    }

    public class PricerangeExtDO
    {
        public int StartPrice { get; set; }
        public int EndPrice { get; set; }
    }

    public class ProductBrandExtDO
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
    }
    public class ProductSizeExtDO
    {
        public string Capacity { get; set; }
    }
    public class RegionExtDO
    {
        public string RegionName { get; set; }
    }
}