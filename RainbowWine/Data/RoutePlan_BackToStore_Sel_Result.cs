//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RainbowWine.Data
{
    using System;
    
    public partial class RoutePlan_BackToStore_Sel_Result
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
        public System.DateTime OrderDate { get; set; }
        public decimal OrderAmount { get; set; }
        public int OrderStatusId { get; set; }
        public int StatusId { get; set; }
        public string OrderStatusName { get; set; }
        public Nullable<int> DeliveryId { get; set; }
        public string DeliveryExecName { get; set; }
        public int CustId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Reason { get; set; }
        public Nullable<double> OrderAmount1 { get; set; }
        public Nullable<System.DateTime> CollectedDate { get; set; }
    }
}
