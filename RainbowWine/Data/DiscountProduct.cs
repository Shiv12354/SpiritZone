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
    
    public partial class DiscountProduct
    {
        public int DiscountProductId { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<double> DiscountUnit { get; set; }
        public Nullable<double> DiscountMaxAmt { get; set; }
        public Nullable<System.DateTime> ValidFrom { get; set; }
        public Nullable<System.DateTime> ValidUntil { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
