using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spDeliveryPayment_ByAgentID_Sel
    {
        public int DeliveryPaymentId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> ShopId { get; set; }
        public Nullable<int> DeliveryAgentId { get; set; }
        public string JobId { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public Nullable<double> AmountPaid { get; set; }
        public Nullable<bool> DelPaymentConfirm { get; set; }
        public Nullable<bool> ShopAcknowledgement { get; set; }
    }
}