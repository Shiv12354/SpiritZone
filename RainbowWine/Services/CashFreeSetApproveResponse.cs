using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services
{
    public class CashFreeSetApproveResponse
    {
        public string OrderId { get; set; }
        public string OrderId2 { get; set; }
        public string OrderAmount { get; set; }
        public string ReferenceId { get; set; }
        public string Status { get; set; }
        public string PaymentMode { get; set; }
        public string Msg { get; set; }
        public string TxtTime { get; set; }
        public string Signature { get; set; }
    }
    public class CashFreePaymentResponse
    {
        public string orderId { get; set; }
        public string orderAmount { get; set; }
        public string referenceId { get; set; }
        public string txStatus { get; set; }
        public string paymentMode { get; set; }
        public string txMsg { get; set; }
        public string txTime { get; set; }
        public string signature { get; set; }
    }
    public class CashFreeOrderCreate
    {
        public string appId { get; set; }
        public string secretKey { get; set; }
        public string orderId { get; set; }
        public string orderAmount { get; set; }
        public string orderCurrency { get; set; }
        public string customerName { get; set; }
        public string customerPhone { get; set; }
        public string customerEmail { get; set; }
        public string returnUrl { get; set; }
        public string notifyUrl { get; set; }
        public string orderNote { get; set; }
    }
    public class CashFreeRefundObject
    {
        public string appId { get; set; }
        public string secretKey { get; set; }
        public string referenceId { get; set; }
        public string refundAmount { get; set; }
        public string refundNote { get; set; }
    }
    public class CashFreePaymentOutput
    {
        public string status { get; set; }
        public string paymentLink { get; set; }
        public string reason { get; set; }
        public string message { get; set; }
    }

    public class PaymentDetails
    {
        public string paymentMode { get; set; }
        public string bankName { get; set; }
    }

    public class CashFreeResponseStatus
    {
        public string orderStatus { get; set; }
        public string txStatus { get; set; }
        public string txTime { get; set; }
        public object txMsg { get; set; }
        public string referenceId { get; set; }
        public string paymentMode { get; set; }
        public string orderCurrency { get; set; }
        public PaymentDetails paymentDetails { get; set; }
        public string status { get; set; }
    }

    public class hookResponse
    {
        public string VenderOutput { get; set; }
        public string InputParam { get; set; }
        public int OrderId { get; set; }
    }
}