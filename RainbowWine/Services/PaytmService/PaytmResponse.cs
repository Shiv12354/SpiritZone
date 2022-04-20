using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.PaytmService
{

    public class PaytmResponse
    {
        public Head head { get; set; }
        public Body body { get; set; }
    }
    public class Head
    {
        public object version { get; set; }
        public string timestamp { get; set; }
        public string responseTimestamp { get; set; } //for refund
        public object channelId { get; set; }
        public string signature { get; set; }
        public string tokenType { get; set; }
        public object clientId { get; set; }
    }

    public class NotificationDetail
    {
        public string customerName { get; set; }
        public string contact { get; set; }
        public string notifyStatus { get; set; }
        public string timestamp { get; set; }
    }

    public class ResultInfo
    {
        public string resultStatus { get; set; }
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
    }

    public class Body
    {
        public int linkId { get; set; }
        public string linkType { get; set; }
        public string longUrl { get; set; }
        public string shortUrl { get; set; }
        public decimal amount { get; set; }
        public string expiryDate { get; set; }
        public bool isActive { get; set; }
        public string merchantHtml { get; set; }
        public string createdDate { get; set; }
        public IList<NotificationDetail> notificationDetails { get; set; }
        public ResultInfo resultInfo { get; set; }

        //For Refund
        public string txnTimestamp { get; set; }
        public string txnId { get; set; }
        public string orderId { get; set; }
        public string mid { get; set; }
        public string refId { get; set; }
        public string refundId { get; set; }
        public string refundAmount { get; set; }
        public string txnAmount { get; set; }
        public string totalRefundAmount { get; set; }
    }

}