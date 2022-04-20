using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spOrderIssue_Sel
    {
        public int OrdId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public decimal OrderAmount { get; set; }
        public int OrderStatusId { get; set; }
        public int IssueStatusId { get; set; }

        public String CustomerName { get; set; }
        public String OrderStatusName { get; set; }
        public int OrderIssueId { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
    }
}