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
    
    public partial class OrderIssueDetailOriginal
    {
        public int OrderIssueDetailOriginalId { get; set; }
        public Nullable<int> OrderIssueId { get; set; }
        public Nullable<int> OrderDetailId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> ItemQty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> ShopId { get; set; }
        public Nullable<bool> Issue { get; set; }
        public Nullable<int> DiscountProductId { get; set; }
        public Nullable<double> DiscountUnit { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
    
        public virtual ProductDetail ProductDetail { get; set; }
    }
}