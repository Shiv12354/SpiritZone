using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class BrandManufacturerDO
    {
        public int BrandManufacturerId { get; set; }
        public int ManufacturerId { get; set; }
        public int BrandId { get; set; }
        public string ManufacturerName { get; set; }
        public string BrandName { get; set; }
    }
}