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
    
    public partial class ScheduleDelivery
    {
        public int ScheduleDeliveryId { get; set; }
        public string OrderId { get; set; }
        public Nullable<int> DeliveryAgentId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ShopId { get; set; }
        public Nullable<System.DateTime> ScheduledStart { get; set; }
        public Nullable<System.DateTime> ScheduledEnd { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
