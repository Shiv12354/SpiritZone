using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class NotifyOrderDO: NotifyOrder
    {
        public WineShop Shop { get; set; }
        public ProductDetail ProductDetail { get; set; }
        public Customer Customer { get; set; }
        public NotifyHandle NotifyReview { get; set; }
        public Inventory Inventory { get; set; }
    }
}