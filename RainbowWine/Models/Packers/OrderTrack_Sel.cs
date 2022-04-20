using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class OrderTrack_Sel
    {
        public int OrderID { get; set; }
        public System.DateTime DeliveryStart { get; set; }
        public System.DateTime DeliveryEnd { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<System.DateTime> SlotStart { get; set; }
        public Nullable<System.DateTime> SlotEnd { get; set; }
        public string OrderStatusName { get; set; }
        public string DeliveryExecName { get; set; }
        public Nullable<System.DateTime> TrackDate { get; set; }
        public decimal OrderAmount { get; set; }
    }
}