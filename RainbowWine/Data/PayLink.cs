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
    
    public partial class PayLink
    {
        public int OrderID { get; set; }
        public int id { get; set; }
        public Nullable<bool> PayExtraction { get; set; }
        public Nullable<int> PayLinkGenerated { get; set; }
        public System.DateTime PayExtractionDate { get; set; }
        public Nullable<System.DateTime> PayLinkGeneratedDate { get; set; }
        public Nullable<bool> PaymentStatus { get; set; }
        public Nullable<bool> SentFlag { get; set; }
    
        public virtual Order Order { get; set; }
    }
}
