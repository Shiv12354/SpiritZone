using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class CollectionDetails
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int ShopID { get; set; }
        public double OrderAmount { get; set; }
        public string  LicPermitNo { get; set; }
        public double? WalletAmountUsed { get; set; }
        public DateTime TrackDate { get; set; }
    }
}