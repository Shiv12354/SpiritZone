using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.OnlinePaymentService
{
    public class OnlinePaymentServiceDO
    {
    }
    public class AppLogsOnlinePaymentHook
    {
        public int AppLogsOnlinePaymentHookId { get; set; }
        public string VenderInput { get; set; }
        public string OrderId { get; set; }
        public string OrderIdPartial { get; set; }
        public string OrderAmount { get; set; }
        public string ReferenceId { get; set; }
        public string Status { get; set; }
        public string PaymentMode { get; set; }
        public string Msg { get; set; }
        public string TxtTime { get; set; }
        public string Signature { get; set; }
        public string SendStatus { get; set; }
        public string MachineName { get; set; }
        public string Error_Desc { get; set; }
        public string Currency { get; set; }
        public string BankName { get; set; }
        public string Country { get; set; }
        public string PaymentGateWayName { get; set; }
       

    }

    public class OnlinePartialPayment
    {
        public int OnlinePartialPaymentId { get; set; }
        public string InputValue { get; set; }
        public string VenderOutput { get; set; }
        public int IssueId { get; set; }
        public int OrderId { get; set; }
        public string Amount { get; set; }
        public string PaymentGateWayName { get; set; }
        public string UniqueId { get; set; }

    }

    public class CallBackWebHookResponse
    {
        public string transaction_id { get; set; }
        public string payment_method { get; set; }
        public string payment_datetime { get; set; }
        public int response_code { get; set; }
        public string response_message { get; set; }
        public string order_id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip_code { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string payment_mode { get; set; }
        public string payment_channel { get; set; }
        public string error_desc { get; set; }
        public string cardmasked { get; set; }
        public string hash { get; set; }



    }

    public class OnlinePaymentRequestLog
    {
        public string InputParam { get; set; }
        public string VenderOutput { get; set; }
        public int OrderId { get; set; }
    }

    public partial class OnlinePaymentLog
    {
        public int PaymentCashFreeLogId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string InputParameters { get; set; }
        public string VenderOutPut { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string OrderIdCF { get; set; }
        public string OrderAmount { get; set; }
        public string ReferenceId { get; set; }
        public string Status { get; set; }
        public string PaymentMode { get; set; }
        public string Msg { get; set; }
        public string TxtTime { get; set; }
        public string Signature { get; set; }
        public string SendStatus { get; set; }
        public string MachineName { get; set; }
        public string PaymentGateWayName { get; set; }
    }

    public class OnlineSetApproveResponse
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

    public class OnlineOrderCreate
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
        public string udf1 { get; set; }
        public string udf2 { get; set; }
    }

    public class PaymentLinkResponse
    {
        public ChallanData data { get; set; }
        public AggeError error { get; set; }

    }
    public class ChallanData
    {
        public string url { get; set; }
        public string uuid { get; set; }
        public int tnp_id { get; set; }

    }
    public class AggeError
    {
        public int code { get; set; }
        public string message { get; set; }

    }

    public class OnlineRefundObject
    {
        public string appId { get; set; }
        public string secretKey { get; set; }
        public string referenceId { get; set; }
        public string refundAmount { get; set; }
        public string refundNote { get; set; }
    }

    public  class OnlineRefund
    {
        public int OnlineRefundId { get; set; }
        public int IssueId { get; set; }
        public int OrderModifyId { get; set; }
        public int OrderId { get; set; }
        public string InputParam { get; set; }
        public string VenderOutput { get; set; }
        public string Amount { get; set; }
        public string PaymentGateWayName { get; set; }
        public string Status { get; set; }
        public string RefundReferenceNo { get; set; }
        public bool IsRefunded { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class OnlineRefundResponse
    {
        public RefundResponse data { get; set; }
        public AggeError error { get; set; }

    }
    public class RefundResponse
    {
        public string transaction_id { get; set; }
        public int refund_id { get; set; }
        public string refund_reference_no { get; set; }
        public string merchant_refund_id { get; set; }
        public string merchant_order_id { get; set; }

    }

    public class PaymentGateWayType
    {
        public string PaymentGateWayName { get; set; }
        public string OrderAmount { get; set; }

    }

    public class RefundDetail
    {
        public int refund_id { get; set; }
        public string refund_reference_no { get; set; }
        public string merchant_refund_id { get; set; }
        public string refund_amount { get; set; }
        public string refund_status { get; set; }
        public string date { get; set; }
    }

    public class Datum
    {
        public string transaction_id { get; set; }
        public string merchant_order_id { get; set; }
        public int refund_amount { get; set; }
        public string transaction_amount { get; set; }
        public List<RefundDetail> refund_details { get; set; }
    }

    public class OnlineRefundStatus
    {
        public List<Datum> data { get; set; }
        public string hash { get; set; }
    }
}