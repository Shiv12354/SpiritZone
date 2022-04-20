using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RainbowWine.Data;
using Microsoft.AspNet.Identity.Owin;
using RainbowWine.Models;
using System.Configuration;
using RainbowWine.Services.Filters;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using RainbowWine.Services;
using RainbowWine.Services.PaytmService;
using RainbowWine.Services.Gateway;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using System.Net.Http;
using static RainbowWine.Services.FireStoreService;
using Google.Cloud.Firestore;
using RainbowWine.Providers;
using System.Threading.Tasks;
using SZInfrastructure;
using RainbowWine.Services.OnlinePaymentService;
using System.IO;
using System.Text.RegularExpressions;

namespace RainbowWine.Controllers
{
    //[AuthorizeSpirit(Roles = "Agent, Shopper, Deliver, Packer, DeliveryManager, Hub")]
    [AuthorizeSpirit]
    [StopAction]
    public class CustomerOrderController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public CustomerOrderController()
        {
        }

        public CustomerOrderController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: CustomerOrder
        public ActionResult Index()
        {
            return RedirectToAction("Create");
            // return View(db.Customers.ToList());
        }

        // GET: CustomerOrder/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }
        private void testpayment()
        {
            PaymentStrategy pay = new PaymentStrategy(new TransactionCashFree());
            
            var paym = pay.MakePayment(new CashFreeModel
            {
                OrderId = "25142",
                OrderAmount = "3000",
                CustomerEmail = "abdul.m@quosphere.com",
                CustomerPhone = "7021620737"
            });
        }
        [AuthorizeSpirit(Roles = "Agent, Shopper, SalesManager")]
        public ActionResult Create()
        {

            var u = db.AspNetUsers.Where(o => o.Email ==User.Identity.Name).FirstOrDefault();
            if (u != null)
            {
                var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
                if (user != null)
                   if((string.Compare(user.UserType1.UserTypeName,"agent",true)!=0) && (string.Compare(user.UserType1.UserTypeName, "SalesManager", true) != 0))
                        return RedirectToAction("Index", "Orders");
            }

            ViewBag.custid = 0;
            ViewBag.msg = "";
            return View();
        }
        public ActionResult Search(string text)
        {
            CustomerDBO customerDBO = new CustomerDBO();
            List<CustomerDO> customer = customerDBO.SearchPhone(text);
            //if (customer.Count() ==0)
            //    customer = db.Customers.Where(o => o.ContactNo.Contains(text)).ToList();
            return Json(customer, JsonRequestBehavior.AllowGet);

            //var customer = db.Customers.Include(o => o.CustomerAddresses).Where(o => o.ContactNo.Contains(text)).Select(o => new { label = o.ContactNo, value = o.ContactNo }).ToList();
            //return Json(customer, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetExsitCust(string contactNo)
        {
            var customer = db.Customers.ToList();
            var customer1 = customer.Where(o => string.Compare(o.ContactNo,contactNo,true)==0);
            var cut = customer1.Select(o=>new { o.Id,o.CustomerName,o.ContactNo,o.Address,o.PlaceId,o.Flat,o.Landmark,o.FormattedAddress })?.FirstOrDefault();
            return Json(cut, JsonRequestBehavior.AllowGet);
        }

        // POST: CustomerOrder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSpirit(Roles = "Agent, Shopper, SalesManager")]
        public ActionResult Create(CustomerViewModel customerview, int customerId = 0, int txtShopId = 0, string txtPlaceId="", int txtZoneId = 0)
        {
            string msg = default(string);
            if (ModelState.IsValid)
            {
                ViewBag.custid = customerId;
                if (txtShopId > 0 && !string.IsNullOrEmpty(txtPlaceId))
                {
                    CustomerAddress address = null;
                    Customer customer = new Customer();
                    if (customerId <= 0)
                    {
                        customer = db.Customers.Where(o => o.ContactNo == customerview.ContactNo
                        && string.Compare(o.RegisterSource, "w", true) == 0).FirstOrDefault();
                        if (customer == null)
                        {
                            customer = new Customer
                            {
                                ContactNo = customerview.ContactNo,
                                CustomerName = customerview.CustomerName,
                                Id = customerview.Id,
                                RegisterSource="w"
                                //Address=customerview.Address,
                                //Flat=customerview.Flat,
                                //Landmark=customerview.Landmark,
                                //PlaceId = txtPlaceId,
                                //FormattedAddress=customerview.FormattedAddress,
                                //ShopId=txtShopId
                            };
                            db.Customers.Add(customer);
                            db.SaveChanges();
                            address = new CustomerAddress
                            {
                                CustomerId = customer.Id,
                                Address = customerview.Address,
                                Flat = customerview.Flat,
                                Landmark = customerview.Landmark,
                                PlaceId = txtPlaceId,
                                FormattedAddress = customerview.FormattedAddress,
                                ShopId = txtShopId,
                                Latitude = customerview.Latitude,
                                Longitude = customerview.Longitude,
                                CreatedDate = DateTime.Now,
                                AddressType = "Home",
                                ZoneId = txtZoneId
                            };
                            db.CustomerAddresses.Add(address);
                            db.SaveChanges();
                        }
                        else
                        {
                            msg = "Phone already exists.";
                            customer = null;
                        }
                    }
                    else
                    {
                        customer = db.Customers.Where(o => o.Id == customerId
                        && string.Compare(o.RegisterSource, "w", true) == 0).FirstOrDefault();
                        address = db.CustomerAddresses.Where(o => o.CustomerId == customer.Id).FirstOrDefault();
                        if (address == null)
                        {
                            address = new CustomerAddress
                            {
                                CustomerId = customer.Id,
                                Address = customerview.Address,
                                Flat = customerview.Flat,
                                Landmark = customerview.Landmark,
                                PlaceId = txtPlaceId,
                                FormattedAddress = customerview.FormattedAddress,
                                ShopId = txtShopId,
                                Latitude = customerview.Latitude,
                                Longitude = customerview.Longitude,
                                CreatedDate=DateTime.Now,
                                AddressType = "Home",
                                ZoneId= txtZoneId
                            };
                            db.CustomerAddresses.Add(address);
                        }
                        else
                        {
                            address.Address = customerview.Address;
                            address.Flat = customerview.Flat;
                            address.Landmark = customerview.Landmark;
                            address.FormattedAddress = customerview.FormattedAddress;
                            address.PlaceId = txtPlaceId;
                            address.Latitude = customerview.Latitude;
                            address.Longitude = customerview.Longitude;
                            address.ShopId = txtShopId;
                            address.ZoneId = txtZoneId;
                        }
                        db.SaveChanges();
                    }

                    bool testorder = (ConfigurationManager.AppSettings["TestOrder"] == "1") ? true : false;

                    if (customer != null)
                    {
                        var order = new Order
                        {
                            OrderDate = DateTime.Now,
                            CustomerId = customer.Id,
                            CustomerAddressId = address.CustomerAddressId,
                            OrderAmount = 0,
                            OrderPlacedBy = User.Identity.Name,
                            OrderTo = customer.ContactNo,
                            OrderStatusId = 1,
                            ShopID = txtShopId,
                            DeliveryPickup = "Delivery",
                            PaymentDevice = "Android",
                            TestOrder = testorder,
                            ZoneId = txtZoneId,
                            OrderType = "w"
                        };
                        db.Orders.Add(order);
                        db.SaveChanges();

                        var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = User.Identity.Name,
                            LogUserId = u.Id,
                            OrderId = order.Id,
                            StatusId = order.OrderStatusId,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();

                        return RedirectToAction("Details", "Orders", new { id = order.Id });
                    }
                }
                else
                {
                    msg = "There is no shop found against your selected location.";
                }
            }
            ViewBag.msg = msg;
            return View(customerview);
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: CustomerOrder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult Edit([Bind(Include = "Id,CustomerName,ContactNo,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: CustomerOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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
        #region Agent update order

        [HttpGet]
        [AuthorizeSpirit(Roles = "Agent, Shopper")]
        [Route("~/update-order/{id}", Name = "GetUpdateOrderByAgent")]
        public ActionResult UpdateOrderByAgent(int Id)
        {
            var order = db.Orders.Find(Id);

            string[] itype = { "Closed", "Approved" };
            var issueType = db.OrderIssueTypes.Where(o => !itype.Contains(o.IssueTypeName)).ToList();

            var modify = db.OrderModifies.Where(o => o.OrderId == Id).FirstOrDefault();
            var modifyOrder = db.OrderDetailModifies.Include(o => o.ProductDetail).Where(o => o.OrderId == Id).ToList();
            var orderDetails = db.OrderDetails.Include(o => o.ProductDetail).Where(o => o.OrderId == Id).ToList();
            order.OrderDetails = orderDetails;
            bool isPODOder = false;
            int payType = (int)OrderPaymentType.POD;
            if (order.PaymentTypeId != null)
            {
                isPODOder = (order.PaymentTypeId == payType);// ? true : false;
            }
            int modifyId = modify==null ? 0 : modify.Id;
            ViewBag.OrderModifyId = modifyId;
            ViewBag.OrderModify = modifyOrder;
            ViewBag.IsPodOrder = isPODOder;
            ViewBag.IssueType = new SelectList(issueType, "OrderIssueTypeId", "IssueTypeName");
            return View(order);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "Agent, Shopper")]
        [Route("~/order-update-detail", Name = "PostOrderUpdateDetail")]
        public ActionResult OrderUpdateDetail(OrderDetailModify ODetailModify)
        {
            var userId= User.Identity.GetUserId();
            OrderDBO orderDBO = new OrderDBO();
            orderDBO.UpdateOrderDetailModify(ODetailModify.OrderId,userId);

            return Json(new { status = true, msg = "Order marked as modify." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "Agent, Shopper")]
        [Route("~/update-orderdetail", Name = "PostUpdateOrderdetail")]
        public ActionResult UpdateOrderdetail(OrderDetailModify oDetailModify)
        {
            string userId = User.Identity.GetUserId();
            var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            var oDetail = db.OrderDetailModifies.Where(o => o.Id == oDetailModify.Id)?.FirstOrDefault();

            var invent = db.Inventories.Where(o => (o.ProductID == oDetail.ProductID) && (o.ShopID == oDetail.ShopID))?.FirstOrDefault();
            var beforeqty = invent.QtyAvailable;
            if (invent == null)
            {
                return Json(new { status = false, msg = "Product is not available." }, JsonRequestBehavior.AllowGet);
            }
            invent.QtyAvailable += oDetail.ItemQty;
            if (invent.QtyAvailable < oDetailModify.ItemQty)
            {
                return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
            }

            oDetail.ItemQty = oDetailModify.ItemQty;
            db.SaveChanges();
            

            invent.QtyAvailable -= oDetailModify.ItemQty;
            var afterqty = invent.QtyAvailable;
            if(afterqty > beforeqty)
            {
                db.InventoryTracks.Add(new InventoryTrack { ProductID = oDetail.ProductID, ShopID = oDetail.ShopID, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = oDetail.OrderId });
            }
            else
            {
                db.InventoryTracks.Add(new InventoryTrack { ProductID = oDetail.ProductID, ShopID = oDetail.ShopID, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = oDetail.OrderId });

            }

            
            invent.LastModified = System.DateTime.Now;
            invent.LastModifiedBy = aspnetuser.Email;
            db.SaveChanges();
           
            var allDetail = db.OrderDetailModifies.Where(o => o.OrderId == oDetail.OrderId).ToList();

            decimal totalAmt = 0;// Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["PremitValue"]);

            foreach (var item in allDetail)
            {
                var q = (item.ItemQty * item.Price);
                totalAmt += q;
            }

            totalAmt = SpiritUtility.CalculateOverAllDiscount(oDetail.OrderId, totalAmt);

            return Json(new { tAmt = totalAmt }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/orderdetail-add", Name = "PostOrderDetailAdd")]
        public ActionResult OrderDetailAdd(OrderIssueDetail orderIssueDetail)
        {
            string userId = User.Identity.GetUserId();
            var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            var modifyId = orderIssueDetail.OrderIssueId ?? 0;
            var orderId = orderIssueDetail.OrderId ?? 0;
            var prodcutId = orderIssueDetail.ProductId ?? 0;
            var price = orderIssueDetail.Price ?? 0;
            var qty = orderIssueDetail.ItemQty ?? 0;
            var shopId = orderIssueDetail.ShopId ?? 0;

            if (modifyId < 1 || prodcutId < 1 || price < 1
                || qty < 1 || orderId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            var invent = db.Inventories.Where(o => (o.ProductID == prodcutId) && (o.ShopID == shopId))?.FirstOrDefault();
            if (invent == null)
            {
                return Json(new { status = false, msg = "Product is not available." }, JsonRequestBehavior.AllowGet);
            }

            if (invent.QtyAvailable < qty)
            {
                return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
            }
            var oDetailModify = new OrderDetailModify
            {
                OrderModifyId = modifyId,
                OrderId = orderId,
                ProductID = prodcutId,
                Price = price,
                ItemQty = qty,
                ShopID = shopId
            };

            db.OrderDetailModifies.Add(oDetailModify);
            db.SaveChanges();
            var beforeqty = invent.QtyAvailable;
            invent.QtyAvailable -= qty;
            var afterqty = invent.QtyAvailable;

            db.InventoryTracks.Add(new InventoryTrack { ProductID = prodcutId, ShopID = shopId, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = orderId });

            invent.LastModified = System.DateTime.Now;
            invent.LastModifiedBy = aspnetuser.Email;
            db.SaveChanges();

            return Json(new { status = true, msg = "Order item added." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/orderdetail-delete/{id}", Name = "PostOrderDetailDelete")]
        public ActionResult OrderDetailDelete(int id)
        {
            string userId = User.Identity.GetUserId();
            var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            if (id < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var oDetailModify = db.OrderDetailModifies.Find(id);
                int orderid = oDetailModify.OrderId;
                var invent = db.Inventories.Where(o => (o.ProductID == oDetailModify.ProductID) && (o.ShopID == oDetailModify.ShopID))?.FirstOrDefault();
                if (invent == null)
                {
                    return Json(new { status = false, msg = "Product is not available." }, JsonRequestBehavior.AllowGet);
                }
                db.OrderDetailModifies.Remove(oDetailModify);
                db.SaveChanges();
                var beforeqty = invent.QtyAvailable;
                invent.QtyAvailable += oDetailModify.ItemQty;
                var afterqty = invent.QtyAvailable;
                db.InventoryTracks.Add(new InventoryTrack { ProductID = invent.ProductID, ShopID = invent.ShopID, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = orderid });
                invent.LastModified = System.DateTime.Now;
                invent.LastModifiedBy = aspnetuser.Email;
                db.SaveChanges();
               
            }
            catch
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/ordedetail-podorder-save", Name = "PostOrderDetailPodOrderSave")]
        public ActionResult OrderdetailPodOrderSave(OrderModifyStatus orderStatus)
        {
            if (orderStatus.OrderModifyId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
                var modifyTotalAmt = orderIssuePay.GetTotalAmtOfOrderModify(orderStatus.OrderModifyId);
                int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);
                if (modifyTotalAmt < MinTotalAmtOrd)
                {
                    return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
                }

                var oModify = db.OrderModifies.Find(orderStatus.OrderModifyId);
                var order = db.Orders.Find(oModify.OrderId);

                int ostatusSubmitted = (int)OrderStatusEnum.Submitted;
                int issueSettle = (int)IssueType.Settlement;
                int issueClosed = (int)IssueType.Closed;

                //Update lineitems
                //update the issue orderdetails to orderdetail table
                OrderDBO orderDBO = new OrderDBO();
                orderDBO.UpdateOrderModify(oModify.Id, order.Id);
                //update the total order value into order table
                PaymentLinkLogsService paymentUpdate = new PaymentLinkLogsService();
                order.OrderAmount = paymentUpdate.GetTotalAmtOfOrder(order.Id);
                db.SaveChanges();


                //Marked issue as closed
                oModify.OrderType = issueSettle;
                oModify.StatusId = issueClosed;
                db.SaveChanges();

                //Marked order as podcancel
                order.OrderStatusId = ostatusSubmitted;
                //db.SaveChanges();
                UpdateOrderStatus(oModify.OrderId ?? 0, order.OrderStatusId);

                //Marked issueTrack as closed
                orderStatus.StatusId = issueClosed;
                UpdateOrderModifyStatus(orderStatus, oModify);

            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Unable to SAVE the Order Modify." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Order Modify saved succeefully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/orderdetail-ff-update", Name = "PostOrderModifyStatusUpdate")]
        public ActionResult OrderModifyStatusUpdate(OrderModifyStatus orderModifyStatus)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();

            OrderModify modify = null;
            if (orderModifyStatus.OrderModifyId <= 0)
            {
                OrderUpdateDetail(new OrderDetailModify { OrderId = orderModifyStatus.OrderId });
                modify = db.OrderModifies.Where(o => o.OrderId == orderModifyStatus.OrderId).FirstOrDefault();
            }
            else
            {
                modify = db.OrderModifies.Find(orderModifyStatus.OrderModifyId);
            }

            var modifyTotalAmt = orderIssuePay.GetTotalAmtOfOrderModify(orderModifyStatus.OrderModifyId);
            int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);
            var disamt = SpiritUtility.CalculateOverAllDiscount(modify.OrderId ?? 0, modifyTotalAmt);

            var ordPaymentTypes = new[] { IssueType.PartialRefund, IssueType.PartialPay };
            var arryPaymentTypes = ordPaymentTypes.Cast<int>().ToArray();

            if (disamt < MinTotalAmtOrd && arryPaymentTypes.Contains(orderModifyStatus.OrderTypeId??0))
            {
                return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                modify.UpdateAmt = orderModifyStatus.UpdateAmt;
                modify.AdjustAmt = orderModifyStatus.DiffAmt;
                modify.OrderType = orderModifyStatus.OrderTypeId;
                modify.StatusId = orderModifyStatus.OrderTypeId;
                db.SaveChanges();
                orderModifyStatus.StatusId = modify.StatusId??0;
                UpdateOrderModifyStatus(orderModifyStatus, modify);
                int modifyProcessStatus = (int)OrderStatusEnum.OrderModifyProcess;
                UpdateOrderStatus(modify.OrderId ?? 0, modifyProcessStatus);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Error while updating the Issue status." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Updated successfully." }, JsonRequestBehavior.AllowGet);

        }
        
        [HttpPost]
        [Route("~/orderdetail-manager-approve", Name = "PostOrderDetailUpdateByManager")]
        public ActionResult OrderDetailUpdateByManager(OrderModifyStatus orderModifyStatus)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
            OrderModify modify = null;
            if (orderModifyStatus.OrderModifyId <= 0)
            {
                OrderUpdateDetail(new OrderDetailModify { OrderId = orderModifyStatus.OrderId });
                modify = db.OrderModifies.Where(o => o.OrderId == orderModifyStatus.OrderId).FirstOrDefault();
            }
            else
            {
                modify = db.OrderModifies.Find(orderModifyStatus.OrderModifyId);
            }
            var modifyTotalAmt = orderIssuePay.GetTotalAmtOfOrderModify(orderModifyStatus.OrderModifyId);
            int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);
            var disamt = SpiritUtility.CalculateOverAllDiscount(modify.OrderId ?? 0, modifyTotalAmt);

            var ordPaymentTypes = new[] { IssueType.PartialRefund, IssueType.PartialPay };
            var arryPaymentTypes = ordPaymentTypes.Cast<int>().ToArray();

            if (disamt < MinTotalAmtOrd && arryPaymentTypes.Contains(orderModifyStatus.OrderTypeId ?? 0))
            {
                return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
            }


            var order = db.Orders.Find(modify.OrderId);
            try
            {
                int approveStatus = (int)IssueType.Approved;
                int ordapproveStatus = (int)OrderStatusEnum.OrderModifyApproved;
                modify.UpdateAmt = orderModifyStatus.UpdateAmt;
                modify.AdjustAmt = orderModifyStatus.DiffAmt;
                modify.OrderType = orderModifyStatus.OrderTypeId;
                modify.StatusId = approveStatus;
                db.SaveChanges();



                //issue.OrderStatusId = issueStatus.OrderIssueTypeId;
                //db.SaveChanges();

                UpdateOrderModifyStatus(orderModifyStatus, modify);
                UpdateOrderStatus(modify.OrderId ?? 0, ordapproveStatus);



                var payType = new[] { OrderPaymentType.POD, OrderPaymentType.OCOD };
                //var arryRevert = Array.ConvertAll<OrderStatusEnum, int>(ordStatusInventoryRevert, (v) => (int)v);
                var payOnDoor = payType.Cast<int>().ToArray();


                if (order.PaymentTypeId != null)
                {
                    if (payOnDoor.Contains(order.PaymentTypeId ?? 0))
                    {
                        PODOrderModify(order, modify);
                        return Json(new { status = true, msg = $"Changes made for Pay on Delivery Payment Type." }, JsonRequestBehavior.AllowGet);

                    }
                }
                if (modify.OrderType == 2)
                {
                    AggrePaymentController aggePaymentController = new AggrePaymentController();
                    OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
                    var paymentGateWayName = onlinePaymentServiceDBO.GetPaymentGateWayName(order.Id);
                    if (paymentGateWayName.PaymentGateWayName == "AggrePay")
                    {
                        aggePaymentController.PartialPaymentLinkForReSend(order.Id.ToString());
                        return Json(new { status = true, msg = "Resent payment link to customer." }, JsonRequestBehavior.AllowGet);
                    }
                    if (PaymentLinkForExistOrder(Convert.ToString(order.Id), Convert.ToString(modify.Id)))
                    {
                        if(paymentGateWayName.PaymentGateWayName == "CashFree")
                        {
                            SendPaymentLink(order.Id);
                            return Json(new { status = true, msg = "Resent payment link to customer." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var payCash = paymentLinkLogsService.CashFreePaymentOrderModify(modify.Id);

                    return Json(new { status = payCash.Status, msg = payCash.Message }, JsonRequestBehavior.AllowGet);
                    //}
                }
                if (modify.OrderType == 3 || modify.OrderType == 4)
                {
                    int ordRefundInitiateStatus = (int)OrderStatusEnum.OrderModifyRefundInitiated;
                    UpdateOrderStatus(modify.OrderId ?? 0, ordRefundInitiateStatus);

                    //if (string.Compare(order.OrderType, "w", true) == 0)
                    //{
                    //    PaytmPayment pay = new PaytmPayment();
                    //    var resp = pay.PaytmOrderModifyRefundApiCall(modify.Id, true);
                    //    return Json(new { status = true, msg = $"The refund request has been initiated. It’ll be processed within the next 24 hours." }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var refund = paymentLinkLogsService.CashFreeReFundForOrderModify(modify.Id);

                    return Json(new { status = refund.Status, msg = refund.Message }, JsonRequestBehavior.AllowGet);
                    //}
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Error while updating the Order Modify status." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Updated Order Modify successfully." }, JsonRequestBehavior.AllowGet);
        }
        public void PODOrderModify(Order order, OrderModify orderModify)
        {

            var oDetail = db.OrderDetailModifies.Where(o => o.OrderModifyId == orderModify.Id);
            decimal totalAmt = 0;
            foreach (var item in oDetail)
            {
                int q = item.ItemQty;
                decimal p = item.Price;
                if (q > 0 && p > 0)
                {
                    decimal t = q * p;
                    totalAmt += t;
                }
            }

            totalAmt = SpiritUtility.CalculateOverAllDiscount(order.Id, totalAmt);

            OrderDBO orderDBO = new OrderDBO();
            orderDBO.UpdateOrderModify(orderModify.Id, order.Id);

            int stPayFullRefund = (int)IssueType.FullRefund;
            int statusSubmitted = (stPayFullRefund == orderModify.OrderType) ? (int)OrderStatusEnum.IssueRefunded : (int)OrderStatusEnum.Submitted;

            int statusMOdifyClose = (int)IssueType.Closed;
            order.OrderAmount = totalAmt;
            order.OrderStatusId = statusSubmitted;
            db.SaveChanges();

            var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
            string uId = u.Id;

            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = u.Email,
                LogUserId = uId,
                OrderId = orderModify.OrderId ?? 0,
                Remark = "Pay on Delivery Type.",
                StatusId = statusSubmitted,
                TrackDate = DateTime.Now
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();

            OrderModifyTrack orderModifyTrack = new OrderModifyTrack
            {
                UserId = uId,
                OrderId = orderModify.OrderId ?? 0,
                OrderModifyId = orderModify.Id,
                Remark = "Pay on Delivery Type",
                OrderStatusId = statusMOdifyClose,
                TrackDate = DateTime.Now
            };
            db.OrderModifyTracks.Add(orderModifyTrack);
            db.SaveChanges();

        }


        [HttpPost]
        [Route("~/orderdetail-order-close", Name = "OrderDetailClose")]
        public ActionResult IssueOrderClose(OrderModifyStatus orderModifyStatus)
        {
            if (orderModifyStatus.OrderModifyId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var modify = db.OrderModifies.Find(orderModifyStatus.OrderModifyId);
                var order = db.Orders.Find(modify.OrderId);
                int payType = (int)OrderPaymentType.POD;
                int payTypeOCOD = (int)OrderPaymentType.OCOD;
                int ostatusApprove = (int)OrderStatusEnum.Approved;
                int ostatusSubmitted = (int)OrderStatusEnum.Submitted;
                int issueSettle = (int)IssueType.Settlement;
                int issueClosed = (int)IssueType.Closed;
                //Marked issue as closed
                modify.OrderType = issueSettle;
                modify.StatusId = issueClosed;
                db.SaveChanges();
                //Marked issueTrack as cancel
                //issueStatus.OrderIssueTypeId = issueCancel;
                //UpdateIssueStatus(issueStatus, issue);

                //Marked order as podcancel
                order.OrderStatusId = (order.PaymentTypeId == payType || order.PaymentTypeId == payTypeOCOD) ? ostatusSubmitted : ostatusApprove;
                //db.SaveChanges();
                UpdateOrderStatus(modify.OrderId ?? 0, order.OrderStatusId);

                //Marked issueTrack as closed
                orderModifyStatus.OrderTypeId = issueClosed;
                UpdateOrderModifyStatus(orderModifyStatus, modify);

                //RevertInventory(order.Id);
                ////Revert inventory
                //PaymentLinkLogsService revert = new PaymentLinkLogsService();
                //revert.RevertInventory(order.Id);

            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Unable to close the Modify." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Modify closed succeefully." }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region %%%%% Issue Order Containter  %%%%%

        [HttpGet]
        //[AuthorizeSpirit(Roles = "Shopper, OrderFullFillment")]
        [Route("~/issue-track/{id}",Name ="GetIssuesOrdersTrack")]
        public ActionResult IssueOrdersTrack(int Id)
        {
            var issueOrders = db.OrderIssues.Include(o => o.Order).Where(o => o.OrderIssueId == Id)?.FirstOrDefault();
            var issueOrdersTrack = db.OrderIssueTracks.Where(o => o.OrderIssueId == Id);
            var ordersTrack = db.OrderTracks.Where(o => o.OrderId== issueOrders.OrderId).OrderBy(x =>x.TrackDate).ToList();
            ViewBag.OrderTrack = ordersTrack;
            return View(issueOrdersTrack);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper, OrderFullFillment")]
        [Route("~/issue", Name = "GetIssuesOrders")]
        public ActionResult IssueOrders(int statusId = 0, string statusname = "", int soid = 0, int issuestatusId = 0, string issuestatusname = "")
        {
            string custId = User.Identity.GetUserId();
            var rUser = db.RUsers.Where(o => o.rUserId == custId)?.FirstOrDefault();
            List<OrderIssue> issueOrders = new List<OrderIssue>();
            //issueOrders = db.OrderIssues.Include(o => o.Order)?.ToList();
            if (issuestatusId > 0)
            {
                issueOrders = db.OrderIssues.Where(o => o.OrderIssueTypeId == issuestatusId)?.ToList();
            }
            else
            {
                //issueOrders = db.OrderIssues.Where(o => o.OrderIssueTypeId == 1)?.ToList();
                issueOrders = db.OrderIssues.OrderByDescending(o =>o.CreatedDate).Take(30).ToList();
            }
            if (statusId > 0)
            {
                issueOrders = issueOrders.Where(o => o.Order.OrderStatusId == statusId)?.ToList();
            }
            if (soid > 0)
            {
                issueOrders = db.OrderIssues.Include(o => o.Order).Where(o=> o.OrderId == soid)?.ToList();
            }
            
            ViewBag.StatusName = statusname;
            ViewBag.IssueStatusName = issuestatusname;
            ViewBag.soid = soid;

            var status = db.OrderStatus.ToList();
            var issueType = db.OrderIssueTypes.ToList();
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", statusId);
            ViewBag.IssueTypeId = new SelectList(issueType, "OrderIssueTypeId", "IssueTypeName", issuestatusId);

            return View(issueOrders);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper, OrderFullFillment")]
        [Route("~/issuedetail/{id}", Name ="IssueDetail")]
        public ActionResult IssueDetail(int Id)
        {
            string custId = User.Identity.GetUserId();
            var issueOrders = db.OrderIssues.Include(o => o.Order).Where(o => o.OrderIssueId == Id)?.FirstOrDefault();

            string[] itype = { "Closed", "Approved" };
            var issueType = db.OrderIssueTypes.Where(o => !itype.Contains(o.IssueTypeName)).ToList();

            var originalOrder = db.OrderIssueDetailOriginals.Where(o => o.OrderIssueId == Id).ToList();
            var giftOrdItem = db.GiftBagOrderItems.Where(a =>a.OrderId ==issueOrders.OrderId).ToList();
            var giftDtlItem = db.GiftBagOrderItemIssueDetails.Where(a => a.OrderIssueId == Id).ToList();
            var q = (from gid in db.GiftBagOrderItemIssueDetails
                     join gd in db.GiftBagDetails on gid.GiftBagDetailId equals gd.GiftBagDetailId
                     join gm in db.GiftBagMasters on gd.GiftBagMasterId equals gm.GiftBagMasterId
                     join giod in db.GiftBagOrderItemIssueDetailOriginals on gid.OrderIssueId equals giod.OrderIssueId
                     select new
                     {
                         GiftBagName = gm.GiftBagName,
                         OrderIssueId=gid.OrderIssueId,
                         OrdId= giod.OrderId,
                         GiftIssuedtId=gid.GiftBagIssueDetailId,
                         GiftIssueOdtlId=giod.GiftBagDetailOriginalId
                     }).Where(a => a.OrderIssueId == Id).ToList();
            
            var originalMixerOrder = db.MixerIssueDetailOriginals.Where(o => o.OrderIssueId == Id).ToList();

            var originalGiftBagOrder = db.GiftBagOrderItemIssueDetailOriginals.Where(o => o.OrderIssueId == Id).ToList();

            foreach (var item in q)
            {
                if (item.GiftBagName != null)
                {
                    giftDtlItem.Where(a => a.OrderIssueId == item.OrderIssueId && a.GiftBagIssueDetailId ==item.GiftIssuedtId).SingleOrDefault().GiftBagName = item.GiftBagName;
                    originalGiftBagOrder.Where(a => a.OrderIssueId == item.OrderIssueId && a.GiftBagDetailOriginalId == item.GiftIssueOdtlId).SingleOrDefault().GiftBagName = item.GiftBagName;
                }
            }
            issueOrders.Order.GiftBagOrderItems = giftOrdItem;
            issueOrders.GiftBagOrderItemIssueDetails = giftDtlItem;
            bool isPODOder = false;
            bool isChange = false;
            //int payType = (int)OrderPaymentType.POD;
            if (issueOrders.Order.PaymentTypeId != null)
            {
                OrderPaymentType[] payTypearray = new[] { OrderPaymentType.POD, OrderPaymentType.OCOD };
                int[] payTypeValue = payTypearray.Cast<int>().ToArray();

                isPODOder = (payTypeValue.Contains(issueOrders.Order.PaymentTypeId ?? 0)) ? true : false;             
            }

            issueOrders.OrderIssueDetails.ForEach((o) => {
                if (o.OrderDetailId == null)
                {
                    isChange = true;
                }
            });


            IssueType[] issueTypePay = new[] { IssueType.PartialPay, IssueType.PartialRefund };
            int[] issueTypePayValue = issueTypePay.Cast<int>().ToArray();

            string orderIssueTypePay = "";
            if (issueOrders.IsCashOnDelivery ?? false)
            {
                if (issueTypePayValue.Contains(issueOrders.OrderIssueTypeId ?? 0))
                {
                    orderIssueTypePay = ((issueOrders.OrderIssueTypeId ?? 0) == (int)IssueType.PartialPay) ? "Cash Pay" : "Cash Refund";
                }
            }
            if (TempData["MessageCall"] != null)
            {
                ViewBag.MessageCall = TempData["MessageCall"];
            }
            
            OrderDBO orderDBO = new OrderDBO();
            var internalUser = orderDBO.GetInterUserContactNo(custId);
            var allTypeRefund = orderDBO.GetAllTypeRefundDetails(issueOrders.OrderId.Value);
            ViewBag.InterUserContNo = internalUser;
            ViewBag.AllTypeRefund = allTypeRefund;
            ViewBag.OrderIssueTypePay = orderIssueTypePay;
            ViewBag.IsChanges = isChange;
            ViewBag.OrderOriginal = originalOrder;
            ViewBag.MixerOrderOriginal = originalMixerOrder;
            ViewBag.OriginalGiftBagOrder = originalGiftBagOrder;
            ViewBag.IsPodOrder = isPODOder;
            ViewBag.IssueType = new SelectList(issueType, "OrderIssueTypeId", "IssueTypeName", issueOrders.OrderIssueTypeId);
            return View(issueOrders);
        }

        [HttpPost]
        [Route("~/detail-update", Name = "DetailUpdate")]
        public ActionResult DetailUpdate(OrderIssueDetail orderIssueDetail)
        {
            decimal totalAmt = 0;
            var oDetail = db.OrderIssueDetails.Where(o => o.OrderIssueDetailId == orderIssueDetail.OrderIssueDetailId)?.FirstOrDefault();
            var mDetail = db.MixerIssueDetails.Where(o => o.MixerIssueDetailId == orderIssueDetail.MixerIssueDetailId)?.FirstOrDefault();
            var gDetail = db.GiftBagOrderItemIssueDetails.Where(o => o.GiftBagIssueDetailId == orderIssueDetail.GiftBagIssueDetailId)?.FirstOrDefault();
            if (gDetail != null)
            {
                var invent = db.GiftBagInventories.Where(o => (o.GiftBagDetailId == gDetail.GiftBagDetailId) && (o.ShopID == orderIssueDetail.ShopId))?.FirstOrDefault();
                if (invent == null)
                {
                    return Json(new { status = false, msg = "GiftBag is not available." }, JsonRequestBehavior.AllowGet);
                }
                var beforeqty = invent.Qty;
                invent.Qty += gDetail.ItemQty ?? 0;
                if (invent.Qty < orderIssueDetail.ItemQty)
                {
                    return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
                }

                gDetail.ItemQty = orderIssueDetail.ItemQty;
                db.SaveChanges();
                invent.Qty -= orderIssueDetail.ItemQty ?? 0;
                if (gDetail.Issue == true)
                {
                    invent.Qty = beforeqty;
                }
                string userId = User.Identity.GetUserId();
                var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
                var afterqty = invent.Qty;
                if (afterqty > beforeqty)
                {
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = gDetail.GiftBagDetailId.Value, ShopID = orderIssueDetail.ShopId.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty.Value, QtyAvailAfter = afterqty.Value, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = mDetail.OrderId });
                }
                else
                {
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = gDetail.GiftBagDetailId.Value, ShopID = orderIssueDetail.ShopId.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty.Value, QtyAvailAfter = afterqty.Value, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = gDetail.OrderId });
                }
                invent.ModifiedDate = System.DateTime.Now;
                invent.ModifiedBy = aspnetuser.Email;
                db.SaveChanges();

                var issueGifts = db.GiftBagOrderItemIssueDetails.Where(o => o.OrderIssueId == gDetail.OrderIssueId).Select(o => new
                {
                    o.OrderDetailId,
                    o.OrderIssueId,
                    o.OrderId,
                    o.ItemQty,
                    o.Price,
                    o.Issue
                }).ToList();

                // Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["PremitValue"]);

                foreach (var item in issueGifts)
                {
                    var q = (item.ItemQty * item.Price) ?? 0;
                    totalAmt += q;
                }

                totalAmt = SpiritUtility.CalculateOverAllDiscount(gDetail.OrderId ?? 0, totalAmt);

                return Json(new { tAmt = totalAmt, details = issueGifts, isGift = 1 }, JsonRequestBehavior.AllowGet);



            }
           else if (mDetail != null)
            {
                var invent = db.InventoryMixers.Where(o => (o.MixerDetailId == mDetail.MixerDetailId) && (o.ShopId == orderIssueDetail.ShopId))?.FirstOrDefault();
                if (invent == null)
                {
                    return Json(new { status = false, msg = "Mixer is not available." }, JsonRequestBehavior.AllowGet);
                }
                var beforeqty = invent.Qty;
                invent.Qty += mDetail.ItemQty ?? 0;
                if (invent.Qty < orderIssueDetail.ItemQty)
                {
                    return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
                }

                mDetail.ItemQty = orderIssueDetail.ItemQty;
                db.SaveChanges();
                invent.Qty -= orderIssueDetail.ItemQty ?? 0;
                if (mDetail.Issue == true)
                {
                    invent.Qty = beforeqty;
                }
                string userId = User.Identity.GetUserId();
                var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
                var afterqty = invent.Qty;
                if (afterqty > beforeqty)
                {
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = mDetail.MixerDetailId.Value, ShopID = orderIssueDetail.ShopId.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty.Value, QtyAvailAfter = afterqty.Value, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = mDetail.OrderId });
                }
                else
                {
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = mDetail.MixerDetailId.Value, ShopID = orderIssueDetail.ShopId.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty.Value, QtyAvailAfter = afterqty.Value, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = mDetail.OrderId });
                }
                invent.LastModified = System.DateTime.Now;
                invent.LastModifiedBy = aspnetuser.Email;
                db.SaveChanges();
               
               var issueMixers = db.MixerIssueDetails.Where(o => o.OrderIssueId == mDetail.OrderIssueId).Select(o => new
                {
                    o.OrderDetailId,
                    o.OrderIssueId,
                    o.OrderId,
                    o.ItemQty,
                    o.Price,
                    o.Issue
                }).ToList();

                // Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["PremitValue"]);

                foreach (var item in issueMixers)
                {
                    var q = (item.ItemQty * item.Price) ?? 0;
                    totalAmt += q;
                }

                totalAmt = SpiritUtility.CalculateOverAllDiscount(mDetail.OrderId ?? 0, totalAmt);

                return Json(new { tAmt = totalAmt, details = issueMixers , isMixer=1 }, JsonRequestBehavior.AllowGet);


                
            }
            else
            {
                var invent = db.Inventories.Where(o => (o.ProductID == oDetail.ProductId) && (o.ShopID == orderIssueDetail.ShopId))?.FirstOrDefault();
                if (invent == null)
                {
                    return Json(new { status = false, msg = "Product is not available." }, JsonRequestBehavior.AllowGet);
                }
                var beforeqty = invent.QtyAvailable;
                invent.QtyAvailable += oDetail.ItemQty ?? 0;
                if (invent.QtyAvailable < orderIssueDetail.ItemQty)
                {
                    return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
                }

                oDetail.ItemQty = orderIssueDetail.ItemQty;
                db.SaveChanges();
                invent.QtyAvailable -= orderIssueDetail.ItemQty ?? 0;
                if (oDetail.Issue == true)
                {
                    invent.QtyAvailable = beforeqty;
                }
                string userId = User.Identity.GetUserId();
                var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
                var afterqty = invent.QtyAvailable;
                if (afterqty > beforeqty)
                {
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = oDetail.ProductId.Value, ShopID = orderIssueDetail.ShopId.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = oDetail.OrderId });
                }
                else
                {
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = oDetail.ProductId.Value, ShopID = orderIssueDetail.ShopId.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = oDetail.OrderId });
                }
                invent.LastModified = System.DateTime.Now;
                invent.LastModifiedBy = aspnetuser.Email;
                db.SaveChanges();
                
                var issueOrders = db.OrderIssueDetails.Where(o => o.OrderIssueId == oDetail.OrderIssueId).Select(o => new
                {
                    o.OrderDetailId,
                    o.OrderIssueId,
                    o.OrderId,
                    o.ItemQty,
                    o.Price,
                    o.Issue
                }).ToList();

                // Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["PremitValue"]);

                foreach (var item in issueOrders)
                {
                    var q = (item.ItemQty * item.Price) ?? 0;
                    totalAmt += q;
                }

                totalAmt = SpiritUtility.CalculateOverAllDiscount(oDetail.OrderId ?? 0, totalAmt);

                return Json(new { tAmt = totalAmt, details = issueOrders, isMixer = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        

        [HttpPost]
        [Route("~/detail-add", Name = "DetailAdd")]
        public ActionResult DetailAdd(OrderIssueDetail orderIssueDetail)
        {
            string userId = User.Identity.GetUserId();
            var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            var issueiId = orderIssueDetail.OrderIssueId ?? 0;
            var orderId = orderIssueDetail.OrderId ?? 0;
            var prodcutId = orderIssueDetail.ProductId ?? 0;
            var price = orderIssueDetail.Price ?? 0;
            var qty = orderIssueDetail.ItemQty ?? 0;
            var shopId = orderIssueDetail.ShopId ?? 0;

            if (issueiId < 1 || prodcutId < 1 || price < 1
                || qty < 1 || orderId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            var invent = db.Inventories.Where(o => (o.ProductID == prodcutId) && (o.ShopID == shopId))?.FirstOrDefault();
            if (invent == null)
            {
                return Json(new { status = false, msg= "Product is not available." }, JsonRequestBehavior.AllowGet);
            }

            if (invent.QtyAvailable < qty)
            {
                return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
            }
            var issueDetail = new OrderIssueDetail
            {
                OrderIssueId = issueiId,
                OrderId = orderId,
                ProductId = prodcutId,
                Price = price,
                ItemQty = qty,
                ShopId = shopId,
                Issue = false
            };

            db.OrderIssueDetails.Add(issueDetail);
            db.SaveChanges();
            var beforeqty = invent.QtyAvailable;
            invent.QtyAvailable -= qty;
            var afterqty = invent.QtyAvailable;
            db.InventoryTracks.Add(new InventoryTrack { ProductID = prodcutId, ShopID = shopId, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = orderId });
            invent.LastModified = System.DateTime.Now;
            invent.LastModifiedBy = aspnetuser.Email;
            db.SaveChanges();
           
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/detail-mixer-add", Name = "DetailMixerAdd")]
        public ActionResult DetailMixerAdd(OrderIssueDetail orderIssueDetail)
        {
            string userId = User.Identity.GetUserId();
            var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            var issueiId = orderIssueDetail.OrderIssueId ?? 0;
            var orderId = orderIssueDetail.OrderId ?? 0;
            var mixerId = orderIssueDetail.ProductId ?? 0;
            var price = orderIssueDetail.Price ?? 0;
            var qty = orderIssueDetail.ItemQty ?? 0;
            var shopId = orderIssueDetail.ShopId ?? 0;

            if (issueiId < 1 || mixerId < 1 || price < 1
                || qty < 1 || orderId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            var mixerDetail = db.MixerDetails.Where(o => (o.MixerId == mixerId))?.FirstOrDefault();
            var invent = db.InventoryMixers.Where(o => (o.MixerDetailId == mixerDetail.MixerDetailId) && (o.ShopId == shopId))?.FirstOrDefault();
            if (invent == null)
            {
                return Json(new { status = false, msg = "Product is not available." }, JsonRequestBehavior.AllowGet);
            }

            if (invent.Qty < qty)
            {
                return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
            }
            var issueMixerDetail = new MixerIssueDetail
            {
                OrderIssueId = issueiId,
                OrderId = orderId,
                MixerDetailId = mixerDetail.MixerDetailId,
                Price = price,
                ItemQty = qty,
                ShopId = shopId,
                Issue = false
            };

            db.MixerIssueDetails.Add(issueMixerDetail);
            db.SaveChanges();
            var beforeqty = invent.Qty;
            invent.Qty -= qty;
            var afterqty = invent.Qty;
            db.InventoryTracks.Add(new InventoryTrack { ProductID = mixerDetail.MixerDetailId, ShopID = shopId, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty.Value, QtyAvailAfter = afterqty.Value, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = orderId });
            invent.LastModified = System.DateTime.Now;
            invent.LastModifiedBy = aspnetuser.Email;
            db.SaveChanges();

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/detail-gift-add", Name = "DetailGiftBagAdd")]
        public ActionResult DetailGiftBagAdd(OrderIssueDetail orderIssueDetail)
        {
            string userId = User.Identity.GetUserId();
            var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            var issueiId = orderIssueDetail.OrderIssueId ?? 0;
            var orderId = orderIssueDetail.OrderId ?? 0;
            var giftBagDetailId = orderIssueDetail.ProductId ?? 0;
            var price = orderIssueDetail.Price ?? 0;
            var qty = orderIssueDetail.ItemQty ?? 0;
            var shopId = orderIssueDetail.ShopId ?? 0;
            var order = db.Orders.Where(x => x.Id == orderIssueDetail.OrderId).ToList();
            if (issueiId < 1 || giftBagDetailId < 1 || price < 1
                || qty < 1 || orderId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            if (order !=null && order.FirstOrDefault().IsGift == false)
            {
                return Json(new { status = false , msg= "You can only add a bag if the order is a gift order. This order is not a gift order." }, JsonRequestBehavior.AllowGet);
            }
            //var mixerDetail = db.MixerDetails.Where(o => (o.MixerId == mixerId))?.FirstOrDefault();
            GiftBagDBO giftBagDBO = new GiftBagDBO();
            var invent = giftBagDBO.GiftBagSelection(shopId,giftBagDetailId);// db.InventoryMixers.Where(o => (o.MixerDetailId == mixerDetail.MixerDetailId) && (o.ShopId == shopId))?.FirstOrDefault();
            if (invent == null)
            {
                return Json(new { status = false, msg = "GiftBag is not available." }, JsonRequestBehavior.AllowGet);
            }

            if (invent.Qty < qty)
            {
                return Json(new { status = false, msg = "Selected qty should not be greater than available qty." }, JsonRequestBehavior.AllowGet);
            }
            var issueGiftBagOrderItemIssueDetail = new GiftBagOrderItemIssueDetail
            {
                OrderIssueId = issueiId,
                OrderId = orderId,
                GiftBagDetailId = invent.GiftBagDetailId,
                Price = price,
                ItemQty = qty,
                ShopId = shopId,
                Issue = false
            };
            //giftBagDBO.AddGiftBagOrderItemIssueDetail(issueGiftBagOrderItemIssueDetail);
            db.GiftBagOrderItemIssueDetails.Add(issueGiftBagOrderItemIssueDetail);
            db.SaveChanges();
            var beforeqty = invent.Qty;
            invent.Qty -= qty;
            var afterqty = invent.Qty;
            db.InventoryTracks.Add(new InventoryTrack { ProductID = invent.GiftBagDetailId, ShopID = shopId, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = orderId });
            //invent.LastModified = System.DateTime.Now;
            //invent.LastModifiedBy = aspnetuser.Email;
            db.SaveChanges();

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/detail-delete/{id}", Name = "DetailDelete")]
        public ActionResult DetailDelete(int id)
        {
            string userId = User.Identity.GetUserId();
            var aspnetuser = db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            if (id < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var issueDetail = db.OrderIssueDetails.Find(id);
                var mixerIssueDetail = db.MixerIssueDetails.Find(id);
                var giftIssueDetail = db.GiftBagOrderItemIssueDetails.Find(id);
                if (giftIssueDetail != null)
                {
                    int orderid = giftIssueDetail.OrderId.Value;
                    var invent = db.GiftBagInventories.Where(o => (o.GiftBagDetailId == giftIssueDetail.GiftBagDetailId) && (o.ShopID == giftIssueDetail.ShopId))?.FirstOrDefault();
                    if (invent == null)
                    {
                        return Json(new { status = false, msg = "Gift is not available." }, JsonRequestBehavior.AllowGet);
                    }
                    db.GiftBagOrderItemIssueDetails.Remove(giftIssueDetail);
                    db.SaveChanges();
                    var beforeqty = invent.Qty;
                    if (giftIssueDetail.Issue == true)
                    {
                        invent.Qty = beforeqty;

                    }
                    else
                    {
                        invent.Qty += giftIssueDetail.ItemQty ?? 0;

                    }
                    var afterqty = invent.Qty;
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = invent.GiftBagDetailId.Value, ShopID = invent.ShopID.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty.Value, QtyAvailAfter = afterqty.Value, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = orderid });
                    invent.ModifiedDate = System.DateTime.Now;
                    invent.ModifiedBy = aspnetuser.Email;
                    db.SaveChanges();

                }
               else if (mixerIssueDetail != null)
                {
                    int orderid = mixerIssueDetail.OrderId.Value;
                    var invent = db.InventoryMixers.Where(o => (o.MixerDetailId == mixerIssueDetail.MixerDetailId) && (o.ShopId == mixerIssueDetail.ShopId))?.FirstOrDefault();
                    if (invent == null)
                    {
                        return Json(new { status = false, msg = "Mixer is not available." }, JsonRequestBehavior.AllowGet);
                    }
                    db.MixerIssueDetails.Remove(mixerIssueDetail);
                    db.SaveChanges();
                    var beforeqty = invent.Qty;
                    if (mixerIssueDetail.Issue == true)
                    {
                        invent.Qty = beforeqty;

                    }
                    else
                    {
                        invent.Qty += mixerIssueDetail.ItemQty ?? 0;

                    }
                    var afterqty = invent.Qty;
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = invent.MixerDetailId.Value, ShopID = invent.ShopId.Value, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty.Value, QtyAvailAfter = afterqty.Value, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = orderid });
                    invent.LastModified = System.DateTime.Now;
                    invent.LastModifiedBy = aspnetuser.Email;
                    db.SaveChanges();

                }
                
                else
                {
                    int orderid = issueDetail.OrderId.Value;
                    var invent = db.Inventories.Where(o => (o.ProductID == issueDetail.ProductId) && (o.ShopID == issueDetail.ShopId))?.FirstOrDefault();
                    if (invent == null)
                    {
                        return Json(new { status = false, msg = "Product is not available." }, JsonRequestBehavior.AllowGet);
                    }
                    db.OrderIssueDetails.Remove(issueDetail);
                    db.SaveChanges();
                    var beforeqty = invent.QtyAvailable;
                    if (issueDetail.Issue == true)
                    {
                        invent.QtyAvailable = beforeqty;

                    }
                    else
                    {
                        invent.QtyAvailable += issueDetail.ItemQty ?? 0;
                        
                    }
                    var afterqty = invent.QtyAvailable;
                    db.InventoryTracks.Add(new InventoryTrack { ProductID = invent.ProductID, ShopID = invent.ShopID, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 11, Order_Id = orderid });
                    invent.LastModified = System.DateTime.Now;
                    invent.LastModifiedBy = aspnetuser.Email;
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/issue-order-cancel", Name = "IssueOrderCancel")]
        public ActionResult IssueOrderCancel(IssueStatus issueStatus)
        {
            if (issueStatus.OrderIssueId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
                var order = db.Orders.Find(issue.OrderId);
                int podCancel = (int)OrderStatusEnum.PODOrderCancel;
                int issueSettle = (int)IssueType.Settlement;
                int issueCancel = (int)IssueType.Cancel;
                int issueClosed = (int)IssueType.Closed;
                //Marked issue as closed
                issue.OrderIssueTypeId = issueSettle;
                issue.OrderStatusId = issueClosed;
                db.SaveChanges();
                //Marked issueTrack as cancel
                issueStatus.OrderIssueTypeId = issueCancel;
                UpdateIssueStatus(issueStatus, issue);

                //Marked order as podcancel
                order.OrderStatusId = podCancel;
                //db.SaveChanges();
                UpdateOrderStatus(issue.OrderId ?? 0, podCancel);

                //Marked issueTrack as closed
                issueStatus.OrderIssueTypeId = issueClosed;
                UpdateIssueStatus(issueStatus, issue);

                //Revert inventory
                PaymentLinkLogsService revert = new PaymentLinkLogsService();
                revert.RevertInventory(order.Id);
                revert.RevertMixerInventory(order.Id);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg="Unable to cancel the Issue." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg="Issue cancelled succeefully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/issue-order-close", Name = "IssueOrderClose")]
        public ActionResult IssueOrderClose(IssueStatus issueStatus)
        {
            if (issueStatus.OrderIssueId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
                var order = db.Orders.Find(issue.OrderId);
                int payType = (int)OrderPaymentType.POD;
                int payTypeOCOD = (int)OrderPaymentType.OCOD;
                int ostatusApprove = (int)OrderStatusEnum.Approved;
                int ostatusSubmitted = (int)OrderStatusEnum.Submitted;
                int issueSettle = (int)IssueType.Settlement;
                int issueClosed = (int)IssueType.Closed;
                //Marked issue as closed
                issue.OrderIssueTypeId = issueSettle;
                issue.OrderStatusId = issueClosed;
                db.SaveChanges();
                //Marked issueTrack as cancel
                //issueStatus.OrderIssueTypeId = issueCancel;
                //UpdateIssueStatus(issueStatus, issue);

                //Marked order as podcancel
                order.OrderStatusId = (order.PaymentTypeId == payType || order.PaymentTypeId == payTypeOCOD) ? ostatusSubmitted : ostatusApprove;
                //db.SaveChanges();
                UpdateOrderStatus(issue.OrderId ?? 0, order.OrderStatusId);

                //Marked issueTrack as closed
                issueStatus.OrderIssueTypeId = issueClosed;
                UpdateIssueStatus(issueStatus, issue);

                //RevertInventory(order.Id);
                ////Revert inventory
                //PaymentLinkLogsService revert = new PaymentLinkLogsService();
                //revert.RevertInventory(order.Id);

            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Unable to close the Issue." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Issue closed succeefully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/issue-podorder-save", Name = "IssuePodOrderSave")]
        public ActionResult IssuePodOrderSave(IssueStatus issueStatus)
        {
            if (issueStatus.OrderIssueId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
                var issueTotalAmt = orderIssuePay.GetTotalAmtOfIssueOrder(issueStatus.OrderIssueId);
                int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);
                if (issueTotalAmt < MinTotalAmtOrd)
                {
                    return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
                }

                var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
                var order = db.Orders.Find(issue.OrderId); 
                
                OrderPaymentType[] payType = new[] { OrderPaymentType.POD, OrderPaymentType.OCOD };
                int[] payTypeValue = payType.Cast<int>().ToArray();

                int ostatusSubmitted = (payTypeValue.Contains(order.PaymentTypeId ?? 0)) ? (int)OrderStatusEnum.Submitted : (int)OrderStatusEnum.Approved;

                int issueSettle = (int)IssueType.Settlement;
                int issueClosed = (int)IssueType.Closed;

                //Update lineitems
                //update the issue orderdetails to orderdetail table
                OrderDBO orderDBO = new OrderDBO();
                orderDBO.UpdateIssueOrder(issue.OrderIssueId, order.Id);
                //update the total order value into order table
                PaymentLinkLogsService paymentUpdate = new PaymentLinkLogsService();
                var totalAmt = paymentUpdate.GetTotalAmtOfOrder(order.Id);
                var disamt = SpiritUtility.CalculateOverAllDiscount(order.Id, totalAmt);
                order.OrderAmount = disamt;
                db.SaveChanges();


                //Marked issue as closed
                issue.OrderIssueTypeId = issueSettle;
                issue.OrderStatusId = issueClosed;
                db.SaveChanges();

                //Marked order as podcancel
                order.OrderStatusId = ostatusSubmitted;
                //db.SaveChanges();
                UpdateOrderStatus(issue.OrderId ?? 0, order.OrderStatusId);

                //Marked issueTrack as closed
                issueStatus.OrderIssueTypeId = issueClosed;
                UpdateIssueStatus(issueStatus, issue);

            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Unable to SAVE the Issue." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Issue saved succeefully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/issue-changedorder-save", Name = "IssueChangedOrderSave")]
        public ActionResult IssueChangedOrderSave(IssueStatus issueStatus)
        {
            if (issueStatus.OrderIssueId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
                var issueTotalAmt = orderIssuePay.GetTotalAmtOfIssueOrder(issueStatus.OrderIssueId);
                int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);
                if (issueTotalAmt < MinTotalAmtOrd)
                {
                    return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
                }

                var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
                var order = db.Orders.Find(issue.OrderId);

                OrderPaymentType[] payType = new[] { OrderPaymentType.POD, OrderPaymentType.OCOD };
                int[] payTypeValue = payType.Cast<int>().ToArray();

                int ostatusSubmitted = (payTypeValue.Contains(order.PaymentTypeId??0)) ? (int)OrderStatusEnum.Submitted : (int)OrderStatusEnum.Approved;

                int issueSettle = (int)IssueType.Settlement;
                int issueClosed = (int)IssueType.Closed;

                //Update lineitems
                //update the issue orderdetails to orderdetail table
                OrderDBO orderDBO = new OrderDBO();
                orderDBO.UpdateIssueOrder(issue.OrderIssueId, order.Id);
                //update the total order value into order table
                PaymentLinkLogsService paymentUpdate = new PaymentLinkLogsService();
                var totalAmt = paymentUpdate.GetTotalAmtOfOrder(order.Id);
                var disamt = SpiritUtility.CalculateOverAllDiscount(order.Id, totalAmt);
                order.OrderAmount = disamt;
                db.SaveChanges();


                //Marked issue as closed
                issue.OrderIssueTypeId = issueSettle;
                issue.OrderStatusId = issueClosed;
                db.SaveChanges();

                //Marked order as podcancel
                order.OrderStatusId = ostatusSubmitted;
                //db.SaveChanges();
                UpdateOrderStatus(issue.OrderId ?? 0, order.OrderStatusId);

                //Marked issueTrack as closed
                issueStatus.OrderIssueTypeId = issueClosed;
                UpdateIssueStatus(issueStatus, issue);

            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Unable to SAVE the Issue." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Issue saved succeefully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/issue-ff-cashpay", Name = "IssueStatusUpdateForCashPay")]
        public ActionResult IssueStatusUpdateForCashPay(IssueStatus issueStatus)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
            var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            var issueTotalAmt = orderIssuePay.GetTotalAmtOfIssueOrder(issueStatus.OrderIssueId);
            var disamt = SpiritUtility.CalculateOverAllDiscount(issue.OrderId ?? 0, issueTotalAmt);
            int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);


            var ordPaymentTypes = new[] { IssueType.PartialRefund, IssueType.PartialPay };
            var arryPaymentTypes = ordPaymentTypes.Cast<int>().ToArray();

            if (disamt < MinTotalAmtOrd && arryPaymentTypes.Contains(issueStatus.OrderIssueTypeId ?? 0))
            {
                return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
            }
            OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
            AggrePaymentController aggePaymentController = new AggrePaymentController();
            var paymentGateWayName = onlinePaymentServiceDBO.GetPaymentGateWayName(issue.OrderId.Value);

            if (paymentGateWayName == null)
            {
                string Message = $"No Online Payment made against the order {issue.OrderId} please do wallet refund";

                return Json(new { status = false, msg = Message }, JsonRequestBehavior.AllowGet);

            }
            //var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            try
            {
                issue.UpdatedAmt = issueStatus.UpdateAmt;
                issue.AdjustAmt = issueStatus.DiffAmt;
                issue.OrderIssueTypeId = issueStatus.OrderIssueTypeId;
                issue.OrderStatusId = issueStatus.OrderIssueTypeId;
                db.SaveChanges();
                UpdateIssueStatus(issueStatus, issue);
                UpdateOrderStatus(issue.OrderId ?? 0, 30);

                //Update for cash collect and refund the amount.
                OrderIssueDBO cashDBO = new OrderIssueDBO();
                cashDBO.AddPendingPay(new OrderIssue
                {
                    OrderId = issue.OrderId,
                    OrderIssueId = issueStatus.OrderIssueId
                });
                var order = db.Orders.Find(issue.OrderId);
                OrderDBO orderDBO = new OrderDBO();
                int isRefundInitiated = orderDBO.CheckRefundInitiated(issue.OrderId.Value, issue.OrderIssueId);
                if (isRefundInitiated == 1)
                {
                    return Json(new { status = false, msg = "This refund has already been initiated. If a different refund is to be done, please raise an issue against the order and try again." }, JsonRequestBehavior.AllowGet);
                }
                CashOrderIssue(order, issue);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Error while updating the Issue status." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Updated Issue successfully." }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Route("~/issue-ff-update", Name = "IssueStatusUpdate")]
        public ActionResult IssueStatusUpdate(IssueStatus issueStatus)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
            var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            var issueTotalAmt = orderIssuePay.GetTotalAmtOfIssueOrder(issueStatus.OrderIssueId);
            var ordAmount = db.Orders.Where(x => x.Id == issue.OrderId).FirstOrDefault();

            var disamt = (ordAmount.OrderAmount) -Convert.ToDecimal(Math.Abs(issueStatus.DiffAmt ?? 0));//SpiritUtility.CalculateOverAllDiscount(issue.OrderId ??0, issueTotalAmt);
            int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);
            OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();

            var ordPaymentTypes = new[] { IssueType.PartialRefund, IssueType.PartialPay };
            var arryPaymentTypes = ordPaymentTypes.Cast<int>().ToArray();

            if (disamt < MinTotalAmtOrd && arryPaymentTypes.Contains(issueStatus.OrderIssueTypeId ?? 0))
            {
                return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
            }
            //var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            try
            {
                var paymentGateWayName = onlinePaymentServiceDBO.GetPaymentGateWayName(issue.OrderId.Value);
                if (paymentGateWayName == null)
                {
                    string Message = $"No Online Payment made against the order {issue.OrderId}. Please do a wallet refund";

                    return Json(new { status = false, msg = Message }, JsonRequestBehavior.AllowGet);

                }
                if (paymentGateWayName.PaymentGateWayName == "CashFree" && Math.Abs(issueStatus.DiffAmt ?? 0) > Convert.ToDouble(paymentGateWayName.OrderAmount) && issueStatus.OrderIssueTypeId == 3 && issueStatus.IsPartialRefund ==false)
                {
                    string Message = $"You paid Rs.{Convert.ToDouble(paymentGateWayName.OrderAmount)} online and Rs.{Convert.ToDouble(ordAmount.WalletAmountUsed)} from wallet.Partial refund into source will only credit Rs. {Convert.ToDouble(paymentGateWayName.OrderAmount)} into your online source and Rs. {Math.Abs(issueStatus.DiffAmt ?? 0) - Convert.ToDouble(paymentGateWayName.OrderAmount)} will be credited into your wallet.Proceed ?";

                    return Json(new { status = false, msg = Message , partialRefund =true }, JsonRequestBehavior.AllowGet);

                }
                issue.UpdatedAmt = issueStatus.UpdateAmt;
                issue.AdjustAmt = issueStatus.DiffAmt;
                issue.OrderIssueTypeId = issueStatus.OrderIssueTypeId;
                issue.OrderStatusId = issueStatus.OrderIssueTypeId;
                db.SaveChanges();
                UpdateIssueStatus(issueStatus, issue);
                UpdateOrderStatus(issue.OrderId ?? 0, 30);

                if (issue.OrderIssueTypeId == 3 && issue.OrderStatusId==6)
                {
                    return Json(new { status = false, msg = "Payment has refunded for this issue. Partial refund can only be done against a given issue." }, JsonRequestBehavior.AllowGet);
                   
                }
                else if (issue.OrderIssueTypeId == 3 && issue.OrderStatusId != 6)
                {
                    //var c = SZIoc.GetSerivce<ISZConfiguration>();
                    //var paymentGateWayName = c.GetConfigValue(ConfigEnums.PaymentGateWayName.ToString());
                    AggrePaymentController aggePaymentController = new AggrePaymentController();
                    UpdateOrderStatus(issue.OrderId ?? 0, 36);

                   
                    if (paymentGateWayName.PaymentGateWayName == "AggrePay")
                    {
                        var refund = aggePaymentController.OnlineReFund(issue.OrderIssueId);
                        return Json(new { status = refund.Status, msg = refund.Message }, JsonRequestBehavior.AllowGet);
                    }
                    else if (paymentGateWayName.PaymentGateWayName == "CashFree")
                    {
                        PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                        var refund = paymentLinkLogsService.CashFreeReFund(issue.OrderIssueId);

                        return Json(new { status = refund.Status, msg = refund.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Error while updating the Issue status." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Updated Issue successfully." }, JsonRequestBehavior.AllowGet);
           
        }                                                                                                                                                                                                                                                                                

        [HttpPost]
        [Route("~/issue-ff-update-Wallet", Name = "IssueStatusUpdateWallet")]
        public ActionResult IssueStatusUpdateWallet(IssueStatus issueStatus)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
            var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            var issueTotalAmt = orderIssuePay.GetTotalAmtOfIssueOrder(issueStatus.OrderIssueId);
            var disamt = SpiritUtility.CalculateOverAllDiscount(issue.OrderId ?? 0, issueTotalAmt);
            int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);


            var ordPaymentTypes = new[] { IssueType.WalletRefund, IssueType.WalletPartialRefund };
            var arryPaymentTypes = ordPaymentTypes.Cast<int>().ToArray();

            if (disamt < MinTotalAmtOrd && arryPaymentTypes.Contains(issueStatus.OrderIssueTypeId ?? 0) && issue.OrderIssueTypeId == 11)
            {
                return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
            }
            //var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            try
            {
                if ((issue.OrderIssueTypeId == 10 || issue.OrderIssueTypeId == 11) && AmountAddedInWallet(issue.OrderId.ToString(), issue.OrderIssueId))
                {
                    return Json(new { status = false, msg = "Payment has added to your wallet for this issue.Wallet partial refund can only be done against a given issue." }, JsonRequestBehavior.AllowGet);

                }
                issue.UpdatedAmt = issueStatus.UpdateAmt;
                issue.AdjustAmt = issueStatus.DiffAmt;
                issue.OrderIssueTypeId = issueStatus.OrderIssueTypeId;
                issue.OrderStatusId = issueStatus.OrderIssueTypeId;
                db.SaveChanges();
                UpdateIssueStatus(issueStatus, issue);
                //UpdateOrderStatus(issue.OrderId ?? 0, 30);
               
                if ((issue.OrderIssueTypeId == 10 || issue.OrderIssueTypeId == 11) && issue.OrderStatusId != 6 && !AmountAddedInWallet(issue.OrderId.ToString(),issue.OrderIssueId))
                {
                    
                    var order = db.Orders.Find(issue.OrderId);
                    OrderDBO orderDBO = new OrderDBO();
                    int isRefundInitiated = orderDBO.CheckRefundInitiated(issue.OrderId.Value ,issue.OrderIssueId);
                    if (isRefundInitiated == 1)
                    {
                        return Json(new { status = false, msg = "This refund has already been initiated. If a different refund is to be done, please raise an issue against the order and try again." }, JsonRequestBehavior.AllowGet);
                    }
                    int issuccess=  WalletOrderIssue(order, issue);

                    //UpdateOrderStatus(issue.OrderId ?? 0, 36);

                    return Json(new { status = true, msg = "Payment Refunded successfully" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Error while updating the Issue status." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Updated Issue successfully." }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Route("~/issue-manager-approve", Name = "PostIssueUpdateByManager")]
        public ActionResult IssueUpdateByManager(IssueStatus issueStatus)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
            var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            var issueTotalAmt = orderIssuePay.GetTotalAmtOfIssueOrder(issueStatus.OrderIssueId);
            int MinTotalAmtOrd = Convert.ToInt32(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);
            var disamt = SpiritUtility.CalculateOverAllDiscount(issue.OrderId ?? 0, issueTotalAmt);

            var ordPaymentTypes = new[] { IssueType.PartialRefund, IssueType.PartialPay };
            var arryPaymentTypes = ordPaymentTypes.Cast<int>().ToArray();

            if (disamt < MinTotalAmtOrd && arryPaymentTypes.Contains(issueStatus.OrderIssueTypeId ?? 0))
            {
                return Json(new { status = false, msg = "The updated total amount is less than Rs. 1000. The minimum order amount needs to be Rs. 1000" }, JsonRequestBehavior.AllowGet);
            }
            var order = db.Orders.Find(issue.OrderId);
            try
            {
                //var c = SZIoc.GetSerivce<ISZConfiguration>();
                //var paymentGateWayName = c.GetConfigValue(ConfigEnums.PaymentGateWayName.ToString());
                OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();
                AggrePaymentController aggePaymentController = new AggrePaymentController();
                var paymentGateWayName = onlinePaymentServiceDBO.GetPaymentGateWayName(order.Id);

                if (paymentGateWayName == null)
                {
                    string Message = $"No Online Payment made against the order {issue.OrderId} please do wallet refund";

                    return Json(new { status = false, msg = Message }, JsonRequestBehavior.AllowGet);

                }
                //if (paymentGateWayName.PaymentGateWayName == "CashFree" && Math.Abs(issueStatus.DiffAmt ?? 0) > Convert.ToDouble(paymentGateWayName.OrderAmount))
                //{
                //    string Message = $"Online Payment made against the order {issue.OrderId} is less than refund amount please do wallet refund";

                //    return Json(new { status = false, msg = Message }, JsonRequestBehavior.AllowGet);

                //}
                if (issue.OrderIssueTypeId == 2)
                {
                    if (paymentGateWayName.PaymentGateWayName == "AggrePay")
                    {
                        aggePaymentController.PartialPaymentLinkForReSend(order.Id.ToString());
                        return Json(new { status = true, msg = "Resent payment link to customer." }, JsonRequestBehavior.AllowGet);
                    }
                    if (PaymentLinkForExistOrder(Convert.ToString(order.Id), Convert.ToString(issue.OrderIssueId)))
                    {
                        if (paymentGateWayName.PaymentGateWayName == "CashFree")
                        {
                            SendPaymentLink(order.Id);
                            return Json(new { status = true, msg = "Resent payment link to customer." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }

                issue.UpdatedAmt = issueStatus.UpdateAmt;
                issue.AdjustAmt = issueStatus.DiffAmt;
                issue.OrderIssueTypeId = issueStatus.OrderIssueTypeId;
                issue.OrderStatusId = 7;
                db.SaveChanges();



                //issue.OrderStatusId = issueStatus.OrderIssueTypeId;
                //db.SaveChanges();

                UpdateIssueStatus(issueStatus, issue);
                UpdateOrderStatus(issue.OrderId ?? 0, 31);


                var payType = new[] { OrderPaymentType.POD, OrderPaymentType.OCOD };
                //var arryRevert = Array.ConvertAll<OrderStatusEnum, int>(ordStatusInventoryRevert, (v) => (int)v);
                var payOnDoor = payType.Cast<int>().ToArray();

                if (order.PaymentTypeId != null)
                {
                    if (payOnDoor.Contains(order.PaymentTypeId ?? 0))
                    {
                        PODOrderIssue(order, issue);
                        return Json(new { status = true, msg = $"Changes made for Pay on Delivery Payment Type." }, JsonRequestBehavior.AllowGet);

                    }
                }
             
                if (issue.OrderIssueTypeId == 2)
                {
                    if (paymentGateWayName.PaymentGateWayName == "AggrePay")
                    { 
                        var payCash=  aggePaymentController.OnlinePartialPayment(issue.OrderIssueId);
                        return Json(new { status = true, msg = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                   else if (paymentGateWayName.PaymentGateWayName == "CashFree")
                    {
                        PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                        var payCash = paymentLinkLogsService.CashFreePayment(issue.OrderIssueId);

                        return Json(new { status = payCash.Status, msg = payCash.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (issue.OrderIssueTypeId == 4)
                {
                    UpdateOrderStatus(issue.OrderId ?? 0, 36);
                    if (paymentGateWayName.PaymentGateWayName == "AggrePay")
                    {
                        var refund= aggePaymentController.OnlineReFund(issue.OrderIssueId);
                        return Json(new { status = true, msg = refund.Message }, JsonRequestBehavior.AllowGet);
                    }
                    else if (paymentGateWayName.PaymentGateWayName == "CashFree")
                    {
                        PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                        var refund = paymentLinkLogsService.CashFreeReFund(issue.OrderIssueId);

                        return Json(new { status = refund.Status, msg = refund.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Error while updating the Issue status." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Updated Issue successfully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/issue-manager-revertAmount", Name = "RevertAddedWalletRefundAmount")]
        public ActionResult RevertAddedWalletRefundAmount(IssueStatus issueStatus)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            PaymentLinkLogsService orderIssuePay = new PaymentLinkLogsService();
            var issue = db.OrderIssues.Find(issueStatus.OrderIssueId);
            
            try
            {
                if ((issue.OrderIssueTypeId == 10 || issue.OrderIssueTypeId == 11) && issue.OrderStatusId == 6)
                {
                    OrderIssueDBO orderIssueDBO = new OrderIssueDBO();
                    orderIssueDBO.RevertAddedWalletRefundAmount(issue.OrderId.Value);
                }
              
                   
            }
            
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Error while reverting amount from wallet." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Amount Reverted successfully." }, JsonRequestBehavior.AllowGet);
        }
        #region Non Action Method

        #endregion
        private bool PaymentLinkForExistOrder(string orderId, string issueId)
        {
            //string checkPartialPay = $"{orderId}_PartialPay";
            int ordId = Convert.ToInt32(orderId);
            var cashPay = db.CashFreePays.Where(o => o.OrderId == ordId);
            if (cashPay.Count() <= 0)
            {
                return false;
            }
            var paylink = cashPay.OrderByDescending(o => o.CashFreePayId).FirstOrDefault();
            var cashfreeLink = JsonConvert.DeserializeObject<CashFreeOrderCreate>(paylink.InputValue);

            if (!cashfreeLink.orderId.Contains("PartialPay")) return false;

            var apiPayCount = db.AppLogsCashFreeHooks.Where(o => o.OrderId == orderId && o.OrderIdPartial.Contains(cashfreeLink.orderId) && o.OrderIdPartial.Contains("PartialPay")).Count();

            if (apiPayCount > 0) return false;

            return true;
        }
        private void SendPaymentLink(int orderId)
        {
            var cashPay = db.CashFreePays.Where(o => o.OrderId == orderId)?.OrderByDescending(o => o.CashFreePayId).FirstOrDefault();
            var cashfreeLink = JsonConvert.DeserializeObject<CashFreePaymentOutput>(cashPay.VenderOutput);

            var order = db.Orders.Find(orderId);

            //Flow SMS
            var link1 = cashfreeLink.paymentLink.Substring(0, 28);
            var link2 = cashfreeLink.paymentLink.Substring(28, 28);
            var link3 = cashfreeLink.paymentLink.Substring(56);
            var dicti = new Dictionary<string, string>();
            dicti.Add("ORDER", order.Id.ToString());
            dicti.Add("AMOUNT", cashPay.Amt.ToString());
            dicti.Add("LINK1", link1);
            dicti.Add("LINK2", link2);
            dicti.Add("LINK3", link3);
            var templeteid = ConfigurationManager.AppSettings["SMSSendPaymentLinkCashfreeFlowId"];
            Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.OrderTo, dicti));
            //End Flow SMS
            //WSendSMS wsms = new WSendSMS();
            //string textmsg = string.Format(ConfigurationManager.AppSettings["CFResendLink"],
            //    cashPay.Amt, order.Id.ToString(), cashfreeLink.paymentLink);
            //wsms.SendMessage(textmsg, order.Customer.ContactNo);

            var stResend = (int)OrderStatusEnum.CashFreePayLinkSmsResent;
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = User.Identity.Name,
                LogUserId = u.Id,
                OrderId = order.Id,
                StatusId = stResend,
                TrackDate = DateTime.Now,
                Remark = "SMS Sent"
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();
        }

        //private void RevertInventory(int orderID)
        //{
        //    //var oDetail = db.OrderDetails.Where(o => o.OrderId == orderID)?.ToList();
        //    var order = db.Orders.Find(orderID);
        //    foreach (var item in order.OrderDetails)
        //    {
        //        var invent = db.Inventories.Where(o => (o.ProductID == item.ProductID) && (o.ShopID == order.ShopID))?.FirstOrDefault();
        //        if (invent != null)
        //        {
        //            invent.QtyAvailable += item.ItemQty;
        //        }
        //    }
        //    db.SaveChanges();
        //}

        public void PODOrderIssue(Order order, OrderIssue orderIssue)
        {

            var oDetail = db.OrderIssueDetails.Where(o => o.OrderIssueId == orderIssue.OrderIssueId);
            decimal totalAmt = 0;
            foreach (var item in oDetail)
            {
                int q = item.ItemQty ?? 0;
                decimal p = item.Price ?? 0;
                if (q > 0 && p > 0)
                {
                    decimal t = q * p;
                    totalAmt += t;
                }
            }
            var mDetail = db.MixerIssueDetails.Where(o => o.OrderIssueId == orderIssue.OrderIssueId);
            foreach (var item in mDetail)
            {
                int q = item.ItemQty ?? 0;
                decimal p = item.Price ?? 0;
                if (q > 0 && p > 0)
                {
                    decimal t = q * p;
                    totalAmt += t;
                }
            }

            totalAmt = SpiritUtility.CalculateOverAllDiscount(order.Id, totalAmt);

            OrderDBO orderDBO = new OrderDBO();
            orderDBO.UpdateIssueOrder(orderIssue.OrderIssueId, order.Id);

            int stPayFullRefund = (int)IssueType.FullRefund;
            int stPayTypeClose = (int)IssueType.Closed;

            int ordStatusPay = (int)OrderStatusEnum.CashPartialPaymentSelected;
            int ordStatusRefund = (int)OrderStatusEnum.CashPartialRefundSelected;
            int ordStatuType = 0;
            int ordStatu = 0;
            if ((orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialPay)
                ordStatuType = ordStatusPay;
            if ((orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialRefund)
                ordStatuType = ordStatusRefund;

            if ((order.PaymentTypeId) == (int)OrderPaymentType.POD)
                ordStatu = ordStatusPay;
            if ((order.PaymentTypeId) == (int)OrderPaymentType.OCOD)
                ordStatu = ordStatusRefund;


            int stRefund = (stPayFullRefund == orderIssue.OrderIssueTypeId) ? (int)OrderStatusEnum.IssueRefunded : (int)OrderStatusEnum.Submitted;
            order.OrderAmount = totalAmt;
            order.OrderStatusId = stRefund;
            db.SaveChanges();

            var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
            string uId = u.Id;

            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = u.Email,
                LogUserId = uId,
                OrderId = orderIssue.OrderId ?? 0,
                Remark = "Pay on Delivery Type.",
                StatusId = stRefund,
                TrackDate = DateTime.Now
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();

            var issue = db.OrderIssues.Find(orderIssue.OrderIssueId);
            issue.OrderStatusId = stPayTypeClose;
            db.SaveChanges();

            OrderIssueTrack orderIssueTrack = new OrderIssueTrack
            {
                UserId = uId,
                OrderId = orderIssue.OrderId ?? 0,
                OrderIssueId = orderIssue.OrderIssueId,
                Remark = "Pay on Delivery Type",
                OrderStatusId = stPayTypeClose,
                TrackDate = DateTime.Now
            };
            db.OrderIssueTracks.Add(orderIssueTrack);
            db.SaveChanges();

            SpiritUtility.GenerateZohoTikect(orderIssue.OrderId ?? 0, orderIssue.OrderIssueId);
        }
        public void CashOrderIssue(Order order, OrderIssue orderIssue)
        {
            decimal totalAmt = 0;
            var oDetail = db.OrderIssueDetails.Where(o => o.OrderIssueId == orderIssue.OrderIssueId);
            foreach (var item in oDetail)
            {
                int q = item.ItemQty ?? 0;
                decimal p = item.Price ?? 0;
                if (q > 0 && p > 0)
                {
                    decimal t = q * p;
                    totalAmt += t;
                }
            }

            var mDetail = db.MixerIssueDetails.Where(o => o.OrderIssueId == orderIssue.OrderIssueId);
            foreach (var item in mDetail)
            {
                int q = item.ItemQty ?? 0;
                decimal p = item.Price ?? 0;
                if (q > 0 && p > 0)
                {
                    decimal t = q * p;
                    totalAmt += t;
                }
            }

            totalAmt = SpiritUtility.CalculateOverAllDiscount(order.Id, totalAmt);

            OrderDBO orderDBO = new OrderDBO();
            orderDBO.UpdateIssueOrder(orderIssue.OrderIssueId, order.Id);

            int stPayFullRefund = (int)OrderStatusEnum.CashPartialRefundSelected;
            int stPayPay = (int)OrderStatusEnum.CashPartialPaymentSelected;
            int stPayTypeClose = (int)IssueType.Closed;
            int stRefund = (int)OrderStatusEnum.Approved;
            int stTrack = 0;

            if ((orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialPay)
                stTrack = stPayPay;
            if ((orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialRefund)
                stTrack = stPayFullRefund;


            int configPremitValue = Convert.ToInt32(ConfigurationManager.AppSettings["PremitValue"]);
            int premitVlaue = (string.IsNullOrWhiteSpace(order.LicPermitNo) && order.OrderType == "m") ?
                configPremitValue : 0;
            totalAmt += premitVlaue;

            order.OrderAmount = totalAmt;
            order.OrderStatusId = stRefund;
            db.SaveChanges();

            var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
            string uId = u.Id;

            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = u.Email,
                LogUserId = uId,
                OrderId = orderIssue.OrderId ?? 0,
                Remark = "Cash on Delivery",
                StatusId = stTrack,
                TrackDate = DateTime.Now
            };
            db.OrderTracks.Add(orderTrack);
            orderTrack = new OrderTrack
            {
                LogUserName = u.Email,
                LogUserId = uId,
                OrderId = orderIssue.OrderId ?? 0,
                Remark = "Pay on Delivery Type.",
                StatusId = stRefund,
                TrackDate = DateTime.Now
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();

            var issue = db.OrderIssues.Find(orderIssue.OrderIssueId);
            issue.OrderStatusId = stPayTypeClose;
            db.SaveChanges();

            OrderIssueTrack orderIssueTrack = new OrderIssueTrack
            {
                UserId = uId,
                OrderId = orderIssue.OrderId ?? 0,
                OrderIssueId = orderIssue.OrderIssueId,
                Remark = "Cash on Delivery",
                OrderStatusId = stTrack,
                TrackDate = DateTime.Now
            };
            db.OrderIssueTracks.Add(orderIssueTrack);
            orderIssueTrack = new OrderIssueTrack
            {
                UserId = uId,
                OrderId = orderIssue.OrderId ?? 0,
                OrderIssueId = orderIssue.OrderIssueId,
                Remark = "Cash on Delivery",
                OrderStatusId = stPayTypeClose,
                TrackDate = DateTime.Now
            };
            db.OrderIssueTracks.Add(orderIssueTrack);
            db.SaveChanges();

            SpiritUtility.GenerateZohoTikect(orderIssue.OrderId ?? 0, orderIssue.OrderIssueId);
        }
        public int WalletOrderIssue(Order order, OrderIssue orderIssue)
        {
            int result = 0;
            var oDetail = db.OrderIssueDetails.Where(o => o.OrderIssueId == orderIssue.OrderIssueId);
            decimal totalAmt = 0;
            foreach (var item in oDetail)
            {
                int q = item.ItemQty ?? 0;
                decimal p = item.Price ?? 0;
                if (q > 0 && p > 0)
                {
                    decimal t = q * p;
                    totalAmt += t;
                }
            }

            totalAmt = SpiritUtility.CalculateOverAllDiscount(order.Id, totalAmt);
            if (orderIssue.OrderIssueTypeId ==11)
            {
                OrderDBO orderDBO = new OrderDBO();
                orderDBO.UpdateIssueOrder(orderIssue.OrderIssueId, order.Id);
            }
            


            int stWalletRefund = (int)OrderStatusEnum.WalletRefundSelected;
            int stWalletRefundFailed = (int)OrderStatusEnum.WalletRefundFailed;
            int stPayTypeClose = (int)IssueType.Closed;
            int stRefund = 0;
            int stTrack = 0;

            if ((orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.WalletRefund || (orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.WalletPartialRefund)
                stTrack = stWalletRefund;

            string remarks = string.Empty;
            string remarks1 = string.Empty;

            if ((orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.WalletRefund)
            {
                remarks = "Wallet Full Refund selected";

                remarks1 = "Wallet Full Refund Successfull";
                stRefund = (int)OrderStatusEnum.WalletRefundSuccessful;
            }
            if ((orderIssue.OrderIssueTypeId ?? 0) == (int)IssueType.WalletPartialRefund)
            {
                remarks = "Wallet Partial Refund selected";
                remarks1 = "Wallet Partial Refund Successfull";
                stRefund = (int)OrderStatusEnum.WalletRefundSuccessful;
            }

                int configPremitValue = Convert.ToInt32(ConfigurationManager.AppSettings["PremitValue"]);
                int premitVlaue = (string.IsNullOrWhiteSpace(order.LicPermitNo) && order.OrderType == "m") ?
                    configPremitValue : 0;
                totalAmt += premitVlaue;

            //order.OrderAmount = totalAmt;
            //order.OrderStatusId = stRefund;
            //db.SaveChanges();

                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                string uId = u.Id;

                OrderIssueTrack orderIssueTrack = new OrderIssueTrack
                {
                    UserId = uId,
                    OrderId = orderIssue.OrderId ?? 0,
                    OrderIssueId = orderIssue.OrderIssueId,
                    Remark = remarks,
                    OrderStatusId = stWalletRefund,
                    TrackDate = DateTime.Now
                };
                db.OrderIssueTracks.Add(orderIssueTrack);

                OrderTrack orderTrack = new OrderTrack
                {
                    LogUserName = u.Email,
                    LogUserId = uId,
                    OrderId = orderIssue.OrderId ?? 0,
                    Remark = remarks,
                    StatusId = stWalletRefund,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();

                var custUserId = db.Customers.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                OrderIssueDBO cashDBO = new OrderIssueDBO();
                if (orderIssue.AdjustAmt < 0)
                {
                    double amt = Math.Abs(Convert.ToDouble(orderIssue.AdjustAmt));
                //var orderIssuesRefunded = db.OrderIssues.Where(x => x.OrderId == order.Id && x.IsCashOnDelivery == true).ToList();
                //if (orderIssuesRefunded !=null)
                //{
                //    var cashRefundedAmt = orderIssuesRefunded.Sum(b => Math.Abs(Convert.ToDouble(b.AdjustAmt)));
                //    if (cashRefundedAmt > 0)
                //    {
                //        amt = amt - cashRefundedAmt;

                //    }
                //}
                
                OnlinePaymentServiceDBO onlinePaymentServiceDBO = new OnlinePaymentServiceDBO();

                decimal walletReturnedAmt = onlinePaymentServiceDBO.GetWalletReturnedAmount(order.Id);
                var appHook = onlinePaymentServiceDBO.GetPaymentGateWayName(order.Id);
                OrderDBO orderDBO = new OrderDBO();
                double refundInitiatedAmt = orderDBO.GetRefundInitiatedAmount(order.Id);
                if (refundInitiatedAmt > 0)
                {
                    appHook.OrderAmount =(Convert.ToDouble(appHook.OrderAmount) -  refundInitiatedAmt).ToString();
                }
                var onlineRefunds = onlinePaymentServiceDBO.GetOnlineRefund(order.Id);
                var refund = onlineRefunds.Where(x => x.Status == "RefundInitiated");
                decimal onlineRefundedAmount = refund.Sum(a => Convert.ToDecimal(a.Amount));
                bool allPayOnline = false;
                if (appHook != null && appHook.OrderAmount == (Convert.ToDecimal(walletReturnedAmt) + onlineRefundedAmount + Convert.ToDecimal(amt)).ToString())
                {
                    allPayOnline = true;
                    appHook.OrderAmount = (Convert.ToDecimal(appHook.OrderAmount) - (Convert.ToDecimal(walletReturnedAmt) +Convert.ToDecimal(onlineRefundedAmount))).ToString();
                }
                if (appHook != null &&  onlineRefundedAmount > 0)
                {
                    appHook.OrderAmount = (Convert.ToDecimal(appHook.OrderAmount) - Convert.ToDecimal(onlineRefundedAmount)).ToString();
                }

                //var appHook = db.AppLogsCashFreeHooks.Where(x => x.OrderId == order.Id.ToString()).FirstOrDefault();

                if (orderIssue.OrderIssueTypeId == 10)
                {
                    //order.WalletAmountUsed = Convert.ToDecimal("1000.00000000");
                    if (order.WalletAmountUsed > 0 && (order.WalletAmountUsed != null))
                    {

                        float walletAmt = (float)order.WalletAmountUsed; //- (float)walletReturnedAmt;
                        result = cashDBO.FullRefundToWallet(orderIssue.OrderId.Value, walletAmt, orderIssue.OrderIssueId);
                        if (result == 1)
                        {
                            WalletRefundNotification(Convert.ToInt32(walletAmt), order.CustomerId, custUserId.UserId);
                        }

                    }

                    if (appHook != null && Convert.ToDecimal(appHook.OrderAmount) > 0)
                    {
                        if (allPayOnline)
                        {

                            result = cashDBO.FullRefundToWallet(orderIssue.OrderId.Value, (float)(Convert.ToDouble(appHook.OrderAmount)), orderIssue.OrderIssueId);

                            if (result == 1)
                            {
                                WalletRefundNotification(Convert.ToInt32(Convert.ToDouble(appHook.OrderAmount)), order.CustomerId, custUserId.UserId);
                            }

                        }
                        else if (appHook != null)
                        {
                            result = cashDBO.FullRefundToWallet(orderIssue.OrderId.Value, (float)Convert.ToDouble(appHook.OrderAmount), orderIssue.OrderIssueId);

                            if (result == 1)
                            {
                                WalletRefundNotification(Convert.ToInt32(Convert.ToDouble(appHook.OrderAmount)), order.CustomerId, custUserId.UserId);
                            }
                        }
                    }
                    var c = SZIoc.GetSerivce<ISZConfiguration>();
                    var isReturnCouponAmount = c.GetConfigValue(ConfigEnums.IsReturnCouponAmount.ToString());
                    if (isReturnCouponAmount == "1")
                    {
                        DiscountDBO discountDBO = new DiscountDBO();
                        var numberOfUse = discountDBO.RevertNumberOfUseSpecificOffer(order.CustomerId, order.Id);
                    }
                }
                else
                {
                    if (amt >= Convert.ToDouble(order.WalletAmountUsed) && (order.WalletAmountUsed != null))
                    {
                        result = cashDBO.FullRefundToWallet(orderIssue.OrderId.Value, (float)order.WalletAmountUsed, orderIssue.OrderIssueId);
                        if (result == 1)
                        {
                            WalletRefundNotification(Convert.ToInt32(order.WalletAmountUsed), order.CustomerId, custUserId.UserId);
                        }
                    }
                    else if (amt < Convert.ToDouble(order.WalletAmountUsed) && (order.WalletAmountUsed != null))
                    {
                        result = cashDBO.FullRefundToWallet(orderIssue.OrderId.Value, (float)amt, orderIssue.OrderIssueId);
                        if (result == 1)
                        {
                            WalletRefundNotification(Convert.ToInt32(amt), order.CustomerId, custUserId.UserId);
                        }

                    }
                    if (appHook != null && Convert.ToDecimal(appHook.OrderAmount) > 0)
                    {
                        amt = amt - (Convert.ToDouble(order.WalletAmountUsed) + Convert.ToDouble(order.PromoDiscountAmount));

                        if (amt > 0)
                        {
                            result = cashDBO.FullRefundToWallet(orderIssue.OrderId.Value, (float)amt, orderIssue.OrderIssueId);

                            if (result == 1)
                            {
                                WalletRefundNotification(Convert.ToInt32(amt), order.CustomerId, custUserId.UserId);
                            }
                        }

                    }

                }

                if (result == 1)
                {
                        OrderIssueTrack orderIssueTrack1 = new OrderIssueTrack
                        {
                            UserId = uId,
                            OrderId = orderIssue.OrderId ?? 0,
                            OrderIssueId = orderIssue.OrderIssueId,
                            Remark = remarks1,
                            OrderStatusId = stRefund,
                            TrackDate = DateTime.Now
                        };
                        db.OrderIssueTracks.Add(orderIssueTrack1);

                        OrderTrack orderTrack1 = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = uId,
                            OrderId = orderIssue.OrderId ?? 0,
                            Remark = remarks1,
                            StatusId = stRefund,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack1);
                        db.SaveChanges();

                        var issue = db.OrderIssues.Find(orderIssue.OrderIssueId);
                        issue.OrderStatusId = stPayTypeClose;
                        db.SaveChanges();

                    if (orderIssue.OrderIssueTypeId == 10)
                    {
                        OrderTrack orderTrack2 = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = uId,
                            OrderId = orderIssue.OrderId ?? 0,
                            Remark = remarks1,
                            StatusId = 68,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack2);
                        db.SaveChanges();
                    }
                   



                }
                else
                {
                        OrderIssueTrack orderIssueTrack2 = new OrderIssueTrack
                        {
                            UserId = uId,
                            OrderId = orderIssue.OrderId ?? 0,
                            OrderIssueId = orderIssue.OrderIssueId,
                            Remark = "Wallet Refund Failed",
                            OrderStatusId = stWalletRefundFailed,
                            TrackDate = DateTime.Now
                        };
                        db.OrderIssueTracks.Add(orderIssueTrack2);

                        OrderTrack orderTrack2 = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = uId,
                            OrderId = orderIssue.OrderId ?? 0,
                            Remark = "Wallet Refund Failed",
                            StatusId = stWalletRefundFailed,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack2);
                        db.SaveChanges();

                        var issue = db.OrderIssues.Find(orderIssue.OrderIssueId);
                        issue.OrderStatusId = stPayTypeClose;
                        db.SaveChanges();
                }
                //HyperTracking Complted
                int orderId = orderIssue.OrderId.Value;
                HyperTracking hyperTracking = new HyperTracking();
                Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(orderId));

                //Live Tracking FireStore
                CustomerApi2Controller.DeleteToFireStore(orderIssue.OrderId.Value);
                
            }
                
            
            return result;
        }
        private void UpdateOrderModifyStatus(OrderModifyStatus orderModifyStatus, OrderModify modify)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            string uId = u.Id;
            var modifytrack = new OrderModifyTrack
            {
                OrderId = modify.OrderId,
                OrderModifyId = modify.Id,
                Remark = orderModifyStatus.Remark,
                OrderStatusId = orderModifyStatus.StatusId,
                TrackDate = DateTime.Now,
                UserId = uId,
            };
            db.OrderModifyTracks.Add(modifytrack);
            db.SaveChanges();

        }
        private void UpdateIssueStatus(IssueStatus issueStatus, OrderIssue issue)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            string uId = u.Id;
            var issetrack = new OrderIssueTrack
            {
                OrderId = issue.OrderId,
                OrderIssueId = issue.OrderIssueId,
                Remark = issueStatus.Remark,
                OrderStatusId = issueStatus.OrderIssueTypeId,
                TrackDate = DateTime.Now,
                UserId = uId,
            };
            db.OrderIssueTracks.Add(issetrack);
            db.SaveChanges();

        }
        private void UpdateOrderStatus(int orderId, int statusId)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            string uId = u.Id;
            var order = db.Orders.Find(orderId);
            order.OrderStatusId = statusId;
            db.SaveChanges();

            WebEngageController webEngageController = new WebEngageController();
            Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Order Refund Intiated"));

            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = User.Identity.Name,
                LogUserId = uId,
                OrderId = order.Id,
                StatusId = order.OrderStatusId,
                TrackDate = DateTime.Now
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();
        }
        private bool AmountAddedInWallet(string orderId, int orderIssueId)
        {
            OrderIssueDBO orderIssueDBO = new OrderIssueDBO();
          bool result=  orderIssueDBO.AmountAddedInWallet(orderId, orderIssueId);
            return result;
        }
        public void WalletRefundNotification(int amount ,int customerId,string userId)
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
