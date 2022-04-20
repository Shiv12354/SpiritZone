using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class PackIssueInput
    {
        public int id { get; set; }
        public string[] odetailIds { get; set; }
        public string shopnumber { get; set; }
        public int shopId { get; set; }
        public string UserId { get; set; }
    }
}