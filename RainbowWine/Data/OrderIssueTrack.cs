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
    
    public partial class OrderIssueTrack
    {
        public int OrderIssueTrackId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> OrderIssueId { get; set; }
        public string UserId { get; set; }
        public Nullable<System.DateTime> TrackDate { get; set; }
        public Nullable<int> OrderStatusId { get; set; }
        public string Remark { get; set; }
    }
}
