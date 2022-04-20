using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Models
{
    public class RoutePlanViewModel
    {
        public int id { get; set; }
        public int OrderID { get; set; }
        public int ShopID { get; set; }
        public int OrderStatusId { get; set; }
        public string JobId { get; set; }
        public Nullable<int> DeliveryAgentId { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual DeliveryAgent DeliveryAgent { get; set; }
        public virtual Order Order { get; set; }
        public virtual OrderStatu OrderStatu { get; set; }
        public virtual WineShop WineShop { get; set; }
        public SelectList SectionAgent { get; set; }
        public SelectList SectionShop { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
    }
}