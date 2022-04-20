using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class OrderDetailMrpIssueDO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductID { get; set; }
        public int ItemQty { get; set; }
        public decimal Price { get; set; }
        public int ShopID { get; set; }
        public bool IsMrpIssue { get; set; }
        public int NewMrp { get; set; }
        public int NewAmount { get; set; }
        public ProductDetail ProductDetail { get; set; }
    }
}