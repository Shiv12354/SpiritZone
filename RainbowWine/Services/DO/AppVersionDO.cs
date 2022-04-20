using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class AppVersionDO
    {
        public int OrderPlatformId { get; set; }
        public int OrderId { get; set; }
        public string AppPlatForm { get; set; }
        public string AppVersion { get; set; }
    }
}