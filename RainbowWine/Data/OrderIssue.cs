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
    
    public partial class OrderIssue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderIssue()
        {
            this.OrderIssueDetails = new HashSet<OrderIssueDetail>();
            this.OrderIssueTickets = new HashSet<OrderIssueTicket>();
            this.MixerIssueDetails = new HashSet<MixerIssueDetail>();
            this.MixerIssueDetailOriginals = new HashSet<MixerIssueDetailOriginal>();
            this.GiftBagOrderItemIssueDetails = new HashSet<GiftBagOrderItemIssueDetail>();
            this.GiftBagOrderItemIssueDetailOriginals = new HashSet<GiftBagOrderItemIssueDetailOriginal>();
        }
    
        public int OrderIssueId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> OrderIssueTypeId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> OrderStatusId { get; set; }
        public string UserId { get; set; }
        public Nullable<double> TotalAmt { get; set; }
        public Nullable<double> UpdatedAmt { get; set; }
        public Nullable<double> AdjustAmt { get; set; }
        public Nullable<bool> IsCashOnDelivery { get; set; }
        public Nullable<bool> IsWalletRefund { get; set; }
    
        public virtual OrderIssueType OrderIssueType { get; set; }
        public virtual OrderIssueType OrderIssueType1 { get; set; }
        public virtual Order Order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderIssueDetail> OrderIssueDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderIssueTicket> OrderIssueTickets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MixerIssueDetail> MixerIssueDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MixerIssueDetailOriginal> MixerIssueDetailOriginals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiftBagOrderItemIssueDetail> GiftBagOrderItemIssueDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiftBagOrderItemIssueDetailOriginal> GiftBagOrderItemIssueDetailOriginals { get; set; }
    }
}
