using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.ThirdParty
{
    public class RecommendedParamters
    {
        public int[] ProductIds { get; set; }
        public int ShopId { get; set; }
        public DateTime? DOB { get; set; }
        public int ProductID { get; set; }
        public string Address { get; set; }
        public int NUM { get; set; }
        public int CustomerId { get; set; }
        public int[] ProductIdsForPurchase { get; set; }
        public int[] ProductIdsForlocation { get; set; }

    }
}