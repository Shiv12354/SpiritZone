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
    
    public partial class ProductDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductDetail()
        {
            this.DiscountProducts = new HashSet<DiscountProduct>();
            this.Inventories = new HashSet<Inventory>();
            this.OrderDetails = new HashSet<OrderDetail>();
            this.OrderDetailModifies = new HashSet<OrderDetailModify>();
            this.OrderDetailModifyOriginals = new HashSet<OrderDetailModifyOriginal>();
            this.OrderIssueDetails = new HashSet<OrderIssueDetail>();
            this.OrderIssueDetailOriginals = new HashSet<OrderIssueDetailOriginal>();
            this.MixerProductLinks = new HashSet<MixerProductLink>();
        }
    
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> Size { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Category { get; set; }
        public string ProductType { get; set; }
        public Nullable<double> Price { get; set; }
        public string ShopItemId { get; set; }
        public string ProductImage { get; set; }
        public string ProductThumbImage { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<bool> IsFeature { get; set; }
        public Nullable<int> ProductRefID { get; set; }
        public Nullable<int> ProductSizeID { get; set; }
        public string ProductHomeImage { get; set; }
        public Nullable<bool> IsShowcaseView { get; set; }
        public Nullable<bool> IsShowcasePremiumView { get; set; }
        public Nullable<bool> IsPremium { get; set; }
        public Nullable<bool> IsGift { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiscountProduct> DiscountProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inventory> Inventories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailModify> OrderDetailModifies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailModifyOriginal> OrderDetailModifyOriginals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderIssueDetail> OrderIssueDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderIssueDetailOriginal> OrderIssueDetailOriginals { get; set; }
        public virtual Product Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MixerProductLink> MixerProductLinks { get; set; }
    }
}
