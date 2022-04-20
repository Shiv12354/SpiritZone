using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ManufacturerDO
    {
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerAbbreviated { get; set; }
        public string Region { get; set; }
        public string CollabSince { get; set; }
    }
}