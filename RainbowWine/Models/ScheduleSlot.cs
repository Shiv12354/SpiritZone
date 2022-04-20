using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class ScheduleSlot
    {
        public string Title { get; set; }
        public int Date { get; set; }
        public string FullDate { get; set; }
        public string DisplayDay { get; set; }
        public string Day { get; set; }
        public string TimeSlot { get; set; }
        public string DeliveryCharge { get; set; }
        public string OrderGroupId { get; set; }
    }
}