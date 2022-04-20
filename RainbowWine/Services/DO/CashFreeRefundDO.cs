using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class CashFreeRefundDO
    {
        public int CashFreeRefundId { get; set; }
        public int IssueId { get; set; }
        public int OrderModifyId { get; set; }
        public int OrderId { get; set; }
        public string InputParam { get; set; }
        public string VenderOutput { get; set; }
        public string Amt { get; set; }
        public string ProcessedDate { get; set; }
        public string ProcessedStatus { get; set; }
        public string ARNNumber { get; set; }
        public bool IsNotify { get; set; }
    }
}