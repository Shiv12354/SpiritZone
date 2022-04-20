using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class CustomerAddressDO
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }

        public int OrderId { get; set; }
        public bool IsOrderStart { get; set; }
        public bool IsDelivered { get; set; }
        public int AgentId { get; set; }

    }
}