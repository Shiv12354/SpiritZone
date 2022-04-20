using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ProductDetailsExtDO: ProductDetail
    {
        public ProductDetailsExtDO()
        { 
            this.ProductBarcodeLinks = new HashSet<ProductBarcodeLinkDO>();
        }
        public ProductSize prdSize { get; set; }
        public virtual ICollection<ProductBarcodeLinkDO> ProductBarcodeLinks { get; set; }
    }
}