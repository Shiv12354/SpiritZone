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
    
    public partial class Order_Zone_Sel_Result
    {
        public int Id { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string OrderTo { get; set; }
        public decimal OrderAmount { get; set; }
        public Nullable<int> ShopID { get; set; }
        public int OrderStatusId { get; set; }
        public int CustomerAddressId { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public string Address { get; set; }
    }
}
