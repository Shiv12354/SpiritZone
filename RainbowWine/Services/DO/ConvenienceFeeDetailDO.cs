using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ConvenienceFeeDetailDO
    {
        public int OrderId { get; set; }
        public double ConvenienceFee { get; set; }
        public double TaxOnConvenienceFee { get; set; }
        public int PermitCost { get; set; }
        public double OrderAmount { get; set; }
        public double WalletAmount { get; set; }
        public double RefundAmount { get; set; }
    }
}