using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.PaytmService
{
    public class PaytmSetApproveResponse
    {
        public string STATUS { get; set; }
        public string CHECKSUMHASH { get; set; }
        public string TXNAMOUNT { get; set; }
        public string ORDERID { get; set; }
        public string MERC_UNQ_REF { get; set; }
        public string LINKNOTES { get; set; }
        public string TXNID { get; set; }
        public string RESPMSG { get; set; }

    }
}