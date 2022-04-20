using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class GiftBagSearchDO
    {
        public int GiftBagDetailId { get; set; }
        public string GiftBagName { get; set; }
        public int Price { get; set; }
        public bool IsGift { get; set; }
        public int Qty { get; set; }
        public string BagImageUrl { get; set; }
        public int? OrderId { get; set; }
        public int? GiftBagOrderItemId { get; set; }
    }

}