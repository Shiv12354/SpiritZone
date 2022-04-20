using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class OrderTrackingDO
    {
        public int OrderStatusId { get; set; }
        public string StatusName  { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
        public bool IsCurrent { get; set; }
        public string DeliveryBoyNumber { get; set; }

       
    }
}