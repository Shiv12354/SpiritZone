using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace RainbowWine.Services.Gateway
{
    public class TransactionCashFree : PaymentService<CashFreeModel>
    {
        public string AppId => ConfigurationManager.AppSettings["PayFreeId"];
        public string SecretKey => ConfigurationManager.AppSettings["PayKey"];
        public string NotifyUrl => ConfigurationManager.AppSettings["CashFreeNotifyUrl"];
        public string ReturnUrl => ConfigurationManager.AppSettings["CashFreeReturnUrl"];
        public string CashFreeOrderCreatUrl => ConfigurationManager.AppSettings["PayCreate"];
        public string CashFreePaymnetLinkSMSUrl => ConfigurationManager.AppSettings["PayCreateSMS"];

        protected override object MakePayment(CashFreeModel model)
        {
            OutputCashFree output = new OutputCashFree();
            try
            {
                

                string payUrl = CashFreeOrderCreatUrl;
                using (HttpClient client = new HttpClient())
                {
                    Uri url = new Uri($"{payUrl}");
                    var body = new Dictionary<string, string>
                        {
                            { "appId", AppId},
                            { "secretKey", SecretKey},
                            { "orderId", model.OrderId },
                            { "orderAmount", model.OrderAmount },
                            { "customerEmail", model.CustomerEmail },
                            { "customerPhone", model.CustomerPhone },
                            { "notifyUrl", NotifyUrl },
                            { "returnUrl", ReturnUrl }
                        };

                    var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                    var response = client.SendAsync(req).Result;
                    var resp = response.Content.ReadAsStringAsync().Result;
                    var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                    AddCashFreeCreateToDb(model, resp);

                    output.PaymentOutput = ret;
                    output.Status = true; ;
                    if (string.Compare(ret.status, "error", true) == 0)
                    {
                        output.Status = false;
                        output.ErrorMessage = ret.reason;
                    }
                    if (!string.IsNullOrWhiteSpace(ret.paymentLink))
                    {
                        var env = Convert.ToString(ConfigurationManager.AppSettings["Env"]);
                        if (!string.IsNullOrWhiteSpace(env) && string.Compare(env,"production",true)==0)
                        {
                            SendPaymentLinkByCashFree(model.OrderId);
                        }
                        else
                        {
                            WSendSMS wsms = new WSendSMS();
                            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSiOSPayLink"], model.CustomerEmail, model.OrderAmount, model.OrderId, ret.paymentLink);
                            wsms.SendMessage(textmsg, model.CustomerPhone);
                            PaymentLinkSendUpdateToDb(Convert.ToInt32(model.OrderId), ret.message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.Logging($"Api_OnlineOnDelivery: {ex.Message}", ex.StackTrace);
                throw;
            }
            return output;
        }
        public void SendPaymentLinkByCashFree(string orderId)
        {
            string payUrl = CashFreePaymnetLinkSMSUrl;
            using (HttpClient client = new HttpClient())
            {
                Uri url = new Uri($"{payUrl}");
                var body = new Dictionary<string, string>
                        {
                            { "appId", AppId},
                            { "secretKey", SecretKey},
                            { "orderId", orderId}
                        };

                var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                var response = client.SendAsync(req).Result;
                var resp = response.Content.ReadAsStringAsync().Result;
                var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                PaymentLinkSendUpdateToDb(Convert.ToInt32(orderId), ret.message);
            }
        }

        private void AddCashFreeCreateToDb(CashFreeModel model, string resp)
        {
            CashFreeOrderCreate cashFreeOrderCreate = new CashFreeOrderCreate
            {
                appId = AppId,
                secretKey = SecretKey,
                orderId = $"{model.OrderId}",
                orderAmount = model.OrderAmount.ToString(),
                orderCurrency = "INR",
                customerEmail = model.CustomerEmail,
                customerPhone = model.CustomerPhone,
                notifyUrl = NotifyUrl,
                returnUrl = ReturnUrl,
                customerName = model.CustomerName
            };

            var json = JsonConvert.SerializeObject(cashFreeOrderCreate);

            using (var db = new rainbowwineEntities())
            {
                db.CashFreePays.Add(new CashFreePay
                {
                    InputValue = json,
                    OrderId =Convert.ToInt32(model.OrderId),
                    VenderOutput = resp,
                    CreatedDate = DateTime.Now,
                    Amt = model.OrderAmount
                });

                db.SaveChanges();
            }
        }

        private void PaymentLinkSendUpdateToDb(int orderId,string msg)
        {
            using (var db = new rainbowwineEntities())
            {
                int stCashLink = (int)OrderStatusEnum.CashLinkSend;
                var order = db.Orders.Find(orderId);
                order.OrderStatusId = stCashLink;
                db.SaveChanges();

                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                string uId = u.Id;
                OrderTrack orderTrack = new OrderTrack
                {
                    LogUserName = u.Email,
                    LogUserId = uId,
                    OrderId = order.Id,
                    Remark = msg,
                    StatusId = stCashLink,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();
            }

        }
    }
}