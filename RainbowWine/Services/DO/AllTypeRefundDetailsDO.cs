using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class AllTypeRefundDetailsDO
    {
        public string Refund { get; set; }
        public string Amount { get; set; }
        public string CreatedDate { get; set; }
        public int IssueId { get; set; }
        public string ProcessedDate { get; set; }
        public string ARNNumber { get; set; }
    }
}