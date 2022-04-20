using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ProductBarcodeLinkDO : ProductBarcodeLink
    {
        public WineShop Shop { get; set; }
    }
}