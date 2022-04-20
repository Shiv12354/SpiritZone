using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class OrderModel
    {
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
    }
}
