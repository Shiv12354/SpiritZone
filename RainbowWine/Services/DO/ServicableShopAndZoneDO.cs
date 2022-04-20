using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ServicableShopAndZoneDO
    {
        public int? ShopID { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public bool OperationalFlag { get; set; }
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
    }

    public class WineShopDO
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string ShopPhoneNo { get; set; }
        public bool OperationFlag { get; set; }
        public string Vat { get; set; }


    }

    public class ZoneDO
    {
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public bool OperationalFlag { get; set; }

    }

}