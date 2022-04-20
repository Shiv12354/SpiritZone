using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using RainbowWine.Services.Email;
using RainbowWine.Models;
using System.Configuration;
using RainbowWine.Providers;
using SZInfrastructure;

namespace RainbowWine.Services
{
    public class ApplicationEmailSend
    {
        public static object SupplierOrderInformation(Order order)
        {
            if (order==null)
            {
                return new { status = false, msg = "Order ncan not be null." };
            }
            if (string.IsNullOrWhiteSpace(order.OrderGroupId))
            {
                return new { status = false, msg = "Order Group ID is not null." };
            }

            try
            {
                Supplier supplier = null;
                AspNetUser user = null;
                CustomerEta oeta = null;
                using (var db = new rainbowwineEntities())
                {
                    var mixerItem = db.MixerOrderItems.Include(o=>o.Supplier).Where(o => (o.OrderId ?? 0) == order.Id).FirstOrDefault();
                    supplier = mixerItem?.Supplier;
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    user = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();

                    oeta = db.CustomerEtas.Where(o => o.OrderId == order.Id)?.OrderByDescending(o=>o.Id).Take(1).FirstOrDefault();
                }
                if (supplier == null)
                {
                    return new { status = false, msg = "Supplier not found." };
                }

                var config = SZIoc.GetSerivce<ISZConfiguration>();
                //SendEMail
                EmailSender _emailSender = new EmailSender();
                _emailSender.SendEmailAsync(new Dictionary<string, string> {
                      {"sname", supplier.Name},
                      {"orderid", order.Id.ToString()},
                    {"sETA",oeta.CommittedTime.ToString() },
                    {"eETA",oeta.CommittedTimeEnd.ToString() },
                    { "website",config.GetConfigValue(ConfigEnums.LiveSite.ToString())}
                }, supplier.Email, string.Format(EmailSelectSubject.SupplierOrderInfo, order.Id.ToString()), EmailSelectTemplate.SupplierOrderInfo);

                using (var db = new rainbowwineEntities())
                {
                    var stEmailsend = (int)OrderStatusEnum.CashFreePayLinkEmailResent;
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = user?.Email,
                        LogUserId = user?.Id,
                        OrderId = order.Id,
                        StatusId = stEmailsend,
                        TrackDate = DateTime.Now,
                        Remark = "Email Sent"
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.Logging(ex.Message, ex.StackTrace);
                return new { status = false, msg = "Error." };
            }
            return new { status = true, msg = "Emailed payment link to customer." };
        }
    }
}