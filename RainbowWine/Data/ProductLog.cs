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
    
    public partial class ProductLog
    {
        public int ProductLogId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> ShopId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}