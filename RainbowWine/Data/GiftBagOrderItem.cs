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
    
    public partial class GiftBagOrderItem
    {
        public int GiftBagOrderItemId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string OrderGroupId { get; set; }
        public Nullable<int> GiftBagDetailId { get; set; }
        public Nullable<int> ItemQty { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<int> ShopId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<bool> Issue { get; set; }
        public string GiftBagName { get; set; }
        public string ShopName { get; set; }
        public string OrderPlacedBy { get; set; }
        public string ImageUrl { get; set; }

        public virtual GiftBagDetail GiftBagDetails { get; set; }
    }
}
