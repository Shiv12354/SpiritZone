using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.PaytmService
{
    public class PaytmRequest
    {

        public string linkNotes { get; set; }
        public string merchantRequestId { get; set; }
        public string mid { get; set; }
        public string linkId { get; set; }
        public string linkName { get; set; }
        public string linkDescription { get; set; }
        public string linkType { get; set; }
        public string statusCallbackUrl { get; set; }
        public string amount { get; set; }
        public string expiryDate { get; set; }
        public string sendSms { get; set; }
        public string sendEmail { get; set; }
        public string maxPaymentsAllowed { get; set; }
        public PaytmCustomerContact customerContact { get; set; }
    }
    public class PaytmCustomerContact
    {
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public string customerMobile { get; set; }
    }

    public class PaytmRefundRequest
    {
        public string mid { get; set; }
        public string txnType { get; set; }
        public string orderId { get; set; }
        public string txnId { get; set; }
        public string refId { get; set; }
        public string refundAmount { get; set; }
    }

}