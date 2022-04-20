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
    
    public partial class Orders_DeliverySearch_Sel_Result
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
        public string OrderType { get; set; }
        public string CommitEta { get; set; }
        public Nullable<bool> Issue { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public string OrderGroupId { get; set; }
        public string OrderContain { get; set; }
        public Nullable<int> DiscountTypeId { get; set; }
        public Nullable<double> DiscountUnit { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public string OrderGroupType { get; set; }
        public Nullable<int> PromoId { get; set; }
        public Nullable<int> CashBackId { get; set; }
        public Nullable<double> CashBackAmount { get; set; }
        public Nullable<decimal> WalletAmountUsed { get; set; }
        public Nullable<double> PromoDiscountAmount { get; set; }
        public int StatusId { get; set; }
        public int Id1 { get; set; }
        public string OrderStatusName { get; set; }
        public string Description { get; set; }
        public int oDetail { get; set; }
        public int Id2 { get; set; }
        public int OrderId { get; set; }
        public int ProductID { get; set; }
        public int ItemQty { get; set; }
        public decimal Price { get; set; }
        public int ShopID1 { get; set; }
        public Nullable<bool> Issue1 { get; set; }
        public Nullable<int> DiscountProductId { get; set; }
        public Nullable<double> DiscountUnit1 { get; set; }
        public Nullable<double> DiscountAmount1 { get; set; }
        public Nullable<int> PromoId1 { get; set; }
    }
}
