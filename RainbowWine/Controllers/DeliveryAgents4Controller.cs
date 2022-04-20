using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using SZInfrastructure;

namespace RainbowWine.Controllers
{
    [RoutePrefix("delapi/v4.0")]
    [Authorize]
    [DisplayName("Operational")]
    [EnableCors("*", "*", "*")]
    public class DeliveryAgents4Controller : ApiController
    {
        FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
        private rainbowwineEntities db = new rainbowwineEntities();
        NewDelAppDBO newDelAppDBO = new NewDelAppDBO();
        [HttpGet]
        [Route("update_ofd/{orderid}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult UpdateOFDStatus(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            var u = db.AspNetUsers.Where(o => o.Id == uId).FirstOrDefault();
            bool isOrderUpdated = false;
            
            bool isOutForDelivery = false;
            bool isCustomerNoti = false;
            bool isWebEngageCall = false;
            bool isFireStoreCall = false;
            bool isRoutePlaneUpdated = false;
            var order = db.Orders.Where(x => x.Id == orderId).FirstOrDefault();
            if (order !=null && order.Id > 0)
            {
                order.OrderStatusId = 9;
                db.SaveChanges();
                isOrderUpdated = true;

            }
            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = u.Email,
                LogUserId = u.Id,
                OrderId = order.Id,
                StatusId = 9,
                TrackDate = DateTime.Now,
                Remark = "Updated a out for delivery"
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();
            var ot = db.OrderTracks.Where(x =>x.OrderId ==orderId && x.StatusId == 9).FirstOrDefault();
            if (ot !=null)
            {
                isOutForDelivery = true;
            }

            Task.Run(async () => await fcmHelper.SendFirebaseNotification(orderId, FirebaseNotificationHelper.NotificationType.Order));
            isCustomerNoti = true;
            WebEngageController webEngageController = new WebEngageController();
            Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Out For Delivery"));
            isWebEngageCall = true;
           
            var routeplan = db.RoutePlans.Where(x => x.OrderID == orderId).FirstOrDefault();
            if (routeplan !=null)
            {
                routeplan.isOutForDelivery = true;
                routeplan.IsOrderStart = true;
                db.SaveChanges();
                isRoutePlaneUpdated = true;
            }

            //Live Tracking FireStore
            CustomerApi2Controller.AddToFireStore(order.Id);
            isFireStoreCall = true;
            responseStatus.Data = new 
            { 
                isOrderUpdated,
                isOutForDelivery,
                isCustomerNoti,
                isWebEngageCall,
                isFireStoreCall,
                isRoutePlaneUpdated
            };

            return Ok(responseStatus);

        }

        [HttpGet]
        [Route("update_slotstatus/{jobId}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult UpdateSlotsStatus(string jobId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            var u = db.AspNetUsers.Where(o => o.Id == uId).FirstOrDefault();

            var slotStatus = newDelAppDBO.UpdateSlotStatus(jobId);
            if (slotStatus == 1)
            {
                var routeplan = db.RoutePlans.Where(x => x.JobId == jobId).ToList();
                foreach (var item in routeplan)
                {
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = item.OrderID,
                        StatusId = 76,
                        TrackDate = DateTime.Now,
                        Remark = "Slot Started"
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();
                    responseStatus.Data = true;
                    responseStatus.Message = "Slot Status Updated";

                    var ord = db.Orders.Where(x => x.Id == item.OrderID).FirstOrDefault();
                    //Live Tracking FireStore
                    CustomerApi2Controller.AddToFireStore(item.OrderID);

                    WSendSMS wsms = new WSendSMS();
                    string textmsg = string.Format(ConfigurationManager.AppSettings["SMSOutForDelivery"], item.OrderID.ToString());
                    wsms.SendMessage(textmsg, ord.OrderTo);
                }

            }
            else
            {
                responseStatus.Data = false;
                responseStatus.Message = "Failed to update";
            }
            return Ok(responseStatus);

        }
    }
}
