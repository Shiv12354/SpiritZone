using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ScheduledDeliveriesDO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ContactNo { get; set; }
        public decimal OrderAmount { get; set; }
        public string CustomerName { get; set; }
        public string ShopName { get; set; }
        public string OrderStatusName { get; set; }
        public string PayType { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
    }
}