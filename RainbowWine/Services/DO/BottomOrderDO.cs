using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{

    public class BottomOrderDO
    {
        public List<CurrentOrderDO> CurrentOrderDO { get; set; }
        public List<CurrentOrderDO> DeliverOrderDO { get; set; }
    }
    public class CurrentOrderDO
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public bool IsRated { get; set; }
        public int AverageRating { get; set; }
        public string Message { get; set; }
        public string ButtonText { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }

    public class DeliverOrderDO
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public bool IsRated { get; set; }
        public int AverageRating { get; set; }
        public string Message { get; set; }
        public string ButtonText { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}