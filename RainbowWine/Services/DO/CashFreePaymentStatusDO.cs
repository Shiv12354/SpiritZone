using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{

    public class Upi
    {
        public string channel { get; set; }
        public string upi_id { get; set; }
    }

    public class PaymentMethod
    {
        public Upi upi { get; set; }
    }

    public class CashFreePaymentStatusDO
    {
        public int cf_payment_id { get; set; }
        public string order_id { get; set; }
        public string entity { get; set; }
        public string payment_currency { get; set; }
        public double payment_amount { get; set; }
        public DateTime payment_time { get; set; }
        public string payment_status { get; set; }
        public string payment_group { get; set; }
        public string payment_message { get; set; }
        public string bank_reference { get; set; }
        public object auth_id { get; set; }
        public PaymentMethod payment_method { get; set; }
        public bool is_captured { get; set; }
        public double captured_amount { get; set; }
        public double order_amount { get; set; }
    }

}