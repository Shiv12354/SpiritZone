using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class DailyPODCashCollectionDO
    {
        public int OrderId { get; set; }
        public string DeliveryDate { get; set; }
        public int PODCashPaymentAmount { get; set; }
        public int LicPermitAmount { get; set; }
        public int Total { get; set; }
        public string ShopName { get; set; }
    }
}