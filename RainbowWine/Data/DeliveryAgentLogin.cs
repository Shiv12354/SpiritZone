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
    
    public partial class DeliveryAgentLogin
    {
        public int Id { get; set; }
        public int DeliveryAgentId { get; set; }
        public System.DateTime OnDuty { get; set; }
        public Nullable<System.DateTime> OffDuty { get; set; }
        public Nullable<bool> IsOnOff { get; set; }
        public Nullable<int> BreaksInMinutes { get; set; }
    
        public virtual DeliveryAgent DeliveryAgent { get; set; }
    }
}
