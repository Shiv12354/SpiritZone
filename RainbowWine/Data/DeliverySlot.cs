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
    
    public partial class DeliverySlot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeliverySlot()
        {
            this.DeliveryAgents = new HashSet<DeliveryAgent>();
        }
    
        public int DeliverySlotID { get; set; }
        public System.DateTime DeliverySlot_StartTime { get; set; }
        public System.DateTime DeliverySlot_EndTime { get; set; }
        public System.DateTime OrderDate_StartTime { get; set; }
        public System.DateTime OrderDate_EndTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryAgent> DeliveryAgents { get; set; }
    }
}
