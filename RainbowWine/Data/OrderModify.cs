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
    
    public partial class OrderModify
    {
        public int Id { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> OrderType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string UserId { get; set; }
        public Nullable<double> TotalAmt { get; set; }
        public Nullable<double> UpdateAmt { get; set; }
        public Nullable<double> AdjustAmt { get; set; }
    }
}
