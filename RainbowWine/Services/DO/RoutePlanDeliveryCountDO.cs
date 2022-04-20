using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class RoutePlanDeliveryCountDO
    {
        public double? onlineamt { get; set; }
        public float? cash { get; set; }
        public int backtostore { get; set; }
    }
}