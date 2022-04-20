using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class RatingStarttExtDO
    {
        public int Star { get; set; }
        public int TotalCount { get; set; }
    }

    public class OrderDetailsExtDO
    {
        public int OrderId { get; set; }
        public int TotalLineItem { get; set; }
        public int OrderAmount { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}