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
    
    public partial class OfflinePayment
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string CollectionType { get; set; }
        public System.DateTime CollectedDate { get; set; }
        public int ShopID { get; set; }
        public int OrderID { get; set; }
    }
}
