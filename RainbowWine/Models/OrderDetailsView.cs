using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class OrderDetailsView
    {
        public Order Ord { get; set; }
        public Customer Cust { get; set; }
        public OrderDetail OrdDetail { get; set; }
        public IList<OrderDetail> CurOrdDetail { get; set; }
        public CustomerSMSSend SmsSent { get; set; }
        public IList<MixerOrderItem> MixerItems { get; set; }

        public IList<GiftBagOrderItem> GiftBagItems { get; set; }
    }
}