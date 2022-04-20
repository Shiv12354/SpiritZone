using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class HTPenaltyDetailsDO
    {
        public int DeliveryAgentId { get; set; }
        public string DeliveryAgentName { get; set; }
        public string PenaltyType { get; set; }
        public int PenaltyTypeCount { get; set; }
        public string ModifiedDate { get; set; }
    }
}