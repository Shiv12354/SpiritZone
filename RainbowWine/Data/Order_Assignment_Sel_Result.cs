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
    
    public partial class Order_Assignment_Sel_Result
    {
        public int Id { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderType { get; set; }
        public int CustomerAddressId { get; set; }
        public string Address { get; set; }
        public string FormattedAddress { get; set; }
        public string Flat { get; set; }
        public string Landmark { get; set; }
        public Nullable<int> EtaId { get; set; }
        public Nullable<int> Id1 { get; set; }
        public string Eta { get; set; }
    }
}
