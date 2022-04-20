using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class LiveOrderTracking
    {
        public int ShopId { get; set; }
        public double CustLongitude { get; set; }
        public double CustLatitude { get; set; }
        public double ShopLongitude { get; set; }
        public double ShopLatitude { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int DeliveryAgentId { get; set; }
        public int CustomerId { get; set; }
        public string StatusName { get; set; }
        public int OrderStatusId { get; set; }
        public int OrderAmount { get; set; }
        public string AgentContactNo { get; set; }
        public int PaymentTypeId { get; set; }
        public string HyperTrackDeviceId { get; set; }
        public string TripId { get; set; }
        public bool Prebook { get; set; }
        public bool IsScheduledOrder { get; set; }
        public int TotalItemQty { get; set; }
        public DateTime DeliveryScheduledStartDate { get; set; }
        public DateTime DeliveryScheduledEndDate { get; set; }


    }

    public class DeliveryAgentLocation
    {
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int DeliveryAgentId { get; set; }
       


    }
}