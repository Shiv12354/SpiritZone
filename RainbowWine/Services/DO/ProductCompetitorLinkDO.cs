using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ProductCompetitorLinkDO
    {
        //public int ProductCompetitorLinkId { get; set; }
        public int ProductRefID { get; set; }
        public int CompetitorProductRefIDs { get; set; }
        public string ProductName { get; set; }
        public string CopProductName { get; set; }
    }
}