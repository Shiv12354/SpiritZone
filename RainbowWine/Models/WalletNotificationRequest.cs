using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class WalletNotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string UserID { get; set; }
        public int CustomerID { get; set; }
       
    }
}