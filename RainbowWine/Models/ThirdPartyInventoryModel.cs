using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class ThirdPartyInventoryModel
    {
        public List<ThirdPartyInventory> Data { get; set; }
    }

    public class ThirdPartyInventory
    {
        public string ItemCode { get; set; }
        public int CLBalance { get; set; }
        public string TransTimestamp { get; set; }
        public int ShopCode { get; set; }
    }

    public class ThirdPartyResponse
    {
        public string Status_Msg { get; set; }
        public string NegativeStock { get; set; }
        public string NonExistentItems { get; set; }
        public DateTime Timestamp { get; set; }
    }
}