using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spDeliveryAgents_ByShopID_Sel_Result
    {
        public int Id { get; set; }
        public string DeliveryExecName { get; set; }

        public int ShopID { get; set; }
        public Nullable<System.DateTime> LastDeliveryOn { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string TravelMode { get; set; }
        public string Coverage { get; set; }
        public Nullable<int> DeliverySlotID { get; set; }
        public Nullable<bool> IsAtShop { get; set; }
        public string ExciseID { get; set; }
        public string DocPath { get; set; }
        public Nullable<bool> Present { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public string DelType { get; set; }
        public Nullable<int> isActive { get; set; }
        public string Email { get; set; }
    }
}