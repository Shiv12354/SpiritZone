using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class ResendPaymentLink
    {
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
    }
}