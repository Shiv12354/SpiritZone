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
    using RainbowWine.Services.DO;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.DeliveryBackToStores = new HashSet<DeliveryBackToStore>();
            this.DeliveryPayments = new HashSet<DeliveryPayment>();
            this.DumpRoutePlans = new HashSet<DumpRoutePlan>();
            this.OrderDetails = new HashSet<OrderDetail>();
            this.OrderIssues = new HashSet<OrderIssue>();
            this.OrderIssueDetails = new HashSet<OrderIssueDetail>();
            this.PayLinks = new HashSet<PayLink>();
            this.RoutePlans = new HashSet<RoutePlan>();
            this.MixerOrderItems = new HashSet<MixerOrderItem>();
            this.GiftBagOrderItems = new HashSet<GiftBagOrderItem>();
        }
    
        public int Id { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string OrderPlacedBy { get; set; }
        public string OrderTo { get; set; }
        public decimal OrderAmount { get; set; }
        public int CustomerId { get; set; }
        public Nullable<int> CustomerAddressId { get; set; }
        public int OrderStatusId { get; set; }
        public Nullable<int> ShopID { get; set; }
        public string LicPermitNo { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string DeliveryPickup { get; set; }
        public string PaymentDevice { get; set; }
        public string PaymentQRImage { get; set; }
        public Nullable<short> PreOrder { get; set; }
        public bool TestOrder { get; set; }
        public string NewOrderId { get; set; }
        public string OrderType { get; set; }
        public string CommitEta { get; set; }
        public Nullable<bool> Issue { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public Nullable<int> DiscountTypeId { get; set; }
        public Nullable<double> DiscountUnit { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public string OrderGroupId { get; set; }
        public string OrderContain { get; set; }
        public string OrderGroupType { get; set; }
        public Nullable<int> PromoId { get; set; }
        public Nullable<int> CashBackId { get; set; }
        public Nullable<double> CashBackAmount { get; set; }
        public Nullable<decimal> WalletAmountUsed { get; set; }
        public Nullable<double> PromoDiscountAmount { get; set; }
        public Nullable<bool> Prebook { get; set; }
        public Nullable<bool> IsGift { get; set; }
        public Nullable<int> MixerIssueDetailId { get; set; }
        public Nullable<System.DateTime> PickedUpDate { get; set; }
        public Nullable<System.DateTime> EtaEndTime { get; set; }
        public Nullable<System.DateTime> EtaStartTime { get; set; }
        public string AppVersion { get; set; }
        public string AppPlatForm { get; set; }
        public string giftDetail { get; set; }
        public string RecipientName { get; set; }
        public string RecipientNumber { get; set; }
        public string ARNNumber { get; set; }
        public string ProcessedDate { get; set; }

        public List<GiftBagSearchDO> listItem
        {
            get
            {
                var items = new List<GiftBagSearchDO>();
                try
                {

                    var str1 = giftDetail != null ? giftDetail.Split('|') : string.Empty.Split('|');

                    if (str1.Length > 0)
                    {
                        foreach (var item in str1)
                        {
                            var str2 = item.Split(',');

                            if (str2.Count() > 0)
                                str2 = item.Split(new string[] { "##" }, StringSplitOptions.None);

                            var newItem = new GiftBagSearchDO();
                            newItem.GiftBagDetailId = Convert.ToInt32(str2[0]);
                            newItem.GiftBagOrderItemId= Convert.ToInt32(str2[1]);
                            newItem.Price = Convert.ToInt32(str2[2]);
                            newItem.BagImageUrl = str2[3];
                            newItem.Qty = Convert.ToInt32(str2[4]);
                            newItem.GiftBagName = str2[5];
                            items.Add(newItem);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                return items;
            }
        }

        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryBackToStore> DeliveryBackToStores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryPayment> DeliveryPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DumpRoutePlan> DumpRoutePlans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderIssue> OrderIssues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderIssueDetail> OrderIssueDetails { get; set; }
        public virtual OrderStatu OrderStatu { get; set; }
        public virtual WineShop WineShop { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayLink> PayLinks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoutePlan> RoutePlans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MixerOrderItem> MixerOrderItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiftBagOrderItem> GiftBagOrderItems { get; set; }
        public virtual PromoCode PromoCode { get; set; }
    }
}