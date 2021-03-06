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
    
    public partial class DeliveryZone
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeliveryZone()
        {
            this.DeliveryAgents = new HashSet<DeliveryAgent>();
        }
    
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public Nullable<int> ShopID { get; set; }
        public Nullable<bool> OperationalFlag { get; set; }
        public Nullable<int> PriceSlab { get; set; }
        public Nullable<int> GiftOperational { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryAgent> DeliveryAgents { get; set; }
        public virtual WineShop WineShop { get; set; }
    }
}
