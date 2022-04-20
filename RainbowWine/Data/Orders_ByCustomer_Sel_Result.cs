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
    
    public partial class Orders_ByCustomer_Sel_Result
    {
        public int Id { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string OrderPlacedBy { get; set; }
        public string OrderTo { get; set; }
        public decimal OrderAmount { get; set; }
        public int CustomerId { get; set; }
        public Nullable<int> CustomerAddressId { get; set; }
        public int OrderStatusId { get; set; }
        public Nullable<int> ShopID { get; set; }
        public string LicPermitNo { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string DeliveryPickup { get; set; }
        public string PaymentDevice { get; set; }
        public string PaymentQRImage { get; set; }
        public Nullable<short> PreOrder { get; set; }
        public bool TestOrder { get; set; }
        public string NewOrderId { get; set; }
        public string CommitEta { get; set; }
        public int OdetailId { get; set; }
        public int Id1 { get; set; }
        public int OrderId { get; set; }
        public int ProductID { get; set; }
        public int ItemQty { get; set; }
        public decimal Price { get; set; }
        public int ShopID1 { get; set; }
        public Nullable<bool> Issue { get; set; }
        public int OStatusId { get; set; }
        public int Id2 { get; set; }
        public string OrderStatusName { get; set; }
        public Nullable<int> id3 { get; set; }
        public Nullable<int> OrderID1 { get; set; }
        public Nullable<int> ShopID2 { get; set; }
        public string DestPlaceID { get; set; }
        public string OrigPlaceID { get; set; }
        public Nullable<int> CustID { get; set; }
        public string RoutePlanLink { get; set; }
        public Nullable<int> OrderStatusId1 { get; set; }
        public Nullable<System.DateTime> DeliveryStart { get; set; }
        public Nullable<System.DateTime> DeliveryEnd { get; set; }
        public Nullable<int> DeliveryAgentId { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<int> ZoneID1 { get; set; }
        public Nullable<System.DateTime> SlotStart { get; set; }
        public Nullable<System.DateTime> SlotEnd { get; set; }
        public string JobId { get; set; }
        public Nullable<bool> isOutForDelivery { get; set; }
    }
}
