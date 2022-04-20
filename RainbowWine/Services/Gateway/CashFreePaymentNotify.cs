
using RainbowWine.Data;
using RainbowWine.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

namespace RainbowWine.Services.Gateway
{
    public class CashFreePaymentNotify
    {
        public string SecretKey => ConfigurationManager.AppSettings["PayKey"];

        public OutputCashFree ProcessPayment(CashFreeSetApproveResponse decodevlaue, string pdecodevalue)
        {
            OutputCashFree outCashFree = new OutputCashFree { Status = false };
            var db = new rainbowwineEntities();
            try
            {
                var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.ReferenceId == decodevlaue.ReferenceId)?.FirstOrDefault();
                if (CheckPaymentComplete(decodevlaue, pdecodevalue, appLogsCashFreeHook))
                {
                    appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.ReferenceId == decodevlaue.ReferenceId)?.FirstOrDefault();
                    int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
                    decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);
                    Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();
                    
                    IList<Order> groupOrders = new List<Order>();
                    if (order == null)
                    {
                        groupOrders = db.Orders.Where(o => string.Compare(o.OrderGroupId, orderIdDecode.ToString(), true) == 0)?.ToList();
                    }

                    if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
                    {
                        if (groupOrders.Count > 0)
                        {
                            decimal mAmt = 0;
                            groupOrders.ForEach((o) =>
                            {
                                mAmt += o.OrderAmount;
                            });

                            if (mAmt == amt && (order.OrderStatusId != 2 || order.OrderStatusId != 16))
                            {
                                foreach (var item in groupOrders)
                                {
                                    NormalProcess(item, appLogsCashFreeHook);
                                }
                                outCashFree.Status = true;
                                outCashFree.Message = "MultipleOrderApproved";
                                return outCashFree;
                            }
                            else
                            {
                                appLogsCashFreeHook.SendStatus = "ShouldCheck";
                                db.SaveChanges();

                                outCashFree.Status = true;
                                outCashFree.Message = "MultipleOrderAmountNotMatch";
                                return outCashFree;
                            }
                        }
                        if (!PODPaymentProcess(order, amt, appLogsCashFreeHook, outCashFree))
                        {
                            if (order.OrderAmount == amt && (order.OrderStatusId != 2 || order.OrderStatusId != 16))
                            {
                                NormalProcess(order, appLogsCashFreeHook);
                            }
                            else
                            {
                                appLogsCashFreeHook.SendStatus = "ShouldCheck";
                                db.SaveChanges();

                                outCashFree.Status = true;
                                outCashFree.Message = "AmountNotMatch";
                                return outCashFree;
                            }
                        }
                        else
                        {
                            return outCashFree;
                        }
                    }
                    else
                    {
                        appLogsCashFreeHook.SendStatus = "ShouldCheck";
                        db.SaveChanges();

                        PODPaymentFail(order);

                        outCashFree.Status = true;
                        outCashFree.Message = "ShouldCheck";
                        return outCashFree;
                    }
                }
                outCashFree.Status = true;
                outCashFree.Message = "AlreadyApproved";

            }
            finally { db.Dispose(); }
            return outCashFree;
        }

        public bool CheckPaymentComplete(CashFreeSetApproveResponse decodevlaue, string pdecodevalue, AppLogsCashFreeHook appLogsCashFreeHook)
        {
            var db = new rainbowwineEntities();
            bool addupdate = false;
            try
            {
                int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
                if (appLogsCashFreeHook != null)
                {
                    string data = "";
                    data = data + decodevlaue.OrderId;
                    data = data + decodevlaue.OrderAmount;
                    data = data + decodevlaue.ReferenceId;
                    data = data + decodevlaue.Status;
                    data = data + decodevlaue.PaymentMode;
                    data = data + decodevlaue.Msg;
                    data = data + decodevlaue.TxtTime.Replace("=", ":"); //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string signature = SpiritUtility.CreateTokenForCashFree(data, SecretKey);
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
            }
            finally { db.Dispose(); }

            return addupdate;
        }

        public bool PODPaymentProcess(Order order, decimal decodeAmount, AppLogsCashFreeHook appLogsCashFreeHook, OutputCashFree outCashFree)
        {
            bool isPOD = false;

            decimal amt = decodeAmount;

            int payType = (int)OrderPaymentType.POD;
            if (order.PaymentTypeId != null)
            {
                if (order.PaymentTypeId == payType)
                {
                    isPOD = true;
                    using (var db = new rainbowwineEntities())
                    {
                        if (order.OrderAmount == amt)
                        {
                            var ord = db.Orders.Find(order.Id);
                            int statusPODCashPaid = (int)OrderStatusEnum.PODPaymentSuccess;
                            ord.OrderStatusId = statusPODCashPaid;
                            db.SaveChanges();

                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
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

                            var applog = db.AppLogsCashFreeHooks.Find(appLogsCashFreeHook.AppLogsCashFreeHookId);
                            applog.SendStatus = "Approved";
                            db.SaveChanges();

                            outCashFree.Message = "Approved";
                            outCashFree.Status = true;
                            //create api to update inventory update
                            //InventoryUpdate(order.Id);

                            WSendSMS wsms = new WSendSMS();
                            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                            wsms.SendMessage(textmsg, order.Customer.ContactNo);
                        }
                        else
                        {
                            appLogsCashFreeHook.SendStatus = "AmountNotMatch";
                            db.SaveChanges();

                            outCashFree.Message = "AmountNotMatch";
                            outCashFree.Status = true;
                        }
                    }
                    //return "Order is PODCashPaid";
                }
            }
            return isPOD;
        }

        public void PODPaymentFail(Order order)
        {
            var db = new rainbowwineEntities();
            try
            {
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
            }
            finally { db.Dispose(); }
        }
        public void NormalProcess(Order order, AppLogsCashFreeHook appLogsCashFreeHook)
        {
            using (var db = new rainbowwineEntities())
            {
                int statusApprove = (int)OrderStatusEnum.Approved;
                var ord = db.Orders.Find(order.Id);
                ord.OrderStatusId = statusApprove;
                db.SaveChanges();

                var ema = ConfigurationManager.AppSettings["TrackEmail"];
                var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
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

                var applog = db.AppLogsCashFreeHooks.Find(appLogsCashFreeHook.AppLogsCashFreeHookId);
                applog.SendStatus = "Approved";
                db.SaveChanges();

                PaymentLinkLogsService paylog = new PaymentLinkLogsService();
                paylog.InventoryUpdate(order.Id);
                paylog.InventoryMixerUpdate(order.Id);

                WSendSMS wsms = new WSendSMS();
                //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                wsms.SendMessage(textmsg, order.Customer.ContactNo);
            }
        }
    }
}