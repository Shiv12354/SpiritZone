using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services.DBO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace RainbowWine.Services.PaytmService
{
    public class PaytmPayment
    {
        public void PaytmAPiCall(int id, bool resend = false)
        {
            rainbowwineEntities db = new rainbowwineEntities();
            Order order = db.Orders.Find(id);
            int orderId = order.Id;
            string amt = Convert.ToString(order.OrderAmount);
            string contactNo = order.Customer.ContactNo;
            string custName = order.Customer.CustomerName;
            var hostName = ConfigurationManager.AppSettings["PtmSpiritUrl"];// HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            var smerch = db.ShopMerchants.Include("MerchantTran").Where(o => o.ShopId == order.ShopID)?.FirstOrDefault();

            string merchant_key = smerch.MerchantTran.MKey;// "0bp1P##B12IkKgiR"; //"46N&7N&AtfIahHgu";
            string pmid = smerch.MerchantTran.MID;// ConfigurationManager.AppSettings["PtmMid"];// "Chetan09383500136615";
            string value = (resend) ? ConfigurationManager.AppSettings["PtmReSendUrl"] : ConfigurationManager.AppSettings["PtmCreateUrl"];
            //string paytmurlexpiry = DateTime.Now.AddMinutes(15).ToString("dd'/'MM'/'yyyy HH:mm:ss");
            string paytmurlexpiry = DateTime.Now.AddHours(1).ToString("dd'/'MM'/'yyyy HH:mm:ss");

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            PaytmRequest paytmRequest = new PaytmRequest();
            if (resend)
            {
                PaymentLinkLog paymentLinkLog = db.PaymentLinkLogs.Where(o => o.OrderId == orderId && string.Compare(o.PtmType, "new", true) == 0)?.FirstOrDefault();
                int paylinkid = paymentLinkLog?.PtmLinkId ?? 0;
                paytmRequest = new PaytmRequest
                {
                    mid = pmid,
                    linkId = $"{paylinkid}",
                    sendSms = "true",
                    sendEmail = "false"
                };
            }
            else
            {
                paytmRequest = new PaytmRequest
                {
                    linkNotes = $"{orderId.ToString()}_{order.ShopID.ToString()}",
                    mid = pmid,
                    linkName = $"Chetan",
                    linkDescription = "Home Delivery",
                    linkType = "FIXED",
                    statusCallbackUrl = $"{hostName}/orders/SetApprovePaytm",
                    amount = $"{amt}",
                    expiryDate = paytmurlexpiry,
                    sendSms = "true",
                    sendEmail = "false",
                    maxPaymentsAllowed="1",
                    customerContact = new PaytmCustomerContact
                    {
                        customerEmail = "subham.m@quosphere.com",
                        customerMobile = contactNo,
                        customerName = custName
                    }
                };
            }

            string json_for_checksum = JsonConvert.SerializeObject(paytmRequest);
            string Check = paytm.CheckSum.generateCheckSumByJson(merchant_key, json_for_checksum);

            string Second_jason = "{\"head\":{\"tokenType\":\"AES\",\"signature\":\"" + Check + "\"},\"body\":" + json_for_checksum + "}";

            try
            {

                String url = value;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//Makes a request to uri
                request.ContentType = "application/json";
                request.MediaType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";

                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))//Write request data
                {
                    requestWriter2.Write(Second_jason);

                }


                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))// Send the 'WebRequest' and wait for response..and  Obtain a 'Stream' object associated with the response object.
                {
                    responseData = responseReader.ReadToEnd();
                    PaytmResponse paytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                    int linkId = 0;
                    if (string.Compare(paytmResponse.body.resultInfo.resultStatus, "success", true) == 0)
                    {
                        linkId = paytmResponse.body.linkId;
                    }

                    //set all active flag to 1
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    paymentLinkLogsService.UpdateOrderLogs(order.Id);

                    PaymentLinkLog paymentLinkLog = new PaymentLinkLog
                    {
                        OrderId = orderId,
                        InputParam = Second_jason,
                        VendorOuput = responseData,
                        CreatedDate = DateTime.Now,
                        CheckSum = Check,
                        PayUrlExpiry = paytmurlexpiry,
                        PtmType = (resend) ? "resend" : "new",
                        PtmLinkId = linkId,
                        PtmStatus = paytmResponse.body.resultInfo.resultStatus,
                        IsActive = false
                    };

                    db.PaymentLinkLogs.Add(paymentLinkLog);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                db.AppLogs.Add(new AppLog
                {
                    CreateDatetime = DateTime.Now,
                    Error = ex.Message,
                    Message = ex.StackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();

                //Stream s = ex.Response.GetResponseStream();
                //StreamReader sr = new StreamReader(s);
                //string m = sr.ReadToEnd();
                //Response.Write(m);

            }
        }
        public int PaytmRefundApiCall(int id, bool toberefund=false)
        {
            rainbowwineEntities db = new rainbowwineEntities();
            Order order = db.Orders.Find(id);
            int orderId = order.Id;
            string mobileno = order.OrderTo;
            int ret = 0;
            AppLogsPaytmHook payHooks = null;

            if (toberefund)
                payHooks = db.AppLogsPaytmHooks.Where(o => o.OrderId == orderId && string.Compare(o.SendStatus, "ToBeRefunded", true) == 0)?.FirstOrDefault();

            if (order.OrderStatusId == 3 || order.OrderStatusId == 16 || payHooks!=null)
            {
                if(payHooks == null)
                    payHooks = db.AppLogsPaytmHooks.Where(o => o.OrderId == orderId && string.Compare(o.PtmStatus, "TXN_SUCCESS", true) == 0)?.OrderByDescending(o=>o.AppLogsPaytmHookId).FirstOrDefault();

                if (payHooks != null)
                {
                    var smerch = db.ShopMerchants.Include("MerchantTran").Where(o => o.ShopId == order.ShopID)?.FirstOrDefault();
                    string merchant_key = smerch.MerchantTran.MKey;
                    string pmid = smerch.MerchantTran.MID;
                    string value = ConfigurationManager.AppSettings["PtmReFundUrl"];

                    var decodevlaue = SpiritUtility.GetHooksDeserialize(payHooks.InputPaytm);

                    PaytmRefundRequest paytmRefundRequest = new PaytmRefundRequest
                    {
                        mid = pmid,
                        txnType = "REFUND",
                        orderId = decodevlaue.ORDERID,
                        txnId = decodevlaue.TXNID,
                        refId = $"REFUND_{orderId}",
                        refundAmount = decodevlaue.TXNAMOUNT
                    };

                    string json_for_checksum = JsonConvert.SerializeObject(paytmRefundRequest);
                    string Check = paytm.CheckSum.generateCheckSumByJson(merchant_key, json_for_checksum);

                    string Second_jason = "{\"head\":{\"clientId\":\"C11\",\"signature\":\"" + Check + "\"},\"body\":" + json_for_checksum + "}";
                    try
                    {

                        String url = value;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//Makes a request to uri
                        request.ContentType = "application/json";
                        request.MediaType = "application/json";
                        request.Accept = "application/json";
                        request.Method = "POST";

                        using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))//Write request data
                        {
                            requestWriter2.Write(Second_jason);

                        }

                        string responseData = string.Empty;

                        using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))// Send the 'WebRequest' and wait for response..and  Obtain a 'Stream' object associated with the response object.
                        {
                            responseData = responseReader.ReadToEnd();
                            PaytmResponse paytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                            int linkId = 0;

                            if ((string.Compare(paytmResponse.body.resultInfo.resultStatus, "success", true) == 0) ||
                                (string.Compare(paytmResponse.body.resultInfo.resultStatus, "pending", true) == 0))
                            {
                                ret = 1;
                                linkId = paytmResponse.body.linkId;

                                WSendSMS wSendSMS = new WSendSMS();
                                string text = string.Format(ConfigurationManager.AppSettings["SMSRefund"], decodevlaue.TXNAMOUNT, order.Id);
                                wSendSMS.SendMessage(text, mobileno);
                            }

                            //set all active flag to 1
                            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                            paymentLinkLogsService.UpdateRefundOrderLogs(order.Id);

                            PaymentRefund paymentRefund = new PaymentRefund
                            {
                                OrderId = orderId,
                                AppLogsHookId = payHooks.AppLogsPaytmHookId,
                                InputParam = Second_jason,
                                VendorOuput = responseData,
                                CreatedDate = DateTime.Now,
                                RefundStatus = "refund",
                                RefId = paytmResponse.body.refId,
                                RefundId = paytmResponse.body.refundId,
                                TxnStatus = paytmResponse.body.resultInfo.resultStatus,
                                TxnMsg = paytmResponse.body.resultInfo.resultMsg,
                                RefundAmount = paytmResponse.body.refundAmount,
                                TxnAmount = paytmResponse.body.txnAmount,
                                TotalRefundAmount = paytmResponse.body.totalRefundAmount,
                                TxnId= paytmResponse.body.txnId,
                                TxnOrderId = paytmResponse.body.orderId,
                                TxnTimestamp = paytmResponse.body.txnTimestamp,
                                IsActive=false
                            };

                            db.PaymentRefunds.Add(paymentRefund);
                            db.SaveChanges();

                            if (toberefund)
                            {
                                payHooks.SendStatus = "SentForRefund";
                                db.SaveChanges();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        db.AppLogs.Add(new AppLog
                        {
                            CreateDatetime = DateTime.Now,
                            Error = ex.Message,
                            Message = ex.StackTrace,
                            MachineName = System.Environment.MachineName
                        });
                        db.SaveChanges();
                        //Stream s = ex.Response.GetResponseStream();
                        //StreamReader sr = new StreamReader(s);
                        //string m = sr.ReadToEnd();
                        ////Response.Write(m);

                    }

                }
            }
            return ret;
        }
        public int PaytmRefundStatusApiCall(int id)
        {
            rainbowwineEntities db = new rainbowwineEntities();
            Order order = db.Orders.Find(id);
            int orderId = order.Id;
            string mobileno = order.OrderTo;
            int ret = 0;
            if (id >0)
            {

                //var payHookList = db.PaymentLinkLogs.Where(o => o.OrderId == orderId && string.Compare(o.PtmType, "refund", true) == 0);

                var payRefundList = db.PaymentRefunds.Where(o => o.OrderId == orderId);
                var payRefund = payRefundList?.OrderByDescending(o=>o.PaymentRefundId).FirstOrDefault();
                if (payRefund != null)
                {
                    var smerch = db.ShopMerchants.Include("MerchantTran").Where(o => o.ShopId == order.ShopID)?.FirstOrDefault();
                    string merchant_key = smerch.MerchantTran.MKey;
                    string pmid = smerch.MerchantTran.MID;
                    string value = ConfigurationManager.AppSettings["PtmReFundStatusUrl"];

                    PaytmResponse dbpaytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(payRefund.VendorOuput);

                    PaytmRefundRequest paytmRefundRequest = new PaytmRefundRequest
                    {
                        mid = pmid,
                        orderId = dbpaytmResponse.body.orderId,
                        refId = dbpaytmResponse.body.refId
                    };

                    string json_for_checksum = JsonConvert.SerializeObject(paytmRefundRequest);
                    string Check = paytm.CheckSum.generateCheckSumByJson(merchant_key, json_for_checksum);

                    string Second_jason = "{\"head\":{\"clientId\":\"C11\",\"signature\":\"" + Check + "\"},\"body\":" + json_for_checksum + "}";
                    try
                    {

                        String url = value;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//Makes a request to uri
                        request.ContentType = "application/json";
                        request.MediaType = "application/json";
                        request.Accept = "application/json";
                        request.Method = "POST";

                        using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))//Write request data
                        {
                            requestWriter2.Write(Second_jason);

                        }

                        string responseData = string.Empty;

                        using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))// Send the 'WebRequest' and wait for response..and  Obtain a 'Stream' object associated with the response object.
                        {
                            responseData = responseReader.ReadToEnd();
                            PaytmResponse paytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                            string ptmtype = "refund";
                            int refundStatusid = 25;
                            string stInfo = paytmResponse.body.resultInfo.resultStatus;
                            if (string.Compare(stInfo, "TXN_SUCCESS", true) == 0)
                            {
                                ret = 1;
                                ptmtype = "refunded";

                                WSendSMS wSendSMS = new WSendSMS();
                                string text = string.Format(ConfigurationManager.AppSettings["SMSRefundStatus"], dbpaytmResponse.body.refundAmount, order.Id);
                                wSendSMS.SendMessage(text, mobileno);
                            }
                            //    if(string.Compare(paytmResponse.body.resultInfo.resultStatus, "pending", true) == 0)
                            //{

                            //}
                            else if (string.Compare(paytmResponse.body.resultInfo.resultStatus, "TXN_FAILURE", true) == 0)
                            {
                                ptmtype = "refundfailed";
                                refundStatusid = 26;
                            }

                            //set all active flag to 1
                            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                            paymentLinkLogsService.UpdateRefundOrderLogs(order.Id);


                            var maxlimit = Convert.ToInt32(ConfigurationManager.AppSettings["PtmMaxLimitRefund"]);
                            if (maxlimit <= payRefundList.Count())
                            {
                                ptmtype = "refundmax";
                                refundStatusid = 25;
                            }

                            PaymentRefund paymentRefund = new PaymentRefund
                            {
                                OrderId = orderId,
                                AppLogsHookId = payRefund.AppLogsHookId,
                                InputParam = Second_jason,
                                VendorOuput = responseData,
                                CreatedDate = DateTime.Now,
                                RefundStatus = ptmtype,
                                RefId = paytmResponse.body.refId,
                                RefundId = paytmResponse.body.refundId,
                                TxnStatus = paytmResponse.body.resultInfo.resultStatus,
                                TxnMsg = paytmResponse.body.resultInfo.resultMsg,
                                RefundAmount = paytmResponse.body.refundAmount,
                                TxnAmount = paytmResponse.body.txnAmount,
                                TotalRefundAmount = paytmResponse.body.totalRefundAmount,
                                TxnId = paytmResponse.body.txnId,
                                TxnOrderId = paytmResponse.body.orderId,
                                TxnTimestamp = paytmResponse.body.txnTimestamp,
                                IsActive = false
                            };

                            db.PaymentRefunds.Add(paymentRefund);
                            db.SaveChanges();

                            if (string.Compare(ptmtype, "refundmax", true) == 0 || string.Compare(ptmtype, "refundfailed", true) == 0)
                            {
                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = u.Id,
                                    OrderId = order.Id,
                                    StatusId = refundStatusid,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        db.AppLogs.Add(new AppLog
                        {
                            CreateDatetime = DateTime.Now,
                            Error = ex.Message,
                            Message = ex.StackTrace,
                            MachineName = System.Environment.MachineName
                        });
                        db.SaveChanges();
                        //Stream s = ex.Response.GetResponseStream();
                        //StreamReader sr = new StreamReader(s);
                        //string m = sr.ReadToEnd();
                        ////Response.Write(m);

                    }

                }
            }

            return ret;
        }


        public void PaytmIssueCall(int issueId, bool resend = false)
        {
            rainbowwineEntities db = new rainbowwineEntities();
            var issue = db.OrderIssues.Find(issueId);
            var issueOrder = db.Orders.Find(issue.OrderId);
            int orderId = issueOrder.Id;
            string amt = Convert.ToString(issue.AdjustAmt);
            string contactNo = issueOrder.Customer.ContactNo;
            string custName = issueOrder.Customer.CustomerName;
            var hostName = ConfigurationManager.AppSettings["PtmSpiritUrl"];// HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            var smerch = db.ShopMerchants.Include("MerchantTran").Where(o => o.ShopId == issueOrder.ShopID)?.FirstOrDefault();

            string merchant_key = smerch.MerchantTran.MKey;// "0bp1P##B12IkKgiR"; //"46N&7N&AtfIahHgu";
            string pmid = smerch.MerchantTran.MID;// ConfigurationManager.AppSettings["PtmMid"];// "Chetan09383500136615";
            string value = (resend) ? ConfigurationManager.AppSettings["PtmReSendUrl"] : ConfigurationManager.AppSettings["PtmCreateUrl"];
            //string paytmurlexpiry = DateTime.Now.AddMinutes(15).ToString("dd'/'MM'/'yyyy HH:mm:ss");
            string paytmurlexpiry = DateTime.Now.AddHours(1).ToString("dd'/'MM'/'yyyy HH:mm:ss");

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            PaytmRequest paytmRequest = new PaytmRequest();
            if (resend)
            {
                PaymentLinkLog paymentLinkLog = db.PaymentLinkLogs.Where(o => o.OrderId == orderId && string.Compare(o.PtmType, "new", true) == 0)?.FirstOrDefault();
                int paylinkid = paymentLinkLog?.PtmLinkId ?? 0;
                paytmRequest = new PaytmRequest
                {
                    mid = pmid,
                    linkId = $"{paylinkid}",
                    sendSms = "true",
                    sendEmail = "false"
                };
            }
            else
            {
                paytmRequest = new PaytmRequest
                {
                    linkNotes = $"{orderId.ToString()}_{issueOrder.ShopID.ToString()}_Issue_{issueId}",
                    mid = pmid,
                    linkName = $"Chetan",
                    linkDescription = "Home Delivery",
                    linkType = "FIXED",
                    statusCallbackUrl = $"{hostName}/orders/SetApprovePaytmIssue",
                    amount = $"{amt}",
                    expiryDate = paytmurlexpiry,
                    sendSms = "true",
                    sendEmail = "false",
                    maxPaymentsAllowed = "1",
                    customerContact = new PaytmCustomerContact
                    {
                        customerEmail = "subham.m@quosphere.com",
                        customerMobile = contactNo,
                        customerName = custName
                    }
                };
            }

            string json_for_checksum = JsonConvert.SerializeObject(paytmRequest);
            string Check = paytm.CheckSum.generateCheckSumByJson(merchant_key, json_for_checksum);

            string Second_jason = "{\"head\":{\"tokenType\":\"AES\",\"signature\":\"" + Check + "\"},\"body\":" + json_for_checksum + "}";

            try
            {

                String url = value;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//Makes a request to uri
                request.ContentType = "application/json";
                request.MediaType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";

                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))//Write request data
                {
                    requestWriter2.Write(Second_jason);

                }


                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))// Send the 'WebRequest' and wait for response..and  Obtain a 'Stream' object associated with the response object.
                {
                    responseData = responseReader.ReadToEnd();
                    PaytmResponse paytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                    int linkId = 0;
                    if (string.Compare(paytmResponse.body.resultInfo.resultStatus, "success", true) == 0)
                    {
                        linkId = paytmResponse.body.linkId;
                    }

                    //set all active flag to 1
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    paymentLinkLogsService.UpdateOrderLogs(issueOrder.Id);

                    PaymentLinkLog paymentLinkLog = new PaymentLinkLog
                    {
                        OrderId = orderId,
                        InputParam = Second_jason,
                        VendorOuput = responseData,
                        CreatedDate = DateTime.Now,
                        CheckSum = Check,
                        PayUrlExpiry = paytmurlexpiry,
                        PtmType = (resend) ? "resend" : "new",
                        PtmLinkId = linkId,
                        PtmStatus = paytmResponse.body.resultInfo.resultStatus,
                        IsActive = false
                    };

                    db.PaymentLinkLogs.Add(paymentLinkLog);
                    db.SaveChanges();

                    issueOrder.OrderStatusId = 15;
                    db.SaveChanges();

                    var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                    string uId = u.Id;

                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = uId,
                        OrderId = issue.OrderId ?? 0,
                        Remark = paytmResponse.body.resultInfo.resultStatus,
                        StatusId = 15,
                        TrackDate = DateTime.Now
                    };

                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    //update the issue orderdetails to orderdetail table
                    OrderDBO orderDBO = new OrderDBO();
                    orderDBO.UpdateIssueOrder(issueId, issueOrder.Id);
                    //update the total order value into order table
                    PaymentLinkLogsService paymentUpdate = new PaymentLinkLogsService();
                    issueOrder.OrderAmount = paymentUpdate.GetTotalAmtOfOrder(issueOrder.Id);
                    db.SaveChanges();

                    SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);

                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["CFPartialPay"], Math.Abs(Convert.ToDouble(issue.AdjustAmt)), issueOrder.Id.ToString(),"");
                    //wsms.SendMessage(textmsg, issueOrder.Customer.ContactNo);
                }

            }
            catch (Exception ex)
            {
                db.AppLogs.Add(new AppLog
                {
                    CreateDatetime = DateTime.Now,
                    Error = ex.Message,
                    Message = ex.StackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();

                //Stream s = ex.Response.GetResponseStream();
                //StreamReader sr = new StreamReader(s);
                //string m = sr.ReadToEnd();
                //Response.Write(m);

            }
        }

        public ResponseStatus PaytmIssueRefundApiCall(int issueId, bool toberefund = false)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            rainbowwineEntities db = new rainbowwineEntities();
            var issue = db.OrderIssues.Find(issueId);
            Order issueOrder = db.Orders.Find(issue.OrderId);
            int orderId = issueOrder.Id;
            string mobileno = issueOrder.OrderTo;
            AppLogsPaytmHook payHooks = null;

            if (toberefund)
                payHooks = db.AppLogsPaytmHooks.Where(o => o.OrderId == orderId)?.FirstOrDefault();

            if (payHooks != null)
            {
                if (payHooks == null)
                    payHooks = db.AppLogsPaytmHooks.Where(o => o.OrderId == orderId && string.Compare(o.PtmStatus, "TXN_SUCCESS", true) == 0)?.OrderByDescending(o => o.AppLogsPaytmHookId).FirstOrDefault();

                if (payHooks != null)
                {
                    var smerch = db.ShopMerchants.Include("MerchantTran").Where(o => o.ShopId == issueOrder.ShopID)?.FirstOrDefault();
                    string merchant_key = smerch.MerchantTran.MKey;
                    string pmid = smerch.MerchantTran.MID;
                    string value = ConfigurationManager.AppSettings["PtmReFundUrl"];

                    var decodevlaue = SpiritUtility.GetHooksDeserialize(payHooks.InputPaytm);

                    PaytmRefundRequest paytmRefundRequest = new PaytmRefundRequest
                    {
                        mid = pmid,
                        txnType = "REFUND",
                        orderId = decodevlaue.ORDERID,
                        txnId = decodevlaue.TXNID,
                        refId = $"REFUND_{orderId}",
                        refundAmount = decodevlaue.TXNAMOUNT
                    };

                    string json_for_checksum = JsonConvert.SerializeObject(paytmRefundRequest);
                    string Check = paytm.CheckSum.generateCheckSumByJson(merchant_key, json_for_checksum);

                    string Second_jason = "{\"head\":{\"clientId\":\"C11\",\"signature\":\"" + Check + "\"},\"body\":" + json_for_checksum + "}";
                    try
                    {

                        String url = value;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//Makes a request to uri
                        request.ContentType = "application/json";
                        request.MediaType = "application/json";
                        request.Accept = "application/json";
                        request.Method = "POST";

                        using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))//Write request data
                        {
                            requestWriter2.Write(Second_jason);

                        }

                        string responseData = string.Empty;

                        using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))// Send the 'WebRequest' and wait for response..and  Obtain a 'Stream' object associated with the response object.
                        {
                            responseData = responseReader.ReadToEnd();
                            PaytmResponse paytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                            int linkId = 0;
                            responseStatus.Message = $"{paytmResponse.body.resultInfo.resultStatus}. {paytmResponse.body.resultInfo.resultMsg}";
                            if ((string.Compare(paytmResponse.body.resultInfo.resultStatus, "success", true) == 0) ||
                                (string.Compare(paytmResponse.body.resultInfo.resultStatus, "pending", true) == 0))
                            {
                                responseStatus.Status = true;
                                linkId = paytmResponse.body.linkId;
                                var ord = db.Orders.Find(issue.OrderId ?? 0);
                                ord.OrderStatusId = 37;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = issue.OrderId ?? 0,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    StatusId = 37,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                //If Partial Refund update the order details
                                if (issue.OrderIssueTypeId == 3)
                                {
                                    //update the issue orderdetails to orderdetail table
                                    OrderDBO orderDBO = new OrderDBO();
                                    orderDBO.UpdateIssueOrder(issueId, issueOrder.Id);
                                    //update the total order value into order table
                                    PaymentLinkLogsService paymentUpdate = new PaymentLinkLogsService();
                                    issueOrder.OrderAmount = paymentUpdate.GetTotalAmtOfOrder(issueOrder.Id);
                                    db.SaveChanges();

                                    ord.OrderStatusId = 3;
                                    db.SaveChanges();

                                    orderTrack = new OrderTrack
                                    {
                                        LogUserName = u.Email,
                                        LogUserId = uId,
                                        OrderId = issue.OrderId ?? 0,
                                        Remark = "Partial Refund re-approved.",
                                        StatusId = 3,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderTracks.Add(orderTrack);
                                    db.SaveChanges();

                                }


                                OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                                {
                                    UserId = uId,
                                    OrderId = issue.OrderId ?? 0,
                                    OrderIssueId = issue.OrderIssueId,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    OrderStatusId = 6,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderIssueTracks.Add(orderIssueTrack);
                                db.SaveChanges();
                                SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);

                                WSendSMS wSendSMS = new WSendSMS();
                                string text = string.Format(ConfigurationManager.AppSettings["CFPartialRefund"], Math.Abs(issue.AdjustAmt ?? 0), issueOrder.Id);
                                wSendSMS.SendMessage(text, mobileno);
                            }
                            else if (string.Compare(paytmResponse.body.resultInfo.resultStatus, "TXN_FAILURE", true) == 0)
                            {

                                responseStatus.Status = false;
                                var ord = db.Orders.Find(issue.OrderId ?? 0);
                                ord.OrderStatusId = 38;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = issue.OrderId ?? 0,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    StatusId = 38,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();


                                OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                                {
                                    UserId = uId,
                                    OrderId = issue.OrderId ?? 0,
                                    OrderIssueId = issue.OrderIssueId,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    OrderStatusId = 6,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderIssueTracks.Add(orderIssueTrack);
                                db.SaveChanges();
                                SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);
                            }
                            //set all active flag to 1
                            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                            paymentLinkLogsService.UpdateRefundOrderLogs(issueOrder.Id);

                            PaymentRefund paymentRefund = new PaymentRefund
                            {
                                OrderId = orderId,
                                AppLogsHookId = payHooks.AppLogsPaytmHookId,
                                InputParam = Second_jason,
                                VendorOuput = responseData,
                                CreatedDate = DateTime.Now,
                                RefundStatus = "refund",
                                RefId = paytmResponse.body.refId,
                                RefundId = paytmResponse.body.refundId,
                                TxnStatus = paytmResponse.body.resultInfo.resultStatus,
                                TxnMsg = paytmResponse.body.resultInfo.resultMsg,
                                RefundAmount = paytmResponse.body.refundAmount,
                                TxnAmount = paytmResponse.body.txnAmount,
                                TotalRefundAmount = paytmResponse.body.totalRefundAmount,
                                TxnId = paytmResponse.body.txnId,
                                TxnOrderId = paytmResponse.body.orderId,
                                TxnTimestamp = paytmResponse.body.txnTimestamp,
                                IsActive = false
                            };

                            db.PaymentRefunds.Add(paymentRefund);
                            db.SaveChanges();

                            if (toberefund)
                            {
                                payHooks.SendStatus = "SentForRefund";
                                db.SaveChanges();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        db.AppLogs.Add(new AppLog
                        {
                            CreateDatetime = DateTime.Now,
                            Error = ex.Message,
                            Message = ex.StackTrace,
                            MachineName = System.Environment.MachineName
                        });
                        db.SaveChanges();
                        //Stream s = ex.Response.GetResponseStream();
                        //StreamReader sr = new StreamReader(s);
                        //string m = sr.ReadToEnd();
                        ////Response.Write(m);
                        responseStatus.Status = false;
                        responseStatus.Message = ex.Message;

                    }

                }
            }
            return responseStatus;
        }

        public void PaytmOrderModifyCall(int orderModifyId, bool resend = false)
        {
            rainbowwineEntities db = new rainbowwineEntities();
            var modify = db.OrderModifies.Find(orderModifyId);
            var modifyOrder = db.Orders.Find(modify.OrderId);
            int orderId = modifyOrder.Id;
            string amt = Convert.ToString(modify.AdjustAmt);
            string contactNo = modifyOrder.Customer.ContactNo;
            string custName = modifyOrder.Customer.CustomerName;
            var hostName = ConfigurationManager.AppSettings["PtmSpiritUrl"];// HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            var smerch = db.ShopMerchants.Include("MerchantTran").Where(o => o.ShopId == modifyOrder.ShopID)?.FirstOrDefault();

            string merchant_key = smerch.MerchantTran.MKey;// "0bp1P##B12IkKgiR"; //"46N&7N&AtfIahHgu";
            string pmid = smerch.MerchantTran.MID;// ConfigurationManager.AppSettings["PtmMid"];// "Chetan09383500136615";
            string value = (resend) ? ConfigurationManager.AppSettings["PtmReSendUrl"] : ConfigurationManager.AppSettings["PtmCreateUrl"];
            //string paytmurlexpiry = DateTime.Now.AddMinutes(15).ToString("dd'/'MM'/'yyyy HH:mm:ss");
            string paytmurlexpiry = DateTime.Now.AddHours(1).ToString("dd'/'MM'/'yyyy HH:mm:ss");

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            PaytmRequest paytmRequest = new PaytmRequest();
            if (resend)
            {
                PaymentLinkLog paymentLinkLog = db.PaymentLinkLogs.Where(o => o.OrderId == orderId && string.Compare(o.PtmType, "new", true) == 0)?.FirstOrDefault();
                int paylinkid = paymentLinkLog?.PtmLinkId ?? 0;
                paytmRequest = new PaytmRequest
                {
                    mid = pmid,
                    linkId = $"{paylinkid}",
                    sendSms = "true",
                    sendEmail = "false"
                };
            }
            else
            {
                paytmRequest = new PaytmRequest
                {
                    linkNotes = $"{orderId.ToString()}_{modifyOrder.ShopID.ToString()}_OrderModify_{orderModifyId}",
                    mid = pmid,
                    linkName = $"Chetan",
                    linkDescription = "Home Delivery",
                    linkType = "FIXED",
                    statusCallbackUrl = $"{hostName}/orders/SetApprovePaytmIssue",
                    amount = $"{amt}",
                    expiryDate = paytmurlexpiry,
                    sendSms = "true",
                    sendEmail = "false",
                    maxPaymentsAllowed = "1",
                    customerContact = new PaytmCustomerContact
                    {
                        customerEmail = "subham.m@quosphere.com",
                        customerMobile = contactNo,
                        customerName = custName
                    }
                };
            }

            string json_for_checksum = JsonConvert.SerializeObject(paytmRequest);
            string Check = paytm.CheckSum.generateCheckSumByJson(merchant_key, json_for_checksum);

            string Second_jason = "{\"head\":{\"tokenType\":\"AES\",\"signature\":\"" + Check + "\"},\"body\":" + json_for_checksum + "}";

            try
            {

                String url = value;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//Makes a request to uri
                request.ContentType = "application/json";
                request.MediaType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";

                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))//Write request data
                {
                    requestWriter2.Write(Second_jason);

                }


                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))// Send the 'WebRequest' and wait for response..and  Obtain a 'Stream' object associated with the response object.
                {
                    responseData = responseReader.ReadToEnd();
                    PaytmResponse paytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                    int linkId = 0;
                    if (string.Compare(paytmResponse.body.resultInfo.resultStatus, "success", true) == 0)
                    {
                        linkId = paytmResponse.body.linkId;
                    }

                    //set all active flag to 1
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    paymentLinkLogsService.UpdateOrderLogs(modifyOrder.Id);

                    PaymentLinkLog paymentLinkLog = new PaymentLinkLog
                    {
                        OrderId = orderId,
                        InputParam = Second_jason,
                        VendorOuput = responseData,
                        CreatedDate = DateTime.Now,
                        CheckSum = Check,
                        PayUrlExpiry = paytmurlexpiry,
                        PtmType = (resend) ? "resend" : "new",
                        PtmLinkId = linkId,
                        PtmStatus = paytmResponse.body.resultInfo.resultStatus,
                        IsActive = false
                    };

                    db.PaymentLinkLogs.Add(paymentLinkLog);
                    db.SaveChanges();

                    int statusPayLinkSend = (int)OrderStatusEnum.PayLinkSend;
                    modifyOrder.OrderStatusId = statusPayLinkSend;
                    db.SaveChanges();

                    var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                    string uId = u.Id;

                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = uId,
                        OrderId = modify.OrderId ?? 0,
                        Remark = paytmResponse.body.resultInfo.resultStatus,
                        StatusId = statusPayLinkSend,
                        TrackDate = DateTime.Now
                    };

                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    //update the issue orderdetails to orderdetail table
                    OrderDBO orderDBO = new OrderDBO();
                    orderDBO.UpdateIssueOrder(orderModifyId, modifyOrder.Id);
                    //update the total order value into order table
                    PaymentLinkLogsService paymentUpdate = new PaymentLinkLogsService();
                    modifyOrder.OrderAmount = paymentUpdate.GetTotalAmtOfOrder(modifyOrder.Id);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                db.AppLogs.Add(new AppLog
                {
                    CreateDatetime = DateTime.Now,
                    Error = ex.Message,
                    Message = ex.StackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();

                //Stream s = ex.Response.GetResponseStream();
                //StreamReader sr = new StreamReader(s);
                //string m = sr.ReadToEnd();
                //Response.Write(m);

            }
        }

        public ResponseStatus PaytmOrderModifyRefundApiCall(int orderModifyId, bool toberefund = false)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            rainbowwineEntities db = new rainbowwineEntities();
            var modify = db.OrderModifies.Find(orderModifyId);
            Order modifyOrder = db.Orders.Find(modify.OrderId);
            int orderId = modifyOrder.Id;
            string mobileno = modifyOrder.OrderTo;
            AppLogsPaytmHook payHooks = null;

            if (toberefund)
                payHooks = db.AppLogsPaytmHooks.Where(o => o.OrderId == orderId)?.FirstOrDefault();

            if (payHooks != null)
            {
                if (payHooks == null)
                    payHooks = db.AppLogsPaytmHooks.Where(o => o.OrderId == orderId && string.Compare(o.PtmStatus, "TXN_SUCCESS", true) == 0)?.OrderByDescending(o => o.AppLogsPaytmHookId).FirstOrDefault();

                if (payHooks != null)
                {
                    var smerch = db.ShopMerchants.Include("MerchantTran").Where(o => o.ShopId == modifyOrder.ShopID)?.FirstOrDefault();
                    string merchant_key = smerch.MerchantTran.MKey;
                    string pmid = smerch.MerchantTran.MID;
                    string value = ConfigurationManager.AppSettings["PtmReFundUrl"];

                    var decodevlaue = SpiritUtility.GetHooksDeserialize(payHooks.InputPaytm);

                    PaytmRefundRequest paytmRefundRequest = new PaytmRefundRequest
                    {
                        mid = pmid,
                        txnType = "REFUND",
                        orderId = decodevlaue.ORDERID,
                        txnId = decodevlaue.TXNID,
                        refId = $"REFUND_{orderId}",
                        refundAmount = decodevlaue.TXNAMOUNT
                    };

                    string json_for_checksum = JsonConvert.SerializeObject(paytmRefundRequest);
                    string Check = paytm.CheckSum.generateCheckSumByJson(merchant_key, json_for_checksum);

                    string Second_jason = "{\"head\":{\"clientId\":\"C11\",\"signature\":\"" + Check + "\"},\"body\":" + json_for_checksum + "}";
                    try
                    {

                        String url = value;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//Makes a request to uri
                        request.ContentType = "application/json";
                        request.MediaType = "application/json";
                        request.Accept = "application/json";
                        request.Method = "POST";

                        using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))//Write request data
                        {
                            requestWriter2.Write(Second_jason);

                        }

                        string responseData = string.Empty;

                        using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))// Send the 'WebRequest' and wait for response..and  Obtain a 'Stream' object associated with the response object.
                        {
                            responseData = responseReader.ReadToEnd();
                            PaytmResponse paytmResponse = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                            int linkId = 0;

                            int issueClosed = (int)IssueType.Closed;

                            responseStatus.Message = $"{paytmResponse.body.resultInfo.resultStatus}. {paytmResponse.body.resultInfo.resultMsg}";
                            if ((string.Compare(paytmResponse.body.resultInfo.resultStatus, "success", true) == 0) ||
                                (string.Compare(paytmResponse.body.resultInfo.resultStatus, "pending", true) == 0))
                            {
                                int statusRefunded = (int)OrderStatusEnum.OrderModifyRefunded;
                                int statusApproved = (int)OrderStatusEnum.Approved;
                                responseStatus.Status = true;
                                linkId = paytmResponse.body.linkId;
                                var ord = modifyOrder;// db.Orders.Find(issue.OrderId ?? 0);
                                ord.OrderStatusId = statusRefunded;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = modify.OrderId ?? 0,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    StatusId = statusRefunded,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                //If Partial Refund update the order details
                                if (modify.OrderType == 3)
                                {
                                    //update the issue orderdetails to orderdetail table
                                    OrderDBO orderDBO = new OrderDBO();
                                    orderDBO.UpdateIssueOrder(orderModifyId, modifyOrder.Id);
                                    //update the total order value into order table
                                    PaymentLinkLogsService paymentUpdate = new PaymentLinkLogsService();
                                    modifyOrder.OrderAmount = paymentUpdate.GetTotalAmtOfOrder(modifyOrder.Id);
                                    db.SaveChanges();

                                    ord.OrderStatusId = statusApproved;
                                    db.SaveChanges();

                                    orderTrack = new OrderTrack
                                    {
                                        LogUserName = u.Email,
                                        LogUserId = uId,
                                        OrderId = modify.OrderId ?? 0,
                                        Remark = "Partial Refund re-approved.",
                                        StatusId = statusApproved,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderTracks.Add(orderTrack);
                                    db.SaveChanges();

                                }

                                OrderModifyTrack orderModifyTrack = new OrderModifyTrack
                                {
                                    UserId = uId,
                                    OrderId = modify.OrderId ?? 0,
                                    OrderModifyId = modify.Id,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    OrderStatusId = issueClosed,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderModifyTracks.Add(orderModifyTrack);
                                db.SaveChanges();
                                

                                WSendSMS wSendSMS = new WSendSMS();
                                string text = string.Format(ConfigurationManager.AppSettings["CFPartialRefund"], Math.Abs(modify.AdjustAmt ?? 0), modifyOrder.Id);
                                wSendSMS.SendMessage(text, mobileno);
                            }
                            else if (string.Compare(paytmResponse.body.resultInfo.resultStatus, "TXN_FAILURE", true) == 0)
                            {

                                int statusRefundFailed = (int)OrderStatusEnum.OrderModifyRefundFailed;
                                responseStatus.Status = false;
                                var ord = db.Orders.Find(modify.OrderId ?? 0);
                                ord.OrderStatusId = statusRefundFailed;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = modify.OrderId ?? 0,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    StatusId = statusRefundFailed,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();


                                var modifytrack = new OrderModifyTrack
                                {
                                    OrderId = modify.OrderId ??0,
                                    OrderModifyId = modify.Id,
                                    Remark = paytmResponse.body.resultInfo.resultStatus,
                                    OrderStatusId = issueClosed,
                                    TrackDate = DateTime.Now,
                                    UserId = uId,
                                };

                                db.OrderModifyTracks.Add(modifytrack);
                                db.SaveChanges();
                            }
                            //set all active flag to 1
                            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                            paymentLinkLogsService.UpdateRefundOrderLogs(modifyOrder.Id);

                            PaymentRefund paymentRefund = new PaymentRefund
                            {
                                OrderId = orderId,
                                AppLogsHookId = payHooks.AppLogsPaytmHookId,
                                InputParam = Second_jason,
                                VendorOuput = responseData,
                                CreatedDate = DateTime.Now,
                                RefundStatus = "refund",
                                RefId = paytmResponse.body.refId,
                                RefundId = paytmResponse.body.refundId,
                                TxnStatus = paytmResponse.body.resultInfo.resultStatus,
                                TxnMsg = paytmResponse.body.resultInfo.resultMsg,
                                RefundAmount = paytmResponse.body.refundAmount,
                                TxnAmount = paytmResponse.body.txnAmount,
                                TotalRefundAmount = paytmResponse.body.totalRefundAmount,
                                TxnId = paytmResponse.body.txnId,
                                TxnOrderId = paytmResponse.body.orderId,
                                TxnTimestamp = paytmResponse.body.txnTimestamp,
                                IsActive = false
                            };

                            db.PaymentRefunds.Add(paymentRefund);
                            db.SaveChanges();

                            if (toberefund)
                            {
                                payHooks.SendStatus = "SentForRefund";
                                db.SaveChanges();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        db.AppLogs.Add(new AppLog
                        {
                            CreateDatetime = DateTime.Now,
                            Error = ex.Message,
                            Message = ex.StackTrace,
                            MachineName = System.Environment.MachineName
                        });
                        db.SaveChanges();
                        //Stream s = ex.Response.GetResponseStream();
                        //StreamReader sr = new StreamReader(s);
                        //string m = sr.ReadToEnd();
                        ////Response.Write(m);
                        responseStatus.Status = false;
                        responseStatus.Message = ex.Message;

                    }

                }
            }
            return responseStatus;
        }
    }
}