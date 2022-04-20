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
using System.Data;
using System.Data.SqlClient;
using RainbowWine.Providers;
using SZInfrastructure;

namespace RainbowWine.Controllers
{
    [AuthorizeSpirit]
    [StopAction]
    public class DeliveryAgentsController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();

        [HttpPost]
        [Route("callback_api")]
        [AllowAnonymous]
        public ActionResult CallbackPush(CallbackPushApi callbackPushApi)
        {
            try
            {
                db.CallbackPushApis.Add(callbackPushApi);
                db.SaveChanges();
            }
            catch { return Json(new { message = "Fail", status = "1" }, JsonRequestBehavior.AllowGet); }

            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/send_app_sms/{phonenumber}")]
        [AllowAnonymous]
        public ActionResult SendSMSToCustomer(string mType, string phonenumber)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(phonenumber))
                {
                    WSendSMS wsms = new WSendSMS();
                    string textmsg = string.Format(ConfigurationManager.AppSettings["SendSMSToCust"]);
                    wsms.SendMessage(textmsg, phonenumber);
                }
            }
            catch { return Json(new { message = "Fail", status = "1" }, JsonRequestBehavior.AllowGet); }

            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult Index()
        {
            var deliveryAgents = db.DeliveryAgents.Include(d => d.WineShop).Include(d => d.WineShop.DeliveryZones).ToList();
            return View(deliveryAgents);
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult LoginDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // var deliveryAgentLogin = db.DeliveryAgentLogins.Include(o => o.DeliveryAgent).Where(o => o.DeliveryAgentId == id).OrderByDescending(o => o.Id).Take(100);
            var deliveryAgentLogin = db.DeliveryAgentLogins.Include(o => o.DeliveryAgent).Where(o => o.DeliveryAgentId == id).OrderByDescending(o => o.OnDuty).ThenByDescending(d => d.OnDuty).Take(100);

            if (deliveryAgentLogin == null)
            {
                return HttpNotFound();
            }
            return View(deliveryAgentLogin.ToList());
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryAgent deliveryAgent = db.DeliveryAgents.Find(id);
            if (deliveryAgent == null)
            {
                return HttpNotFound();
            }
            return View(deliveryAgent);
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult Create()
        {
            ViewBag.DeliverySlotID = new SelectList(db.DeliverySlots, "DeliverySlotID", "DeliverySlotID");
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName");
           // ViewBag.DeliveryAgentTypeId = new SelectList(db.DeliveryAgentTypes, "DeliveryAgentTypeId", "Descriptions");   // ShiptName
            ViewBag.WeekOffs = new SelectList(db.WeekDays, "Id", "DayName");

          var data = (from s in db.DeliveryAgentTypes                     
                     select new { DeliveryAgentTypeId=s.DeliveryAgentTypeId, Descriptions = s.Descriptions +"_" + s.ShiptName }).ToList();
            ViewBag.DeliveryAgentTypeId = new SelectList(data, "DeliveryAgentTypeId", "Descriptions");

            return View();
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DeliveryExecName,ShopID,LastDeliveryOn,Contact,Address,TravelMode,Coverage,DeliverySlotID,IsAtShop,ExciseID,DocPath,WeekOffs,DeliveryAgentTypeId")] DeliveryAgent deliveryAgent, string [] WeekOffs, HttpPostedFileBase file)
        {

            string result = string.Empty;
            if (WeekOffs != null)
            {
                foreach (var entry in WeekOffs)
                {
                    result += entry + ',';
                }
            }
            deliveryAgent.WeekOffs = result;
            if (ModelState.IsValid)
            {
                deliveryAgent.isActive = 1;
                db.DeliveryAgents.Add(deliveryAgent);
                db.SaveChanges();
                if (file!=null && file.ContentLength > 0)
                {
                    string qrImagePtath = $"/Content/images/delivery";
                    string path = Server.MapPath($"~{qrImagePtath}");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    qrImagePtath = $"{qrImagePtath}/{deliveryAgent.Id}";
                    path = Server.MapPath($"~{qrImagePtath}");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    Guid newguid = Guid.NewGuid();
                    string fname = file.FileName;
                    string extention = fname.Substring(fname.LastIndexOf("."));
                    qrImagePtath = $"{qrImagePtath}/{Convert.ToString(deliveryAgent.Id)}-{newguid.ToString()}{extention}";
                    path = Server.MapPath($"~{qrImagePtath}");
                    file.SaveAs(path);

                    deliveryAgent.DocPath = qrImagePtath;
                    db.SaveChanges();
                }
                return RedirectToAction("List");
            }

            ViewBag.DeliverySlotID = new SelectList(db.DeliverySlots, "DeliverySlotID", "DeliverySlotID", deliveryAgent.DeliverySlotID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", deliveryAgent.ShopID);
           // ViewBag.DeliveryAgentTypeId = new SelectList(db.DeliveryAgentTypes, "DeliveryAgentTypeId", "Descriptions");   // ShiptName
            ViewBag.WeekOffs = new SelectList(db.WeekDays, "Id", "DayName");

            var data = (from s in db.DeliveryAgentTypes
                        select new { DeliveryAgentTypeId = s.DeliveryAgentTypeId, Descriptions = s.Descriptions + "_" + s.ShiptName }).ToList();
            ViewBag.DeliveryAgentTypeId = new SelectList(data, "DeliveryAgentTypeId", "Descriptions");

            return View(deliveryAgent);
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryAgent deliveryAgent = db.DeliveryAgents.Find(id);
            if (deliveryAgent == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeliverySlotID = new SelectList(db.DeliverySlots, "DeliverySlotID", "DeliverySlotID", deliveryAgent.DeliverySlotID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", deliveryAgent.ShopID);
            return View(deliveryAgent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DeliveryExecName,ShopID,LastDeliveryOn,Contact,Address,TravelMode,Coverage,DeliverySlotID,IsAtShop,ExciseID,DocPath")] DeliveryAgent deliveryAgent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deliveryAgent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeliverySlotID = new SelectList(db.DeliverySlots, "DeliverySlotID", "DeliverySlotID", deliveryAgent.DeliverySlotID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", deliveryAgent.ShopID);
            return View(deliveryAgent);
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryAgent deliveryAgent = db.DeliveryAgents.Find(id);
            if (deliveryAgent == null)
            {
                return HttpNotFound();
            }
            return View(deliveryAgent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DeliveryAgent deliveryAgent = db.DeliveryAgents.Find(id);
            db.DeliveryAgents.Remove(deliveryAgent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpGet]
        [AuthorizeSpirit(Roles = "Deliver")]
        public ActionResult OnOffDuty(int duty)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            if (duty == 0)
            {
                DeliveryAgentLogin deliveryAgentLogin = new DeliveryAgentLogin
                {
                    DeliveryAgentId = user.DeliveryAgentId ?? 0,
                    OnDuty = DateTime.Now
                };
                db.DeliveryAgentLogins.Add(deliveryAgentLogin);
                db.SaveChanges();
            }
            else if (duty == 1)
            {
                var deliveryUser = db.DeliveryAgentLogins.Where(o => o.DeliveryAgentId == user.DeliveryAgentId)?.OrderByDescending(o => o.Id).FirstOrDefault();
                deliveryUser.OffDuty = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        [HttpGet]
        public ActionResult List(int shop = 0, string shopname = "", int agent = 0, string agentname = "")
        {
            IList<DeliveryAgent> deliveryAgents = new List<DeliveryAgent>();
            bool isSearch = false;
            if (shop > 0)
            {
                deliveryAgents = db.DeliveryAgents.Include(d => d.WineShop).Include(o=>o.DeliveryAgentLogins).Include(d => d.DeliveryZone)
                    .Where(o => o.ShopID == shop)
                    .OrderByDescending(o => o.Id)
                        .Take(1000).ToList();
                isSearch = true;
            }
            if (agent > 0)
            {
                if (deliveryAgents.Count > 0) deliveryAgents = deliveryAgents.Where(o => o.Id == agent).ToList();
                else deliveryAgents = db.DeliveryAgents.Include(d => d.WineShop).Include(d => d.DeliveryZone)
                         .Where(o => o.Id == agent)
                        .OrderByDescending(o => o.Id)
                        .Take(1000).ToList();
                isSearch = true;
            }
            if (!isSearch)
            {
                //  deliveryAgents = db.DeliveryAgents.Include(d => d.WineShop).Include(d => d.DeliveryZone).ToList();
                deliveryAgents = db.DeliveryAgents.Include(d => d.WineShop).Include(o => o.DeliveryAgentLogins).Include(d => d.DeliveryZone)
                   .Where(o => o.ShopID != 17)
                    .OrderByDescending(o => o.Id)
                        .Take(10000).ToList();

            }
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            ViewBag.TrackDelAgent = c.GetConfigValue(ConfigEnums.TrackDelAgent.ToString());
            ViewBag.TrackAllDelAgent = c.GetConfigValue(ConfigEnums.TrackAllDelAgent.ToString());
            var shops = db.WineShops.ToList();
            var agents = db.DeliveryAgents.ToList();
            ViewBag.ShopName = shopname;
            ViewBag.AgentName = agentname;
            ViewBag.ShopId = new SelectList(shops, "Id", "ShopName", shop);
            ViewBag.AgentId = new SelectList(agents, "Id", "DeliveryExecName", agent);

            return View(deliveryAgents);
        }

        [HttpPost]
        public ActionResult UpdateDelAgentStatus(DeliveryAgent deliveryAgent)
        {
            if (deliveryAgent.Id <= 0)
            {
                return Json(new { status=false, msg="Delivery agent Id should be greater than 0." }, JsonRequestBehavior.AllowGet);
            }
            var del = db.DeliveryAgents.Find(deliveryAgent.Id);
            del.isActive = (del.isActive == 0) ? 1 : 0;
            db.SaveChanges();
            return Json(new { status=true, msg="Status update successfully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult ListEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryAgent deliveryAgent = db.DeliveryAgents.Include(d => d.WineShop).Include(d => d.DeliveryZone).Where(o => o.Id == id)?.FirstOrDefault();
            if (deliveryAgent == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeliverySlotID = new SelectList(db.DeliverySlots, "DeliverySlotID", "DeliverySlotID", deliveryAgent.DeliverySlotID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", deliveryAgent.ShopID);
           // ViewBag.DeliveryAgentTypeId = new SelectList(db.DeliveryAgentTypes, "DeliveryAgentTypeId", "Descriptions", deliveryAgent.DeliveryAgentTypeId);   // ShiptName
            ViewBag.WeekOffs = new SelectList(db.WeekDays, "Id", "DayName", deliveryAgent.WeekOffs);

            var data = (from s in db.DeliveryAgentTypes
                        select new { DeliveryAgentTypeId = s.DeliveryAgentTypeId, Descriptions = s.Descriptions + "_" + s.ShiptName }).ToList();
            ViewBag.DeliveryAgentTypeId = new SelectList(data, "DeliveryAgentTypeId", "Descriptions");

            return View(deliveryAgent);
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListEdit([Bind(Include = "Id,DeliveryExecName,ShopID,LastDeliveryOn,Contact,Address,TravelMode,HTrackDisallow,Coverage,DeliverySlotID,IsAtShop,ExciseID,DocPath,WeekOffs,DeliveryAgentTypeId")] DeliveryAgent deliveryAgent,string [] WeekOffs)
        {
            string result = string.Empty;
            if (WeekOffs != null)
            {
                foreach (var entry in WeekOffs)
                {
                    result += entry + ',';
                }
            }
            deliveryAgent.WeekOffs = result;

            if (ModelState.IsValid)
            {
                //DeliveryAgent dAgent = db.DeliveryAgents.Find(deliveryAgent.Id);
                //dAgent.ShopID = deliveryAgent.ShopID;
                //db.SaveChanges();
                db.Entry(deliveryAgent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.DeliverySlotID = new SelectList(db.DeliverySlots, "DeliverySlotID", "DeliverySlotID", deliveryAgent.DeliverySlotID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", deliveryAgent.ShopID);
           // ViewBag.DeliveryAgentTypeId = new SelectList(db.DeliveryAgentTypes, "DeliveryAgentTypeId", "Descriptions", deliveryAgent.DeliveryAgentTypeId);   // ShiptName
            ViewBag.WeekOffs = new SelectList(db.WeekDays, "Id", "DayName", deliveryAgent.WeekOffs);
            var data = (from s in db.DeliveryAgentTypes
                        select new { DeliveryAgentTypeId = s.DeliveryAgentTypeId, Descriptions = s.Descriptions + "_" + s.ShiptName }).ToList();
            ViewBag.DeliveryAgentTypeId = new SelectList(data, "DeliveryAgentTypeId", "Descriptions");

            return View(deliveryAgent);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult ListDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //DeliveryAgent deliveryAgent = db.DeliveryAgents.Include(d => d.WineShop).Include(d => d.WineShop.DeliveryZones).Where(o => o.Id == id)?.FirstOrDefault();
            var routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order).Where(o => o.DeliveryAgentId == id && o.Order.OrderStatusId == 6 && !o.Order.TestOrder);
            if (routePlans == null)
            {
                return HttpNotFound();
            }
            //ViewBag.DeliverySlotID = new SelectList(db.DeliverySlots, "DeliverySlotID", "DeliverySlotID", deliveryAgent.DeliverySlotID);
            //ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", deliveryAgent.ShopID);
            return View(routePlans.ToList());
        }
        [HttpPost]
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult SearchZone(int shoId)
        {
            var p = db.DeliveryZones.Where(o => o.ShopID == shoId).Select(o => new { Id = o.ZoneID, Name = o.ZoneName });

            return Json(p, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult Analytics()
        {
                string qlikUrl = default(string);
            using (HttpClient client = new HttpClient())
            {
                string outTicket = default(string);
                HttpResponseMessage response = client.GetAsync(ConfigurationManager.AppSettings["QAuth"]).Result;
                if (response.IsSuccessStatusCode)
                {
                    outTicket = response.Content.ReadAsStringAsync().Result;

                    outTicket = outTicket.Replace(@"\""", "");
                    qlikUrl += outTicket;
                }
                if (!outTicket.Contains("qlikTicket="))
                {
                    qlikUrl = default(string);
                }
            }
            ViewBag.QlikUrl = $"{ConfigurationManager.AppSettings["QContent"]}?{qlikUrl}";
            return View();
        }
        [HttpGet]
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult AnalyticsLoad()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult Reassign(int shop = 0, string shopname = "", int agent = 0, string agentname = "", int soid = 0)
        {
            IQueryable<RoutePlan> routePlans = null;
            bool isSearch = false;
            if (shop > 0)
            {
                routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order)
                    .Where(o => o.ShopID == shop && (((o.Order.OrderStatusId == 2) && (o.Order.PaymentTypeId == 2 || o.Order.PaymentTypeId == 5)) || ((o.Order.OrderStatusId == 3 || o.Order.OrderStatusId == 6 ) && (o.Order.PaymentTypeId == 2 || o.Order.PaymentTypeId == 5 || o.Order.PaymentTypeId == 1)))  && !o.Order.TestOrder);
                isSearch = true;
            }
            if (agent > 0)
            {
                if (routePlans != null && routePlans.Count() > 0) routePlans = routePlans.Where(o => o.DeliveryAgentId == agent);
                else routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order)
                         .Where(o => o.DeliveryAgentId == agent && (((o.Order.OrderStatusId == 2) && (o.Order.PaymentTypeId == 2 || o.Order.PaymentTypeId == 5)) || ((o.Order.OrderStatusId == 3 || o.Order.OrderStatusId == 6) && (o.Order.PaymentTypeId == 2 || o.Order.PaymentTypeId == 5 || o.Order.PaymentTypeId == 1))) && !o.Order.TestOrder);
                isSearch = true;
            }
            if (soid > 0)
            {
                if (routePlans != null && routePlans.Count() > 0) routePlans = routePlans.Where(o => o.OrderID == soid);
                else routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order).Where(o => o.OrderID == soid && (((o.Order.OrderStatusId == 2) && (o.Order.PaymentTypeId == 2 || o.Order.PaymentTypeId == 5)) || ((o.Order.OrderStatusId == 3 || o.Order.OrderStatusId == 6) && (o.Order.PaymentTypeId == 2 || o.Order.PaymentTypeId == 5 || o.Order.PaymentTypeId == 1))) && !o.Order.TestOrder);


                // else routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order).Where(o => o.OrderID == soid && (o.Order.OrderStatusId == 3 || o.Order.OrderStatusId == 6) && !o.Order.TestOrder);

                isSearch = true;
            }
            if (!isSearch)

                //routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order).Where(o=>(o.Order.OrderStatusId == 3 || o.Order.OrderStatusId == 6) && !o.Order.TestOrder).Take(200); 


                // routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order).Where(o=>(o.Order.OrderStatusId == 3 || o.Order.OrderStatusId == 6) || (o.Order.OrderStatusId == 2 && o.Order.PaymentTypeId==2 || o.Order.PaymentTypeId == 5) && !o.Order.TestOrder).Take(2000); 

                routePlans = db.RoutePlans.Include(o => o.DeliveryAgent).Include(o => o.Order).Where(o => ((o.Order.OrderStatusId == 3 || o.Order.OrderStatusId == 6 ) && (o.Order.PaymentTypeId == 2 || o.Order.PaymentTypeId == 5 || o.Order.PaymentTypeId == 1)) || ((o.Order.OrderStatusId==2) &&(o.Order.PaymentTypeId == 2|| o.Order.PaymentTypeId == 5)) && !o.Order.TestOrder).Take(2000);


            if (routePlans == null)
            {
                return HttpNotFound();
            }
            var shops = db.WineShops.ToList();
            var agents = db.DeliveryAgents.ToList();
            ViewBag.ShopName = shopname;
            ViewBag.AgentName = agentname;
            ViewBag.soid = soid;
            ViewBag.ShopId = new SelectList(shops, "Id", "ShopName", shop);
            ViewBag.AgentId = new SelectList(agents, "Id", "DeliveryExecName", agent);

            var routePlanViewModel = routePlans.Select(o => new RoutePlanViewModel
            {
                id = o.id,
                AssignedDate = o.AssignedDate,
                Customer = o.Customer,
                DeliveryAgent = o.DeliveryAgent,
                DeliveryAgentId = o.DeliveryAgentId,
                Order = o.Order,
                OrderID = o.OrderID,
                OrderStatu = o.OrderStatu,
                OrderStatusId = o.OrderStatusId,
                ShopID = o.ShopID,
                WineShop = o.WineShop,
                //Description=o.OrderStatu.Description,
                JobId=o.JobId
            }).ToList().OrderBy(x =>x.AssignedDate);
            routePlanViewModel.ForEach((o) =>
            {
                var address = db.CustomerAddresses.Find(o.Order.CustomerAddressId);
                if (address != null)
                {
                    o.Customer.Address = address.Address;
                    o.Customer.Landmark = address.Landmark;
                    o.Customer.Flat = address.Flat;
                    o.Customer.PlaceId = address.PlaceId;
                    o.Customer.FormattedAddress = address.FormattedAddress;
                }
                var status = db.OrderStatus.Find(o.Order.OrderStatusId);
                if (status !=null)
                {
                    o.Description = status.Description;
                }
                var ca = db.CustomerAddresses.Find(o.Order.CustomerAddressId);
                if (status != null)
                {
                    o.Address = ca.Address;
                }
                SelectList objsection = new SelectList(agents, "Id", "DeliveryExecName", o.DeliveryAgentId);
                o.SectionAgent = objsection;
                SelectList objshopsection = new SelectList(shops, "Id", "ShopName", o.ShopID);
                o.SectionShop = objshopsection;
            });

            return View(routePlanViewModel);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult ReassignUpdate(int plan, int agent, int shop)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            RoutePlan routePlan = db.RoutePlans.Find(plan);
            var jobId = routePlan.JobId;;
            var existDeliveryAgentId = routePlan.DeliveryAgentId;
            var routePlan1 = db.RoutePlans.Where(o=>o.JobId==jobId && o.DeliveryAgentId== existDeliveryAgentId).ToList();
            var delJob = db.DeliveryJobs.Where(o => o.JobId == jobId && o.DeliveryAgentId == existDeliveryAgentId)?.FirstOrDefault();
            var delJobNew = db.DeliveryJobs.Where(o => o.JobId == jobId && o.DeliveryAgentId == agent)?.FirstOrDefault();
            if (agent != existDeliveryAgentId)
            {
                routePlan1.ForEach((o) =>
                {
                    o.ShopID = shop;
                    o.DeliveryAgentId = agent;
                    o.IsPickedUp = false;
                    o.PickedUpDate = null;
                });

            }
            else
            {
                routePlan1.ForEach((o) =>
                            {
                                o.ShopID = shop;
                                o.DeliveryAgentId = agent;
                            });
            }
            db.SaveChanges();
            
            if (delJob!=null && delJobNew==null)
            {
                delJob.DeliveryAgentId = agent;
                db.SaveChanges();
            }
            db.OrderTracks.Add(new OrderTrack { OrderId = routePlan.OrderID, StatusId = 24, TrackDate = DateTime.Now, LogUserName = u.Email, LogUserId = u.Id });
            db.DeliveryAgentTracks.Add(new DeliveryAgentTrack { OrderId = routePlan.OrderID, DeliveryAgentId = agent, StatusId = 24, TrackDate = DateTime.Now });
            db.SaveChanges();
            var ord = db.Orders.Where(a => a.Id == routePlan.OrderID).FirstOrDefault();
            if (agent != existDeliveryAgentId)
            {

                var delAgent = db.DeliveryAgents.Where(x => x.Id == agent).FirstOrDefault();
                if (delAgent != null && ord.OrderStatusId == 9 && (delAgent.DeliveryAgentTypeId != 1 && delAgent.DeliveryAgentTypeId != 3))
                {
                    FireStoreAccess fireStoreAccess = new FireStoreAccess();
                    fireStoreAccess.UpdateFireStore(routePlan.OrderID, delAgent.Contact);
                    db.OrderTracks.Add(new OrderTrack { OrderId = routePlan.OrderID, StatusId = 6, TrackDate = DateTime.Now, LogUserName = u.Email, LogUserId = u.Id });
                    db.OrderTracks.Add(new OrderTrack { OrderId = routePlan.OrderID, StatusId = 14, TrackDate = DateTime.Now, LogUserName = u.Email, LogUserId = u.Id });
                    ord.OrderStatusId = 6;
                    db.SaveChanges();
                }
                else
                {
                    FireStoreAccess fireStoreAccess = new FireStoreAccess();
                    fireStoreAccess.UpdateFireStore(routePlan.OrderID, delAgent.Contact);
                    db.OrderTracks.Add(new OrderTrack { OrderId = routePlan.OrderID, StatusId = 14, TrackDate = DateTime.Now, LogUserName = u.Email, LogUserId = u.Id });
                    db.SaveChanges();

                }
            }
            else
            {
                var delAgent = db.DeliveryAgents.Where(x => x.Id == agent).FirstOrDefault();
                if (delAgent != null && (delAgent.DeliveryAgentTypeId != 1 && delAgent.DeliveryAgentTypeId != 3))
                {
                    FireStoreAccess fireStoreAccess = new FireStoreAccess();
                    fireStoreAccess.UpdateFireStore(routePlan.OrderID, delAgent.Contact);
                    db.OrderTracks.Add(new OrderTrack { OrderId = routePlan.OrderID, StatusId = 14, TrackDate = DateTime.Now, LogUserName = u.Email, LogUserId = u.Id });
                    db.SaveChanges();
                }
            }
            routePlan = db.RoutePlans.Include(o => o.WineShop).Include(o => o.DeliveryAgent).Where(o => o.id == plan)?.FirstOrDefault();
            return Json(new { agent= routePlan.DeliveryAgentId, shop=routePlan.ShopID, agentname = routePlan.DeliveryAgent.DeliveryExecName, shopname = routePlan.WineShop.ShopName }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult DelAgentTrack(int shopId = 0, string shopname = "", int soid = 0, string custno = "", int isId = 0, int statusId = 0, string statusname = "")
        {
            IList<RoutePlanDO> routePlans = null;

            if (isId > 0)
            {
                shopId = 0;
                custno = "";
                shopname = "";
                soid = 0;
            }

            var uId = User.Identity.GetUserId();
            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            routePlans = routePlanDBO.DeliveryManagerTrackAgent(uId, shopId, soid, isId);
            if (statusId > 0)
            {
                routePlans = routePlans.Where(o => o.Order.OrderStatusId == statusId).ToList();
            }
            routePlans.ForEach(o =>
            {
                var track = db.OrderTracks.Include(i => i.OrderStatu).Where(j => j.OrderId == o.OrderID);
                var orderETA = db.CustomerEtas.Where(i => i.CustomerId == o.Customer.Id)?.OrderByDescending(j => j.Id).FirstOrDefault();

                //var orderApprovedStatu = track.Where(k => k.StatusId == 3)?.FirstOrDefault();
                var orderOutDeliveryStatu = track.OrderBy(k => k.OrderTrackId).Take(1).FirstOrDefault();

                o.OutForDelivery = (orderOutDeliveryStatu != null && orderOutDeliveryStatu.StatusId == 9);
                //o.ApprovedDate = (orderApprovedStatu != null) ? orderApprovedStatu.TrackDate : (DateTime?)null;

                o.CustomerETA = (orderETA != null) ? orderETA.Eta : "";
                o.DelTrackUrl = string.Format(ConfigurationManager.AppSettings["DelTrackUrl"], o.OrderID);
            });
            //var sh = db.WineShops.Join(db.DelManageShops,
            //    o => o.Id, j => j.ShopID, (o, j) => new { o, j }).Where(o => o.j.rUserId == uId).ToList();
            if (shopId > 0)
                routePlans = routePlans.Where(o => o.ShopID == shopId).ToList();
            if (soid > 0)
                routePlans = routePlans.Where(o => o.OrderID == soid).ToList();
            if (!string.IsNullOrWhiteSpace(custno))
                routePlans = routePlans.Where(o => o.Customer.ContactNo == custno).ToList();

            var shops = db.WineShops.ToList();
            var status = db.OrderStatus.ToList();
            ViewBag.ShopId = new SelectList(shops, "Id", "ShopName", shopId);

            ViewBag.DelTrackUrl = string.Format(ConfigurationManager.AppSettings["DelTrackUrl"], "");
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", statusId);
            ViewBag.ShopName = shopname;
            ViewBag.soid = soid;
            ViewBag.CustNo = custno;
            ViewBag.isid = isId;
            ViewBag.StatusName = statusname;
            return View(routePlans);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        public ActionResult DeleteFromRoutePlan(int plan)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            RoutePlan routePlan = db.RoutePlans.Find(plan);
            db.RoutePlans.Remove(routePlan);
            db.SaveChanges();

            routePlan = db.RoutePlans.Include(o => o.WineShop).Include(o => o.DeliveryAgent).Where(o => o.id == plan)?.FirstOrDefault();
            return Json(new { agent = 0, shop = 0, agentname = "", shopname = "" }, JsonRequestBehavior.AllowGet);
        }
        #region Delivery Agent Penalty
        [AuthorizeSpirit(Roles = "DeliveryManager")]
        [HttpGet]
        public ActionResult agentwisepenalty(PenaltyDetails_Sel_Result PenaltyDetails)
        {

            // var query = db.PenaltyDetails_Agent_Month().ToList();
            var query = db.PenaltyDetails_Sel().ToList();

            return View(query);
        }


        [AuthorizeSpirit(Roles = "DeliveryManager")]
        [HttpGet]
        public ActionResult agentwisepenaltyDetail(string DeliveryAgentID,string DeliveryExecName, string ShopName,string month)
        {

            #region month ddl
            List<SelectListItem> month_list = new List<SelectListItem>() {
        new SelectListItem {
            Text = "Jan", Value = "1"
        },
        new SelectListItem {
            Text = "Feb", Value = "2"
        },
        new SelectListItem {
            Text = "Mar", Value = "3"
        },
        new SelectListItem {
            Text = "Apr", Value = "4"
        },
        new SelectListItem {
            Text = "May", Value = "5"
        },
        new SelectListItem {
            Text = "Jun", Value = "6"
        },
        new SelectListItem {
            Text = "Jul", Value = "7"
        },
        new SelectListItem {
            Text = "Aug", Value = "8"
        },
        new SelectListItem {
            Text = "Sep", Value = "9"
        },
        new SelectListItem {
            Text = "Oct", Value = "10"
        },
         new SelectListItem {
            Text = "Nov", Value = "11"
        },
          new SelectListItem {
            Text = "Dec", Value = "12"
        },
    };


            ViewBag.month_list = month_list;


            if (month == null)
            {
                month = DateTime.Now.ToString("MM");
                Session["DeliveryAgentID"] = DeliveryAgentID;
                Session["DeliveryExecName"] = DeliveryExecName;
                Session["ShopName"] = ShopName;
            }

            else
            {
                DeliveryAgentID = Session["DeliveryAgentID"].ToString();
                DeliveryExecName = Session["DeliveryExecName"].ToString();
                ShopName = Session["ShopName"].ToString();

            }


            #endregion



            SqlParameter[] Parameters =
         {
            //new SqlParameter("@AgentId", 180),
           // new SqlParameter("@CurrMonth", 3)
            new SqlParameter("@AgentId", DeliveryAgentID),
            new SqlParameter("@CurrMonth", month)
        };

            ViewBag.DeliveryExecName = DeliveryExecName;
            ViewBag.ShopName = ShopName;

            var data = db.Database.SqlQuery<PenaltyDetails_Agent_Month_Result>("PenaltyDetails_Agent_Month @AgentId, @CurrMonth", Parameters).ToList();
                      

            int PenaltyAmountRad = 0;
            int PenaltyAmountGreen = 0;


            foreach (var amount in data)
            {
                if(amount.IsPenalty==1)
                {
                    
                    PenaltyAmountRad += Convert.ToInt32((amount.PenaltyAmount));
                }

                if(amount.IsPenalty==0)
                {
                    PenaltyAmountGreen += Convert.ToInt32((amount.PenaltyAmount));
                }
            }

            ViewBag.PenaltyAmount = PenaltyAmountGreen - PenaltyAmountRad;

            return View(data);
        }

        public ActionResult TrackDelAgent(int id)
        {
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var url = c.GetConfigValue(ConfigEnums.TrackDelAgent.ToString());
            ViewBag.TrackDelAgent = url.Replace("deliveryAgentId",id.ToString());
            return View();
        }
        public  ActionResult TrackAllAgents()
        {
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            ViewBag.TrackAllDelAgent = c.GetConfigValue(ConfigEnums.TrackAllDelAgent.ToString());
            return View();
        }

        #endregion









    }
}
