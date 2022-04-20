using RainbowWine.Data;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Models
{
    public class DeliveryAgentAssignmentModel
    {
        public int ShopId { get; set; }
        public SelectList Shops { get; set; }
        public IList<OrderDO> Orders { get; set; }
        public SelectList DeliveryAgents { get; set; }
    }
}