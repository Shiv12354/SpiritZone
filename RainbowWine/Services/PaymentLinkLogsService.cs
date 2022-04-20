using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using RainbowWine.Controllers;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using RainbowWine.Services.OnlinePaymentService;
using SZData.Interfaces;
using SZInfrastructure;
using WebGrease.Css.Extensions;
using static RainbowWine.Services.FireStoreService;

namespace RainbowWine.Services
{
    public class PaymentLinkLogsService
    {
        private readonly object fcmHelper;

        public void UpdateOrderLogs(int orderId)
        {

            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("PaymentLinkLogs_Active_Upd", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@OrderId", orderId));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();
                }
            }
        }

        public void UpdateRefundOrderLogs(int orderId)
        {

            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("PaymentRefund_Active_Upd", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@OrderId", orderId));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();
                }
            }
        }

        //public string UpdateOrderToApprove(CashFreeSetApproveResponse decodevlaue, string pdecodevalue)
        //{
        //    var db = new rainbowwineEntities();
        //    int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
        //    var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodevlaue.OrderId)?.FirstOrDefault();
        //    bool addupdate = false;
        //    if (appLogsCashFreeHook != null)
        //    {
        //        string secret = ConfigurationManager.AppSettings["PayKey"];
        //        string data = "";

        //        data = data + decodevlaue.OrderId;
        //        data = data + decodevlaue.OrderAmount;
        //        data = data + decodevlaue.ReferenceId;
        //        data = data + decodevlaue.Status;
        //        data = data + decodevlaue.PaymentMode;
        //        data = data + decodevlaue.Msg;
        //        data = data + decodevlaue.TxtTime.Replace("=", ":"); //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        string signature = SpiritUtility.CreateTokenForCashFree(data, secret);
        //        signature = HttpContext.Current.Server.UrlDecode(signature);
        //        if (decodevlaue.Signature == signature)
        //        {
        //            if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
        //            {
        //                addupdate = false;
        //            }
        //            else if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
        //            {
        //                addupdate = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        addupdate = true;
        //        appLogsCashFreeHook = new AppLogsCashFreeHook
        //        {
        //            CreatedDate = DateTime.Now,
        //            VenderInput = pdecodevalue,
        //            MachineName = System.Environment.MachineName
        //        };
        //        db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
        //        db.SaveChanges();



        //        appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
        //        appLogsCashFreeHook.OrderIdPartial = decodevlaue.OrderId2;
        //        appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
        //        appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
        //        appLogsCashFreeHook.Status = decodevlaue.Status;
        //        appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
        //        appLogsCashFreeHook.Msg = decodevlaue.Msg;
        //        appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
        //        appLogsCashFreeHook.Signature = decodevlaue.Signature;
        //        appLogsCashFreeHook.MachineName = System.Environment.MachineName;
        //        db.SaveChanges();
        //    }
        //    if (addupdate)
        //    {
        //        Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();
        //        if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
        //        {

        //            bool isPOD = false;

        //            decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);

        //            int payType = (int)OrderPaymentType.POD;
        //            if (order.PaymentTypeId != null)
        //            {
        //                if (order.PaymentTypeId == payType)
        //                {
        //                    isPOD = true;
        //                    int statusPODCashPaid = (int)OrderStatusEnum.PODPaymentSuccess;
        //                    order.OrderStatusId = statusPODCashPaid;
        //                    db.SaveChanges();

        //                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
        //                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
        //                    OrderTrack orderTrack = new OrderTrack
        //                    {
        //                        LogUserName = u.Email,
        //                        LogUserId = u.Id,
        //                        OrderId = order.Id,
        //                        StatusId = order.OrderStatusId,
        //                        TrackDate = DateTime.Now
        //                    };
        //                    db.OrderTracks.Add(orderTrack);
        //                    db.SaveChanges();

        //                    appLogsCashFreeHook.SendStatus = "Approved";
        //                    db.SaveChanges();

        //                    //create api to update inventory update
        //                    //InventoryUpdate(order.Id);

        //                    WSendSMS wsms = new WSendSMS();
        //                    string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
        //                    wsms.SendMessage(textmsg, order.Customer.ContactNo);

        //                    return "Order is PODCashPaid";
        //                }
        //            }

        //            if (!isPOD && order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
        //            {
        //                order.OrderStatusId = 3;
        //                db.SaveChanges();

        //                var ema = ConfigurationManager.AppSettings["TrackEmail"];
        //                var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
        //                OrderTrack orderTrack = new OrderTrack
        //                {
        //                    LogUserName = u.Email,
        //                    LogUserId = u.Id,
        //                    OrderId = order.Id,
        //                    StatusId = order.OrderStatusId,
        //                    TrackDate = DateTime.Now
        //                };
        //                db.OrderTracks.Add(orderTrack);
        //                db.SaveChanges();

        //                appLogsCashFreeHook.SendStatus = "Approved";
        //                db.SaveChanges();

        //                InventoryUpdate(order.Id);

        //                WSendSMS wsms = new WSendSMS();
        //                //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
        //                string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
        //                wsms.SendMessage(textmsg, order.Customer.ContactNo);

        //                FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
        //                Task.Run(async () => await fcmHelper.SendFirebaseNotification(order.Id, FirebaseNotificationHelper.NotificationType.Order));

        //                return "Order is Approved";
        //            }
        //            else
        //            {
        //                appLogsCashFreeHook.SendStatus = "ShouldCheck";
        //                db.SaveChanges();


        //                return "ShouldCheck";
        //            }
        //        }
        //        else
        //        {
        //            appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
        //            db.SaveChanges();

        //            int payType = (int)OrderPaymentType.POD;
        //            if (order.PaymentTypeId != null)
        //            {
        //                if (order.PaymentTypeId == payType)
        //                {
        //                    int statusPODPaymentFail = (int)OrderStatusEnum.PODPaymentFail;
        //                    order.OrderStatusId = statusPODPaymentFail;
        //                    db.SaveChanges();

        //                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
        //                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
        //                    OrderTrack orderTrack = new OrderTrack
        //                    {
        //                        LogUserName = u.Email,
        //                        LogUserId = u.Id,
        //                        OrderId = order.Id,
        //                        StatusId = order.OrderStatusId,
        //                        TrackDate = DateTime.Now
        //                    };
        //                    db.OrderTracks.Add(orderTrack);
        //                    db.SaveChanges();
        //                }
        //            }

        //            return $"Cashfree is {decodevlaue.Status}";
        //        }
        //    }
        //    return "NoChanges";
        //}


        public string UpdateOrderToApprove(CashFreeSetApproveResponse decodevlaue, string pdecodevalue)
        {
            var db = new rainbowwineEntities();
            int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId.Replace("OG_", ""));
            var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodevlaue.OrderId)?
                .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

            bool addupdate = false;
            if (appLogsCashFreeHook != null)
            {
                string secret = ConfigurationManager.AppSettings["PayKey"];
                string data = "";

                data = data + decodevlaue.OrderId;
                data = data + decodevlaue.OrderAmount;
                data = data + decodevlaue.ReferenceId;
                data = data + decodevlaue.Status;
                data = data + decodevlaue.PaymentMode;
                data = data + decodevlaue.Msg;
                data = data + decodevlaue.TxtTime.Replace("=", ":"); //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string signature = SpiritUtility.CreateTokenForCashFree(data, secret);
                signature = HttpContext.Current.Server.UrlDecode(signature);
                if (decodevlaue.Signature == signature)
                {
                    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = false;
                    }
                    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = true;
                    }
                }
            }
            else
            {
                addupdate = true;
                appLogsCashFreeHook = new AppLogsCashFreeHook
                {
                    CreatedDate = DateTime.Now,
                    VenderInput = pdecodevalue,
                    MachineName = System.Environment.MachineName
                };
                db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
                db.SaveChanges();

                appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
                appLogsCashFreeHook.OrderIdPartial = decodevlaue.OrderId2;
                appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
                appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
                appLogsCashFreeHook.Status = decodevlaue.Status;
                appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
                appLogsCashFreeHook.Msg = decodevlaue.Msg;
                appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
                appLogsCashFreeHook.Signature = decodevlaue.Signature;
                appLogsCashFreeHook.MachineName = System.Environment.MachineName;
                db.SaveChanges();
            }
            if (addupdate)
            {
                Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();
                if (decodevlaue.OrderId.Contains("OG_"))
                {
                    order = null;
                }

                IList<Order> groupOrders = new List<Order>();
                if (order == null)
                {
                    groupOrders = db.Orders.Where(o => string.Compare(o.OrderGroupId, decodevlaue.OrderId, true) == 0)?.ToList();
                }

                if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                {

                    bool isPOD = false;
                    bool isGorup = false;
                    decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);
                    if (groupOrders.Count > 0)
                    {
                        decimal mAmt = 0;
                        groupOrders.ForEach((o) =>
                        {
                            mAmt += o.OrderAmount;
                        });


                        if (mAmt == amt)// && (order.OrderStatusId != 2 || order.OrderStatusId != 16))
                        {
                            var ret = GroupOrderPlaced(groupOrders, decodevlaue.OrderId);
                            //foreach (var item in groupOrders)
                            //{
                            //    item.OrderStatusId = 3;
                            //    db.SaveChanges();

                            //    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            //    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            //    OrderTrack orderTrack = new OrderTrack
                            //    {
                            //        LogUserName = u.Email,
                            //        LogUserId = u.Id,
                            //        OrderId = item.Id,
                            //        StatusId = item.OrderStatusId,
                            //        TrackDate = DateTime.Now
                            //    };
                            //    db.OrderTracks.Add(orderTrack);
                            //    db.SaveChanges();

                            //    appLogsCashFreeHook.SendStatus = "Approved";
                            //    db.SaveChanges();

                            //    InventoryUpdate(item.Id);
                            //    InventoryMixerUpdate(item.Id);

                            //    WSendSMS wsms = new WSendSMS();
                            //    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                            //    string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], item.Id.ToString());
                            //    wsms.SendMessage(textmsg, item.Customer.ContactNo);

                            //    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                            //    Task.Run(async () => await fcmHelper.SendFirebaseNotification(item.Id, FirebaseNotificationHelper.NotificationType.Order));

                            //    var supOut = ApplicationEmailSend.SupplierOrderInformation(item);

                            //    int statusEmailSupplier = (int)OrderStatusEnum.EmailedToSupplier;
                            //    orderTrack = new OrderTrack
                            //    {
                            //        LogUserName = u.Email,
                            //        LogUserId = u.Id,
                            //        OrderId = item.Id,
                            //        StatusId = statusEmailSupplier,
                            //        TrackDate = DateTime.Now
                            //    };
                            //    db.OrderTracks.Add(orderTrack);
                            //    db.SaveChanges();
                            //}

                            return "Order is Approved";
                        }
                        else
                        {

                            appLogsCashFreeHook.SendStatus = "ShouldCheck";
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

                            appLogsCashFreeHook.SendStatus = "Approved";
                            db.SaveChanges();

                            //create api to update inventory update
                            //InventoryUpdate(order.Id);

                            WSendSMS wsms = new WSendSMS();
                            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                            wsms.SendMessage(textmsg, order.Customer.ContactNo);

                            return "Order is PODCashPaid";
                        }
                    }

                    if (!isPOD && order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
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

                        appLogsCashFreeHook.SendStatus = "Approved";
                        db.SaveChanges();

                        InventoryUpdate(order.Id);
                        InventoryMixerUpdate(order.Id);

                        WSendSMS wsms = new WSendSMS();
                        //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                        string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                        wsms.SendMessage(textmsg, order.Customer.ContactNo);

                        FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                        Task.Run(async () => await fcmHelper.SendFirebaseNotification(order.Id, FirebaseNotificationHelper.NotificationType.Order));

                        return "Order is Approved";
                    }
                    else
                    {
                        appLogsCashFreeHook.SendStatus = "ShouldCheck";
                        db.SaveChanges();


                        return "ShouldCheck";
                    }
                }
                else
                {
                    appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
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

                    return $"Cashfree is {decodevlaue.Status}";
                }
            }
            return "NoChanges";
        }

        public string UpdateWalletOrderToApprove(CashFreeSetApproveResponse decodevlaue, string pdecodevalue)
        {
            var db = new rainbowwineEntities();
            int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId.Replace("OG_", ""));
            var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodevlaue.OrderId)?
                .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

            bool addupdate = false;
            PostdatedOfferDBO postdatedOfferDBO = new PostdatedOfferDBO();
            var orderData = postdatedOfferDBO.GetOrderOrderDetails(orderIdDecode);
            var orddata = db.Orders.Where(x => x.Id == orderIdDecode).FirstOrDefault();
            var userId = db.Customers.Where(x => x.Id == orddata.CustomerId).FirstOrDefault().UserId;
            bool preBook = orderData.Select(x => x.PreBook).FirstOrDefault();

            if (appLogsCashFreeHook != null)
            {
                string secret = ConfigurationManager.AppSettings["PayKey"];
                string data = "";

                data = data + decodevlaue.OrderId;
                data = data + decodevlaue.OrderAmount;
                data = data + decodevlaue.ReferenceId;
                data = data + decodevlaue.Status;
                data = data + decodevlaue.PaymentMode;
                data = data + decodevlaue.Msg;
                data = data + decodevlaue.TxtTime.Replace("=", ":"); //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string signature = SpiritUtility.CreateTokenForCashFree(data, secret);
                signature = HttpContext.Current.Server.UrlDecode(signature);
                if (decodevlaue.Signature == signature)
                {
                    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = false;
                    }
                    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = true;
                    }
                }
            }
            else
            {
                addupdate = true;
                appLogsCashFreeHook = new AppLogsCashFreeHook
                {
                    CreatedDate = DateTime.Now,
                    VenderInput = pdecodevalue,
                    MachineName = System.Environment.MachineName
                };
                db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
                db.SaveChanges();

                appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
                appLogsCashFreeHook.OrderIdPartial = decodevlaue.OrderId2;
                appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
                appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
                appLogsCashFreeHook.Status = decodevlaue.Status;
                appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
                appLogsCashFreeHook.Msg = decodevlaue.Msg;
                appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
                appLogsCashFreeHook.Signature = decodevlaue.Signature;
                appLogsCashFreeHook.MachineName = System.Environment.MachineName;
                db.SaveChanges();
            }
            if (addupdate)
            {
                Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();
                if (decodevlaue.OrderId.Contains("OG_"))
                {
                    order = null;
                }
                Decimal odAmount = 0;
                IList<Order> groupOrders = new List<Order>();
                if (order == null)
                {
                    groupOrders = db.Orders.Where(o => string.Compare(o.OrderGroupId, decodevlaue.OrderId, true) == 0)?.ToList();
                    odAmount = groupOrders.Sum(x => x.OrderAmount);
                }
                else
                {
                    odAmount = order.OrderAmount;
                }

                if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                {

                    bool isPOD = false;
                    bool isGorup = false;
                    decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);
                    decimal disCountedAmt = 0;
                    DiscountDBO discountDBO = new DiscountDBO();
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

                            var promodata = postdatedOfferDBO.PromoCodeApplyValidate(promocode1.Code, (float)odAmount, order.Customer.UserId, orddata.Id);
                            if (promodata.IsValid)
                            {
                                mAmt = mAmt - Convert.ToDecimal(promodata.DiscountAmount);
                                disCountedAmt = Convert.ToDecimal(promodata.DiscountAmount);
                            }
                        }
                        if (mAmt == amt)// && (order.OrderStatusId != 2 || order.OrderStatusId != 16))
                        {
                            var ret = GroupOrderPlaced(groupOrders, decodevlaue.OrderId);

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
                            PopupBannerDBO popupBannerDBO = new PopupBannerDBO();
                            if (popupBannerDBO.IsGoodies(order.Id))
                            {

                                Task.Run(async () => await TriggerSilentBannerNotification(userId, order.CustomerId, order.ShopID.Value, "VAPOrder", 0));
                            }
                            if (preBook)
                            {

                                var productList = orderData.Select(x => x.ProductID).ToList();

                                string productIds = string.Join(",", productList);

                                var preBookingOffer = postdatedOfferDBO.GetPreBookingPromoOfferProductLink(productIds);
                                foreach (var item in preBookingOffer)
                                {

                                    var postOffer = new PostdatedOfferDO
                                    {
                                        PromoId = item.PromoId,
                                        UserId = userId,
                                        OrderId = order.Id,
                                        OfferStartDate = DateTime.Now.AddDays(item.DaysFromCurrent),
                                        OfferEndDate = DateTime.Now.AddDays(item.DaysFromCurrent + item.DaysFromStart),
                                        Title = ConfigurationManager.AppSettings["PrebookConfettiTitle"],
                                        SubTitle = ConfigurationManager.AppSettings["PrebookConfettiTitle"]
                                    };
                                    postdatedOfferDBO.AddPostdatedOffer(postOffer);

                                }
                            }


                            var uSOffer = discountDBO.GetUserSpecificOffer(order.CustomerId, promocode1.PromoId, "CouponCode");
                            if (uSOffer != null && disCountedAmt > 0)
                            {
                                var res = discountDBO.UpdateUserSpecificOfferMapping(uSOffer.CustomerID, uSOffer.OfferId, uSOffer.OfferType, order.Id, disCountedAmt);

                            }
                            return "Order is Approved";
                        }

                        else
                        {

                            appLogsCashFreeHook.SendStatus = "ShouldCheck";
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

                            appLogsCashFreeHook.SendStatus = "Approved";
                            db.SaveChanges();

                            //create api to update inventory update
                            //InventoryUpdate(order.Id);

                            if (preBook)
                            {

                                var productList = orderData.Select(x => x.ProductID).ToList();

                                string productIds = string.Join(",", productList);

                                var preBookingOffer = postdatedOfferDBO.GetPreBookingPromoOfferProductLink(productIds);
                                foreach (var item in preBookingOffer)
                                {

                                    var postOffer = new PostdatedOfferDO
                                    {
                                        PromoId = item.PromoId,
                                        UserId = u.Id,
                                        OrderId = order.Id,
                                        OfferStartDate = DateTime.Now.AddDays(item.DaysFromCurrent),
                                        OfferEndDate = DateTime.Now.AddDays(item.DaysFromCurrent + item.DaysFromStart),
                                        Title = ConfigurationManager.AppSettings["PrebookConfettiTitle"],
                                        SubTitle = ConfigurationManager.AppSettings["PrebookConfettiTitle"]
                                    };
                                    postdatedOfferDBO.AddPostdatedOffer(postOffer);

                                }
                            }
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

                        var promodata = postdatedOfferDBO.PromoCodeApplyValidate(promocode.Code, (float)odAmount, order.Customer.UserId, orddata.Id); ;
                        if (promodata.IsValid)
                        {
                            ordAmt = ordAmt - Convert.ToDecimal(promodata.DiscountAmount);
                            disCountedAmt = Convert.ToDecimal(promodata.DiscountAmount);
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
                            LogUserId = userId,
                            OrderId = order.Id,
                            StatusId = order.OrderStatusId,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();

                        appLogsCashFreeHook.SendStatus = "Approved";
                        db.SaveChanges();
                        OrderDBO orderDBO1 = new OrderDBO();
                        InventoryUpdate(order.Id);
                        InventoryMixerUpdate(order.Id);
                        orderDBO1.GoodiesInventoryQtyDeduction(order.Id);

                        if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                        {
                            var ser2 = SZIoc.GetSerivce<IPromoCodeService>();
                            var usetranAmount = ser2.UsesTransactionAmount(order.Customer.UserId, ordAmt, Convert.ToInt32(decodevlaue.OrderId));
                        }
                        if (preBook)
                        {

                            var productList = orderData.Select(x => x.ProductID).ToList();

                            string productIds = string.Join(",", productList);

                            var preBookingOffer = postdatedOfferDBO.GetPreBookingPromoOfferProductLink(productIds);
                            foreach (var item in preBookingOffer)
                            {

                                var postOffer = new PostdatedOfferDO
                                {
                                    PromoId = item.PromoId,
                                    UserId = userId,
                                    OrderId = order.Id,
                                    OfferStartDate = DateTime.Now.AddDays(item.DaysFromCurrent),
                                    OfferEndDate = DateTime.Now.AddDays(item.DaysFromCurrent + item.DaysFromStart),
                                    Title = ConfigurationManager.AppSettings["PrebookConfettiTitle"],
                                    SubTitle = ConfigurationManager.AppSettings["PrebookConfettiTitle"]
                                };
                                postdatedOfferDBO.AddPostdatedOffer(postOffer);

                            }
                        }
                        if (promocode != null)
                        {
                            var uSOffer = discountDBO.GetUserSpecificOffer(order.CustomerId, promocode.PromoId, "CouponCode");
                            if (uSOffer != null && disCountedAmt > 0)
                            {
                                var res = discountDBO.UpdateUserSpecificOfferMapping(uSOffer.CustomerID, uSOffer.OfferId, uSOffer.OfferType, order.Id, disCountedAmt);

                            }
                        }
                       
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
                        PopupBannerDBO popupBannerDBO = new PopupBannerDBO();
                        if (popupBannerDBO.IsGoodies(order.Id))
                        {

                            Task.Run(async () => await TriggerSilentBannerNotification(userId, order.CustomerId, order.ShopID.Value, "VAPOrder", 0));
                        }
                        var c = SZIoc.GetSerivce<ISZConfiguration>();
                        var assignAPICall = c.GetConfigValue(ConfigEnums.AssignAPICall.ToString());
                        if (assignAPICall =="1")
                        {
                            Task.Run(async () => await OrdersAssign(order.ShopID.Value,order.Id));
                        }
                        return "Order is Approved";
                    }
                    else
                    {
                        appLogsCashFreeHook.SendStatus = "ShouldCheck";
                        db.SaveChanges();


                        return "ShouldCheck";
                    }
                }
                else
                {
                    appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
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

                    return $"Cashfree is {decodevlaue.Status}";
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

                        InventoryUpdate(ord.Id);
                        InventoryMixerUpdate(ord.Id);

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

        public string UpdateOrderIssueToApprove(CashFreeSetApproveResponse decodevlaue, string pdecodevalue)
        {
            var db = new rainbowwineEntities();
            int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
            var ord2 = decodevlaue.OrderId2.Split('_');
            var issueId = Convert.ToInt32(ord2[3]);
            AppLogsCashFreeHook appLogsCashFreeHook = db.AppLogsCashFreeHooks
                .Where(o => o.OrderId == decodevlaue.OrderId && o.OrderIdPartial.Contains(issueId.ToString()))?
                .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

            bool addupdate = false;
            if (appLogsCashFreeHook != null)
            {
                string secret = ConfigurationManager.AppSettings["PayKey"];
                string data = "";

                data = data + decodevlaue.OrderId;
                data = data + decodevlaue.OrderAmount;
                data = data + decodevlaue.ReferenceId;
                data = data + decodevlaue.Status;
                data = data + decodevlaue.PaymentMode;
                data = data + decodevlaue.Msg;
                data = data + decodevlaue.TxtTime.Replace("=", ":"); //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string signature = SpiritUtility.CreateTokenForCashFree(data, secret);
                signature = HttpContext.Current.Server.UrlDecode(signature);
                if (decodevlaue.Signature == signature)
                {
                    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = false;
                    }
                    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = true;
                    }
                }
            }
            else
            {
                addupdate = true;
                appLogsCashFreeHook = new AppLogsCashFreeHook
                {
                    CreatedDate = DateTime.Now,
                    VenderInput = pdecodevalue,
                    MachineName = System.Environment.MachineName
                };
                db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
                db.SaveChanges();
                appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
                appLogsCashFreeHook.OrderIdPartial = decodevlaue.OrderId2;
                appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
                appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
                appLogsCashFreeHook.Status = decodevlaue.Status;
                appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
                appLogsCashFreeHook.Msg = decodevlaue.Msg;
                appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
                appLogsCashFreeHook.Signature = decodevlaue.Signature;
                appLogsCashFreeHook.MachineName = System.Environment.MachineName;
                db.SaveChanges();
            }
            if (addupdate)
            {
                if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                {

                    Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();
                    var orderIssue = db.OrderIssues.Where(o => o.OrderIssueId == issueId)?.FirstOrDefault();

                    decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);
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

                        appLogsCashFreeHook.SendStatus = "Approved";
                        db.SaveChanges();

                        //deduct inventory when order is not POD
                        //int podId = (int)OrderPaymentType.POD;
                        //if (order.PaymentTypeId != podId)
                        //{
                        //    InventoryUpdate(order.Id);
                        //}

                        WSendSMS wsms = new WSendSMS();
                        //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                        string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                        wsms.SendMessage(textmsg, order.Customer.ContactNo);

                        return "Order is Approved";
                    }
                    else
                    {
                        appLogsCashFreeHook.SendStatus = "ShouldCheck";
                        db.SaveChanges();


                        return "ShouldCheck";
                    }
                }
                else
                {
                    appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
                    db.SaveChanges();
                    return $"Cashfree is {decodevlaue.Status}";
                }
            }
            return "NoChanges";
        }

        public string UpdateOrderModifyToApprove(CashFreeSetApproveResponse decodevlaue, string pdecodevalue)
        {
            var db = new rainbowwineEntities();
            int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
            var ord2 = decodevlaue.OrderId2.Split('_');
            var modifyId = Convert.ToInt32(ord2[3]);
            AppLogsCashFreeHook appLogsCashFreeHook = db.AppLogsCashFreeHooks
                .Where(o => o.OrderId == decodevlaue.OrderId && o.OrderIdPartial.Contains(modifyId.ToString()))?
                .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

            bool addupdate = false;
            if (appLogsCashFreeHook != null)
            {
                string secret = ConfigurationManager.AppSettings["PayKey"];
                string data = "";

                data = data + decodevlaue.OrderId;
                data = data + decodevlaue.OrderAmount;
                data = data + decodevlaue.ReferenceId;
                data = data + decodevlaue.Status;
                data = data + decodevlaue.PaymentMode;
                data = data + decodevlaue.Msg;
                data = data + decodevlaue.TxtTime.Replace("=", ":"); //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string signature = SpiritUtility.CreateTokenForCashFree(data, secret);
                signature = HttpContext.Current.Server.UrlDecode(signature);
                if (decodevlaue.Signature == signature)
                {
                    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = false;
                    }
                    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                    {
                        addupdate = true;
                    }
                }
            }
            else
            {
                addupdate = true;
                appLogsCashFreeHook = new AppLogsCashFreeHook
                {
                    CreatedDate = DateTime.Now,
                    VenderInput = pdecodevalue,
                    MachineName = System.Environment.MachineName
                };
                db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
                db.SaveChanges();

                appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
                appLogsCashFreeHook.OrderIdPartial = decodevlaue.OrderId2;
                appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
                appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
                appLogsCashFreeHook.Status = decodevlaue.Status;
                appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
                appLogsCashFreeHook.Msg = decodevlaue.Msg;
                appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
                appLogsCashFreeHook.Signature = decodevlaue.Signature;
                appLogsCashFreeHook.MachineName = System.Environment.MachineName;
                db.SaveChanges();
            }
            if (addupdate)
            {
                if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                {

                    Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();

                    var orderIssue = db.OrderModifies.Where(o => o.Id == modifyId)?.FirstOrDefault();

                    decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);
                    decimal adjustAmt = Convert.ToDecimal(orderIssue.AdjustAmt ?? 0);

                    if (adjustAmt == amt)
                    {
                        int statusApprove = (int)OrderStatusEnum.Approved;
                        int statusSubmitted = (int)OrderStatusEnum.Submitted;

                        int payType = (int)OrderPaymentType.POD;
                        order.OrderStatusId = (order.PaymentTypeId == payType) ? statusSubmitted : statusApprove;
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

                        appLogsCashFreeHook.SendStatus = "Approved";
                        db.SaveChanges();

                        //deduct inventory when order is not POD
                        //int podId = (int)OrderPaymentType.POD;
                        //if (order.PaymentTypeId != podId)
                        //{
                        //    InventoryUpdate(order.Id);
                        //}

                        WSendSMS wsms = new WSendSMS();
                        //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                        string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                        wsms.SendMessage(textmsg, order.Customer.ContactNo);

                        return "Order is Approved";
                    }
                    else
                    {
                        appLogsCashFreeHook.SendStatus = "ShouldCheck";
                        db.SaveChanges();


                        return "ShouldCheck";
                    }
                }
                else
                {
                    appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
                    db.SaveChanges();
                    return $"Cashfree is {decodevlaue.Status}";
                }
            }
            return "NoChanges";
        }

        public void RevertInventory(int orderID)
        {

            using (var db = new rainbowwineEntities())
            {
                var custId = db.Orders.Where(o => o.Id == orderID).FirstOrDefault();

                //var oDetail = db.OrderDetails.Where(o => o.OrderId == orderID)?.ToList();
                var order = db.Orders.Find(orderID);
                foreach (var item in order.OrderDetails)
                {
                    var invent = db.Inventories.Where(o => (o.ProductID == item.ProductID) && (o.ShopID == order.ShopID))?.FirstOrDefault();
                    var beforeqty = invent.QtyAvailable;
                    if (invent != null)
                    {
                        invent.QtyAvailable += item.ItemQty;
                        var afterqty = invent.QtyAvailable;
                        db.InventoryTracks.Add(new InventoryTrack { ProductID = item.ProductID, ShopID = order.ShopID.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = orderID });
                    }
                    invent.LastModified = System.DateTime.Now;
                    invent.LastModifiedBy = custId.OrderPlacedBy;
                }
                db.SaveChanges();
            }
        }
        public void RevertMixerInventory(int orderID)
        {
            using (var db = new rainbowwineEntities())
            {
                //var oDetail = db.OrderDetails.Where(o => o.OrderId == orderID)?.ToList();
                var order = db.Orders.Find(orderID);
                foreach (var item in order.MixerOrderItems)
                {
                    var invent = db.InventoryMixers.Where(o => (o.MixerDetailId == item.MixerDetailId) && (o.ShopId == order.ShopID))?.FirstOrDefault();
                    if (invent != null)
                    {
                        invent.Qty += item.ItemQty;
                    }
                    db.SaveChanges();
                }
                //db.SaveChanges();
            }
        }
        public void InventoryUpdate(int orderId)
        {
            using (var db = new rainbowwineEntities())
            {


                try
                {
                    var custId = db.Orders.Where(o => o.Id == orderId).FirstOrDefault();
                    var ord = db.OrderDetails.Where(o => o.OrderId == orderId)?.ToList();
                    foreach (var item in ord)
                    {
                        var invent = db.Inventories.Where(o => (o.ProductID == item.ProductID) && (o.ShopID == item.ShopID))?.FirstOrDefault();
                        var beforeqty = invent.QtyAvailable;
                        if (invent != null)
                        {
                            if (invent.QtyAvailable > 0)
                            {
                                int remainQty = invent.QtyAvailable - item.ItemQty;
                                remainQty = (remainQty > 0) ? remainQty : 0;
                                invent.QtyAvailable = remainQty;
                                var afterqty = invent.QtyAvailable;
                                db.InventoryTracks.Add(new InventoryTrack { ProductID = item.ProductID, ShopID = custId.ShopID.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = orderId });
                                invent.LastModified = System.DateTime.Now;
                                invent.LastModifiedBy = custId.OrderPlacedBy;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.AppLogs.Add(new AppLog
                    {
                        CreateDatetime = DateTime.Now,
                        Error = $"Api_InventoryUpdate: {ex.Message}",
                        Message = ex.StackTrace,
                        MachineName = System.Environment.MachineName
                    });
                    db.SaveChanges();
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
        public void InventoryMixerUpdate(int orderId)
        {
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var mixerList = db.MixerOrderItems.Where(o => o.OrderId == orderId)?.ToList();
                    foreach (var item in mixerList)
                    {
                        var invent = db.InventoryMixers.Where(o => (o.MixerDetailId == item.MixerDetailId) && (o.ShopId == item.ShopId))?.FirstOrDefault();
                        invent = invent ?? db.InventoryMixers.Where(o => (o.MixerDetailId == item.MixerDetailId) && (o.SupplierId == item.SupplierId))?.FirstOrDefault();
                        if (invent != null)
                        {
                            if (invent.Qty > 0)
                            {
                                int remainQty = (invent.Qty ?? 0) - (item.ItemQty ?? 0);
                                remainQty = (remainQty > 0) ? remainQty : 0;
                                invent.Qty = remainQty;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.AppLogs.Add(new AppLog
                    {
                        CreateDatetime = DateTime.Now,
                        Error = $"Api_InventoryMixerUpdate: {ex.Message}",
                        Message = ex.StackTrace,
                        MachineName = System.Environment.MachineName
                    });
                    db.SaveChanges();
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
        public decimal GetTotalAmtOfOrder(int orderId)
        {
            decimal totalAmt = 0;
            using (var db = new rainbowwineEntities())
            {
                var model = db.Orders.Find(orderId);
                var oDetails = db.OrderDetails.Where(o => o.OrderId == orderId).Select(o => new
                {
                    o.ItemQty,
                    o.Price
                }).ToList();


                foreach (var item in oDetails)
                {
                    var q = item.ItemQty * item.Price;
                    totalAmt += q;
                }
                int configPremitValue = Convert.ToInt32(ConfigurationManager.AppSettings["PremitValue"]);
                int premitVlaue = (string.IsNullOrWhiteSpace(model.LicPermitNo) && model.OrderType == "m") ?
                    configPremitValue : 0;
                totalAmt += premitVlaue;
            }
            return totalAmt;
        }

        public decimal GetTotalAmtOfIssueOrder(int issueorderId)
        {
            decimal totalAmt = 0;
            using (var db = new rainbowwineEntities())
            {
                var oDetails = db.OrderIssueDetails.Where(o => o.OrderIssueId == issueorderId).Select(o => new
                {
                    o.ItemQty,
                    o.Price
                }).ToList();

                foreach (var item in oDetails)
                {
                    var q = item.ItemQty * item.Price;
                    totalAmt += q ?? 0;
                }
            }
            return totalAmt;
        }
        public decimal GetTotalAmtOfOrderModify(int orderModifyId)
        {
            decimal totalAmt = 0;
            using (var db = new rainbowwineEntities())
            {
                var oDetails = db.OrderDetailModifies.Where(o => o.OrderModifyId == orderModifyId).Select(o => new
                {
                    o.ItemQty,
                    o.Price
                }).ToList();

                foreach (var item in oDetails)
                {
                    var q = item.ItemQty * item.Price;
                    totalAmt += q;
                }
            }
            return totalAmt;
        }
        public ResponseStatus CashFreePayment(int issueId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            string payUrl = ConfigurationManager.AppSettings["PayCreate"];
            CashFreeOrderCreate cashFreeOrderCreate = null;

            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var issue = db.OrderIssues.Find(issueId);
                    var order = db.Orders.Find(issue.OrderId);
                    var maxFreePay = db.CashFreePays.Where(o => o.OrderId == order.Id).Count();
                    cashFreeOrderCreate = new CashFreeOrderCreate
                    {
                        appId = ConfigurationManager.AppSettings["PayFreeId"],
                        secretKey = ConfigurationManager.AppSettings["PayKey"],
                        orderId = $"{order.Id}_PartialPay_{maxFreePay}_{issueId}",
                        orderAmount = issue.AdjustAmt.ToString(),
                        orderCurrency = "INR",
                        customerEmail = "subhamautomation@rainmail.com",//order.Customer.CustomerName,
                        customerPhone = order.Customer.ContactNo,
                        notifyUrl = ConfigurationManager.AppSettings["PayNotifyUrl"],
                        returnUrl = ConfigurationManager.AppSettings["PayReturnUrl"],
                        customerName = order.Customer.CustomerName,
                        //orderNote="Test"
                    };
                    if (cashFreeOrderCreate != null)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            Uri url = new Uri($"{payUrl}");
                            var json = JsonConvert.SerializeObject(cashFreeOrderCreate);
                            var body = new Dictionary<string, string>
                        {
                            { "appId", cashFreeOrderCreate.appId },
                            { "secretKey", cashFreeOrderCreate.secretKey },
                            { "orderId", cashFreeOrderCreate.orderId },
                            { "orderAmount", cashFreeOrderCreate.orderAmount },
                            { "customerEmail", cashFreeOrderCreate.customerEmail },
                            { "customerPhone", cashFreeOrderCreate.customerPhone },
                            { "notifyUrl", cashFreeOrderCreate.notifyUrl },
                            { "returnUrl", cashFreeOrderCreate.returnUrl }
                        };

                            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                            var response = client.SendAsync(req).Result;
                            var resp = response.Content.ReadAsStringAsync().Result;
                            var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                            db.CashFreePays.Add(new CashFreePay
                            {
                                InputValue = json,
                                IssueId = issueId,
                                OrderId = issue.OrderId,
                                VenderOutput = resp,
                                CreatedDate = DateTime.Now,
                                Amt = issue.AdjustAmt.ToString()
                            });

                            db.SaveChanges();

                            if (string.Compare(ret.status, "ok", true) == 0)
                            {

                                order.OrderStatusId = 35;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = issue.OrderId ?? 0,
                                    Remark = ret.status,
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
                                order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, GetTotalAmtOfOrder(order.Id));
                                db.SaveChanges();

                                responseStatus.Message = $"Payment Link Send to Customer.";
                                responseStatus.Status = true;
                                SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);
                                //Flow SMS
                                var link1 = ret.paymentLink.Substring(0, 28);
                                var link2 = ret.paymentLink.Substring(28, 28);
                                var link3 = ret.paymentLink.Substring(56);
                                var dicti = new Dictionary<string, string>();
                                dicti.Add("ORDER", order.Id.ToString());
                                dicti.Add("AMOUNT", cashFreeOrderCreate.orderAmount.ToString());
                                dicti.Add("LINK1", link1);
                                dicti.Add("LINK2", link2);
                                dicti.Add("LINK3", link3);
                                var templeteid = ConfigurationManager.AppSettings["SMSSendPaymentLinkCashfreeFlowId"];
                                Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.OrderTo, dicti));
                                //End Flow SMS

                                //WSendSMS wsms = new WSendSMS();
                                //string textmsg = string.Format(ConfigurationManager.AppSettings["CFPartialPay"], cashFreeOrderCreate.orderAmount, order.Id.ToString(), ret.paymentLink);
                                //wsms.SendMessage(textmsg, order.Customer.ContactNo);

                            }
                            else if (string.Compare(ret.status, "error", true) == 0)
                            {
                                responseStatus.Message = $"Error while Refund the Amount. {ret.reason}";
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

        public ResponseStatus CashFreeReFund(int issueId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };

            string payUrl = ConfigurationManager.AppSettings["PayRefund"];
            CashFreeRefundObject cashFreeRefundObject = null;
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    OrderDBO orderDBO = new OrderDBO();
                    var issue = db.OrderIssues.Find(issueId);
                    var order = db.Orders.Find(issue.OrderId);
                    var onlinePayment = db.AppLogsCashFreeHooks.Where(o => string.Compare(o.OrderId.Trim(), order.Id.ToString(), true) == 0).ToList();
                    //.OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();
                    foreach (var applogs in onlinePayment)
                    {
                        if (applogs == null)
                        {
                            responseStatus.Message = $"No Payment made against the order {issue.OrderId}";
                            return responseStatus;
                        }
                        int isRefundInitiated = orderDBO.CheckRefundInitiated(issue.OrderId.Value, issue.OrderIssueId);
                        if (isRefundInitiated == 1)
                        {
                            responseStatus.Message = $"This refund has already been initiated. If a different refund is to be done, please raise an issue against the order and try again.";
                            return responseStatus;
                        }
                        double refundAmt = Math.Abs(issue.AdjustAmt ?? 0);
                        double oAmt = Convert.ToDouble(applogs.OrderAmount);
                        var walletAmt = order.WalletAmountUsed;
                        double walletPartialRefund = 0;

                        var refunddet = db.CashFreeRefunds.Where(x => x.OrderId == order.Id).ToList();

                        if (refunddet != null)
                        {
                            var refundedAmount = refunddet.Where(y => y.RefundParam != null && y.RefundParam.referenceId == applogs.ReferenceId);
                            if (refundedAmount != null && refundedAmount.Sum(x => Convert.ToDouble(x.Amt)) > 0)
                            {
                                oAmt = oAmt - refundedAmount.Sum(x => Convert.ToDouble(x.Amt));
                            }

                        }
                        //double refundInitiatedAmt = orderDBO.GetRefundInitiatedAmount(order.Id);


                        OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();

                        decimal walletReturnedAmt = onlinePaymentServiceDBO.GetWalletReturnedAmount(order.Id);
                        if (walletReturnedAmt > 0 && (oAmt - Convert.ToDouble(walletReturnedAmt)) > 0)
                        {
                            oAmt = oAmt - Convert.ToDouble(walletReturnedAmt);
                        }


                        if (oAmt == 0)
                        {
                            responseStatus.Message = $"Already Refunded For This Order {issue.OrderId}";
                            return responseStatus;
                        }
                        if (issue.OrderIssueTypeId == 3)
                        {
                            if (refundAmt > oAmt)
                            {
                                walletPartialRefund = refundAmt - oAmt;
                                refundAmt = oAmt;
                            }

                        }
                        if (issue.OrderIssueTypeId == 4)
                        {
                            refundAmt = oAmt;
                            //OrderIssueDBO cashDBO = new OrderIssueDBO();
                            //cashDBO.AddCashToWallet(orderIssue.OrderId.Value, (float)Amt);
                        }
                        if (refundAmt > oAmt)
                        {
                            responseStatus.Message = "Last Payment made is less than Refund amount.";
                            return responseStatus;
                        }
                        var orderIssueDBO = new OrderIssueDBO();
                        cashFreeRefundObject = new CashFreeRefundObject
                        {
                            appId = ConfigurationManager.AppSettings["PayFreeId"],
                            secretKey = ConfigurationManager.AppSettings["PayKey"],
                            referenceId = $"{applogs.ReferenceId}",
                            refundAmount = refundAmt.ToString(),
                            refundNote = issue.OrderIssueType.IssueTypeName
                        };
                        if (cashFreeRefundObject != null)
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                Uri url = new Uri($"{payUrl}");
                                var json = JsonConvert.SerializeObject(cashFreeRefundObject);
                                var body = new Dictionary<string, string>
                            {
                                { "appId", cashFreeRefundObject.appId },
                                { "secretKey", cashFreeRefundObject.secretKey },
                                { "referenceId", cashFreeRefundObject.referenceId },
                                { "refundAmount", cashFreeRefundObject.refundAmount },
                                { "refundNote", cashFreeRefundObject.refundNote },
                            };


                                var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                                var response = client.SendAsync(req).Result;
                                var resp = response.Content.ReadAsStringAsync().Result;
                                var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                                db.CashFreeRefunds.Add(new CashFreeRefund
                                {
                                    InputParam = json,
                                    IssueId = issueId,
                                    OrderId = issue.OrderId,
                                    VenderOutput = resp,
                                    Amt = refundAmt.ToString(),
                                    CreatedDate = DateTime.Now
                                });
                                db.SaveChanges();

                                int issueClosed = (int)IssueType.Closed;
                                if (string.Compare(ret.status, "ok", true) == 0)
                                {
                                    var ord = db.Orders.Find(issue.OrderId ?? 0);
                                    ord.OrderStatusId = 37;
                                    db.SaveChanges();

                                    WebEngageController webEngageController = new WebEngageController();
                                    Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Order Refund Completed"));


                                    var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                    string uId = u.Id;
                                    OrderTrack orderTrack = new OrderTrack
                                    {
                                        LogUserName = u.Email,
                                        LogUserId = uId,
                                        OrderId = issue.OrderId ?? 0,
                                        Remark = ret.status,
                                        StatusId = 37,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderTracks.Add(orderTrack);
                                    db.SaveChanges();

                                    //If Partial Refund update the order details
                                    if (issue.OrderIssueTypeId == 3)
                                    {
                                        //update the issue orderdetails to orderdetail table

                                        orderDBO.UpdateIssueOrder(issueId, order.Id);
                                        //update the total order value into order table
                                        //order.OrderAmount = GetTotalAmtOfOrder(order.Id);
                                        order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, GetTotalAmtOfOrder(order.Id));

                                        db.SaveChanges();

                                        ord.OrderStatusId = 3;
                                        db.SaveChanges();
                                        issue.OrderStatusId = issueClosed;
                                        db.SaveChanges();

                                        //Flow SMS
                                        var dicti = new Dictionary<string, string>();
                                        dicti.Add("ORDERID", issue.OrderId.ToString());
                                        dicti.Add("AMOUNT", refundAmt.ToString());
                                        dicti.Add("DATE", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                                        var templeteid = ConfigurationManager.AppSettings["SMSRefundFlowId"];
                                        Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, ord.OrderTo, dicti));
                                        //End Flow SMS

                                        if (walletPartialRefund > 0)
                                        {
                                            var result = orderIssueDBO.FullRefundToWallet(issue.OrderId.Value, (float)walletPartialRefund, issue.OrderIssueId);
                                            if (result == 1)
                                            {
                                                CustomerOrderController customerOrderController = new CustomerOrderController();
                                                customerOrderController.WalletRefundNotification(Convert.ToInt32(walletPartialRefund), order.CustomerId, order.Customer.UserId);
                                            }
                                        }
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

                                    if ((issue.OrderIssueTypeId == 4) && (string.Compare(ret.status, "ok", true) == 0))
                                    {
                                        //Flow SMS
                                        var dicti = new Dictionary<string, string>();
                                        dicti.Add("ORDERID", issue.OrderId.ToString());
                                        dicti.Add("AMOUNT", refundAmt.ToString());
                                        dicti.Add("DATE", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                                        var templeteid = ConfigurationManager.AppSettings["SMSRefundFlowId"];
                                        Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, ord.OrderTo, dicti));
                                        //End Flow SMS

                                        if (walletAmt != null)
                                        {
                                            var result1 = orderIssueDBO.FullRefundToWallet(issue.OrderId.Value, (float)walletAmt, issue.OrderIssueId);
                                            if (result1 == 1)
                                            {
                                                CustomerOrderController customerOrderController = new CustomerOrderController();
                                                customerOrderController.WalletRefundNotification(Convert.ToInt32(walletAmt), order.CustomerId, order.Customer.UserId);
                                            }
                                        }


                                        if (order.OrderStatusId == 5 || order.OrderStatusId == 37 || order.OrderStatusId == 68 || order.OrderStatusId == 48)
                                        {
                                            //HyperTracking Complted
                                            HyperTracking hyperTracking = new HyperTracking();
                                            Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(issue.OrderId.Value));

                                            //Live Tracking FireStore

                                            CustomerApi2Controller.DeleteToFireStore(issue.OrderId.Value);
                                            orderDBO.UpdatedScheduledOrder(issue.OrderId.Value);


                                        }

                                        var c = SZIoc.GetSerivce<ISZConfiguration>();
                                        var isReturnCouponAmount = c.GetConfigValue(ConfigEnums.IsReturnCouponAmount.ToString());
                                        if (isReturnCouponAmount == "1")
                                        {
                                            DiscountDBO discountDBO = new DiscountDBO();
                                            var numberOfUse = discountDBO.RevertNumberOfUseSpecificOffer(ord.CustomerId, ord.Id);
                                        }
                                    }


                                    OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                                    {
                                        UserId = uId,
                                        OrderId = issue.OrderId ?? 0,
                                        OrderIssueId = issue.OrderIssueId,
                                        Remark = ret.message,
                                        OrderStatusId = issueClosed,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderIssueTracks.Add(orderIssueTrack);
                                    db.SaveChanges();
                                    issue.OrderStatusId = issueClosed;
                                    db.SaveChanges();
                                    SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);



                                    //WSendSMS wSendSMS = new WSendSMS();
                                    //string text = string.Format(ConfigurationManager.AppSettings["CFPartialRefund"], Math.Abs(Convert.ToDouble(issue.AdjustAmt)), issue.OrderId);
                                    //wSendSMS.SendMessage(text, ord.Customer.ContactNo);

                                }
                                else if (string.Compare(ret.status, "error", true) == 0)
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
                                        Remark = ret.status,
                                        StatusId = 38,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderTracks.Add(orderTrack);
                                    db.SaveChanges();

                                    issue.OrderStatusId = issueClosed;
                                    db.SaveChanges();

                                    OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                                    {
                                        UserId = uId,
                                        OrderId = issue.OrderId ?? 0,
                                        OrderIssueId = issue.OrderIssueId,
                                        Remark = ret.message,
                                        OrderStatusId = issueClosed,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderIssueTracks.Add(orderIssueTrack);
                                    db.SaveChanges();
                                    SpiritUtility.GenerateZohoTikect(issue.OrderId ?? 0, issue.OrderIssueId);
                                    var failed = orderDBO.UpdateRefundInitiatedFailed(issue.OrderId.Value, issue.OrderIssueId);
                                    responseStatus.Message = $"Error while Refund the Amount. {ret.message}";
                                }
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

        public ResponseStatus CashFreeReFundForBackToStore(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };

            string payUrl = ConfigurationManager.AppSettings["PayRefund"];
            CashFreeRefundObject cashFreeRefundObject = null;
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    OrderDBO orderDBO = new OrderDBO();

                    var order = db.Orders.Find(orderId);
                    var onlinePayment = db.AppLogsCashFreeHooks.Where(o => string.Compare(o.OrderId.Trim(), order.Id.ToString(), true) == 0).ToList();
                    //.OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

                    if (onlinePayment.Count() == 0 || onlinePayment ==null)
                    {
                        var walletAmt = order.WalletAmountUsed;
                        OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();

                        //decimal walletReturnedAmt = onlinePaymentServiceDBO.GetWalletReturnedAmount(order.Id);
                        //if (walletReturnedAmt > 0 && (Convert.ToDouble(walletAmt) - Convert.ToDouble(walletReturnedAmt)) > 0)
                        //{
                        //    walletAmt = walletAmt - walletReturnedAmt;
                        //}
                        
                        if (walletAmt != null && walletAmt > 0)
                        {
                            OrderIssueDBO orderIssueDBO = new OrderIssueDBO();
                            var result1 = orderIssueDBO.FullRefundToWallet(orderId, (float)walletAmt, 0);
                            if (result1 == 1)
                            {
                                order.OrderStatusId = 37;
                                db.SaveChanges();
                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = orderId,
                                    Remark = "Back To Store Full Refund Successful",
                                    StatusId = 72,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                responseStatus.Message = $"Refund successful.";
                                responseStatus.Status = true;

                                CustomerOrderController customerOrderController = new CustomerOrderController();
                                customerOrderController.WalletRefundNotification(Convert.ToInt32(walletAmt), order.CustomerId, order.Customer.UserId);
                            }
                            else
                            {
                                order.OrderStatusId = 38;
                                db.SaveChanges();
                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = orderId,
                                    Remark = "Back To Store Full Refund Failed",
                                    StatusId = 73,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                responseStatus.Message = $"Full Refund Failed.";
                                responseStatus.Status = false;

                            }
                        }

                    }
                    
                    foreach (var applogs in onlinePayment)
                    {
                        if (applogs == null)
                        {
                            responseStatus.Message = $"No Payment made against the order {orderId}";
                            return responseStatus;
                        }
                        

                        //double refundAmt = 0.0;
                        double refundAmt = Convert.ToDouble(applogs.OrderAmount);
                        var walletAmt = order.WalletAmountUsed;

                        var refunddet = db.CashFreeRefunds.Where(x => x.OrderId == order.Id).ToList();

                        if (refunddet != null && refunddet.Count() > 0)
                        {
                            var refundedAmount = refunddet.Where(y => y.RefundParam != null && y.RefundParam.referenceId == applogs.ReferenceId);
                            if (refundedAmount != null && refundedAmount.Sum(x => Convert.ToDouble(x.Amt)) > 0)
                            {
                                refundAmt = refundAmt - refundedAmount.Sum(x => Convert.ToDouble(x.Amt));
                            }

                        }
                        //double refundInitiatedAmt = orderDBO.GetRefundInitiatedAmount(order.Id);


                        OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();

                        decimal walletReturnedAmt = onlinePaymentServiceDBO.GetWalletReturnedAmount(order.Id);
                        if (walletReturnedAmt > 0 && (Convert.ToDouble(walletAmt) - Convert.ToDouble(walletReturnedAmt)) > 0)
                        {
                            walletAmt = walletAmt - walletReturnedAmt;
                        }
                        if (walletAmt < walletReturnedAmt)
                        {
                            decimal extOnlineAmt = onlinePaymentServiceDBO.GetWalletExceededAmount(order.Id);
                            double reAmt = Convert.ToDouble(walletReturnedAmt) - Convert.ToDouble(extOnlineAmt);
                            refundAmt = refundAmt - reAmt;
                        }

                        if (walletAmt != null && walletAmt > 0)
                        {
                            OrderIssueDBO orderIssueDBO = new OrderIssueDBO();
                            var result1 = orderIssueDBO.FullRefundToWallet(orderId, (float)walletAmt, 0);
                            if (result1 == 1)
                            {
                                order.OrderStatusId = 37;
                                db.SaveChanges();
                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = orderId,
                                    Remark = "Back To Store Full Refund Successful",
                                    StatusId = 72,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                responseStatus.Message = $"Refund successful.";
                                responseStatus.Status = true;

                                CustomerOrderController customerOrderController = new CustomerOrderController();
                                customerOrderController.WalletRefundNotification(Convert.ToInt32(walletAmt), order.CustomerId, order.Customer.UserId);
                            }
                            else
                            {
                                order.OrderStatusId = 38;
                                db.SaveChanges();
                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = orderId,
                                    Remark = "Back To Store Full Refund Failed",
                                    StatusId = 73,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                responseStatus.Message = $"Full Refund Failed.";
                                responseStatus.Status = false;

                            }
                        }
                        if (refundAmt > 0)
                        {

                            var orderIssueDBO = new OrderIssueDBO();
                            cashFreeRefundObject = new CashFreeRefundObject
                            {
                                appId = ConfigurationManager.AppSettings["PayFreeId"],
                                secretKey = ConfigurationManager.AppSettings["PayKey"],
                                referenceId = $"{applogs.ReferenceId}",
                                refundAmount = refundAmt.ToString(),
                                refundNote = "BackToStoreRefund"
                            };
                            if (cashFreeRefundObject != null)
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    Uri url = new Uri($"{payUrl}");
                                    var json = JsonConvert.SerializeObject(cashFreeRefundObject);
                                    var body = new Dictionary<string, string>
                            {
                                { "appId", cashFreeRefundObject.appId },
                                { "secretKey", cashFreeRefundObject.secretKey },
                                { "referenceId", cashFreeRefundObject.referenceId },
                                { "refundAmount", cashFreeRefundObject.refundAmount },
                                { "refundNote", cashFreeRefundObject.refundNote },
                            };


                                    var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                                    var response = client.SendAsync(req).Result;
                                    var resp = response.Content.ReadAsStringAsync().Result;
                                    var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                                    db.CashFreeRefunds.Add(new CashFreeRefund
                                    {
                                        InputParam = json,
                                        IssueId = 0,
                                        OrderId = orderId,
                                        VenderOutput = resp,
                                        Amt = refundAmt.ToString(),
                                        CreatedDate = DateTime.Now
                                    });
                                    db.SaveChanges();

                                    //int issueClosed = (int)IssueType.Closed;
                                    if (string.Compare(ret.status, "ok", true) == 0)
                                    {
                                        var ord = db.Orders.Find(orderId);
                                        ord.OrderStatusId = 37;
                                        db.SaveChanges();

                                        WebEngageController webEngageController = new WebEngageController();
                                        Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Order Refund Completed"));


                                        var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                        string uId = u.Id;
                                        OrderTrack orderTrack = new OrderTrack
                                        {
                                            LogUserName = u.Email,
                                            LogUserId = uId,
                                            OrderId = orderId,
                                            Remark = ret.status,
                                            StatusId = 37,
                                            TrackDate = DateTime.Now
                                        };
                                        db.OrderTracks.Add(orderTrack);
                                        db.SaveChanges();

                                        responseStatus.Message = $"Refund successful.";
                                        responseStatus.Status = true;

                                        if ((string.Compare(ret.status, "ok", true) == 0))
                                        {
                                            //Flow SMS
                                            var dicti = new Dictionary<string, string>();
                                            dicti.Add("ORDERID", orderId.ToString());
                                            dicti.Add("AMOUNT", refundAmt.ToString());
                                            dicti.Add("DATE", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                                            var templeteid = ConfigurationManager.AppSettings["SMSRefundFlowId"];
                                            Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, ord.OrderTo, dicti));
                                            //End Flow SMS
                                            if (order.OrderStatusId == 5 || order.OrderStatusId == 37 || order.OrderStatusId == 68 || order.OrderStatusId == 48)
                                            {
                                                //HyperTracking Complted
                                                HyperTracking hyperTracking = new HyperTracking();
                                                Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(orderId));

                                                //Live Tracking FireStore

                                                CustomerApi2Controller.DeleteToFireStore(orderId);
                                                orderDBO.UpdatedScheduledOrder(orderId);


                                            }

                                            var c = SZIoc.GetSerivce<ISZConfiguration>();
                                            var isReturnCouponAmount = c.GetConfigValue(ConfigEnums.IsReturnCouponAmount.ToString());
                                            if (isReturnCouponAmount == "1")
                                            {
                                                DiscountDBO discountDBO = new DiscountDBO();
                                                var numberOfUse = discountDBO.RevertNumberOfUseSpecificOffer(ord.CustomerId, ord.Id);
                                            }
                                        }

                                    }
                                    else if (string.Compare(ret.status, "error", true) == 0)
                                    {
                                        var ord = db.Orders.Find(orderId);
                                        ord.OrderStatusId = 38;
                                        db.SaveChanges();

                                        var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                        string uId = u.Id;
                                        OrderTrack orderTrack = new OrderTrack
                                        {
                                            LogUserName = u.Email,
                                            LogUserId = uId,
                                            OrderId = orderId,
                                            Remark = ret.status,
                                            StatusId = 38,
                                            TrackDate = DateTime.Now
                                        };
                                        db.OrderTracks.Add(orderTrack);
                                        db.SaveChanges();

                                        //var failed = orderDBO.UpdateRefundInitiatedFailed(issue.OrderId.Value, issue.OrderIssueId);
                                        responseStatus.Message = $"Error while Refund the Amount. {ret.message}";
                                    }
                                }
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

        public ResponseStatus CashFreePaymentForMrpIssue(int orderId, int AdjustAmt, int MrpIssueId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            string payUrl = ConfigurationManager.AppSettings["PayCreate"];
            CashFreeOrderCreate cashFreeOrderCreate = null;

            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //var issue = db.OrderIssues.Find(issueId);
                    var order = db.Orders.Find(orderId);
                    var maxFreePay = db.CashFreePays.Where(o => o.OrderId == order.Id).Count();
                    cashFreeOrderCreate = new CashFreeOrderCreate
                    {
                        appId = ConfigurationManager.AppSettings["PayFreeId"],
                        secretKey = ConfigurationManager.AppSettings["PayKey"],
                        orderId = $"{order.Id}_MrpPartialPay_{maxFreePay}_{MrpIssueId}",
                        orderAmount = AdjustAmt.ToString(),
                        orderCurrency = "INR",
                        customerEmail = "subhamautomation@rainmail.com",//order.Customer.CustomerName,
                        customerPhone = order.Customer.ContactNo,
                        notifyUrl = ConfigurationManager.AppSettings["PayNotifyUrl"],
                        returnUrl = ConfigurationManager.AppSettings["PayReturnUrl"],
                        customerName = order.Customer.CustomerName,
                        //orderNote="Test"
                    };
                    if (cashFreeOrderCreate != null)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            Uri url = new Uri($"{payUrl}");
                            var json = JsonConvert.SerializeObject(cashFreeOrderCreate);
                            var body = new Dictionary<string, string>
                        {
                            { "appId", cashFreeOrderCreate.appId },
                            { "secretKey", cashFreeOrderCreate.secretKey },
                            { "orderId", cashFreeOrderCreate.orderId },
                            { "orderAmount", cashFreeOrderCreate.orderAmount },
                            { "customerEmail", cashFreeOrderCreate.customerEmail },
                            { "customerPhone", cashFreeOrderCreate.customerPhone },
                            { "notifyUrl", cashFreeOrderCreate.notifyUrl },
                            { "returnUrl", cashFreeOrderCreate.returnUrl }
                        };

                            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                            var response = client.SendAsync(req).Result;
                            var resp = response.Content.ReadAsStringAsync().Result;
                            var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                            db.CashFreePays.Add(new CashFreePay
                            {
                                InputValue = json,
                                IssueId = MrpIssueId,
                                OrderId = orderId,
                                VenderOutput = resp,
                                CreatedDate = DateTime.Now,
                                Amt = AdjustAmt.ToString()
                            });

                            db.SaveChanges();

                            if (string.Compare(ret.status, "ok", true) == 0)
                            {

                                order.OrderStatusId = 35;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = orderId,
                                    Remark = ret.status,
                                    StatusId = 35,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, GetTotalAmtOfOrder(order.Id));
                                db.SaveChanges();

                                responseStatus.Message = $"Payment Link Send to Customer.";
                                responseStatus.Status = true;
                                //SpiritUtility.GenerateZohoTikect(orderId, MrpIssueId);

                                WSendSMS wsms = new WSendSMS();
                                string textmsg = string.Format(ConfigurationManager.AppSettings["CFPartialPay"], AdjustAmt, order.Id.ToString(), ret.paymentLink);
                                wsms.SendMessage(textmsg, order.Customer.ContactNo);

                            }
                            else if (string.Compare(ret.status, "error", true) == 0)
                            {
                                responseStatus.Message = $"Error while Refund the Amount. {ret.reason}";
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

        public ResponseStatus CashFreeReFundForMrpIssue(int orderId, int AdjustAmt, int MrpIssueId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };

            string payUrl = ConfigurationManager.AppSettings["PayRefund"];
            CashFreeRefundObject cashFreeRefundObject = null;
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //var issue = db.OrderIssues.Find(issueId);
                    var order = db.Orders.Find(orderId);
                    var applogs = db.AppLogsCashFreeHooks.Where(o => string.Compare(o.OrderId.Trim(), order.Id.ToString(), true) == 0)?
                        .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

                    if (applogs == null)
                    {
                        responseStatus.Message = $"No Payment made against the order {orderId}";
                        return responseStatus;
                    }
                    double refundAmt = Math.Abs(AdjustAmt);
                    double oAmt = Convert.ToDouble(applogs.OrderAmount);
                    var walletAmt = order.WalletAmountUsed;

                    if (refundAmt > oAmt)
                    {
                        responseStatus.Message = "Last Payment made is less than Refund amount.";
                        return responseStatus;
                    }
                    cashFreeRefundObject = new CashFreeRefundObject
                    {
                        appId = ConfigurationManager.AppSettings["PayFreeId"],
                        secretKey = ConfigurationManager.AppSettings["PayKey"],
                        referenceId = $"{applogs.ReferenceId}",
                        refundAmount = refundAmt.ToString(),
                        refundNote = "MRP Partial Refund"
                    };
                    if (cashFreeRefundObject != null)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            Uri url = new Uri($"{payUrl}");
                            var json = JsonConvert.SerializeObject(cashFreeRefundObject);
                            var body = new Dictionary<string, string>
                            {
                                { "appId", cashFreeRefundObject.appId },
                                { "secretKey", cashFreeRefundObject.secretKey },
                                { "referenceId", cashFreeRefundObject.referenceId },
                                { "refundAmount", cashFreeRefundObject.refundAmount },
                                { "refundNote", cashFreeRefundObject.refundNote },
                            };


                            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                            var response = client.SendAsync(req).Result;
                            var resp = response.Content.ReadAsStringAsync().Result;
                            var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                            db.CashFreeRefunds.Add(new CashFreeRefund
                            {
                                InputParam = json,
                                IssueId = MrpIssueId,
                                OrderId = orderId,
                                VenderOutput = resp,
                                Amt = refundAmt.ToString(),
                                CreatedDate = DateTime.Now
                            });
                            db.SaveChanges();


                            if (string.Compare(ret.status, "ok", true) == 0)
                            {
                                var ord = db.Orders.Find(orderId);
                                ord.OrderStatusId = 37;
                                db.SaveChanges();


                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = orderId,
                                    Remark = ret.status,
                                    StatusId = 37,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();


                                responseStatus.Message = $"Refund successful.";
                                responseStatus.Status = true;

                                if (AdjustAmt <= walletAmt && (string.Compare(ret.status, "ok", true) == 0))
                                {
                                    var orderIssueDBO = new OrderIssueDBO();
                                    if (walletAmt != null)
                                    {
                                        orderIssueDBO.FullRefundToWallet(orderId, (float)AdjustAmt, 0);
                                    }
                                    if (order.OrderStatusId == 5 || order.OrderStatusId == 37 || order.OrderStatusId == 68 || order.OrderStatusId == 48)
                                    {
                                        //Live Tracking FireStore
                                        OrderDBO orderDBO = new OrderDBO();
                                        CustomerApi2Controller.DeleteToFireStore(orderId);
                                        orderDBO.UpdatedScheduledOrder(orderId);

                                        //HyperTracking Complted
                                        //HyperTracking hyperTracking = new HyperTracking();
                                        //Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(orderId));
                                    }
                                }
                                //SpiritUtility.GenerateZohoTikect(orderId, MrpIssueId);

                                WSendSMS wSendSMS = new WSendSMS();
                                string text = string.Format(ConfigurationManager.AppSettings["CFPartialRefund"], Math.Abs(Convert.ToDouble(AdjustAmt)), orderId);
                                wSendSMS.SendMessage(text, ord.Customer.ContactNo);

                            }
                            else if (string.Compare(ret.status, "error", true) == 0)
                            {
                                var ord = db.Orders.Find(orderId);
                                ord.OrderStatusId = 38;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = orderId,
                                    Remark = ret.status,
                                    StatusId = 38,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                //SpiritUtility.GenerateZohoTikect(orderId, MrpIssueId);

                                responseStatus.Message = $"Error while Refund the Amount. {ret.message}";
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

        public ResponseStatus CashFreePaymentOrderModify(int orderModifyId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            string payUrl = ConfigurationManager.AppSettings["PayCreate"];
            CashFreeOrderCreate cashFreeOrderCreate = null;

            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var modify = db.OrderModifies.Find(orderModifyId);
                    var order = db.Orders.Find(modify.OrderId);
                    var maxFreePay = db.CashFreePays.Where(o => o.OrderId == order.Id).Count();
                    cashFreeOrderCreate = new CashFreeOrderCreate
                    {
                        appId = ConfigurationManager.AppSettings["PayFreeId"],
                        secretKey = ConfigurationManager.AppSettings["PayKey"],
                        orderId = $"{order.Id}_PartialPayModify_{maxFreePay}_{orderModifyId}",
                        orderAmount = modify.AdjustAmt.ToString(),
                        orderCurrency = "INR",
                        customerEmail = "subhamautomation@rainmail.com",//order.Customer.CustomerName,
                        customerPhone = order.Customer.ContactNo,
                        notifyUrl = ConfigurationManager.AppSettings["PayModifyNotifyUrl"],
                        returnUrl = ConfigurationManager.AppSettings["PayModifyReturnUrl"],
                        customerName = order.Customer.CustomerName,
                        //orderNote="Test"
                    };
                    if (cashFreeOrderCreate != null)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            Uri url = new Uri($"{payUrl}");
                            var json = JsonConvert.SerializeObject(cashFreeOrderCreate);
                            var body = new Dictionary<string, string>
                        {
                            { "appId", cashFreeOrderCreate.appId },
                            { "secretKey", cashFreeOrderCreate.secretKey },
                            { "orderId", cashFreeOrderCreate.orderId },
                            { "orderAmount", cashFreeOrderCreate.orderAmount },
                            { "customerEmail", cashFreeOrderCreate.customerEmail },
                            { "customerPhone", cashFreeOrderCreate.customerPhone },
                            { "notifyUrl", cashFreeOrderCreate.notifyUrl },
                            { "returnUrl", cashFreeOrderCreate.returnUrl }
                        };

                            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                            var response = client.SendAsync(req).Result;
                            var resp = response.Content.ReadAsStringAsync().Result;
                            var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                            db.CashFreePays.Add(new CashFreePay
                            {
                                InputValue = json,
                                OrderModifyId = orderModifyId,
                                OrderId = modify.OrderId,
                                VenderOutput = resp,
                                CreatedDate = DateTime.Now,
                                Amt = modify.AdjustAmt.ToString()
                            });

                            db.SaveChanges();

                            if (string.Compare(ret.status, "ok", true) == 0)
                            {

                                int statusCashLinkSend = (int)OrderStatusEnum.CashLinkSend;
                                order.OrderStatusId = statusCashLinkSend;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = modify.OrderId ?? 0,
                                    Remark = ret.status,
                                    StatusId = statusCashLinkSend,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                //update the issue orderdetails to orderdetail table
                                OrderDBO orderDBO = new OrderDBO();
                                orderDBO.UpdateOrderModify(orderModifyId, order.Id);
                                //update the total order value into order table
                                //order.OrderAmount = GetTotalAmtOfOrder(order.Id);
                                order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, GetTotalAmtOfOrder(order.Id));
                                db.SaveChanges();

                                responseStatus.Message = $"Payment Link Send to Customer.";
                                responseStatus.Status = true;

                                WSendSMS wsms = new WSendSMS();
                                string textmsg = string.Format(ConfigurationManager.AppSettings["CFPartialPay"], cashFreeOrderCreate.orderAmount, order.Id.ToString(), ret.paymentLink);
                                wsms.SendMessage(textmsg, order.Customer.ContactNo);

                            }
                            else if (string.Compare(ret.status, "error", true) == 0)
                            {
                                responseStatus.Message = $"Error while Refund the Amount. {ret.reason}";
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

        public ResponseStatus CashFreeReFundForOrderModify(int orderModifyId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };

            string payUrl = ConfigurationManager.AppSettings["PayRefund"];
            CashFreeRefundObject cashFreeRefundObject = null;
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var modify = db.OrderModifies.Find(orderModifyId);
                    var order = db.Orders.Find(modify.OrderId);
                    var applogs = db.AppLogsCashFreeHooks.Where(o => string.Compare(o.OrderId.Trim(), order.Id.ToString(), true) == 0)?
                        .OrderByDescending(o => o.AppLogsCashFreeHookId).FirstOrDefault();

                    if (applogs == null)
                    {
                        responseStatus.Message = $"No Payment made against the order {modify.OrderId}";
                        return responseStatus;
                    }
                    double refundAmt = Math.Abs(modify.AdjustAmt ?? 0);
                    double oAmt = Convert.ToDouble(applogs.OrderAmount);
                    if (refundAmt > oAmt)
                    {
                        responseStatus.Message = "Last Payment made is less than Refund amount.";
                        return responseStatus;
                    }
                    string orderTypeName = Enum.GetName(typeof(IssueType), modify.OrderType);
                    cashFreeRefundObject = new CashFreeRefundObject
                    {
                        appId = ConfigurationManager.AppSettings["PayFreeId"],
                        secretKey = ConfigurationManager.AppSettings["PayKey"],
                        referenceId = $"{applogs.ReferenceId}",
                        refundAmount = refundAmt.ToString(),
                        refundNote = orderTypeName
                    };
                    if (cashFreeRefundObject != null)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            Uri url = new Uri($"{payUrl}");
                            var json = JsonConvert.SerializeObject(cashFreeRefundObject);
                            var body = new Dictionary<string, string>
                            {
                                { "appId", cashFreeRefundObject.appId },
                                { "secretKey", cashFreeRefundObject.secretKey },
                                { "referenceId", cashFreeRefundObject.referenceId },
                                { "refundAmount", cashFreeRefundObject.refundAmount },
                                { "refundNote", cashFreeRefundObject.refundNote },
                            };


                            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
                            var response = client.SendAsync(req).Result;
                            var resp = response.Content.ReadAsStringAsync().Result;
                            var ret = JsonConvert.DeserializeObject<CashFreePaymentOutput>(resp);

                            db.CashFreeRefunds.Add(new CashFreeRefund
                            {
                                InputParam = json,
                                OrderModifyId = orderModifyId,
                                OrderId = modify.OrderId,
                                VenderOutput = resp,
                                Amt = refundAmt.ToString(),
                                CreatedDate = DateTime.Now
                            });
                            db.SaveChanges();

                            int issueClosed = (int)IssueType.Closed;
                            if (string.Compare(ret.status, "ok", true) == 0)
                            {
                                int statusRefunded = (int)OrderStatusEnum.OrderModifyRefunded;
                                int statusApproved = (int)OrderStatusEnum.Approved;
                                var ord = db.Orders.Find(modify.OrderId ?? 0);
                                ord.OrderStatusId = statusRefunded;
                                db.SaveChanges();

                                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                                string uId = u.Id;
                                OrderTrack orderTrack = new OrderTrack
                                {
                                    LogUserName = u.Email,
                                    LogUserId = uId,
                                    OrderId = modify.OrderId ?? 0,
                                    Remark = ret.status,
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
                                    orderDBO.UpdateOrderModify(orderModifyId, order.Id);
                                    //update the total order value into order table
                                    //order.OrderAmount = GetTotalAmtOfOrder(order.Id);
                                    order.OrderAmount = SpiritUtility.CalculateOverAllDiscount(order.Id, GetTotalAmtOfOrder(order.Id));
                                    db.SaveChanges();

                                    ord.OrderStatusId = 3;
                                    db.SaveChanges();
                                    modify.StatusId = issueClosed;
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
                                responseStatus.Message = $"Refund successful.";
                                responseStatus.Status = true;

                                OrderModifyTrack orderModifyTrack = new OrderModifyTrack
                                {
                                    UserId = uId,
                                    OrderId = modify.OrderId ?? 0,
                                    OrderModifyId = modify.Id,
                                    Remark = ret.message,
                                    OrderStatusId = issueClosed,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderModifyTracks.Add(orderModifyTrack);
                                db.SaveChanges();

                                WSendSMS wSendSMS = new WSendSMS();
                                string text = string.Format(ConfigurationManager.AppSettings["CFPartialRefund"], Math.Abs(Convert.ToDouble(modify.AdjustAmt)), modify.OrderId);
                                wSendSMS.SendMessage(text, ord.Customer.ContactNo);

                            }
                            else if (string.Compare(ret.status, "error", true) == 0)
                            {
                                int statusRefundFailed = (int)OrderStatusEnum.OrderModifyRefundFailed;

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
                                    Remark = ret.status,
                                    StatusId = statusRefundFailed,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderTracks.Add(orderTrack);
                                db.SaveChanges();

                                modify.StatusId = issueClosed;
                                db.SaveChanges();

                                OrderModifyTrack orderModifyTrack = new OrderModifyTrack
                                {
                                    UserId = u.Id,
                                    OrderId = modify.OrderId ?? 0,
                                    OrderModifyId = modify.Id,
                                    Remark = ret.message,
                                    OrderStatusId = issueClosed,
                                    TrackDate = DateTime.Now
                                };
                                db.OrderModifyTracks.Add(orderModifyTrack);
                                db.SaveChanges();

                                responseStatus.Message = $"Error while Refund the Amount. {ret.message}";
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
        public async Task TriggerSilentBannerNotification(string userid, int customerid, int shopid, string pageName, int id = 0)
        {
            try
            {
                PopupBannerDBO popupBannerDBO = new PopupBannerDBO();
                var banner = popupBannerDBO.GetPagePopupBanner(pageName, userid, shopid, id);
                if (banner != null)
                {
                    var slnt = new SilentNotification();
                    var bannerData = new
                    {
                        PromoPopCode = $"{DateTime.Now.ToString("ddMMyyyy")}{banner.PopupBannerId}",
                        Filepath = banner.FilePath,
                        banner.OtherText,
                        banner.Payload,
                        NumberOfTimes = banner.NumberView,
                        BrandId = banner.BrandId.HasValue ? banner.BrandId.Value : 0,
                        banner.PopupBannerId
                    };
                    slnt.BannerData = JsonConvert.SerializeObject(bannerData);
                    slnt.CustomerID = customerid;
                    slnt.PageName = pageName;
                    slnt.UserID = userid;
                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    await Task.Delay(3000);
                    await fcmHelper.SendFirebaseNotificationSilent(slnt);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task OrdersAssign(int shopId, int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            try
            {

                var url = ConfigurationManager.AppSettings["ExtOrderAssign_Url"];
                var authHeader = ConfigurationManager.AppSettings["ExtAuthHeader"];

                var input = new
                {
                    shop_id = shopId,
                    order_id = orderId
                };

                using (HttpClient client = new HttpClient())
                {
                    var serializeJson = JsonConvert.SerializeObject(input);
                    var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", authHeader);
                    var resp = client.PostAsync(url, content).Result;
                    var res =await resp.Content.ReadAsStringAsync();
                    if (resp.IsSuccessStatusCode)
                    {
                        responseStatus.Status = true;
                        responseStatus.Message = "Order Assigned";


                    }
                    else
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "External Api Failed";
                    }


                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}