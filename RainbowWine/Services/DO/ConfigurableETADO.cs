using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ConfigurableETADO
    {
        public int ConfigurableETAId { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string DeliveryStartHours { get; set; }
        public string DeliveryEndHours { get; set; }
        
        public DateTime? DryDay { get; set; }
        public string FirstDeliverySTInMin { get; set; }
        public string FirstDeliveryETInMin { get; set; }
        public int DelDeadLineStart { get; set; }
        public int DelDeadLineEnd { get; set; }
        public string Remarks { get; set; }
        public string ModifiedBy { get; set; }
    }
}