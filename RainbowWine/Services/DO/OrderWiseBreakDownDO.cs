using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class OrderWiseBreakDownDO
    {
        public int OrderId { get; set; }
        public int PrepaidOnlinecollectionamount { get; set; }
        public int LicPermitAmount { get; set; }
        public int WalletAmountUsed { get; set; }
        public int PODonlinepaymentamount { get; set; }
        public int PODCashPaymentAmount { get; set; }
        public int Cashcollectedbyshop { get; set; }
        public int CashRefundedbyshop { get; set; }
        public string OrderType { get; set; }
        public int MixerSales { get; set; }
        public int Total { get; set; }
        public string ShopName { get; set; }
    }
}