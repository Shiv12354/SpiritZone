using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spRoutePlan_BackToStore_Archive_Sel
    {
        public int id { get; set; }
        public int OrderID { get; set; }
        public int ShopID { get; set; }
        public System.DateTime DeliveryStart { get; set; }
        public System.DateTime DeliveryEnd { get; set; }
        public Nullable<int> DeliveryAgentId { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<System.DateTime> SlotStart { get; set; }
        public Nullable<System.DateTime> SlotEnd { get; set; }
        public string JobId { get; set; }
        public Nullable<bool> isOutForDelivery { get; set; }
        public int OrdId { get; set; }
        public decimal OrderAmount { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int OrderStatusId { get; set; }
        public int StatusId { get; set; }
        public string OrderStatusName { get; set; }
        public int DeliveryId { get; set; }
        public string DeliveryExecName { get; set; }
        public int CustId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> CollectedDate { get; set; }
        public decimal AOrderAmount { get; set; }
        public string Reason { get; set; }
    }
}