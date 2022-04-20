using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class PenaltyNotification
    {
        public int DeliveryAgentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public double PenaltyAmount { get; set; }
        public string PenaltyDescription { get; set; }
        public string Solution { get; set; }
    }
}