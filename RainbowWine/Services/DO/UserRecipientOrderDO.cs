using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class UserRecipientOrderDO
    {
        public int UserRecipientOrderId { get; set; }
        public string UserId { get; set; }
        public int GiftRecipientId { get; set; }
        public int OrderId { get; set; }
        public string Occasion { get; set; }
        public string SplMessage { get; set; }
        public bool IsOTPVerified { get; set; }
        public string ContactNo { get; set; }
        public string RecipientName { get; set; }
        public string GiftUrl { get; set; }
        public string Content { get; set; }
        public string CustContactNo { get; set; }
        public string CustomerName { get; set; }
    }

    public class OTPCount
    {
        public int Minutes { get; set; }
        public int cnt { get; set; }

    }

    public class UserRecipient
    {
        public string RecipientName { get; set; }
        public string RecipientNumber { get; set; }

    }
}