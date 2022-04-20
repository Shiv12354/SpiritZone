using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using RainbowWine.Services.Filters;

namespace RainbowWine.Controllers
{
    [StopAction]
    public class DelManagerController : Controller
    {

        private rainbowwineEntities db = new rainbowwineEntities();

        [AuthorizeSpirit(Roles = "DeliverySubManager, DeliverySupervisor")]
        public ActionResult Index(int shopId = 0, string shopname = "", int oId = 0, int statusId = 0, int isId = 0)
        {
            var uId = User.Identity.GetUserId();
            IList<RoutePlanDO> routePlans = null;
            if (isId > 0)
            {
                shopId = 0;
                statusId = 0;
                shopname = "";
                oId = 0;
            }

            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            routePlans = routePlanDBO.DeliveryManagerTrackAgent(uId, shopId, oId, isId);
            //if (shopId > 0)
            //{
            //    routePlans = routePlans.Where(o => o.ShopID == shopId).ToList();
            //}
            //if (shopId > 0)
            //{
            //    routePlans = routePlans.Where(o => o.ShopID == shopId).ToList();
            //}
            if (statusId > 0)
            {
                routePlans = routePlans.Where(o => o.Order.OrderStatusId == statusId).ToList();
            }
            if (routePlans.Count() > 0)
            {
                routePlans.ForEach(o =>
                {
                    var track = db.OrderTracks.Include(i => i.OrderStatu).Where(j => j.OrderId == o.OrderID);
                    var orderETA = db.CustomerEtas.Where(i => i.CustomerId == o.Customer.Id)?.OrderByDescending(j => j.Id).FirstOrDefault();

                    //var orderApprovedStatu = track.Where(k => k.StatusId == 3)?.FirstOrDefault();
                    var orderOutDeliveryStatu = track.OrderBy(k => k.OrderTrackId).Take(1).FirstOrDefault();

                    o.OutForDelivery = (orderOutDeliveryStatu != null && orderOutDeliveryStatu.StatusId == 9);
                    o.OutForDeliveryDate = (o.OutForDelivery) ? orderOutDeliveryStatu.TrackDate : (DateTime?)null;

                    o.CustomerETA = (orderETA != null) ? orderETA.Eta : "";
                    o.ETA = orderETA;
                    o.DelTrackUrl = string.Format(ConfigurationManager.AppSettings["DelTrackUrl"], uId);
                });
            }

            var sh = db.WineShops.Join(db.DelManageShops,
                o => o.Id, j => j.ShopID, (o, j) => new { o, j }).Where(o => o.j.rUserId == uId).ToList();
            var shops = db.WineShops.ToList();
            string[] statusIds = { "Approved", "Packed", "OutForDelivery", "BackToStore" };
            var status = db.OrderStatus.Where(o => statusIds.Contains(o.OrderStatusName)).ToList();

            ViewBag.ShopId = new SelectList(sh, "o.Id", "o.ShopName", shopId);
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", statusId);

            ViewBag.DelTrackUrl = string.Format(ConfigurationManager.AppSettings["DelTrackUrl"], uId);
            ViewBag.ShopName = shopname;
            ViewBag.IssueId = isId;
            ViewBag.OrderId = oId;
            return View(routePlans);
        }
        [AuthorizeSpirit(Roles = "DeliverySubManager, DeliverySupervisor")]
        public ActionResult DeliveredOrder(int shopId = 0, string shopname = "", int oId = 0, int statusId = 0, int isId = 0)
        {
            var uId = User.Identity.GetUserId();
            IList<RoutePlanDO> routePlans = null;
            if (isId > 0)
            {
                shopId = 0;
                statusId = 0;
                shopname = "";
                oId = 0;
            }

            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            routePlans = routePlanDBO.DeliveryManagerTrackAgentDelivered(uId, shopId, oId, isId);
            if (statusId > 0)
            {
                routePlans = routePlans.Where(o => o.Order.OrderStatusId == statusId).ToList();
            }
            if (routePlans.Count() > 0)
            {
                routePlans.ForEach(o =>
                {
                    var track = db.OrderTracks.Include(i => i.OrderStatu).Where(j => j.OrderId == o.OrderID);
                    var orderETA = db.CustomerEtas.Where(i => i.CustomerId == o.Customer.Id)?.OrderByDescending(j => j.Id).FirstOrDefault();

                    //var orderApprovedStatu = track.Where(k => k.StatusId == 3)?.FirstOrDefault();
                    var orderOutDeliveryStatu = track.OrderBy(k => k.OrderTrackId).Take(1).FirstOrDefault();

                    o.OutForDelivery = (orderOutDeliveryStatu != null && orderOutDeliveryStatu.StatusId == 9);
                    o.OutForDeliveryDate = (o.OutForDelivery) ? orderOutDeliveryStatu.TrackDate : (DateTime?)null;

                    o.CustomerETA = (orderETA != null) ? orderETA.Eta : "";
                    o.ETA = orderETA;
                    o.DelTrackUrl = string.Format(ConfigurationManager.AppSettings["DelTrackUrl"], uId);
                });
            }

            var sh = db.WineShops.Join(db.DelManageShops,
                o => o.Id, j => j.ShopID, (o, j) => new { o, j }).Where(o => o.j.rUserId == uId).ToList();
            var shops = db.WineShops.ToList();
            string[] statusIds = { "Approved", "Packed", "OutForDelivery", "BackToStore" };
            var status = db.OrderStatus.Where(o => statusIds.Contains(o.OrderStatusName)).ToList();

            ViewBag.ShopId = new SelectList(sh, "o.Id", "o.ShopName", shopId);
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", statusId);

            ViewBag.DelTrackUrl = string.Format(ConfigurationManager.AppSettings["DelTrackUrl"], uId);
            ViewBag.ShopName = shopname;
            ViewBag.IssueId = isId;
            ViewBag.OrderId = oId;
            return View(routePlans);
        }

        [AuthorizeSpirit(Roles = "DeliverySubManager")]
        public ActionResult ShowTracking(int? Id)
        {
            RoutePlan routePlan = null;
            if (Id == null)
            {
                return View(routePlan);
            }
            if (Id <= 0)
            {
                return View(routePlan);
            }

            routePlan = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order).Include(o => o.Customer).Where(o => o.OrderID == Id)?.FirstOrDefault();

            return View(routePlan);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "DeliverySupervisor")]
        public ActionResult Assignment(int shopId = 0)
        {
            var uId = User.Identity.GetUserId();
            DeliveryAgentAssignmentModel deliveryAgentAssignmentModel = new DeliveryAgentAssignmentModel();
            var sh = db.WineShops.Join(db.DelManageShops,
                o => o.Id, j => j.ShopID, (o, j) => new { o, j }).Where(o => o.j.rUserId == uId).ToList();
            deliveryAgentAssignmentModel.Shops = new SelectList(sh, "o.Id", "o.ShopName", shopId);
            
            if(shopId>0)
            {
                var agents = db.DeliveryAgents.Where(o => o.ShopID == shopId);
                OrderDBO orderDBO = new OrderDBO();
                var order = orderDBO.GetAssignmentOrder(shopId);
                deliveryAgentAssignmentModel.ShopId = shopId;
                deliveryAgentAssignmentModel.DeliveryAgents = new SelectList(agents, "Id", "DeliveryExecName") ;
                deliveryAgentAssignmentModel.Orders = order;

            }

            return View(deliveryAgentAssignmentModel);
        }
        [HttpPost]
        [AuthorizeSpirit(Roles = "DeliverySupervisor")]
        public ActionResult Assignment(FormCollection formCollection)
        {
            var uId = User.Identity.GetUserId();
            var jobId = SpiritUtility.UniqueAlphaNumeric(11);
            //string[] ids = formCollection["oId"].Split(new char[] { ',' });
            string orderIds = formCollection["oId"];
            int shopId = Convert.ToInt32(formCollection["dpShopId"]);
            int agentId = Convert.ToInt32(formCollection["dpAgentId"]);

            OrderDBO orderDBO = new OrderDBO();
            var ret = orderDBO.UpateAssignmentOrder(shopId, jobId, orderIds, agentId, uId);

            return RedirectToAction("Assignment");
        }
    }
}