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
    
    public partial class Get_Order_With_Review_address_Result
    {
        public int Id { get; set; }
        public System.DateTime OrderDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string LicPermitNo { get; set; }
        public Nullable<int> ShopID { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public Nullable<double> OrderAmount { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public int OStatisId { get; set; }
        public string OrderStatusName { get; set; }
        public string UserID { get; set; }
        public int CustomerAddressId { get; set; }
        public int CustomerId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Address { get; set; }
        public string FormattedAddress { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public string PlaceId { get; set; }
        public string Flat { get; set; }
        public string Landmark { get; set; }
        public int ShopId1 { get; set; }
        public string AddressType { get; set; }
        public Nullable<int> ZoneId1 { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<int> Rating { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
    }
}
