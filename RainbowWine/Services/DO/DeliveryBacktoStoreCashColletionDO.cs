using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class DeliveryBacktoStoreCashColletionDO
    {
        public int DeliveryAgentId { get; set; }
        public string DeliveryExecName { get; set; }
        public int OrderId { get; set; }
        public string ShopName { get; set; }
        public bool ShopAcknowledgement { get; set; }
        public string Type { get; set; }
    }
}