using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RainbowWine.Data;

namespace RainbowWine.Services.DO
{
    public class RoutePlanDO:RoutePlan
    {
        public RoutePlanDO()
        {
            this.OrderTrack = new HashSet<OrderTrack>();
        }

        public CustomerAddress OrderAddress { get; set; }
        public bool OutForDelivery { get; set; }
        public DateTime? OutForDeliveryDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CustomerETA { get; set; }
        public CustomerEta ETA { get; set; }
        public string DelTrackUrl { get; set; }
        public int OrderIssueId { get; set; }
        public ICollection<OrderTrack> OrderTrack { get; set; }

    }
}