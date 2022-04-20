using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spDeliveryBackToStore_ByAgentID_Sel
    {
        public int DeliveryBackToStoreId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int OrderId { get; set; }
        public int ShopId { get; set; }
        public Nullable<int> DeliveryAgentId { get; set; }
        public string JobId { get; set; }
        public Nullable<bool> ShopAcknowledgement { get; set; }
        public double OrderAmount { get; set; }
        public string Reason { get; set; }
    }
}