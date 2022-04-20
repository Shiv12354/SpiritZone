using Microsoft.AspNet.Identity;
using RainbowWine.Data;
using RainbowWine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Configuration;
using RainbowWine.Services;
using RainbowWine.Services.PaytmService;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using RainbowWine.Services.DBO;
using System.Text;
using System.Web.Http.Cors;
using RainbowWine.Services.DO;
using Microsoft.Ajax.Utilities;
using RainbowWine.Providers;
using SZData.Interfaces;
using static RainbowWine.Services.FireStoreService;
using Google.Cloud.Firestore;
using SZInfrastructure;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;

namespace RainbowWine.Controllers
{
    [RoutePrefix("delapi/v3")]
    [Authorize]
    [DisplayName("Operational")]
    [EnableCors("*", "*", "*")]
    public class DeliveryAgents2Controller : ApiController
    {
        const string API_KEY = "Past your google url shortener api key here";
        NewDelAppDBO newDelAppDBO = new NewDelAppDBO();
        [HttpGet]
        [Route("orderpickedup/{orderid}/{deliveryagentid}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage GetOrderPickedUp(int orderId, int deliveryAgentId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                
                db.Configuration.ProxyCreationEnabled = false;
                string uId = User.Identity.GetUserId();
                var cust = db.Customers.Where(o => o.UserId == uId).FirstOrDefault();
                var prod = newDelAppDBO.OrderPickedUp(orderId, deliveryAgentId,uId);
                responseStatus.Message = prod;
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
            }
            
        }

        [HttpGet]
        [Route("homepagedetail/{index}/{size}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage HomePageDetails(int index = 1, int size = 5)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string uId = User.Identity.GetUserId();
                var c = SZIoc.GetSerivce<ISZConfiguration>();
                string airtelIQUsername = string.Empty;
                string airtelIQPassword = string.Empty;
                string loginDistCheck = string.Empty;
                string slotStartWOLogin = string.Empty;
                string firebaseTrack = string.Empty;
                string firebaseInterval = string.Empty;
                string slotStartedStatusList = string.Empty;
                string slotStartWithoutHandover = string.Empty; 
                string isDelAppHyperTrackOn = string.Empty;
                string PODSucessStatus = string.Empty;
                HandOver handOver = new HandOver();
                EarningAndAttendance earningAndAttendance = new EarningAndAttendance();
                DeliveryAgentDetail deliveryAgentDetail = new DeliveryAgentDetail();
                if (index == 1)
                {
                    airtelIQUsername = c.GetConfigValue(ConfigEnums.AirtelIQUsername.ToString());
                    airtelIQPassword = c.GetConfigValue(ConfigEnums.AirtelIQPassword.ToString());
                    loginDistCheck = c.GetConfigValue(ConfigEnums.LoginDistCheck.ToString());
                    slotStartWOLogin = c.GetConfigValue(ConfigEnums.SlotStartWOLogin.ToString());
                    firebaseTrack = c.GetConfigValue(ConfigEnums.FirebaseTrack.ToString());
                    firebaseInterval = c.GetConfigValue(ConfigEnums.FirebaseInterval.ToString());
                    slotStartedStatusList = c.GetConfigValue(ConfigEnums.SlotStartedStatusList.ToString());
                    slotStartWithoutHandover = c.GetConfigValue(ConfigEnums.SlotStartWithoutHandover.ToString());
                    isDelAppHyperTrackOn = c.GetConfigValue(ConfigEnums.isDelAppHyperTrackOn.ToString());
                    PODSucessStatus = c.GetConfigValue(ConfigEnums.PODSucessStatus.ToString());
                    earningAndAttendance = newDelAppDBO.EarningWithAttendance(uId);
                    deliveryAgentDetail = newDelAppDBO.GetDeliveryAgentDetail(uId);
                    handOver = newDelAppDBO.HandOver(uId);
                }
                var condata = new
                {
                    airtelIQUsername,
                    airtelIQPassword,
                    loginDistCheck ,
                    slotStartWOLogin,
                    firebaseTrack,
                    firebaseInterval ,
                    slotStartedStatusList ,
                    slotStartWithoutHandover ,
                    isDelAppHyperTrackOn ,
                    PODSucessStatus
                };
                
                var oDetails = newDelAppDBO.NewDelOrderDetails(index, size, uId);
                var groupOrdDetails = oDetails.GroupBy(u => u.JobId).Select(grp => new { JobId = grp.Key, Orders = grp.OrderBy(a =>a.AssignedDate).ToList() }).ToList();
                responseStatus.Data = new
                {
                    AgentDetail= deliveryAgentDetail,
                    Earning = earningAndAttendance != null ? earningAndAttendance : new object(),
                    HandOver= handOver,
                    ConfigValues=condata,
                    OrderDetails = groupOrdDetails
                }; 
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);


            }
        }

        [HttpPost]
        [Route("delivery/orders/start/{jobid}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage DeliveryOrdersStart(string jobid)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string uId = User.Identity.GetUserId();
                    var aspUser = db.AspNetUsers.Find(uId);
                    RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                    //db.Configuration.ProxyCreationEnabled = false;

                    RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                    var route = routePlanDBO.DeliveryStart(jobid);
                    //var route = db.RoutePlans.Where(o => string.Compare(o.JobId, jobid, true) == 0 && (o.Order.OrderStatusId == 6 || o.Order.OrderStatusId == 9)).ToList();
                    if (route == null || route.Count() <= 0)
                    {
                        responseStatus.Message = "Currently there is no orders for delivery.";
                    }
                    else
                    {
                        foreach (var item in route)
                        {
                            Order order = db.Orders.Find(item.OrderID);
                            if (order.OrderStatusId == 6)
                            {
                                var r = db.RoutePlans.Where(o => o.OrderID == item.OrderID)?.FirstOrDefault();
                                r.isOutForDelivery = true;
                                db.SaveChanges();


                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = aspUser.Email,//User.Identity.Name,
                                    LogUserId = uId,
                                    OrderId = item.OrderID,
                                    StatusId = 9,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                //db.SaveChanges();

                                var dagent = db.DeliveryAgents.Where(o => o.Id == regUser.DeliveryAgentId).FirstOrDefault();

                                if (dagent != null)
                                {
                                    dagent.IsAtShop = (dagent != null);
                                }

                                order.OrderStatusId = 9;

                                db.SaveChanges();

                                //Live Tracking FireStore
                                CustomerApi2Controller.AddToFireStore(order.Id);
                                WSendSMS wsms = new WSendSMS();
                                string textmsg = string.Format(ConfigurationManager.AppSettings["SMSOutForDelivery"], order.Id.ToString());
                                wsms.SendMessage(textmsg, order.Customer.ContactNo);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrdersStart: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }
            return Request.CreateResponse(responseStatus);
        }

        [HttpPost]
        [Route("delivery/order-delivered/{orderid}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult DeliveryOrderPlaced(int orderId)
        {
            if (orderId == 0)
            {
                return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string uId = User.Identity.GetUserId();
                    var aspUser = db.AspNetUsers.Find(uId);
                    Order order = db.Orders.Find(orderId);
                    if (order == null)
                    {
                        db.Dispose();
                        return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = $"This is no such OrderID {orderId}." });
                    }

                    db.Configuration.ProxyCreationEnabled = false;

                    var statusCancel = (int)OrderStatusEnum.CancelByCustomer;
                    if (order.OrderStatusId == statusCancel)
                    {
                        responseStatus.Status = true;
                        responseStatus.Message = "Order cancel by customer.";
                        return Ok(responseStatus);
                    }

                    order.OrderStatusId = 5;
                    order.DeliveryDate = DateTime.Now;
                    db.SaveChanges();

                    OrderDBO orderDBO = new OrderDBO();

                    if (order.OrderStatusId == 5)
                    {
                        //HyperTracking Complted
                        HyperTracking hyperTracking = new HyperTracking();
                        Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(order.Id));

                        //Live Tracking FireStore
                        CustomerApi2Controller.DeleteToFireStore(order.Id);
                        orderDBO.UpdatedScheduledOrder(order.Id);

                        WebEngageController webEngageController = new WebEngageController();
                        Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Order Delivered"));

                    }
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = aspUser.Email,//User.Identity.Name,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = order.OrderStatusId,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    int pageId = (int)PageNameEnum.MYWALLET;
                    string pageVersion = "1.6.2";
                    var cont = SZIoc.GetSerivce<IPageService>();
                    var content = cont.GetPageContent(pageId, pageVersion);
                    var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
                    var c = SZIoc.GetSerivce<ISZConfiguration>();
                    var numberOfOrder = c.GetConfigValue(ConfigEnums.NumberOfOrder.ToString());
                    var ordCount = db.Orders.Where(o => o.CustomerId == order.CustomerId && o.OrderStatusId == 5);
                    bool isFirsrOrder = true;
                    if (ordCount.Count() > Convert.ToInt32(numberOfOrder))
                    {
                        isFirsrOrder = false;

                    }
                    var ser = SZIoc.GetSerivce<IPromoCodeService>();
                    if (isFirsrOrder)
                    {
                        var discount = ser.GetDiscountOnFirstOrder(order.Customer.UserId);
                        if (discount.CreditAmount > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = discount.CreditAmount + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = discount.CreditAmount + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = discount.CustomerId;
                            walletNotificationRequest.UserID = discount.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }

                    var promocode = db.PromoCodes.Where(o => o.PromoId == order.PromoId).FirstOrDefault();
                    bool isPromoCodeApply = false;
                    if (order.PromoId.HasValue && order.PromoId > 0)
                    {
                        
                        var promoCashback = ser.PromoCodeCashBack(order.Id);
                        isPromoCodeApply = true;
                        if (promoCashback > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = promoCashback + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = promoCashback + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = order.CustomerId;
                            walletNotificationRequest.UserID = order.Customer.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }
                    var cashBackApplicable = c.GetConfigValue(ConfigEnums.CashBackApplicable.ToString());

                    bool isCashBackApplicable = false;
                    if (cashBackApplicable == "1")
                    {
                        isCashBackApplicable = true;

                    }
                    if (isCashBackApplicable)
                    {
                        var Cashback = ser.CashBack(order.Id);
                        if (Cashback.CashBackAmount > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = Cashback.CashBackAmount + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = Cashback.CashBackAmount + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = order.CustomerId;
                            walletNotificationRequest.UserID = order.Customer.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }

                    var incentive = c.GetConfigValue(ConfigEnums.Incentive.ToString());
                    bool isIncentive = false;
                    int tEarning = 0;
                    if (incentive == "1")
                    {
                        isIncentive = true;
                        tEarning = EarningUpdateNew(uId,orderId);
                    }
                    responseStatus.Data = new
                    {
                        IsFirsrOrder = isFirsrOrder,
                        IsPromoCodeApply = isPromoCodeApply,
                        IsCashBackApplicable = isCashBackApplicable,
                        IsIncentive = isIncentive,
                        Earning = tEarning
                    };
                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSDelivered"], order.Id.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                    //wsms.SendMessage(textmsg, order.Customer.ContactNo);
                    //Flow SMS
                    if (order.IsGift == true)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpContext httpContext = HttpContext.Current;

                            string authHeader = httpContext.Request.Headers["Authorization"];

                            var jsonValue = new
                            {
                                OrderId = orderId
                            };
                            var serializeJson = JsonConvert.SerializeObject(jsonValue);
                            var content1 = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                            client.DefaultRequestHeaders.Add("Authorization", authHeader);
                            var resp = client.PostAsync(ConfigurationManager.AppSettings["Giftnotiurl"] + orderId, null).Result;
                            var ret = resp.Content.ReadAsStringAsync().Result;
                        }
                        //SMS Flow
                        //var cust = db.Customers.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                        GiftBagDBO giftBagDBO = new GiftBagDBO();
                        var recipient = giftBagDBO.GetUserRecipientOrderDetails(orderId);
                        if ((recipient != null) && (!string.IsNullOrEmpty(recipient.Occasion) || !string.IsNullOrEmpty(recipient.SplMessage)))
                        {
                            var smsGiftUrl = ConfigurationManager.AppSettings["GiftSmsurl"].ToString();
                            smsGiftUrl = smsGiftUrl + "?mobile=" + recipient.ContactNo + "&token=" + GiftRecipientController.Encrypt(recipient.ContactNo) + "&orderId=" + orderId;
                            var smsUrl = ShrinkURL(smsGiftUrl);
                            var res = giftBagDBO.UpdateUserRecipientOrder(orderId, smsUrl);
                            //var link1 = smsUrl.Substring(0, 28);
                            //var link2 = smsUrl.Substring(28, smsUrl.Length - 28);
                            var dict = new Dictionary<string, string>();
                            dict.Add("ORDERID", order.Id.ToString());
                            dict.Add("DATE", DateTime.Now.ToString("dd-MM-yyyy HH:MM"));
                            dict.Add("SENDER", recipient.CustomerName);
                            dict.Add("LINK", smsUrl);
                            //dict.Add("LINK1", link2);

                            var tempid = ConfigurationManager.AppSettings["SMSGiftRecipientFlowId"];

                            Task.Run(async () => await Services.Msg91.Msg91Service.SendRecipientFlowSms(tempid, recipient.ContactNo, dict));
                            //END SMS Flow
                        }
                        else
                        {
                            var smsGiftUrl = ConfigurationManager.AppSettings["GiftSmsurl"].ToString();
                            var appdownloadlink = ConfigurationManager.AppSettings["Appdownloadlink"].ToString();
                            //smsGiftUrl = smsGiftUrl + "?mobile=" + recipient.ContactNo + "&token=" + GiftRecipientController.Encrypt(recipient.ContactNo) + "&orderId=" + orderId;
                            var smsUrl = ShrinkURL(appdownloadlink);
                            var res = giftBagDBO.UpdateUserRecipientOrder(orderId, smsUrl);
                            var dict = new Dictionary<string, string>();
                            dict.Add("OrderID", order.Id.ToString());
                            dict.Add("Date", DateTime.Now.ToString("dd-MM-yyyy HH:MM"));
                            dict.Add("Sender", recipient.CustomerName);
                            dict.Add("DownloadLink", smsUrl);
                            var tempid = ConfigurationManager.AppSettings["SMSGiftAppdownloadlinkFlowId"];

                            Task.Run(async () => await Services.Msg91.Msg91Service.SendRecipientFlowSms(tempid, recipient.ContactNo, dict));
                        }
                    }

                    

                    var dicti = new Dictionary<string, string>();
                    dicti.Add("ORDERID", order.Id.ToString());
                    dicti.Add("DATETIME", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));

                    var templeteid = ConfigurationManager.AppSettings["SMSDeliveredFlowId"];

                    Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.OrderTo, dicti));
                    //End Flow
                    Task.Run(async () => await fcmHelper.SendFirebaseNotification(orderId, FirebaseNotificationHelper.NotificationType.Order));
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

            return Ok(responseStatus);
        }
        [HttpPost]
        [Route("dashboard/{index}/{size}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage DashBoardDetails(Dates dates, int index = 1, int size = 5)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
               
                db.Configuration.ProxyCreationEnabled = false;
                string uId = User.Identity.GetUserId();
                var earning = newDelAppDBO.Earning(uId, dates.Startdate, dates.Enddate);
                var lastsevendays = newDelAppDBO.LastSevenDaysEarning(uId, dates.Startdate, dates.Enddate);
                var ordersList = newDelAppDBO.OrdersList(uId,dates.Startdate,dates.Enddate);
                

                responseStatus.Data = new
                {
                    Earning = earning,
                    LastSevenDaysEarning = lastsevendays,
                    OrdersList = ordersList
                   
                };
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);


            }
        }
        [HttpPost]
        [Route("detailedview/{type}/{index}/{size}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage DashBoardDetailedView(Dates dates,string type, int index = 1, int size = 5)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {

                db.Configuration.ProxyCreationEnabled = false;
                string uId = User.Identity.GetUserId();
               
                var detailsview = newDelAppDBO.GetDetailsViews(index, size, type, uId, dates.Startdate, dates.Enddate);

                responseStatus.Data = new
                {
                    DetailedView = detailsview
                };
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);


            }
        }

        [HttpPost]
        [Route("myearning/earning-history-penalty/{index}/{size}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetEarningHistoryWithPenalty(Dates dates,int index =1,int size =5)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            var totalearn = newDelAppDBO.TotalEarning(uId, dates.Startdate, dates.Enddate);
            var earn = newDelAppDBO.GetDeliveryEarningWithPenalty(uId,index,size,dates.Startdate,dates.Enddate);
            responseStatus.Data = new { totalEarning = totalearn, earning = earn };

            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("orderdetail/{orderid}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetOrderItemDetails(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            
            var orderItem = newDelAppDBO.GetOrderItemDetails(orderId);
            var data = ManupulateOrderData(orderItem);
            responseStatus.Data = new { OrderItems = data };

            return Ok(responseStatus);

        }
        
        #region Mark Attendance via toggle
        
        [HttpPost]
        [Route("delivery/logon/{onoff}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult OnDeliveryAsync(int onoff)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var isHyperTrackOn = c.GetConfigValue(ConfigEnums.IsHyperTrackOn.ToString());
            
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    if (onoff < 2)
                    {
                        string uId = User.Identity.GetUserId();
                        RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                        bool isAgentNotAllowed = newDelAppDBO.GetDeliveryAgent(regUser.DeliveryAgentId.Value);
                        db.Configuration.ProxyCreationEnabled = false;
                        var cdate = DateTime.Now;
                        var loginAgent = db.DeliveryAgentLogins.Where(o => o.DeliveryAgentId == regUser.DeliveryAgentId
                        && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day && o.OffDuty == null).FirstOrDefault();
                        if (loginAgent == null)
                        {
                            DeliveryAgentLogin deliveryAgentLogin = new DeliveryAgentLogin
                            {
                                DeliveryAgentId = regUser.DeliveryAgentId ?? 0,
                                IsOnOff = true,
                                OnDuty = DateTime.Now
                            };
                            db.DeliveryAgentLogins.Add(deliveryAgentLogin);
                            db.SaveChanges();
                            loginAgent = db.DeliveryAgentLogins.Where(o => o.DeliveryAgentId == regUser.DeliveryAgentId
                             && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day).OrderByDescending(x => x.Id).FirstOrDefault();
                            var loginAgentSecondTime = db.DeliveryAgentLogins.Where(o => o.DeliveryAgentId == regUser.DeliveryAgentId
                              && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day).OrderBy(o => o.Id).ToList();
                            if (loginAgentSecondTime.Count() > 1)
                            {

                                TimeSpan varTime = (DateTime)loginAgentSecondTime.LastOrDefault().OnDuty - (DateTime)loginAgentSecondTime[loginAgentSecondTime.Count() - 2].OffDuty;
                                double fractionalMinutes = varTime.TotalMinutes;
                                int minutesRounded = (int)Math.Round(varTime.TotalMinutes);
                                loginAgentSecondTime.LastOrDefault().BreaksInMinutes = minutesRounded;
                                db.SaveChanges();
                            }

                            if (isHyperTrackOn == "1" && (isAgentNotAllowed == false || isAgentNotAllowed == null))
                            {
                                Task.Run(async () => await HyperTrackStart(uId));
                            }
                        }
                        else
                        {
                            if (onoff == 1)
                            {
                                loginAgent.OffDuty = DateTime.Now;
                                if (isHyperTrackOn == "1")
                                {
                                    Task.Run(async () => await HyperTrackStop(uId));
                                }
                                
                            }
                            loginAgent.IsOnOff = (onoff == 0);
                            db.SaveChanges();
                        }
                        responseStatus.Status = true;
                        responseStatus.Data = loginAgent;
                    }
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_OnAgent: {ex.Message}", ex.StackTrace);

                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }
            return Content(HttpStatusCode.OK, responseStatus);
        }
        #endregion

        #region Navigation Page
        
        [HttpPost]
        [Route("delivery/order-reached/{orderId}")]
        [Authorize]
        public IHttpActionResult DeliveryOrderReached(int orderId)
        {
            if (orderId == 0)
            {
                return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string uId = User.Identity.GetUserId();

                    var statusCancel = (int)OrderStatusEnum.CancelByCustomer;
                    var statusDelReached = (int)OrderStatusEnum.DeliveryReached;
                    var statusPODCashSel = (int)OrderStatusEnum.PODCashSelected;
                    var order = db.Orders.Find(orderId);
                    if (order.OrderStatusId == statusDelReached || order.OrderStatusId == statusPODCashSel)
                    {
                        responseStatus.Status = true;
                        responseStatus.Message = "Update order status as REACHED";

                    }
                    else if (order.OrderStatusId == statusCancel)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "The customer has cancelled this order. Please proceed with the next order.";

                    }

                    else
                    {
                        //Live Tracking FireStore
                        OrderDBO orderDBO = new OrderDBO();
                        orderDBO.UpdatedOrderStatus(orderId, uId, OrderStatusEnum.DeliveryReached.ToString());
                        CustomerApi2Controller.AddToFireStore(orderId);
                        OrderIssueDBO cashDBO = new OrderIssueDBO();
                        var issue = cashDBO.GetPendingPay(orderId);
                        //var issue = db.OrderIssues.Where(o => o.OrderId == orderId && (o.IsCashOnDelivery ?? false) == true)?.OrderByDescending(o => o.OrderIssueId).FirstOrDefault();


                        if (issue != null)
                        {
                            IssueType[] issueTypePay = new[] { IssueType.PartialPay, IssueType.PartialRefund };
                            int[] issueTypePayValue = issueTypePay.Cast<int>().ToArray();

                            string orderIssueTypePay = "";
                            if (issue.IsCashOnDelivery ?? false)
                            {
                                if (issueTypePayValue.Contains(issue.OrderIssueTypeId ?? 0))
                                {
                                    orderIssueTypePay = ((issue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialPay) ? "CashPay" : "CashRefund";
                                }
                            }

                            responseStatus.Data = new { AdjustAmt = issue.AdjustAmt, AmtType = orderIssueTypePay };
                        }

                        responseStatus.Status = true;
                        responseStatus.Message = "Update order status as REACHED.";
                    }
                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    Task.Run(async () => await fcmHelper.SendFirebaseNotification(orderId, FirebaseNotificationHelper.NotificationType.Order));
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_DeliveryOrderReached: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

        }

        [HttpPost]
        [Route("verifypin")]
        [AllowAnonymous]
        public IHttpActionResult UserVerifyPin(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
                using (var db = new rainbowwineEntities())
                {
                    {
                        try
                        {
                            string uId = User.Identity.GetUserId();
                            var delEamil = db.AspNetUsers.Where(x => x.Id == uId).FirstOrDefault().Email;

                            db.Configuration.ProxyCreationEnabled = false;
                            //var rcust = db.AspNetUsers.Where(o => string.Compare(o.Email, req.Email, true) == 0)?.FirstOrDefault();
                            var otp = db.CustomerPayPins.Where(o => string.Compare(o.Email, delEamil, true) == 0 && o.OrderId == req.OrderId && o.IsDeleted == false)?.FirstOrDefault();
                            if (otp == null)
                            {
                                responseStatus.Status = false;
                                responseStatus.Message = "There is no verification pin.";
                            }
                            else
                            {
                                if (string.Compare(req.Pin, otp.Pin) == 0)
                                {
                                    otp.VerifiedDate = DateTime.Now;
                                    otp.IsDeleted = true;
                                    db.SaveChanges();
                                    responseStatus.Status = true;
                                    responseStatus.Message = "Verification Successful.";

                                }
                                else
                                {
                                    responseStatus.Status = false;
                                    responseStatus.Message = "Verification pin does not match.";
                                }
                            }
                            if (responseStatus.Status == true)
                            {
                                int statusDelPinVerified = (int)OrderStatusEnum.DelPinVerified;
                                var ema = ConfigurationManager.AppSettings["TrackEmail"];
                                var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = u.Id,
                                    OrderId = req.OrderId,
                                    StatusId = statusDelPinVerified,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();
                                
                               
                            }
                            return Ok(responseStatus);
                        }
                        catch (Exception ex)
                        {

                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = true,
                                Message = "Some error occurred while processign the request"
                            };
                            SpiritUtility.AppLogging($"Api_UserVerifyPin: {ex.Message}", ex.StackTrace);
                            db.Dispose();
                            return Content(HttpStatusCode.InternalServerError, responseStatus);

                        }
                    }
                }
        }

        [HttpPost]
        [Route("delivery/orders")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage DeliveryOrders()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                string uId = User.Identity.GetUserId();
                RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                db.Configuration.ProxyCreationEnabled = false;
                RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                var routePlans = routePlanDBO.DevlieryOrders(regUser.DeliveryAgentId ?? 0);

                List<object> list = new List<object>();
                foreach (var item in routePlans)
                {
                    //var custAddress = db.CustomerAddresses.Find(item.Order.CustomerAddressId);
                    var orderWine = db.Orders.Include(o => o.OrderDetails).Include(o => o.WineShop).Include(o => o.Customer).Where(o => o.Id == item.OrderID).FirstOrDefault();
                    var customerLogin = db.AspNetUsers.Find(orderWine.Customer.UserId);
                    var custAddress = db.CustomerAddresses.Find(orderWine.CustomerAddressId);

                    var oSerialize = JsonConvert.SerializeObject(orderWine?.OrderDetails, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerialize = JsonConvert.DeserializeObject<List<OrderDetail>>(oSerialize);


                    list.Add(new
                    {
                        AgentId = regUser.DeliveryAgentId,
                        JobId = item.JobId,
                        OrderId = item.OrderID,
                        OrderDetails = deSerialize,
                        CustomerId = orderWine.Customer.Id,
                        EmailId = customerLogin?.Email,
                        PaymentTypeId = orderWine?.PaymentTypeId,
                        CustomerNo = orderWine.Customer.ContactNo,
                        AddressId = item.Order.CustomerAddressId,
                        Address = custAddress.Address,
                        FormattedAddress = custAddress.FormattedAddress,
                        Flat = custAddress.Flat,
                        Landmark = custAddress.Landmark,
                        Latitude = custAddress.Latitude,
                        Longitude = custAddress.Longitude,
                        ShopId = orderWine.WineShop.Id,
                        ShopName = orderWine.WineShop.ShopName,
                        ShopLatitude = orderWine.WineShop.Latitude,
                        ShopLongitude = orderWine.WineShop.Longitude
                    });
                }
                responseStatus.Data = new { orderPlan = routePlans, orderTrackAddress = list };
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("delivery/order/backtostore")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage DeliveryOrderBacktostore(BackToStoreStatus backToStoreStatus)
        {
            if (backToStoreStatus.OrderId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    int orderId = backToStoreStatus.OrderId;
                    string remark = backToStoreStatus.Remark;
                    string uId = User.Identity.GetUserId();
                    var aspUser = db.AspNetUsers.Find(uId);
                    Order order = db.Orders.Find(orderId);
                    if (order == null)
                    {
                        db.Dispose();
                        return Request.CreateResponse(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = $"This is no such OrderID {orderId}." });
                    }

                    db.Configuration.ProxyCreationEnabled = false;
                    order.OrderStatusId = 27;
                    order.DeliveryDate = DateTime.Now;
                    db.SaveChanges();

                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = aspUser.Email,//User.Identity.Name,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = order.OrderStatusId,
                        TrackDate = DateTime.Now,
                        Remark = remark
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    //Added for Back to store for packer

                    var routeOrder = db.RoutePlans.Include(o => o.Order).Where(o => o.OrderID == orderId)?.FirstOrDefault();
                    DeliveryBackToStore delBackToStore = new DeliveryBackToStore
                    {
                        OrderAmount = Convert.ToDouble(order.OrderAmount),
                        CreatedDate = DateTime.Now,
                        DeliveryAgentId = routeOrder.DeliveryAgentId,
                        Reason = remark,
                        JobId = routeOrder.JobId,
                        OrderId = order.Id,
                        ShopId = order.ShopID,
                        ShopAcknowledgement = false
                    };
                    db.DeliveryBackToStores.Add(delBackToStore);
                    db.SaveChanges();
                    db.RoutePlans.Remove(routeOrder);
                    db.SaveChanges();
                    //HyperTracking Complted
                    HyperTracking hyperTracking = new HyperTracking();
                    Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(backToStoreStatus.OrderId));


                    WebEngageController webEngageController = new WebEngageController();
                    Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Order Return Initiated"));
                    if(backToStoreStatus.Remark == "Customer not Available" || backToStoreStatus.Remark == "Call not answered")
                    {
                       
                        Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Delivery Failed"));
                    }
                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSDelivered"], order.Id.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                    //wsms.SendMessage(textmsg, order.Customer.ContactNo);
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("delivery/track-order-start/{orderId}")]
        [Authorize]
        public IHttpActionResult UpdateTrackOrderStart(int orderId)
        {
            
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (orderId <= 0)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order Id is not valid.";
                return Content(HttpStatusCode.BadRequest, responseStatus);
            }
            using (var db = new rainbowwineEntities())
            {

                string Id = User.Identity.GetUserId();
                var ruser = db.RUsers.Where(o => o.rUserId == Id).FirstOrDefault();
                NewDelAppDBO newDelAppDBO = new NewDelAppDBO();
                bool isAgentNotAllowed = newDelAppDBO.GetDeliveryAgent(ruser.DeliveryAgentId.Value);

                var agentRoute = db.RoutePlans.Where(o => o.OrderID == orderId && o.DeliveryAgentId == ruser.DeliveryAgentId)?.FirstOrDefault();
                if (agentRoute == null)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Order does not found against the delivery boy.";
                    return Content(HttpStatusCode.BadRequest, responseStatus);
                }
                if (agentRoute.IsOrderStart == false)
                {
                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    Task.Run(() => fcmHelper.SendFirebaseNotification(orderId, FirebaseNotificationHelper.NotificationType.Order));
                }

                agentRoute.IsOrderStart = true;
                db.SaveChanges();

                var tripResponse = newDelAppDBO.GetHyperTrackTripId(orderId);

                var c = SZIoc.GetSerivce<ISZConfiguration>();
                var isHyperTrackOn = c.GetConfigValue(ConfigEnums.IsHyperTrackOn.ToString());
                if (isHyperTrackOn == "1" && tripResponse == null && (isAgentNotAllowed == false || isAgentNotAllowed == null))
                {
                    HyperTrackTripDetails(orderId, Id);
                }

                // Live Tracking for hypertrack

                var tripResponseData = newDelAppDBO.GetHyperTrackTripId(orderId);

                if (tripResponseData != null)
                {
                    if (!string.IsNullOrEmpty(tripResponseData.TripId) && !string.IsNullOrEmpty(tripResponseData.DeviceId))
                    {
                        CustomerApi2Controller.AddToFireStore(orderId);
                    }
                }

                // Live Tracking for firestore
                else
                {
                    CustomerApi2Controller.AddToFireStore(orderId);
                }

                WebEngageController webEngageController = new WebEngageController();
                Task.Run(async () => await webEngageController.WebEngageStatusCall(orderId, "Out For Delivery"));


                db.Dispose();



                responseStatus.Status = true;
                responseStatus.Message = "Order start for delivery by delivery boy.";
            }
            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("generatepinbydelivery")]
        [AllowAnonymous]
        public IHttpActionResult UserGeneratePinByDelivery(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var pinNo = SpiritUtility.GenerateToken4D();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                using (var db = new rainbowwineEntities())
                {

                    try
                    {
                        var rcust = db.AspNetUsers.Where(o => string.Compare(o.Email, req.Email, true) == 0)?.FirstOrDefault();
                        var otp = db.CustomerPayPins.Where(o => o.UserId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                        if (otp == null)
                        {
                            CustomerPayPin customerPayPin = new CustomerPayPin
                            {
                                ContactNo = rcust.PhoneNumber,
                                CreatedDate = DateTime.Now,
                                UserId = rcust.Id,
                                Email = req.Email,
                                Pin = pinNo,
                                IsDeleted = false
                            };
                            db.CustomerPayPins.Add(customerPayPin);
                            db.SaveChanges();
                        }
                        else
                        {
                            var pno = string.IsNullOrWhiteSpace(rcust.PhoneNumber) ? "" : rcust.PhoneNumber;
                            var _pinno = new SqlParameter { ParameterName = "_pinno", Value = pinNo };
                            var _phoneNo = new SqlParameter { ParameterName = "_phoneNo", Value = pno };
                            var _userId = new SqlParameter { ParameterName = "_UserId", Value = rcust.Id };
                            var _email = new SqlParameter { ParameterName = "_Email", Value = req.Email };


                            SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.VarChar, 200);
                            returnCode.Direction = ParameterDirection.Output;

                            db.Database.ExecuteSqlCommand("CustomerPayPin_InsUpd @_pinno,@_phoneNo,@_UserId,@_Email,@Result out", _pinno, _phoneNo, _userId, _email, returnCode);
                            string Status = Convert.ToString(returnCode.Value);
                            if (Status != null)
                            {
                                pinNo = Status;
                            }
                        }

                        int statusCustGeneratePin = (int)OrderStatusEnum.DelPinGenerated;
                        var ema = ConfigurationManager.AppSettings["TrackEmail"];
                        var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                        foreach (var item in req.OrderIds)
                        {
                            OrderTrack orderTrack = new OrderTrack
                            {
                                LogUserName = u.Email,
                                LogUserId = u.Id,
                                OrderId = item,
                                StatusId = statusCustGeneratePin,
                                TrackDate = DateTime.Now
                            };
                            db.OrderTracks.Add(orderTrack);
                        }
                        db.SaveChanges();

                        responseStatus.Data = new { pin = pinNo, contactNo = req.PhoneNo };
                        responseStatus.Message = $"A verification code has been sent to {req.PhoneNo}. Standard rates apply";

                        return Ok(responseStatus);
                    }
                    catch (Exception ex)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Some error occurred while processing the request"
                        };
                        SpiritUtility.AppLogging($"Api_UserGeneratePinPackerSubmit: {ex.Message}", ex.StackTrace);
                        db.Dispose();
                        return Content(HttpStatusCode.InternalServerError, responseStatus);
                    }
                }
            }

        }


        [HttpPost]
        [Route("delivery-handover-comfirm/{jobId}")]
        public IHttpActionResult DeliveryHandoverConfirm(string jobId)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                using (var db = new rainbowwineEntities())
                {

                    try
                    {

                        string uId = User.Identity.GetUserId();
                        db.Configuration.ProxyCreationEnabled = false;
                        var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();

                        var delpay = db.DeliveryPayments.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.JobId == jobId && o.ShopAcknowledgement != true);
                        var delback = db.DeliveryBackToStores.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.JobId == jobId && o.ShopAcknowledgement != true);
                        bool packerConfirm = (delpay.Count() == 0 && delback.Count() == 0);

                        responseStatus.Data = new { packerConfirm = packerConfirm };
                        responseStatus.Status = true;
                        return Ok(responseStatus);
                    }
                    catch (Exception ex)
                    {

                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Some error occurred while processign the request"
                        };
                        SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                        db.Dispose();
                        return Content(HttpStatusCode.InternalServerError, responseStatus);

                    }
                    finally
                    {
                        db.Dispose();
                    }
                }
            }

        }

        [HttpPost]
        [Route("delivery-handover")]
        public IHttpActionResult DeliveryHandoverNew()
        {
            ResponseStatus responseStatus = new ResponseStatus();



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                using (var db = new rainbowwineEntities())
                {
                    try
                    {
                        string uId = User.Identity.GetUserId();
                        db.Configuration.ProxyCreationEnabled = false;
                        var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();
                        var delagent = db.DeliveryAgents.Include(o => o.WineShop).Where(o => o.Id == ruser.DeliveryAgentId).FirstOrDefault();
                        var aspUSer = db.AspNetUsers.Find(uId);

                        OrderDBO orderDBO = new OrderDBO();
                        var delpay = orderDBO.GetDeliveryPaymentDetails(ruser.DeliveryAgentId ?? 0);
                        var delback = orderDBO.GetDeliveryBacktoStoreDetailsNew(ruser.DeliveryAgentId ?? 0);
                       
                        var oSerializeAgent = JsonConvert.SerializeObject(delagent, Formatting.None,
                               new JsonSerializerSettings()
                               {
                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                               });
                        var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryAgent>(oSerializeAgent);

                        var cashDel = delpay.GroupBy(x =>x.JobId).Select(y => new 
                        {
                            JobId=y.Key,
                            Orders =y.Select(d => new 
                            {
                                OrderId=d.Order.Id,
                                d.Order.OrderDate,
                                d.Order.OrderPlacedBy,
                                d.Order.OrderTo,
                                d.Order.OrderAmount,
                                d.Order.ShopID,
                                d.Order.DeliveryDate,
                                d.DeliveryPaymentId,
                                d.PaymentTypeId,
                                d.AmountPaid,
                                d.CreatedDate,
                                d.DelPaymentConfirm,
                                OrderDetails = d.Order.OrderDetails.Select(a => new {
                                    a.Id,
                                    a.OrderId,
                                    a.ItemQty,
                                    a.Price,
                                    a.ShopID,
                                    a.Issue,
                                    a.ProductID,
                                    ProductDetails=new
                                    {
                                        a.ProductDetail.ProductID,
                                        a.ProductDetail.ProductName,
                                        Volume=a.ProductDetail.Category,
                                        a.ProductDetail.Price,
                                        a.ProductDetail.ShopItemId,
                                        a.ProductDetail.ProductImage,
                                        a.ProductDetail.ProductType
                                       

                                    }
                               })

                            })
                        });

                        var backToStoreDel = delback.GroupBy(x => x.JobId).Select(y => new
                        {
                            JobId=y.Key,
                            Orders= y.Select(b => new
                            {
                                OrderId=b.Order.Id,
                                b.Order.OrderDate,
                                b.Order.OrderPlacedBy,
                                b.Order.OrderTo,
                                b.Order.OrderAmount,
                                b.Order.ShopID,
                                b.Order.DeliveryDate,
                                b.CreatedDate,
                                PickedDate = b.Order.PickedUpDate,
                                OrderDetails= b.Order.OrderDetails.Select(c => new 
                                {
                                    c.Id,
                                    c.OrderId,
                                    c.ItemQty,
                                    c.Price,
                                    c.ShopID,
                                    c.Issue,
                                    c.ProductID,
                                    ProductDetails=new
                                    {
                                        c.ProductDetail.ProductID,
                                        c.ProductDetail.ProductName,
                                        Volume = c.ProductDetail.Category,
                                        c.ProductDetail.ProductType,
                                        c.ProductDetail.Price,
                                        c.ProductDetail.ShopItemId,
                                        c.ProductDetail.ProductImage,
                                    }
                                })
                            })

                        });
                        responseStatus.Data = new { cash = cashDel, backtostore = backToStoreDel};
                        responseStatus.Status = true;
                        return Ok(responseStatus);
                    }
                    catch (Exception ex)
                    {



                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Some error occurred while processign the request"
                        };
                        SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                        db.Dispose();
                        return Content(HttpStatusCode.InternalServerError, responseStatus);



                    }
                    finally
                    {
                        db.Dispose();
                    }
                }

            }

        }

        [HttpGet]
        [Route("configdata")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetConfigData()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var deliveryReachedDistCheckFlag = c.GetConfigValue(ConfigEnums.DeliveryReachedDistCheckFlag.ToString());
            var minDistDeliveryReached = c.GetConfigValue(ConfigEnums.MinDistDeliveryReached.ToString());
            var verifyPrepaidOTPFlag = c.GetConfigValue(ConfigEnums.VerifyPrepaidOTPFlag.ToString());
            var verifyPostpaidOTPFlag = c.GetConfigValue(ConfigEnums.VerifyPostpaidOTPFlag.ToString());
            var verifyPostpaidUrl = c.GetConfigValue(ConfigEnums.VerifyPostpaidUrl.ToString());
            var verifyPrepaidUrl = c.GetConfigValue(ConfigEnums.VerifyPrepaidUrl.ToString());

            var condata = new
            {
                DeliveryReachedDistCheckFlag = deliveryReachedDistCheckFlag,
                MinDistDeliveryReached = minDistDeliveryReached,
                VerifyPrepaidOTPFlag = verifyPrepaidOTPFlag,
                VerifyPostpaidOTPFlag = verifyPostpaidOTPFlag,
                VerifyPostpaidUrl = verifyPostpaidUrl,
                VerifyPrepaidUrl = verifyPrepaidUrl
            };
            responseStatus.Data = new { ConfigValues = condata };

            return Ok(responseStatus);

        }

        [HttpGet]
        [Route("tripcompleted/{orderid}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetTripCompleted(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            //HyperTracking Complted
            HyperTracking hyperTracking = new HyperTracking();
            Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(orderId));
            return Ok(responseStatus);

        }

        #endregion

        #region Non action
        public int EarningUpdateNew(string userId,int orderId)
        {

            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var earn = deliveryEarningService.AddEarningNew(userId, orderId);
            if (earn < 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    string fcm_title = default(string);
                    string fcm_msg = default(string);

                    fcm_title = string.Format("Order {0} LATE DELIVERY PENALTY!", orderId.ToString());
                    fcm_msg = string.Format("You just lost Rs. {0} because you delivered order {1} after 75 mins!!", earn.ToString(), orderId.ToString());
                    var jsonValue = new
                    {
                        OrderId = orderId,
                        Title = fcm_title,
                        Message = fcm_msg
                    };
                    var serializeJson = JsonConvert.SerializeObject(jsonValue);
                    var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                    var resp = client.PostAsync(ConfigurationManager.AppSettings["DeliveryAppGeneralNotifURL"], content).Result;
                    var ret = resp.Content.ReadAsStringAsync().Result;
                }
            }


            return earn;

        }

        public dynamic ManupulateOrderData(IEnumerable<OrderItem> data)
        {
            return data.Select(x => new
            {
                x.OrderId,
                x.OrderStatusId,
                x.DeliveryDate,
                x.OrderStatusName,
                x.OrderDate,
                x.OrderAmount,
                x.ShopID,
                x.LicPermitNo,
                x.PermitCost,
                x.PackedDate,
                x.PaymentTypeId,
                x.LineItems
            });
        }

        public async Task<IHttpActionResult> HyperTrackStart(string uId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //string uId = User.Identity.GetUserId();
                    RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();


                    using (HttpClient client = new HttpClient())
                    {
                        var hyperTrackBasicAuthToken = c.GetConfigValue(ConfigEnums.HyperTrackBasicAuthToken.ToString());
                        var hyperTracStartUrl = ConfigurationManager.AppSettings["HyperTrackStart"].ToString();
                        var hyperTracPatchUrl = ConfigurationManager.AppSettings["HyperTrackPatchUrl"].ToString();
                        var deviceId = newDelAppDBO.GetHyperTrackDeviceId(uId);
                        if (deviceId != null)
                        {
                            if (!string.IsNullOrEmpty(deviceId.HyperTrackDeviceId))
                            {
                                client.DefaultRequestHeaders.Add("Authorization", "Basic " + hyperTrackBasicAuthToken);
                                var resp = await client.PostAsync(hyperTracStartUrl.Replace("{DeviceID}", deviceId.HyperTrackDeviceId.ToString()), null);
                                var ret = JsonConvert.DeserializeObject<object>(resp.Content.ReadAsStringAsync().Result);

                                if (resp.IsSuccessStatusCode && deviceId.isMetaSet == false)
                                {
                                    var delAgentName = db.DeliveryAgents.Where(x => x.Id == regUser.DeliveryAgentId).FirstOrDefault();
                                    var shop = db.WineShops.Where(a => a.Id == delAgentName.ShopID).FirstOrDefault();
                                    var daType = db.DeliveryAgentTypes.Where(b => b.DeliveryAgentTypeId == delAgentName.DeliveryAgentTypeId).FirstOrDefault();
                                    var method = new HttpMethod("PATCH");
                                    var body = new
                                    {
                                        name = delAgentName.Id.ToString(),
                                       
                                        metadata = new
                                        {
                                            patched = true,
                                            Shop = shop.ShopName,
                                            Vehicle = "Bike",
                                            AgentType = daType.Descriptions,
                                            DeliveryAgentId=delAgentName.Id.ToString(),
                                            AgentName =delAgentName.DeliveryExecName
                                        }

                                    };
                                    var json = JsonConvert.SerializeObject(body);

                                    var request = new HttpRequestMessage(method, hyperTracPatchUrl.Replace("{DeviceID}", deviceId.HyperTrackDeviceId.ToString()))
                                    {
                                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                                    };
                                    request.Headers.Add("Authorization", "Basic " + hyperTrackBasicAuthToken);
                                    var result = client.SendAsync(request).Result;
                                    if (result.IsSuccessStatusCode)
                                    {
                                        var upd = newDelAppDBO.UpdateHyperTrackDevice(uId, 1);
                                    }
                                }
                                if (ret != null)
                                {
                                    responseStatus.Message = "Hyper Tracking Start...";
                                    responseStatus.Status = true;
                                    return Content(HttpStatusCode.OK, responseStatus);
                                }

                                else
                                {
                                    responseStatus.Message = "External Api Is Not Respond";
                                    responseStatus.Status = false;
                                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                                }
                            }
                            else
                            {
                                responseStatus.Message = "Device Id Is Not found";
                                responseStatus.Status = false;
                                return Content(HttpStatusCode.InternalServerError, responseStatus);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Content(HttpStatusCode.OK, responseStatus);

        }

        public async Task<IHttpActionResult> HyperTrackStop(string uId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //string uId = User.Identity.GetUserId();
                    RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                    using (HttpClient client = new HttpClient())
                    {

                        var hyperTrackBasicAuthToken = c.GetConfigValue(ConfigEnums.HyperTrackBasicAuthToken.ToString());
                        var hyperTracStopUrl = ConfigurationManager.AppSettings["HyperTrackStop"].ToString();
                        var deviceId = newDelAppDBO.GetHyperTrackDeviceId(uId);
                        if (deviceId != null)
                        {
                            if (!string.IsNullOrEmpty(deviceId.HyperTrackDeviceId))
                            {
                                client.DefaultRequestHeaders.Add("Authorization", "Basic " + hyperTrackBasicAuthToken);
                                var resp = await client.PostAsync(hyperTracStopUrl.Replace("{DeviceID}", deviceId.HyperTrackDeviceId.ToString()), null);


                                var ret = JsonConvert.DeserializeObject<object>(resp.Content.ReadAsStringAsync().Result);
                                if (ret != null)
                                {
                                    responseStatus.Message = "Hyper Tracking Stop...";
                                    responseStatus.Status = true;
                                    return Content(HttpStatusCode.OK, responseStatus);
                                }
                                else
                                {
                                    responseStatus.Message = "External Api Is Not Respond";
                                    responseStatus.Status = false;
                                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                                }
                            }
                            else
                            {
                                responseStatus.Message = "Device Id Is Not found";
                                responseStatus.Status = false;
                                return Content(HttpStatusCode.InternalServerError, responseStatus);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Content(HttpStatusCode.OK, responseStatus);
        }

        public IHttpActionResult HyperTrackTripDetails(int orderId, string uId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //string uId = User.Identity.GetUserId();
                    RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                    using (HttpClient client = new HttpClient())
                    {

                        var hyperTrackBasicAuthToken = c.GetConfigValue(ConfigEnums.HyperTrackBasicAuthToken.ToString());
                        var hyperTrackTripUrl = ConfigurationManager.AppSettings["HyperTrackTripUrl"].ToString();
                        var radius = ConfigurationManager.AppSettings["Radius"].ToString();
                        var service_time = ConfigurationManager.AppSettings["ServiceTime"].ToString();
                        var deviceId = newDelAppDBO.GetHyperTrackDeviceId(uId);
                        if (deviceId !=null)
                        {
                            var details = newDelAppDBO.GetHypertrackTripDetail(orderId);
                            if (details != null)
                            {
                                if (!string.IsNullOrEmpty(deviceId.HyperTrackDeviceId) && !string.IsNullOrEmpty(details.OrderId))
                                {
                                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + hyperTrackBasicAuthToken);
                                    var body = new
                                    {
                                        device_id = deviceId.HyperTrackDeviceId,
                                        orders = new List<dynamic>()
                                        { 
                                            new
                                        {
                                            order_id=details.OrderId,
                                            destination=new
                                            {
                                                geometry=new
                                                {
                                                    type="Point",
                                                    coordinates=new List<dynamic>()
                                                    {
                                                       details.Longitude,
                                                       details.Latitude
                                                    },

                                                },
                                                address=details.Address,
                                                radius=Convert.ToInt32(radius)
                                            },
                                            //scheduled_at="2021-06-20T22:00:00.000Z",
                                            scheduled_at=details.Scheduled_At.ToUniversalTime(),
                                            service_time=Convert.ToInt32(service_time)

                                         }

                                 }
                                    };
                                    var json = JsonConvert.SerializeObject(body);
                                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                                    var resp =  client.PostAsync(hyperTrackTripUrl, content).Result;
                                    var ret = JsonConvert.DeserializeObject<Root>(resp.Content.ReadAsStringAsync().Result);
                                    if (resp.IsSuccessStatusCode)
                                    {
                                        var res = newDelAppDBO.HyperTrackTripResponseAdd(ret.trip_id, ret.started_at, Convert.ToDateTime((ret.completed_at != null) ? ret.completed_at : DateTime.Now), ret.orders.FirstOrDefault().share_url, ret.device_id, regUser.DeliveryAgentId.Value, orderId);
                                       
                                        responseStatus.Message = "Hyper Tracking trip Created Successfully";
                                        responseStatus.Status = true;
                                        return Content(HttpStatusCode.InternalServerError, responseStatus);

                                    }
                                    else
                                    {
                                        responseStatus.Message = "External Api Is Not Respond";
                                        responseStatus.Status = false;
                                        return Content(HttpStatusCode.InternalServerError, responseStatus);


                                    }

                                }
                                else
                                {
                                    responseStatus.Message = "Device Id Or OrderId Is Not Found";
                                    responseStatus.Status = false;
                                    return Content(HttpStatusCode.InternalServerError, responseStatus);


                                }
                            }

                        }
                       

                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Content(HttpStatusCode.InternalServerError, responseStatus);
        }

        public string ShrinkURL(string strURL)
        {

            string URL;
            URL = "http://tinyurl.com/api-create.php?url=" +
               strURL;

            System.Net.HttpWebRequest objWebRequest;
            System.Net.HttpWebResponse objWebResponse;

            System.IO.StreamReader srReader;

            string strHTML;

            objWebRequest = (System.Net.HttpWebRequest)System.Net
               .WebRequest.Create(URL);
            objWebRequest.Method = "GET";

            objWebResponse = (System.Net.HttpWebResponse)objWebRequest
               .GetResponse();
            srReader = new System.IO.StreamReader(objWebResponse
               .GetResponseStream());

            strHTML = srReader.ReadToEnd();

            srReader.Close();
            objWebResponse.Close();
            objWebRequest.Abort();

            return (strHTML);

        }

        //[HttpGet]
        //[Route("cashfreestatus/{orderid}")]
        //public IHttpActionResult CachFreeStatuCheck(int orderId)
        //{
        //    ResponseStatus responseStatus = new ResponseStatus { Status = false };
        //    var c = SZIoc.GetSerivce<ISZConfiguration>();
        //    string payUrl = "https://test.cashfree.com/api/v1/order/info/status"; //ConfigurationManager.AppSettings["PayCreate"];
        //    using (var db = new rainbowwineEntities())
        //    {
        //        try
        //        {
        //            //string uId = User.Identity.GetUserId();
        //            //RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
        //            var appId = ConfigurationManager.AppSettings["PayFreeId"];
        //            var secretKey = ConfigurationManager.AppSettings["PayKey"];
        //            using (HttpClient client = new HttpClient())
        //            {
        //                Uri url = new Uri($"{payUrl}");
        //                var body = new Dictionary<string, string>
        //                {
        //                    { "appId", appId },
        //                    { "secretKey", secretKey },
        //                    { "orderId", orderId.ToString() }
        //                };


        //                var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
        //                var response = client.SendAsync(req).Result;
        //                var resp = response.Content.ReadAsStringAsync().Result;
        //                var ret = JsonConvert.DeserializeObject<CashFreeResponseStatus>(resp);
        //                if (ret.txStatus == "SUCCESS")
        //                {
        //                ConvenienceFeeDetailDBO convenienceFeeDetailDBO = new ConvenienceFeeDetailDBO();
        //                var data = convenienceFeeDetailDBO.GetConvenienceFeeDetails(orderId, ret.paymentDetails.bankName);
        //                    responseStatus.Data = data;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    return Content(HttpStatusCode.InternalServerError, responseStatus);
        //}




        #endregion
    }
}
