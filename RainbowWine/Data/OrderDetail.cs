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
    using System.Collections.Generic;
    
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductID { get; set; }
        public int ItemQty { get; set; }
        public decimal Price { get; set; }
        public int ShopID { get; set; }
        public Nullable<bool> Issue { get; set; }
        public Nullable<int> DiscountProductId { get; set; }
        public Nullable<double> DiscountUnit { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<int> PromoId { get; set; }
        public Nullable<bool> IsMrpIssue { get; set; }
        public Nullable<bool> IsDiscountApplied { get; set; }
        public Nullable<decimal> OriginalPrice { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual WineShop WineShop { get; set; }
        public virtual PromoCode PromoCode { get; set; }
        public string GName { get; set; }
    }
}
