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
    
    public partial class RecommendationLog
    {
        public int RecoId { get; set; }
        public int CustomerId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<int> ProductIDReq { get; set; }
        public int ProductIDRecom { get; set; }
        public int ShopID { get; set; }
        public string CustAddress { get; set; }
        public bool isClicked { get; set; }
        public Nullable<int> RecoType { get; set; }
    }
}
