using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ConfigMasterDO
    {
        public int ConfigMasterId { get; set; }
        public string KeyText { get; set; }
        public bool ValueText { get; set; }
        public string Description { get; set; }
    }
}