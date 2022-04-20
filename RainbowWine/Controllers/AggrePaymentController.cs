using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using RainbowWine.Services.OnlinePaymentService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using SZData.Interfaces;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/aggrepay")]
    [EnableCors("*", "*", "*")]
    public class AggrePaymentController : ApiController
    {
        string saltKey = ConfigurationManager.AppSettings["SaltKey"].ToString();
        string apiKey = ConfigurationManager.AppSettings["ApiKey"].ToString();
        string aggrePayMode = ConfigurationManager.AppSettings["AggrePayMode"].ToString();
        OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
        public AggrePaymentController()
        {

        }
        [HttpPost]
        [Route("payment-generate-hashcode/{orderno}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage PaymentGenerateHashCode(string OrderNo)
        {
            LogResult(OrderNo);
            string[] hash_columns = {
            "address_line_1",
            "address_line_2",
            "amount",
            "api_key",
            "city",
            "country",
            "currency",
            "description",
            "email",
            "mode",
            "name",
            "order_id",
            "phone",
            "return_url",
            "state",
            "udf1",
            "udf2",
            "udf3",
            "udf4",
            "udf5",
            "zip_code"
            };
            HttpResponseMessage response = null;
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var db = new rainbowwineEntities();
            //var hostName = Request.RequestUri.GetLeftPart(UriPartial.Authority); 

            try
            {
                if (OrderNo == null)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Input object is null.";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
                else if (string.IsNullOrWhiteSpace(OrderNo))
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Input object order number is null.";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }

                Order order = null;
                decimal amt = 0;
                if (OrderNo.Contains("OG_"))
                {
                    var morder = db.Orders.Where(o => string.Compare(o.OrderGroupId, OrderNo, true) == 0);
                    if (morder != null && morder.Count() > 0)
                    {
                        foreach (var item in morder)
                        {
                            amt += item.OrderAmount;
                        }
                    }
                    int orderIdDecode = Convert.ToInt32(OrderNo.Replace("OG_", ""));
                    order = morder.Where(o => o.Id == orderIdDecode).FirstOrDefault();
                    
                    if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                    {
                        amt = amt - order.WalletAmountUsed.Value;
                    }
                    var promocode = db.PromoCodes.Where(o => o.PromoId == order.PromoId).FirstOrDefault();
                    var ser = SZIoc.GetSerivce<IPromoCodeService>();
                    if (order.PromoId.HasValue && order.PromoId > 0)
                    {

                        var promodata = ser.PromoCodeApply(promocode.Code, (float)amt, order.Customer.UserId);
                        if (promodata.IsValid)
                        {
                            amt = amt - Convert.ToDecimal(promodata.DiscountAmount);

                        }
                    }
                }
                else
                {
                    order = db.Orders.Find(Convert.ToInt32(OrderNo));
                    if (order == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "Order object is null.";
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                    }
                    else
                    {
                        amt = order.OrderAmount;
                    }
                    if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                    {
                        amt = amt - order.WalletAmountUsed.Value;
                    }
                    var promocode = db.PromoCodes.Where(o => o.PromoId == order.PromoId).FirstOrDefault();
                    var ser = SZIoc.GetSerivce<IPromoCodeService>();
                    if (order.PromoId.HasValue && order.PromoId > 0)
                    {

                        var promodata = ser.PromoCodeApply(promocode.Code, (float)order.OrderAmount, order.Customer.UserId);
                        if (promodata.IsValid)
                        {
                            amt = amt - Convert.ToDecimal(promodata.DiscountAmount);

                        }
                    }
                }
                var custDetail = db.Customers.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                var email = db.AspNetUsers.Where(a => a.Id == custDetail.UserId).FirstOrDefault();
                Dictionary<string, string> form = new Dictionary<string, string>();
                form.Add("address_line_1", "ABCD");
                form.Add("address_line_2", "ABCD");
                form.Add("amount",Convert.ToString(Math.Round(amt)));
                form.Add("api_key", apiKey);
                form.Add("city", "Mumbai");
                form.Add("country", "IND");
                form.Add("currency", "INR");
                form.Add("description", "description");
                form.Add("email", email.Email);
                form.Add("mode", aggrePayMode);
                form.Add("name", custDetail.CustomerName);
                form.Add("order_id", OrderNo);
                form.Add("phone", email.PhoneNumber);
                form.Add("return_url", "https://dev.spiritzone.in/aggrepay/callback");
                form.Add("state", "Maharastra");
                form.Add("udf1", "");
                form.Add("udf2","");
                form.Add("udf3","");
                form.Add("udf4","");
                form.Add("udf5", "");
                form.Add("zip_code", "421301");
                LogResult(JsonConvert.SerializeObject(form));
                var hash = gethash(form.Keys.ToArray(), form);
                LogResult(hash);
                response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Data = hash });

            }
            catch (Exception ex)
            {

                SpiritUtility.AppLogging($"Api_PaymentGetToken: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);

            }
            finally
            {
                db.Dispose();

            }
            return response;
        }

        [HttpPost]
        [Route("orders-payment-log")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage OrderPaymentOnlineLog(OnlinePaymentRequestLog onlinePaymentRequestLog)
        {
            if (onlinePaymentRequestLog == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Data = onlinePaymentRequestLog, Message = "Object is null.", Status = false });
            }
            else if (onlinePaymentRequestLog.InputParam == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Data = onlinePaymentRequestLog, Message = "Object input is null.", Status = false });
            }
            else if (onlinePaymentRequestLog.VenderOutput == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Data = onlinePaymentRequestLog, Message = "Object vender ouput is null.", Status = false });
            }

            HttpResponseMessage response = null;
            ResponseStatus responseStatus = new ResponseStatus();
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    CallBackWebHookResponse callBackWebHookResponse = JsonConvert.DeserializeObject<CallBackWebHookResponse>(onlinePaymentRequestLog.VenderOutput.Trim());
                    //CashFreeSetApproveResponse cashFreeSetApproveResponse = JsonConvert.DeserializeObject<CashFreeSetApproveResponse>(cashFreePayment.VenderOutput);
                    OnlinePaymentLog onlinePaymentLog = new OnlinePaymentLog
                    {
                        InputParameters = onlinePaymentRequestLog.InputParam,
                        VenderOutPut = onlinePaymentRequestLog.VenderOutput,
                        CreatedDate = DateTime.Now,
                        OrderIdCF = callBackWebHookResponse.order_id,
                        OrderAmount = callBackWebHookResponse.amount,
                        ReferenceId = callBackWebHookResponse.transaction_id,
                        Msg = callBackWebHookResponse.response_message,
                        PaymentMode = callBackWebHookResponse.payment_method,
                        Status = callBackWebHookResponse.response_code.ToString(),
                        TxtTime = callBackWebHookResponse.payment_datetime.ToString(),
                        Signature = callBackWebHookResponse.hash,
                        MachineName = System.Environment.MachineName
                    };

                    int orderIdDecode = Convert.ToInt32(callBackWebHookResponse.order_id.Replace("OG_", ""));
                    onlinePaymentLog.OrderId = orderIdDecode;
                    OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
                    onlinePaymentServiceDBO.OnlinePaymentLogAdd(onlinePaymentLog);
                    //db.PaymentCashFreeLogs.Add(onlinePaymentLog);
                    //db.SaveChanges();

                    var callBackWebHookResponse1 = new CallBackWebHookResponse
                    {
                        order_id = callBackWebHookResponse.order_id,
                        amount = callBackWebHookResponse.amount,
                        transaction_id = callBackWebHookResponse.transaction_id,
                        response_code = callBackWebHookResponse.response_code,
                        payment_mode = callBackWebHookResponse.payment_method,
                        response_message = callBackWebHookResponse.response_message,
                        payment_datetime = callBackWebHookResponse.payment_datetime,
                        hash = callBackWebHookResponse.hash
                    };
                    string paymentGetWayName = "AggrePay";
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = OnlineOrderToApprove(callBackWebHookResponse1, onlinePaymentRequestLog.VenderOutput, paymentGetWayName);
                    responseStatus.Data = ret;
                    responseStatus.Status = true;
                    response = Request.CreateResponse(HttpStatusCode.OK, responseStatus);



                }
                catch (Exception ex)
                {

                    SpiritUtility.AppLogging($"Api_OrderPaymentCashFreeLog: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();

                }
            }
            return response;
        }

        [HttpPost]
        [Route("refund-status/{orderid}")]
        [Authorize(Roles = "Customer")]
        public ResponseStatus OnlineReFundStatus(int orderId,int issueId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            string statusUrl = ConfigurationManager.AppSettings["AggreRefundStatus"];
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //var order = db.Orders.Find(orderId);
                    var onlineRefunds = onlinePaymentServiceDBO.GetOnlineRefund(orderId);
                    var refund = onlineRefunds.Where(x=>x.IssueId == issueId).OrderByDescending(o => o.OnlineRefundId).FirstOrDefault();
                    var refundDetails = JsonConvert.DeserializeObject<RefundResponse>(JsonConvert.SerializeObject(refund));
                    if (refund == null)
                    {
                        responseStatus.Message = $"No Refund made against the order {orderId}";
                        return responseStatus;
                    }
                    Dictionary<string, string> form = new Dictionary<string, string>();
                    form.Add("api_key", ConfigurationManager.AppSettings["ApiKey"]);
                    form.Add("merchant_order_id", orderId.ToString());
                    form.Add("merchant_refund_id", $"{orderId}_Refund");
                    form.Add("transaction_id", refundDetails.transaction_id);
                    var hash = gethash(form.Keys.ToArray(), form);
                    form.Add("hash", hash);
                    using (HttpClient client = new HttpClient())
                    {
                        Uri url = new Uri($"{statusUrl}");
                        var json = JsonConvert.SerializeObject(form);
                        var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(form) };
                        var response = client.SendAsync(req).Result;
                        var resp = response.Content.ReadAsStringAsync().Result;
                        var ret = JsonConvert.DeserializeObject<OnlineRefundStatus>(resp);
                        if (response.IsSuccessStatusCode && ret.data !=null)
                        {
                            int statusResult = onlinePaymentServiceDBO.UpdateOnlineRefund(refund.OnlineRefundId,ret.data.FirstOrDefault().refund_details.FirstOrDefault().refund_reference_no);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_CashFreeReFund: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = "Error occured.";
                }
                finally
                {
                    db.Dispose();
                }
            }
            return responseStatus;
        }

        #region Non Action

        public static void LogResult(string result)
        {
            var line = Environment.NewLine + Environment.NewLine;



            try
            {
            
                string filepath = "C:/SpiritZone/winedev/AggePayCallBackResult/";  //Text File Path

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);

                }
                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {


                    File.Create(filepath).Dispose();

                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "result :" + " " + result + line ;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }
        }

        private string gethash(string[] hash_columns, Dictionary<string, string> requests)
        {
            string checksumString;
            checksumString = saltKey;
            foreach (string column in hash_columns)
            {
                if (requests.ContainsKey(column) && column != "hash")
                {
                    if (!string.IsNullOrEmpty(requests[column]))
                    {
                        checksumString += "|" + requests[column];
                    }
                }
            }
            LogResult(checksumString);
            string result = Generatehash512(checksumString);
            return result;
        }
        public string Generatehash512(string text)
        {

            byte[] message = System.Text.Encoding.UTF8.GetBytes(text);

            System.Text.UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex.ToUpper();

        }

        public string OnlineOrderToApprove(CallBackWebHookResponse decodevlaue, string pdecodevalue,string paymentGetWayName)
        {
            var db = new rainbowwineEntities();
            int orderIdDecode = Convert.ToInt32(decodevlaue.order_id.Replace("OG_", ""));
            OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
            var appLogsOnlineHooks = onlinePaymentServiceDBO.GetAppLogsOnlinePaymentHook(orderIdDecode.ToString());
            var appLogsOnlineHook = appLogsOnlineHooks.OrderByDescending(o => o.AppLogsOnlinePaymentHookId).FirstOrDefault();
            int appLogsOnlineHookId = 0;
            if (paymentGetWayName =="AggrePay")
            {
                bool addupdate = false;
                if (appLogsOnlineHook != null)
                {
                    appLogsOnlineHookId = appLogsOnlineHook.AppLogsOnlinePaymentHookId;


                    Dictionary<string, string> form = new Dictionary<string, string>();
                    form.Add("address_line_1", decodevlaue.address_line_1);
                    form.Add("address_line_2", decodevlaue.address_line_2);
                    form.Add("amount", decodevlaue.amount);
                    form.Add("api_key", apiKey);
                    form.Add("city", decodevlaue.city);
                    form.Add("country", decodevlaue.country);
                    form.Add("currency", decodevlaue.currency);
                    form.Add("description", decodevlaue.description);
                    form.Add("email", decodevlaue.email);
                    form.Add("mode", aggrePayMode);
                    form.Add("name", decodevlaue.name);
                    form.Add("order_id", decodevlaue.order_id);
                    form.Add("phone", decodevlaue.phone);
                    form.Add("return_url", "https://devcore.spiritzone.in/api/v1.0/manage/callback2");
                    form.Add("state", decodevlaue.state);
                    form.Add("udf1", decodevlaue.udf1);
                    form.Add("udf2", decodevlaue.udf2);
                    form.Add("udf3", decodevlaue.udf3);
                    form.Add("udf4", decodevlaue.udf4);
                    form.Add("udf5", decodevlaue.udf5);
                    form.Add("zip_code", decodevlaue.zip_code);
                    var hash = gethash(form.Keys.ToArray(), form);

                    if (decodevlaue.hash == hash)
                    {
                        if (string.Compare(appLogsOnlineHook.Status, "0", true) == 0)
                        {
                            addupdate = false;
                        }
                        else if (string.Compare(decodevlaue.response_code.ToString(), "0", true) == 0)
                        {
                            addupdate = true;
                        }
                    }
                }
                else
                {
                    appLogsOnlineHook = new AppLogsOnlinePaymentHook();
                    addupdate = true;
                    appLogsOnlineHook.VenderInput = pdecodevalue;
                    appLogsOnlineHook.OrderId = decodevlaue.order_id;
                    //appLogsOnlineHook.OrderIdPartial = decodevlaue.order_id;
                    appLogsOnlineHook.OrderAmount = decodevlaue.amount;
                    appLogsOnlineHook.ReferenceId = decodevlaue.transaction_id;
                    appLogsOnlineHook.Status = decodevlaue.response_code.ToString();
                    appLogsOnlineHook.PaymentMode = decodevlaue.payment_mode;
                    appLogsOnlineHook.Msg = decodevlaue.response_message;
                    appLogsOnlineHook.TxtTime = decodevlaue.payment_datetime;
                    appLogsOnlineHook.Signature = decodevlaue.hash;
                    appLogsOnlineHook.MachineName = System.Environment.MachineName;
                    appLogsOnlineHook.BankName = decodevlaue.payment_channel;
                    appLogsOnlineHook.Currency = decodevlaue.currency;
                    appLogsOnlineHook.Country = decodevlaue.country;
                    appLogsOnlineHookId = onlinePaymentServiceDBO.AppLogsOnlinePaymentHookAdd(appLogsOnlineHook);
                }
                if (addupdate)
                {
                    Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();
                    if (decodevlaue.order_id.Contains("OG_"))
                    {
                        order = null;
                    }
                    Decimal odAmount = 0;
                    IList<Order> groupOrders = new List<Order>();
                    if (order == null)
                    {
                        groupOrders = db.Orders.Where(o => string.Compare(o.OrderGroupId, decodevlaue.order_id, true) == 0)?.ToList();
                        odAmount = groupOrders.Sum(x => x.OrderAmount);
                    }
                    else
                    {
                        odAmount = order.OrderAmount;
                    }

                    if (string.Compare(decodevlaue.response_code.ToString(), "0", true) == 0)
                    {

                        bool isPOD = false;
                        bool isGorup = false;
                        decimal amt = Convert.ToDecimal(decodevlaue.amount);
                        if (groupOrders.Count > 0)
                        {
                            decimal mAmt = 0;
                            groupOrders.ForEach((o) =>
                            {
                                mAmt += o.OrderAmount;
                            });

                            order = groupOrders.Where(o => o.Id == orderIdDecode).FirstOrDefault();
                            if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                            {
                                mAmt = mAmt - order.WalletAmountUsed.Value;
                            }
                            var promocode1 = db.PromoCodes.Where(o => o.PromoId == order.PromoId).FirstOrDefault();
                            var ser1 = SZIoc.GetSerivce<IPromoCodeService>();
                            if (order.PromoId.HasValue && order.PromoId > 0)
                            {

                                var promodata = ser1.PromoCodeApply(promocode1.Code, (float)odAmount, order.Customer.UserId);
                                if (promodata.IsValid)
                                {
                                    mAmt = mAmt - Convert.ToDecimal(promodata.DiscountAmount);

                                }
                            }
                            if (mAmt == amt)// && (order.OrderStatusId != 2 || order.OrderStatusId != 16))
                            {
                                var ret = GroupOrderPlaced(groupOrders, decodevlaue.order_id);

                                if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                                {

                                    var usetranAmount = ser1.UsesTransactionAmount(order.Customer.UserId, mAmt, orderIdDecode);
                                }
                                // Live Tracking Changes For Group Orders

                                //Live Tracking FireStore
                                CustomerApi2Controller.AddToFireStore(order.Id);
                                if (order.OrderStatusId == 5)
                                {

                                    //HyperTracking Complted
                                    HyperTracking hyperTracking = new HyperTracking();
                                    Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(order.Id));

                                    OrderDBO orderDBO = new OrderDBO();
                                    CustomerApi2Controller.DeleteToFireStore(order.Id);
                                    orderDBO.UpdatedScheduledOrder(order.Id);

                                }

                                return "Order is Approved";
                            }

                            else
                            {

                                appLogsOnlineHook.SendStatus = "ShouldCheck";
                                db.SaveChanges();

                                return "ShouldCheck";
                            }
                        }

                        int payType = (int)OrderPaymentType.POD;
                        if (order.PaymentTypeId != null)
                        {
                            if (order.PaymentTypeId == payType)
                            {
                                isPOD = true;
                                int statusPODCashPaid = (int)OrderStatusEnum.PODPaymentSuccess;
                                order.OrderStatusId = statusPODCashPaid;
                                db.SaveChanges();

                                var ema = ConfigurationManager.AppSettings["TrackEmail"];
                                var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = u.Id,
                                    OrderId = order.Id,
                                    StatusId = order.OrderStatusId,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                appLogsOnlineHook.SendStatus = "Approved";
                                db.SaveChanges();

                                //create api to update inventory update
                                //InventoryUpdate(order.Id);

                                WSendSMS wsms = new WSendSMS();
                                string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                                wsms.SendMessage(textmsg, order.Customer.ContactNo);

                                CustomerApi2Controller.AddToFireStore(order.Id);
                                return "Order is PODCashPaid";
                            }
                        }
                        decimal ordAmt = order.OrderAmount;
                        var promocode = db.PromoCodes.Where(o => o.PromoId == order.PromoId).FirstOrDefault();
                        var ser = SZIoc.GetSerivce<IPromoCodeService>();
                        if (order.PromoId.HasValue && order.PromoId > 0)
                        {

                            var promodata = ser.PromoCodeApply(promocode.Code, (float)order.OrderAmount, order.Customer.UserId);
                            if (promodata.IsValid)
                            {
                                ordAmt = ordAmt - Convert.ToDecimal(promodata.DiscountAmount);

                            }
                        }
                        if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                        {
                            ordAmt = ordAmt - order.WalletAmountUsed.Value;
                        }


                        if (!isPOD && ordAmt == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
                        {
                            order.OrderStatusId = 3;
                            db.SaveChanges();

                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            OrderTrack orderTrack = new OrderTrack
                            {
                                LogUserName = u.Email,
                                LogUserId = u.Id,
                                OrderId = order.Id,
                                StatusId = order.OrderStatusId,
                                TrackDate = DateTime.Now
                            };
                            db.OrderTracks.Add(orderTrack);
                            db.SaveChanges();

                            appLogsOnlineHook.SendStatus = "Approved";
                            onlinePaymentServiceDBO.UpdateAppLogsOnlinePaymentHook(appLogsOnlineHookId, appLogsOnlineHook.SendStatus,"");
                            //db.SaveChanges();
                            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                            paymentLinkLogsService.InventoryUpdate(order.Id);
                            paymentLinkLogsService.InventoryMixerUpdate(order.Id);

                            if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                            {
                                var ser2 = SZIoc.GetSerivce<IPromoCodeService>();
                                var usetranAmount = ser2.UsesTransactionAmount(order.Customer.UserId, ordAmt, Convert.ToInt32(decodevlaue.order_id));
                            }
                            OrderDBO orderDBO1 = new OrderDBO();
                            //orderDBO1.UpdateOrderDetailMrpIssueAfterPaymentSuccess(order.Id);
                            WSendSMS wsms = new WSendSMS();
                            //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                            wsms.SendMessage(textmsg, order.Customer.ContactNo);

                            FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                            Task.Run(async () => await fcmHelper.SendFirebaseNotification(order.Id, FirebaseNotificationHelper.NotificationType.Order));

                            //Live Tracking FireStore
                            CustomerApi2Controller.AddToFireStore(order.Id);
                            if (order.OrderStatusId == 5)
                            {

                                //HyperTracking Complted
                                HyperTracking hyperTracking = new HyperTracking();
                                Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(order.Id));


                                OrderDBO orderDBO = new OrderDBO();
                                CustomerApi2Controller.DeleteToFireStore(order.Id);
                                orderDBO.UpdatedScheduledOrder(order.Id);

                            }
                            return "Order is Approved";
                        }
                        else
                        {
                            appLogsOnlineHook.SendStatus = "ShouldCheck";
                            onlinePaymentServiceDBO.UpdateAppLogsOnlinePaymentHook(appLogsOnlineHookId, appLogsOnlineHook.SendStatus,"");
                            //db.SaveChanges();


                            return "ShouldCheck";
                        }
                    }
                    else
                    {
                        appLogsOnlineHook.SendStatus = $"AggrePay St {decodevlaue.response_code}";
                        db.SaveChanges();

                        int payType = (int)OrderPaymentType.POD;
                        if (order.PaymentTypeId != null)
                        {
                            if (order.PaymentTypeId == payType)
                            {
                                int statusPODPaymentFail = (int)OrderStatusEnum.PODPaymentFail;
                                order.OrderStatusId = statusPODPaymentFail;
                                db.SaveChanges();

                                var ema = ConfigurationManager.AppSettings["TrackEmail"];
                                var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = u.Id,
                                    OrderId = order.Id,
                                    StatusId = order.OrderStatusId,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();
                            }
                        }

                        return $"AggrePay is {decodevlaue.response_code}";
                    }
                }
                return "NoChanges";
            }
            return "NoChanges";
        }

        public string UpdateOrderIssueToApprove(CallBackWebHookResponse decodevlaue, string pdecodevalue, string paymentGetWayName)
        {
            var db = new rainbowwineEntities();
            OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
            var partialPayDetails = onlinePaymentServiceDBO.GetOnlinePartialPaymentOrderId(decodevlaue.udf5);

            var appLogsOnlineHooks = onlinePaymentServiceDBO.GetAppLogsOnlinePaymentHook(partialPayDetails.OrderId.ToString());
            var appLogsOnlineHook = appLogsOnlineHooks.OrderByDescending(o => o.AppLogsOnlinePaymentHookId)?.FirstOrDefault();
            bool addupdate = false;
            int appLogsOnlineHookId = 0;
            if (appLogsOnlineHook != null)
            {
                appLogsOnlineHookId = appLogsOnlineHook.AppLogsOnlinePaymentHookId;
                string secret = ConfigurationManager.AppSettings["PayKey"];

                Dictionary<string, string> form = new Dictionary<string, string>();
                form.Add("address_line_1", decodevlaue.address_line_1);
                form.Add("address_line_2", decodevlaue.address_line_2);
                form.Add("amount", decodevlaue.amount);
                form.Add("api_key", apiKey);
                form.Add("city", decodevlaue.city);
                form.Add("country", decodevlaue.country);
                form.Add("currency", decodevlaue.currency);
                form.Add("description", decodevlaue.description);
                form.Add("email", decodevlaue.email);
                form.Add("mode", aggrePayMode);
                form.Add("name", decodevlaue.name);
                form.Add("order_id", decodevlaue.order_id);
                form.Add("phone", decodevlaue.phone);
                form.Add("return_url", "https://devcore.spiritzone.in/api/v1.0/manage/callback2");
                form.Add("state", decodevlaue.state);
                form.Add("udf1", decodevlaue.udf1);
                form.Add("udf2", decodevlaue.udf2);
                form.Add("udf3", decodevlaue.udf3);
                form.Add("udf4", decodevlaue.udf4);
                form.Add("udf5", decodevlaue.udf5);
                form.Add("zip_code", decodevlaue.zip_code);
                var hash = gethash(form.Keys.ToArray(), form);
                if (decodevlaue.hash == hash)
                {
                    if (string.Compare(appLogsOnlineHook.Status, "0", true) == 0)
                    {
                        addupdate = false;
                    }
                    else if (string.Compare(decodevlaue.response_code.ToString(), "0", true) == 0)
                    {
                        addupdate = true;
                    }
                }
            }
            else
            {
                addupdate = true;
                appLogsOnlineHook = new AppLogsOnlinePaymentHook();
                addupdate = true;
                appLogsOnlineHook.VenderInput = pdecodevalue;
                appLogsOnlineHook.OrderId = partialPayDetails.OrderId.ToString();
                appLogsOnlineHook.OrderIdPartial = partialPayDetails.OrderId.ToString();
                appLogsOnlineHook.OrderAmount = decodevlaue.amount;
                appLogsOnlineHook.ReferenceId = decodevlaue.transaction_id;
                appLogsOnlineHook.Status = decodevlaue.response_code.ToString();
                appLogsOnlineHook.PaymentMode = decodevlaue.payment_mode;
                appLogsOnlineHook.Msg = decodevlaue.response_message;
                appLogsOnlineHook.TxtTime = decodevlaue.payment_datetime;
                appLogsOnlineHook.Signature = decodevlaue.hash;
                appLogsOnlineHook.MachineName = System.Environment.MachineName;
                appLogsOnlineHook.BankName = decodevlaue.payment_channel;
                appLogsOnlineHook.Currency = decodevlaue.currency;
                appLogsOnlineHook.Country = decodevlaue.country;
                appLogsOnlineHookId= appLogsOnlineHookId = onlinePaymentServiceDBO.AppLogsOnlinePaymentHookAdd(appLogsOnlineHook);
            }
            if (addupdate)
            {
                if (string.Compare(decodevlaue.response_code.ToString(), "0", true) == 0)
                {

                    Order order = db.Orders.Where(o => o.Id == partialPayDetails.OrderId)?.FirstOrDefault();
                    var orderIssue = db.OrderIssues.Where(o => o.OrderIssueId == partialPayDetails.IssueId)?.FirstOrDefault();

                    decimal amt = Convert.ToDecimal(decodevlaue.amount);
                    decimal adjustAmt = Convert.ToDecimal(orderIssue.AdjustAmt ?? 0);

                    if (adjustAmt == amt)
                    {

                        int payType = (int)OrderPaymentType.POD;
                        order.OrderStatusId = (order.PaymentTypeId == payType) ? 2 : 3;
                        db.SaveChanges();

                        var ema = ConfigurationManager.AppSettings["TrackEmail"];
                        var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = order.Id,
                            StatusId = order.OrderStatusId,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();

                        appLogsOnlineHook.SendStatus = "Approved";
                        onlinePaymentServiceDBO.UpdateAppLogsOnlinePaymentHook(appLogsOnlineHookId, appLogsOnlineHook.SendStatus,"");
                        //db.SaveChanges();

                        WSendSMS wsms = new WSendSMS();
                        //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                        string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                        wsms.SendMessage(textmsg, order.Customer.ContactNo);

                        using (var db1 = new rainbowwineEntities())
                        {

                            var issue = db.OrderIssues.Find(partialPayDetails.IssueId);
                            var ema1 = ConfigurationManager.AppSettings["TrackEmail"];
                            var u1 = db.AspNetUsers.Where(o => o.Email == ema1).FirstOrDefault();
                            string uId = u1.Id;

                            int issueClosed = (int)IssueType.Closed;
                            issue.OrderStatusId = issueClosed;
                            db.SaveChanges();

                            var issetrack = new OrderIssueTrack
                            {
                                OrderId = issue.OrderId,
                                OrderIssueId = issue.OrderIssueId,
                                Remark = "Payment Succesfull",
                                OrderStatusId = 6,
                                TrackDate = DateTime.Now,
                                UserId = uId,
                            };
                            db.OrderIssueTracks.Add(issetrack);
                            db.SaveChanges();
                        }

                        return "Order is Approved";
                    }
                    else
                    {
                        appLogsOnlineHook.SendStatus = "ShouldCheck";
                        onlinePaymentServiceDBO.UpdateAppLogsOnlinePaymentHook(appLogsOnlineHookId, appLogsOnlineHook.SendStatus,"");
                        //db.SaveChanges();


                        return "ShouldCheck";
                    }
                }
                else
                {
                    appLogsOnlineHook.SendStatus = $"AggrePay St {decodevlaue.response_code}";
                    onlinePaymentServiceDBO.UpdateAppLogsOnlinePaymentHook(appLogsOnlineHookId, appLogsOnlineHook.SendStatus,"");
                    //db.SaveChanges();
                    return $"AggrePay is {decodevlaue.response_code}";
                }
            }
            return "NoChanges";
        }

        private string GroupOrderPlaced(IList<Order> groupOrders, string decodeOrderId)
        {
            using (var db = new rainbowwineEntities())
            {
                var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodeOrderId)?
                    .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

                decimal mAmt = 0;
                int payType = (int)OrderPaymentType.POD;
                groupOrders.ForEach((o) =>
                {
                    mAmt += o.OrderAmount;
                });

                var ema = ConfigurationManager.AppSettings["TrackEmail"];
                var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                foreach (var imodle in groupOrders)
                {
                    var ord = db.Orders.Find(imodle.Id);
                    if (ord.PaymentTypeId == payType)
                    {
                        int statusPODCashPaid = (int)OrderStatusEnum.PODPaymentSuccess;
                        ord.OrderStatusId = statusPODCashPaid;
                        db.SaveChanges();

                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = ord.Id,
                            StatusId = ord.OrderStatusId,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();

                        appLogsCashFreeHook.SendStatus = "Approved";
                        db.SaveChanges();

                        //create api to update inventory update
                        //InventoryUpdate(order.Id);

                        WSendSMS wsms = new WSendSMS();
                        string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], ord.Id.ToString());
                        wsms.SendMessage(textmsg, ord.Customer.ContactNo);
                    }
                    else
                    {

                        ord.OrderStatusId = 3;
                        db.SaveChanges();

                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = ord.Id,
                            StatusId = ord.OrderStatusId,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();

                        appLogsCashFreeHook.SendStatus = "Approved";
                        db.SaveChanges();
                        PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                        paymentLinkLogsService.InventoryUpdate(ord.Id);
                        paymentLinkLogsService.InventoryMixerUpdate(ord.Id);

                        WSendSMS wsms = new WSendSMS();
                        //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                        string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], ord.Id.ToString());
                        wsms.SendMessage(textmsg, ord.Customer.ContactNo);

                        FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                        Task.Run(async () => await fcmHelper.SendFirebaseNotification(ord.Id, FirebaseNotificationHelper.NotificationType.Order));

                        var supOut = ApplicationEmailSend.SupplierOrderInformation(ord);

                        int statusEmailSupplier = (int)OrderStatusEnum.EmailedToSupplier;
                        orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = ord.Id,
                            StatusId = statusEmailSupplier,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();

                    }
                }
            }

            return "Group order updated";
        }

        public async Task<ResponseStatus> OnlinePartialPayment(int issueId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            string payUrl = ConfigurationManager.AppSettings["AggreCreatePaymentLinkUrl"];
            OnlineOrderCreate onlineOrderCreate = null;

            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var issue = db.OrderIssues.Find(issueId);
                    var order = db.Orders.Find(issue.OrderId);
                    OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
                    var maxFreePay = onlinePaymentServiceDBO.GetOnlinePartialPayment(order.Id);
                    onlineOrderCreate = new OnlineOrderCreate
                    {
                        secretKey = ConfigurationManager.AppSettings["ApiKey"],
                        orderId = $"{order.Id}_PartialPay_{maxFreePay.Count()}_{issueId}",
                        orderAmount =issue.AdjustAmt.ToString(),
                        orderCurrency = "INR",
                        customerEmail = "subhamautomation@rainmail.com",//order.Customer.CustomerName,
                        customerPhone = order.Customer.ContactNo,
                        udf1 = order.Id.ToString(),
                        udf2 = "LinkPayment",
                        customerName = order.Customer.CustomerName,
                    };
                    if (onlineOrderCreate != null)
                    {
                        Dictionary<string, string> form = new Dictionary<string, string>();
                        form.Add("amount", onlineOrderCreate.orderAmount);
                        form.Add("api_key", onlineOrderCreate.secretKey);
                        form.Add("email", onlineOrderCreate.customerEmail);
                        form.Add("mobile", onlineOrderCreate.customerPhone);
                        form.Add("name", onlineOrderCreate.customerName);
                        form.Add("purpose", "PartialPayment");
                        var hash = gethash(form.Keys.ToArray(), form);
                        using (HttpClient client = new HttpClient())
                        {
                            form.Add("hash", hash);
                            form.Add("udf1", onlineOrderCreate.udf1);
                            form.Add("udf2", onlineOrderCreate.udf2);
                            form.Add("order_id", onlineOrderCreate.orderId);
                            var json = JsonConvert.SerializeObject(form);
                            var content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
                            var ret =  client.PostAsync(payUrl, content).Result;
                            PaymentLinkResponse paymentLinkResponse = new PaymentLinkResponse();
                            if (ret != null)
                            {
                                if (ret.IsSuccessStatusCode)
                                {
                                    var jsonnn = await ret.Content.ReadAsStringAsync();
                                    paymentLinkResponse = JsonConvert.DeserializeObject<PaymentLinkResponse>(jsonnn);
                                }
                            }
                            if (paymentLinkResponse.data != null)
                            {
                                OnlinePartialPayment onlinePartialPayment = null;
                                onlinePartialPayment = new OnlinePartialPayment
                                {
                                    InputValue = JsonConvert.SerializeObject(form),
                                    IssueId = issueId,
                                    OrderId = issue.OrderId.Value,
                                    VenderOutput = JsonConvert.SerializeObject(paymentLinkResponse.data),
                                    Amount = issue.AdjustAmt.ToString(),
                                    UniqueId = paymentLinkResponse.data.uuid
                                };
                                onlinePaymentServiceDBO.OnlinePartialPaymentAdd(onlinePartialPayment);
                                order.OrderStatusId = 35;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = issue.OrderId ?? 0,
                                    Remark = ret.StatusCode.ToString(),
                                    StatusId = 35,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                //update the issue orderdetails to orderdetail table
                                OrderDBO orderDBO = new OrderDBO();
                                orderDBO.UpdateIssueOrder(issueId, order.Id);
                                //update the total order value into order table
                                //order.OrderAmount = GetTotalAmtOfOrder(order.Id);
                                PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                                order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, paymentLinkLogsService.GetTotalAmtOfOrder(order.Id));
                                db.SaveChanges();

                                responseStatus.Message = $"Payment Link Send to Customer.";
                                responseStatus.Status = true;
                                SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);

                                //WSendSMS wsms = new WSendSMS();
                                //string textmsg = string.Format(ConfigurationManager.AppSettings["CFPartialPay"], onlineOrderCreate.orderAmount, order.Id.ToString(), paymentLinkResponse.data.url);
                                //wsms.SendMessage(textmsg, order.Customer.ContactNo);
                                //Flow SMS
                                var link1 = paymentLinkResponse.data.url.Substring(0, 28);
                                var link2 = paymentLinkResponse.data.url.Substring(28, 28);
                                var link3 = paymentLinkResponse.data.url.Substring(56);
                                var dicti = new Dictionary<string, string>();
                                dicti.Add("ORDERID", order.Id.ToString());
                                dicti.Add("LINK", link1);
                                dicti.Add("LINK1", link2);
                                dicti.Add("LINK2", link3);
                                dicti.Add("Amount", issue.AdjustAmt.ToString());
                                var templeteid =  ConfigurationManager.AppSettings["SMSSendPaymentLinkFlowId"];
                                await Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.Customer.ContactNo, dicti));
                                //End Flow SMS
                            }
                            else
                            {
                                    responseStatus.Message = $"Error while Refund the Amount. {paymentLinkResponse.error.message}";
                               
                            }
                        }
                    }
                }
                
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_CashFreePayment: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = "Error occured.";
                }
                finally
                {
                    db.Dispose();
                }
            }
            return responseStatus;
        }

        public ResponseStatus OnlineReFund(int issueId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            string payUrl = ConfigurationManager.AppSettings["AggreRefundUrl"];
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var issue = db.OrderIssues.Find(issueId);
                    var order = db.Orders.Find(issue.OrderId);
                    var applog = onlinePaymentServiceDBO.GetAppLogsOnlinePaymentHook(order.Id.ToString());
                    if (applog == null)
                    {
                        responseStatus.Message = $"No Online Payment made against the order {issue.OrderId} please do wallet refund";
                        return responseStatus;
                    }
                    var applogs = applog.OrderByDescending(o => o.AppLogsOnlinePaymentHookId).FirstOrDefault();
                    //var applogs = db.AppLogsCashFreeHooks.Where(o => string.Compare(o.OrderId.Trim(), order.Id.ToString(), true) == 0)?
                    //    .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();
                    int issueClosed = (int)IssueType.Closed;
                    //if (applogs == null)
                    //{
                    //    responseStatus.Message = $"No Payment made against the order {issue.OrderId}";
                    //    return responseStatus;
                    //}
                    var onlineRefunds = onlinePaymentServiceDBO.GetOnlineRefund(order.Id);
                    var refund = onlineRefunds.Where(x =>x.Status == "RefundInitiated");
                    decimal onlineRefundedAmount = refund.Sum(a => Convert.ToDecimal(a.Amount));
                    double refundAmt = Math.Abs(issue.AdjustAmt ?? 0);
                    double walletRefundAmt = 0.00;
                    double oAmt = Convert.ToDouble(applogs.OrderAmount);
                    string convenienceFee = "0";
                    var walletAmt = order.WalletAmountUsed;
                    if (issue.OrderIssueTypeId == 4)
                    {
                        var convenienceFees = onlinePaymentServiceDBO.GetAggrePayConvenienceFeeDetails(order.Id);
                        if (convenienceFees != null && convenienceFees.ConvenienceFee > 0)
                        {
                            refundAmt = refundAmt + convenienceFees.ConvenienceFee;
                            convenienceFee = convenienceFees.ConvenienceFee.ToString();
                        }
                       
                    }
                    double onlineRemAmt = (Convert.ToDouble(applogs.OrderAmount) - Convert.ToDouble(onlineRefundedAmount));
                    if (onlineRemAmt == 0)
                    {
                        walletRefundAmt = refundAmt - (onlineRemAmt + Convert.ToDouble(convenienceFee));
                        if (!string.IsNullOrEmpty(convenienceFee))
                        {
                            onlinePaymentServiceDBO.UpdateAppLogsOnlinePaymentHook(applogs.AppLogsOnlinePaymentHookId, "", convenienceFee);
                        }
                        if (walletRefundAmt > 0)
                        {
                            var custUserId = db.Customers.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                            OrderIssueDBO cashDBO = new OrderIssueDBO();
                            var result = cashDBO.FullRefundToWallet(issue.OrderId.Value, (float)Math.Round(walletRefundAmt), issue.OrderIssueId);
                            if (result == 1)
                            {
                                WalletRefundNotification(Convert.ToInt32(walletRefundAmt), order.CustomerId, custUserId.UserId);
                            }
                        }
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
                            Remark = "Refund Success",
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
                            orderDBO.UpdateIssueOrder(issueId, order.Id);
                            //update the total order value into order table
                            //order.OrderAmount = GetTotalAmtOfOrder(order.Id);
                            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                            order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, paymentLinkLogsService.GetTotalAmtOfOrder(order.Id));

                            db.SaveChanges();

                            ord.OrderStatusId = 3;
                            db.SaveChanges();
                            issue.OrderStatusId = issueClosed;
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
                        responseStatus.Message = $"Refund successful.";
                        responseStatus.Status = true;

                        if ((issue.OrderIssueTypeId == 4))
                        {
                            var orderIssueDBO = new OrderIssueDBO();
                            //if (walletAmt != null)
                            //{
                            //    orderIssueDBO.FullRefundToWallet(issue.OrderId.Value, (float)walletAmt, issue.OrderIssueId);
                            //}


                            if (order.OrderStatusId == 5 || order.OrderStatusId == 37 || order.OrderStatusId == 68 || order.OrderStatusId == 48)
                            {
                                //HyperTracking Complted
                                HyperTracking hyperTracking = new HyperTracking();
                                Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(issue.OrderId.Value));

                                //Live Tracking FireStore
                                OrderDBO orderDBO = new OrderDBO();
                                CustomerApi2Controller.DeleteToFireStore(issue.OrderId.Value);
                                orderDBO.UpdatedScheduledOrder(issue.OrderId.Value);


                            }

                        }
                        OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                        {
                            UserId = uId,
                            OrderId = issue.OrderId ?? 0,
                            OrderIssueId = issue.OrderIssueId,
                            Remark = "Refund Success",
                            OrderStatusId = issueClosed,
                            TrackDate = DateTime.Now
                        };
                        db.OrderIssueTracks.Add(orderIssueTrack);
                        db.SaveChanges();
                        SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);

                        if ((refundAmt - walletRefundAmt) > 0)
                        {
                            //Flow SMS
                            var dicti = new Dictionary<string, string>();
                            dicti.Add("ORDERID", issue.OrderId.ToString());
                            dicti.Add("AMOUNT", (refundAmt - walletRefundAmt).ToString());
                            var templeteid = ConfigurationManager.AppSettings["SMSRefundFlowId"];
                            Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, ord.OrderTo, dicti));
                            //End Flow SMS

                        }

                    }
                    else
                    {
                        if (refundAmt > onlineRemAmt)
                        {
                            walletRefundAmt = refundAmt - (onlineRemAmt + Convert.ToDouble(convenienceFee));
                            refundAmt = refundAmt - walletRefundAmt;

                        }

                        if (refundAmt > oAmt && string.IsNullOrEmpty(convenienceFee))
                        {
                            responseStatus.Message = "Last Payment made is less than Refund amount.";
                            return responseStatus;
                        }

                        Dictionary<string, string> form = new Dictionary<string, string>();
                        form.Add("amount", refundAmt.ToString());
                        form.Add("api_key", ConfigurationManager.AppSettings["ApiKey"]);
                        form.Add("description", "Refund");
                        form.Add("merchant_order_id", order.Id.ToString());
                        form.Add("merchant_refund_id", $"{order.Id}_Refund");
                        form.Add("transaction_id", applogs.ReferenceId);
                        var hash = gethash(form.Keys.ToArray(), form);
                        form.Add("hash", hash);
                        using (HttpClient client = new HttpClient())
                        {
                            Uri url = new Uri($"{payUrl}");
                            var json = JsonConvert.SerializeObject(form);
                            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(form) };
                            var response = client.SendAsync(req).Result;
                            var resp = response.Content.ReadAsStringAsync().Result;
                            var ret = JsonConvert.DeserializeObject<OnlineRefundResponse>(resp);
                            OnlineRefund onlineRefund = null;
                            onlineRefund = new OnlineRefund
                            {
                                InputParam = json,
                                IssueId = issueId,
                                //OrderModifyId = $"{order.Id}_Refund",
                                OrderId = issue.OrderId.Value,
                                VenderOutput = resp,
                                Status = ret.data == null ? "Failed" : "RefundInitiated",
                                Amount = refundAmt.ToString()
                            };
                            onlinePaymentServiceDBO.OnlineRefundAdd(onlineRefund);
                            if (response.IsSuccessStatusCode && ret.data != null)
                            {
                                if (ret.data != null)
                                {
                                    if (!string.IsNullOrEmpty(convenienceFee))
                                    {
                                        onlinePaymentServiceDBO.UpdateAppLogsOnlinePaymentHook(applogs.AppLogsOnlinePaymentHookId, "", convenienceFee);
                                    }
                                    if (walletRefundAmt > 0)
                                    {
                                        var custUserId = db.Customers.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                                        OrderIssueDBO cashDBO = new OrderIssueDBO();
                                        var result = cashDBO.FullRefundToWallet(issue.OrderId.Value, (float)Math.Round(walletRefundAmt), issue.OrderIssueId);
                                        if (result == 1)
                                        {
                                            WalletRefundNotification(Convert.ToInt32(walletRefundAmt), order.CustomerId, custUserId.UserId);
                                        }
                                    }
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
                                        Remark = "Refund Success",
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
                                        orderDBO.UpdateIssueOrder(issueId, order.Id);
                                        //update the total order value into order table
                                        //order.OrderAmount = GetTotalAmtOfOrder(order.Id);
                                        PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                                        order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, paymentLinkLogsService.GetTotalAmtOfOrder(order.Id));

                                        db.SaveChanges();

                                        ord.OrderStatusId = 3;
                                        db.SaveChanges();
                                        issue.OrderStatusId = issueClosed;
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
                                    responseStatus.Message = $"Refund successful.";
                                    responseStatus.Status = true;

                                    if ((issue.OrderIssueTypeId == 4))
                                    {
                                        var orderIssueDBO = new OrderIssueDBO();
                                        //if (walletAmt != null)
                                        //{
                                        //    orderIssueDBO.FullRefundToWallet(issue.OrderId.Value, (float)walletAmt, issue.OrderIssueId);
                                        //}


                                        if (order.OrderStatusId == 5 || order.OrderStatusId == 37 || order.OrderStatusId == 68 || order.OrderStatusId == 48)
                                        {
                                            //HyperTracking Complted
                                            HyperTracking hyperTracking = new HyperTracking();
                                            Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(issue.OrderId.Value));

                                            //Live Tracking FireStore
                                            OrderDBO orderDBO = new OrderDBO();
                                            CustomerApi2Controller.DeleteToFireStore(issue.OrderId.Value);
                                            orderDBO.UpdatedScheduledOrder(issue.OrderId.Value);


                                        }

                                    }
                                    OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                                    {
                                        UserId = uId,
                                        OrderId = issue.OrderId ?? 0,
                                        OrderIssueId = issue.OrderIssueId,
                                        Remark = "Refund Success",
                                        OrderStatusId = issueClosed,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderIssueTracks.Add(orderIssueTrack);
                                    db.SaveChanges();
                                    SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);

                                    if (onlineRefundedAmount <= Convert.ToDecimal(applogs.OrderAmount))
                                    {
                                        //Flow SMS
                                        var dicti = new Dictionary<string, string>();
                                        dicti.Add("ORDERID", issue.OrderId.ToString());
                                        dicti.Add("AMOUNT", (refundAmt).ToString());
                                        var templeteid = ConfigurationManager.AppSettings["SMSRefundFlowId"];
                                        Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, ord.OrderTo, dicti));
                                        //End Flow SMS

                                    }
                                }
                            }
                            else
                            {
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
                                    Remark = ret.error.message,
                                    StatusId = 38,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                //issue.OrderStatusId = issueClosed;
                                db.SaveChanges();

                                OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                                {
                                    UserId = uId,
                                    OrderId = issue.OrderId ?? 0,
                                    OrderIssueId = issue.OrderIssueId,
                                    Remark = ret.error.message,
                                    OrderStatusId = issueClosed,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderIssueTracks.Add(orderIssueTrack);
                                db.SaveChanges();
                                SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);

                                responseStatus.Message = $"Error while Refund the Amount. {ret.error.message}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_CashFreeReFund: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = "Error occured.";
                }
                finally
                {
                    db.Dispose();
                }
            }
            return responseStatus;
        }
        public void PartialPaymentLinkForReSend(string orderId)
        {
            var db = new rainbowwineEntities();
            int ordId = Convert.ToInt32(orderId);
            var cashPay = onlinePaymentServiceDBO.GetOnlinePartialPayment(ordId);
            var paylink = cashPay.OrderByDescending(o => o.OnlinePartialPaymentId).FirstOrDefault();
            var aggrePayLink = JsonConvert.DeserializeObject<ChallanData>(paylink.VenderOutput);
            var contactNo = db.Orders.Where(o => o.Id == ordId).FirstOrDefault().OrderTo;
            //Flow SMS
            var link1 = aggrePayLink.url.Substring(0, 28);
            var link2 = aggrePayLink.url.Substring(28,28);
            var link3 = aggrePayLink.url.Substring(56);
            var dicti = new Dictionary<string, string>();
            dicti.Add("ORDERID", orderId);
            dicti.Add("LINK", link1);
            dicti.Add("LINK1", link2);
            dicti.Add("LINK2", link3);
            dicti.Add("Amount", paylink.Amount);
            var templeteid = ConfigurationManager.AppSettings["SMSSendPaymentLinkFlowId"];
            Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, contactNo, dicti));
            //End Flow SMS
           
        }

        public void WalletRefundNotification(int amount, int customerId, string userId)
        {
            try
            {


                string wallerRefundMsg = ConfigurationManager.AppSettings["WalletRefundMgs"].ToString();
                string title = ConfigurationManager.AppSettings["WalletRefundTitle"].ToString();

                WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                walletNotificationRequest.Title = title;
                walletNotificationRequest.Message = wallerRefundMsg.Replace("{0}", amount.ToString()).ToString();
                walletNotificationRequest.CustomerID = customerId;
                walletNotificationRequest.UserID = userId;
                FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}
