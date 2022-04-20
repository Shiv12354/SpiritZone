using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class ScheduleParameters
    {
        public string OrderId { get; set; }
        public int DeliveryAgentId { get; set; }
        public int ShopId { get; set; }
        public DateTime ScheduleStart { get; set; }
        public DateTime ScheduleEnd { get; set; }
        public int CustomerId { get; set; }
    }
}