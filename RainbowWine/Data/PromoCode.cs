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
    
    public partial class PromoCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PromoCode()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Orders = new HashSet<Order>();
            this.PromoApplies = new HashSet<PromoApply>();
        }
    
        public int PromoId { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> PromoTypeId { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string CalculationType { get; set; }
        public string Title { get; set; }
        public string SubText { get; set; }
        public string Description { get; set; }
        public Nullable<double> MaximumDiscount { get; set; }
        public Nullable<bool> ForCredibleUser { get; set; }
        public Nullable<bool> BrandPromotion { get; set; }
        public Nullable<bool> PreLaunchOffer { get; set; }
        public Nullable<bool> isPostdated { get; set; }
        public Nullable<int> DaysFromCurrent { get; set; }
        public Nullable<int> DaysFromStart { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromoApply> PromoApplies { get; set; }
        public virtual PromoType PromoType { get; set; }
    }
}