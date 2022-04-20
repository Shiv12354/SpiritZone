using Microsoft.AspNet.Identity;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data.Entity;
using Newtonsoft.Json;
using RainbowWine.Services.DO;
using System.Text;
using RainbowWine.Models.ThirdParty;
using SZInfrastructure;
using SZData.Interfaces;
using Microsoft.Ajax.Utilities;
using SZModels;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using static RainbowWine.Services.FireStoreService;
using Google.Cloud.Firestore;
using System.Security.Cryptography;
using System.IO;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/1.6.2")]
    [EnableCors("*", "*", "*")]
    public class CustomerApi2Controller : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        
        public CustomerApi2Controller()
        {
        }

        public CustomerApi2Controller(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Route("promocode")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetPromoCode()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            var data = c.PromoCode();
            responseStatus.Data = data;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("promocodebyproduct/{productid}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult ValidatePromoCodeByProduct(int productId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            var data = c.ValidatePromoCodeByProduct(productId);
            responseStatus.Data = data;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("promocodebyshop/{shopid}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult ValidatePromoCodeByShop(int shopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            var data = c.ValidatePromoCodeByShop(shopId);
            responseStatus.Data = data;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("promocodeapply/{promocode}/{totalamount}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult PromoCodeApply(string promoCode,int totalAmount)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var c = SZIoc.GetSerivce<IPromoCodeService>();
                var data = c.PromoCodeApply(promoCode, totalAmount,Id);

                responseStatus.Data = data;
            }
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("validcode/{code}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult ValidCode(string code)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            var data = c.ValidCode(code);
            responseStatus.Data = data;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("dynamiccontent")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetDynamicPageContent()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            int pageId = (int)PageNameEnum.MYWALLET;
            string pageVersion = "1.6.2";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var pageImages = content.PageImages.Select(o => new { Key = o.ImageKey, Value = o.ImageUrl }).ToDictionary(o => o.Key, o => o.Value);
            var Title = conCart[PageContentEnum.Text2.ToString()];
            
            responseStatus.Data = content;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("generateuniquereferralcode")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GenerateUniqueReferralCode()
        {
            
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var data = c.UniqueReferralCode(Id);
                responseStatus.Data = data;

            }

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("order/cart")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddOrderByLoginUserCart(APIOrder model)
        {

            if (model == null)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Failed to add order. Please try again.", Data = model });
            else if (model.AddressId <= 0)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Address field cannot be blank.", Data = model });
            else if ((model.OrderItems == null && model.MixerItems == null) || (model.OrderItems.Count == 0 && model.MixerItems.Count == 0))
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Your cart is empty", Data = model });

            //else if (model.OrderItems == null || model.OrderItems.Count <= 0)
            //    return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Your cart is empty", Data = model });

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            decimal intconfigAmtTotal = Convert.ToDecimal(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);

            var inventItem = UserCustomerV2Controller.InventoryCheck(model.OrderItems);
            var inventMixerItem = UserCustomerV2Controller.InventoryMixerCheck(model.MixerItems);
            inventItem.TotalAmount += inventMixerItem.TotalAmount;
            if (inventItem.TotalAmount < intconfigAmtTotal)
            {
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = $"Minimum order amount is {intconfigAmtTotal} rs." });
            }

            if (inventItem.OrderItems.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Inventory issue.", ErrorType = "inventory", Data = new { orderId = 0, rejectedItems = inventItem } });
            }

            if (inventMixerItem.MixerItems.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Inventory issue.", ErrorType = "inventory", Data = new { orderId = 0, rejectedItems = inventMixerItem } });
            }

            var currentDate = DateTime.Now;
            OrderDBO orderDBO = new OrderDBO();
            int index = 0;
            var grouName = "group";
            var ordergrouName = default(string);
            DateTime orderCommittedStartDate = DateTime.Now;
            if (model.OrderItems.Count > 0)
            {
                var data = orderDBO.GetCommittedDate(currentDate, model.ShopId);
                index += 1;
                foreach (var item in model.OrderItems)
                {
                    item.CommittedEndDate = data.FirstOrDefault().CommittedEndDate;
                    item.CommittedStartDate = data.FirstOrDefault().CommittedStartDate;
                    ordergrouName = grouName + index.ToString();
                    item.OrderGroupId = ordergrouName;
                    orderCommittedStartDate = item.CommittedStartDate;
                }
            }

            if (model.MixerItems != null && model.MixerItems.Count > 0)
            {

                foreach (var item in model.MixerItems)
                {
                    var result = orderDBO.GetMixerCommittedDate(currentDate, item.MixerDetailId, model.ShopId).FirstOrDefault();
                    item.CommittedEndDate = result.CommittedEndDate;
                    item.CommittedStartDate = result.CommittedStartDate;
                    item.SupplierId = result.SupplierId;
                    item.MixerType = result.MixerType;
                }
                var mixergroup = model.MixerItems.GroupBy(o => new { o.CommittedStartDate, o.SupplierId });
                foreach (var item in mixergroup)
                {
                    var gItem = item.ToList();
                    index += 1;
                    grouName += index.ToString();
                    if (gItem != null)
                    {
                        foreach (var item2 in gItem)
                        {
                            if (item2.CommittedStartDate == orderCommittedStartDate && item2.SupplierId == 0)
                            {
                                item2.OrderGroupId = ordergrouName;
                                item2.ShopID = model.ShopId;
                            }
                            else
                            {
                                item2.OrderGroupId = grouName;
                            }

                        }
                    }
                }

            }
            //responseStatus.Data = model;

            int pageId = (int)PageNameEnum.CART;
            string pageVersion = "1.6";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var Title = conCart[PageContentEnum.Text2.ToString()];

            var Od = model.OrderItems.Select(a => new OrderCartDetails()
            {
                ProductId = a.ProductId,
                ProductName = $"{a.ProductName} - Volume: {a.Capacity}",
                Quantity = a.Qty,
                OrderGroupId = a.OrderGroupId,
                IsMixer = false

            }).ToList();
            if (model.MixerItems.Count > 0)
            {
                Od.AddRange(model.MixerItems.Select(a => new OrderCartDetails()
                {
                    ProductId = a.MixerDetailId,
                    ProductName = $"{a.ProductName} - Size: {a.Capacity}",
                    Quantity = a.Qty,
                    OrderGroupId = a.OrderGroupId,
                    SupplierId = a.SupplierId,
                    IsMixer = true

                }).ToList());
            }
            var groupData = model.OrderItems.GroupBy(x => x.OrderGroupId).Select(x => new ScheduleSlot()
            {
                OrderGroupId = x.FirstOrDefault().OrderGroupId,
                TimeSlot = $"{x.FirstOrDefault().CommittedStartDate.ToString("hh:mm tt")} - {x.FirstOrDefault().CommittedEndDate.ToString("hh:mm tt")}",
                Title = "Scheduled Delivery Slot",
                Date = x.FirstOrDefault().CommittedStartDate.Day,
                FullDate = x.FirstOrDefault().CommittedStartDate.ToString("dd/MM/yyyy"),
                DisplayDay = x.FirstOrDefault().CommittedStartDate.ToString("dddd"),
                Day = x.FirstOrDefault().CommittedStartDate.ToString("MMM").Substring(0, 3).ToUpper(),
                DeliveryCharge = "Free Delivery"
            }).ToList();

            groupData.AddRange(model.MixerItems.GroupBy(x => x.OrderGroupId).Select(x => new ScheduleSlot()
            {
                OrderGroupId = x.FirstOrDefault().OrderGroupId,
                TimeSlot = $"{x.FirstOrDefault().CommittedStartDate.ToString("hh:mm tt")} - {x.FirstOrDefault().CommittedEndDate.ToString("hh:mm tt")}",
                Title = "Scheduled Delivery Slot",
                Date = x.FirstOrDefault().CommittedStartDate.Day,
                FullDate = x.FirstOrDefault().CommittedStartDate.ToString("dd/MM/yyyy"),
                DisplayDay = x.FirstOrDefault().CommittedStartDate.ToString("dddd"),
                Day = x.FirstOrDefault().CommittedStartDate.ToString("MMM").Substring(0, 3).ToUpper(),
                DeliveryCharge = "Free Delivery"
            }).ToList());

            var payType = orderDBO.GetOrderPaymentType();

            if (payType == null)
            {
                responseStatus.Status = false;
                responseStatus.Message = "No Payment Type found.";
                return Content(HttpStatusCode.NotFound, responseStatus);
            }

            string Description = "";

            Description = payType.Where(o => o.PaymentTypeId == model.PaymentTypeId).FirstOrDefault().Description;
           
                var dt = from d in groupData.DistinctBy(x => x.OrderGroupId)
                     select new
                     {
                         SlotID = d.OrderGroupId,
                         Title = "Order Details",
                         OrderDetails = Od.Where(x => x.OrderGroupId == d.OrderGroupId).ToList(),
                         SheduledSlot = d,
                         ModeOfPayment = Description,
                     };

            responseStatus.Data = new
            {
                Cart = dt,
                ApiOrder = model
            };

            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("add-order-new")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddOrderByLoginUserNew(APIOrder model)
        {
            if (model == null)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Failed to add order. Please try again.", Data = model });
            else if (model.AddressId <= 0)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Address field cannot be blank.", Data = model });
            else if ((model.OrderItems == null && model.MixerItems == null) || (model.OrderItems.Count == 0 && model.MixerItems.Count == 0))
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Your cart is empty", Data = model });

            
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            decimal intconfigAmtTotal = Convert.ToDecimal(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);

            var inventItem = UserCustomerV2Controller.InventoryCheck(model.OrderItems);
            var inventMixerItem = UserCustomerV2Controller.InventoryMixerCheck(model.MixerItems);
            inventItem.TotalAmount += inventMixerItem.TotalAmount;
            if (inventItem.TotalAmount < intconfigAmtTotal)
            {
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = $"Minimum order amount is {intconfigAmtTotal} rs." });
            }

            if (inventItem.OrderItems.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Inventory issue.", ErrorType = "inventory", Data = new { orderId = 0, rejectedItems = inventItem } });
            }

            if (inventMixerItem.MixerItems.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Inventory issue.", ErrorType = "inventory", Data = new { orderId = 0, rejectedItems = inventMixerItem } });
            }

            using (var db = new rainbowwineEntities())
            {
                int custAdress = model.AddressId;
                var aspUser = db.AspNetUsers.Where(o => o.Id == uId)?.FirstOrDefault();
                Customer customer = db.Customers.Where(o => o.UserId == uId)?.FirstOrDefault();
                string userName = aspUser.Email;
                CustomerAddress customerAddress = db.CustomerAddresses.Where(o => o.CustomerId == customer.Id && o.CustomerAddressId == custAdress)?.FirstOrDefault();

                bool operationFlag = db.WineShops.Find(customerAddress.ShopId)?.OperationFlag ?? false;

                //Zone check added
                var zoneId = db.CustomerAddresses.Where(o => o.CustomerAddressId == custAdress).FirstOrDefault().ZoneId;
                bool? zoneOperational = db.DeliveryZones.Where(o => o.ZoneID == zoneId).FirstOrDefault().OperationalFlag;


                if (!operationFlag || !zoneOperational.Value)
                    return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Unfortunately, the selected shop is not operating at the moment. Please try a different location or try later.", Data = model });

                bool testorder = (ConfigurationManager.AppSettings["TestOrder"] == "1") ? true : false;

                db.Configuration.ProxyCreationEnabled = false;
                Order order = null;
                OrderTrack orderTrack = null;
                string ordergroupId = default(string);
                CustomerEta eta;
                MixerOrderItem mixerOrderItem = null;
                IList<APIMixerItem> mixerwithorder = null;
                IList<APIMixerItem> mixerwithoutorder = null;
                decimal totalAmt = 0;

                DateTime committedOrderStart = default(DateTime);
                DateTime committedOrderEnd = default(DateTime);
               
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    string Id = User.Identity.GetUserId();
                   
                    int configPremitValue = Convert.ToInt32(ConfigurationManager.AppSettings["PremitValue"]);
                    int premitVlaue = (string.IsNullOrWhiteSpace(model.PremitNo)) ?
                        configPremitValue : 0;

                    if (model.MixerItems != null && model.MixerItems.Count > 0)
                    {
                        mixerwithorder = model.MixerItems.Where(o => o.SupplierId == 0)?.ToList();
                        mixerwithoutorder = model.MixerItems.Where(o => o.SupplierId > 0)?.ToList();
                    }
                    if ((model.OrderItems != null && model.OrderItems.Count > 0) || (mixerwithorder != null && mixerwithorder.Count > 0))
                    {
                        order = new Order
                        {
                            OrderDate = DateTime.Now,
                            CustomerId = customer.Id,
                            OrderAmount = 0,
                            OrderPlacedBy = userName,
                            OrderTo = customer.ContactNo,
                            OrderStatusId = 1,
                            ShopID = customerAddress.ShopId,
                            DeliveryPickup = "Delivery",
                            PaymentDevice = "Android",
                            TestOrder = testorder,
                            ZoneId = 0,
                            LicPermitNo = model.PremitNo,
                            CustomerAddressId = model.AddressId,
                            OrderType = "m",
                            PaymentTypeId = model.PaymentTypeId
                        };
                        db.Orders.Add(order);
                        db.SaveChanges();

                        //OrderTrackingLog(order.Id, uId, User.Identity.Name, 1);
                        orderTrack = new OrderTrack
                        {
                            LogUserName = userName,
                            LogUserId = uId,
                            OrderId = order.Id,
                            StatusId = 1,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();
                        OrderDetail orderDetail = null;
                        //if (model.OrderItems.Count > 0)
                        //{
                        foreach (var item in model.OrderItems)
                        {
                            committedOrderStart = item.CommittedStartDate;
                            committedOrderEnd = item.CommittedEndDate;
                            orderDetail = new OrderDetail
                            {
                                OrderId = order.Id,
                                ItemQty = item.Qty,
                                ProductID = item.ProductId,
                                Price = item.Price,
                                ShopID = item.ShopID
                            };
                            db.OrderDetails.Add(orderDetail);
                            int q = item.Qty;
                            decimal p = item.Price;
                            decimal t = q * p;
                            totalAmt += t;
                        }
                        db.SaveChanges();

                        ordergroupId = (mixerwithoutorder?.Count() > 0 && order != null) ?
                            $"OG_{order.Id}"
                       : default(string);

                        if (mixerwithorder != null && mixerwithorder.Count() > 0)
                        {
                            foreach (var item in mixerwithorder)
                            {
                                committedOrderStart = item.CommittedStartDate;
                                committedOrderEnd = item.CommittedEndDate;
                                mixerOrderItem = new MixerOrderItem
                                {
                                    OrderId = order.Id,
                                    ItemQty = item.Qty,
                                    MixerDetailId = item.MixerDetailId,
                                    Price = item.Price,
                                    ShopId = model.ShopId,
                                    OrderGroupId = ordergroupId
                                };
                                db.MixerOrderItems.Add(mixerOrderItem);
                                int q = item.Qty;
                                decimal p = Convert.ToDecimal(item.Price);
                                decimal t = q * p;
                                totalAmt += t;
                            }
                            db.SaveChanges();
                        }

                        order.OrderAmount = totalAmt + ((model.OrderItems.Count > 0) ? premitVlaue : 0);
                        order.OrderStatusId = 2;
                        order.OrderGroupId = ordergroupId;
                        orderTrack = new OrderTrack
                        {
                            LogUserName = userName,
                            LogUserId = uId,
                            OrderId = order.Id,
                            StatusId = 2,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();

                        //add eta for and order
                        eta = new CustomerEta
                        {
                            CommittedTime = committedOrderStart,
                            CommittedTimeEnd = committedOrderEnd,
                            CustomerId = order.CustomerId,
                            OrderId = order.Id,
                            CreatedDate = DateTime.Now,
                            Eta = $"{committedOrderStart:dd/MM/yyyy HH tt} - {committedOrderEnd:HH tt}"
                        };
                        db.CustomerEtas.Add(eta);
                        db.SaveChanges();
                        //}
                        //OrderTrackingLog(order.Id, uId, User.Identity.Name, 2);
                    }

                    if (mixerwithoutorder != null)
                    {
                        if (mixerwithoutorder.Count() > 0)
                        {
                            var groupbymixer = mixerwithoutorder.GroupBy(o => o.OrderGroupId).ToList();
                            foreach (var groupItem in groupbymixer)
                            {
                                totalAmt = 0;
                                var itemgroup = groupItem.ToList();
                                if (itemgroup.Count() > 0)
                                {
                                    int mixerDetailId = itemgroup[0].MixerDetailId;
                                    var mixerDetail = db.MixerDetails.Include(o => o.Mixer).Where(o => o.MixerDetailId == mixerDetailId)?.FirstOrDefault();
                                    string mixerType = mixerDetail?.Mixer?.MixerType;
                                    //group wise order add
                                    var order2 = new Order
                                    {
                                        OrderDate = DateTime.Now,
                                        CustomerId = customer.Id,
                                        OrderAmount = 0,
                                        OrderPlacedBy = userName,
                                        OrderTo = customer.ContactNo,
                                        OrderStatusId = 1,
                                        ShopID = customerAddress.ShopId,
                                        DeliveryPickup = "Delivery",
                                        PaymentDevice = "Android",
                                        TestOrder = testorder,
                                        ZoneId = 0,
                                        LicPermitNo = model.PremitNo,
                                        CustomerAddressId = model.AddressId,
                                        OrderType = "m",
                                        PaymentTypeId = model.PaymentTypeId,
                                        OrderGroupId = ordergroupId,
                                        OrderGroupType = mixerType
                                    };
                                    db.Orders.Add(order2);
                                    db.SaveChanges();

                                    var orderTrack2 = new OrderTrack
                                    {
                                        LogUserName = userName,
                                        LogUserId = uId,
                                        OrderId = order2.Id,
                                        StatusId = 1,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderTracks.Add(orderTrack2);
                                    db.SaveChanges();

                                    if (order == null) { order = order2; }

                                    foreach (var item2 in itemgroup)
                                    {
                                        committedOrderStart = item2.CommittedStartDate;
                                        committedOrderEnd = item2.CommittedEndDate;
                                        mixerOrderItem = new MixerOrderItem
                                        {
                                            OrderId = order2.Id,
                                            ItemQty = item2.Qty,
                                            MixerDetailId = item2.MixerDetailId,
                                            Price = item2.Price,
                                            //ShopId = item2.ShopID,
                                            SupplierId = item2.SupplierId,
                                            OrderGroupId = ordergroupId
                                        };
                                        db.MixerOrderItems.Add(mixerOrderItem);
                                        int q = item2.Qty;
                                        decimal p = Convert.ToDecimal(item2.Price);
                                        decimal t = q * p;
                                        totalAmt += t;
                                    }
                                    db.SaveChanges();


                                    //add eta for and order
                                    eta = new CustomerEta
                                    {
                                        CommittedTime = committedOrderStart,
                                        CommittedTimeEnd = committedOrderEnd,
                                        CustomerId = order2.CustomerId,
                                        OrderId = order2.Id,
                                        CreatedDate = DateTime.Now,
                                        Eta = $"{committedOrderStart:dd/MM/yyyy HH tt} - {committedOrderEnd:HH tt}"
                                    };
                                    db.CustomerEtas.Add(eta);
                                    db.SaveChanges();


                                    ordergroupId = (string.IsNullOrWhiteSpace(ordergroupId)) ?
                                        $"OG_{order2.Id}"
                                   : ordergroupId;

                                    order2.OrderAmount = totalAmt;// + premitVlaue;
                                    order2.OrderStatusId = 2;
                                    order2.OrderGroupId = ordergroupId;
                                    orderTrack2 = new OrderTrack
                                    {
                                        LogUserName = userName,
                                        LogUserId = uId,
                                        OrderId = order2.Id,
                                        StatusId = 2,
                                        TrackDate = DateTime.Now
                                    };
                                    db.OrderTracks.Add(orderTrack2);
                                    db.SaveChanges();

                                }
                            }

                        }
                    }
                    else
                    {

                        order.OrderAmount = totalAmt + premitVlaue;
                        order.OrderStatusId = 2;
                        orderTrack = new OrderTrack
                        {
                            LogUserName = userName,
                            LogUserId = uId,
                            OrderId = order.Id,
                            StatusId = 2,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();
                    }
                    //transaction.Commit();
                }
                catch (Exception ex)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Your order hasn’t been placed yet. Please try again";

                    db.Dispose();
                    string json = JsonConvert.SerializeObject(model);
                    SpiritUtility.AppLogging($"Api_add-order_Transaction: {ex.Message}", ex.StackTrace + json);
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
                if (order != null && order.Id > 0)
                {
                    try
                    {
                        var ord = (string.IsNullOrWhiteSpace(ordergroupId)) ? Convert.ToString(order.Id) : ordergroupId;

                        responseStatus.Data = new { orderId = ord, rejectedItems = new List<APIOrderDetails>() };
                        var ser = SZIoc.GetSerivce<IPromoCodeService>();
                        
                        if (model.IsPromoApplied)
                        {
                            var promoId = ser.PromoCodeApply(model.PromoCode, (float)order.OrderAmount, customer.UserId);
                            if (promoId.IsValid)
                            {
                                var usetranAmount = ser.UpdatePromoIdForOrder(promoId.PromoId, order.Id,promoId.DiscountAmount);
                            }
                            
                        }
                        if (model.IsUsingWalletBalance)
                        {

                            var usetranAmount = ser.UpdateWalletAmountForOrder(customer.UserId, order.Id);
                           
                        }

                        //Update orderid to Customer ETA 
                        //var orderETAobject = db.CustomerEtas.Where(i => i.CustomerId == customer.Id);
                        //if (orderETAobject != null && orderETAobject.Count() > 0)
                        //{
                        //    var orderETA = orderETAobject.OrderByDescending(j => j.Id).Take(1).FirstOrDefault();
                        //    orderETA.OrderId = order.Id;
                        //    db.SaveChanges();
                        //}

                    }
                    catch (Exception ex)
                    {
                        SpiritUtility.AppLogging($"Api_add-order_SMS-Payment: {ex.Message}", ex.StackTrace);

                        //responseStatus.Status = false;
                        //responseStatus.Message = $"{ex.Message}";
                    }
                    //OrderTrackingLog(order.Id, uId, User.Identity.Name, 17);
                    orderTrack = new OrderTrack
                    {
                        LogUserName = userName,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = 35,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    responseStatus.Status = true;
                    responseStatus.Message = "Your order has been placed successfully.";

                    //////Live Tracking ///
                    AddToFireStore(order.Id);
                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    Task.Run(async () => await fcmHelper.SendFirebaseNotification(order.Id, FirebaseNotificationHelper.NotificationType.Order));
                }
                else
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Your order hasn’t been placed yet. Please try again.";
                }
            }

            //}
            return Ok(responseStatus);

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        //[Authorize(Roles = "Customer")]
        public HttpResponseMessage APIUserRegister(CustomerRegisterModel model)
        {
            HttpResponseMessage response = null;
            ReferralCode referralCode = new ReferralCode();
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            bool isSignWithoutCode = false;
            int val =Convert.ToInt32(c.GetConfigValue(ConfigEnums.IsSignWithoutCode.ToString()));
            if (val==1)
            {
                isSignWithoutCode = true;
            }
            var ser = SZIoc.GetSerivce<IPromoCodeService>();
            try
            {
               
                if (ModelState.IsValid)
                {
                    using (var db = new rainbowwineEntities())
                    {
                        var otpNumber = db.CustomerOTPVerifies.Where(o => o.ContactNo == model.ContactNo && o.VerifiedDate !=null && o.IsDeleted == true).LastOrDefault();
                        if (otpNumber == null)
                        {
                            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = true, Message = "This is not a verified contact number" });
                            return response;

                        }
                        var custExists = db.Customers.Where(o => o.ContactNo == model.ContactNo
                        && string.Compare(o.RegisterSource, "m", true) == 0)?.FirstOrDefault();
                        if (model.Code != null && model.Code != "")
                        {
                           
                            referralCode = ser.IsSignUpCode(model.Code);
                            if (referralCode.ValidCode == 1 || referralCode.ValidCode == 2 || referralCode.ValidCode == 3)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = true, Message = "This is valid Code." });

                            }
                            else
                            {
                                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = true, Message = "This is an invalid/expired Code." });
                                return response;
                            }
                        }
                        if (custExists != null)
                        {
                            bool isVerifiedOtp = true;
                            var otp = db.CustomerOTPVerifies.Where(o => o.CustomerId == custExists.Id);
                            if (otp.Count() > 0)
                            {
                                var fotp = otp.OrderByDescending(o => o.CustomerOTPVerifyId).Where(o => !o.IsDeleted ?? false)?.FirstOrDefault();
                                if (fotp != null)
                                { isVerifiedOtp = false; }
                            }
                            return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                new ResponseStatus
                                {
                                    Status = false,
                                    Message = "This phone number already exists. Please try logging in with another phone number. ",
                                    Data = new { IsOTPVerified = isVerifiedOtp }
                                });
                        }
                        db.Configuration.ProxyCreationEnabled = false;
                        var usertype = db.UserTypes.Where(o => string.Compare(o.UserTypeName, "customer", true) == 0)?.FirstOrDefault();
                        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.ContactNo };
                        var result = UserManager.CreateAsync(user, model.Password).Result;
                        if (result.Succeeded)
                        {
                            var userUpdate = db.AspNetUsers.Find(user.Id);
                            userUpdate.CreatedDate = DateTime.Now;
                            userUpdate.ModifiedDate = DateTime.Now;
                            db.SaveChanges();

                            AccountController accountController = new AccountController(UserManager, SignInManager);
                            accountController.AddRoleToUser(user.Id, usertype.UserTypeId);
                            Customer customer = new Customer
                            {
                                CustomerName = model.CustomerName,
                                ContactNo = model.ContactNo,
                                CreatedDate = DateTime.Now,
                                DOB = model.DOB,
                                UserId = user.Id,
                                RegisterSource = "m",
                                RefCode=(referralCode.ValidCode != 0 && referralCode.ValidCode==3) ? model.Code :null
                            };
                            db.Customers.Add(customer);
                            db.SaveChanges();

                            if (referralCode.ValidCode == 1)
                            {                               
                                var vc = ser.ReferralTypeCodeCal(model.Code, user.Id);
                            }
                            if (referralCode.ValidCode == 2)
                            {                                
                                var vc = ser.SignReferralCal(model.Code, user.Id);
                            }
                            if (referralCode.ValidCode == 3)
                            {                                
                                var vc = ser.ReferralCal(model.Code, user.Id);
                            }
                            if (isSignWithoutCode)
                            {
                                if (model.Code == null || model.Code == "")
                                {
                                    model.Code = ConfigurationManager.AppSettings["SignUpCode"].ToString();
                                    var vc = ser.SignReferralCal(model.Code, user.Id);
                                }
                            }
                            response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = true, Message = "Account created successfully." });
                        }
                        else
                        {
                            string message = default(string);
                            foreach (var error in result.Errors)
                            {
                                message += error + ", ";
                            }
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = message });
                        }
                       
                    }
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                         .SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage));
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = messages });
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + " 'InnerException':" + ex.InnerException.Message;
                SpiritUtility.AppLogging($"Api_AllUserRegister: {ex.Message}", ex.StackTrace);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = message });
            }

            return response;
        }


        [HttpGet]
        [Route("delivery/earning")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetTotalEarning()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();

            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var earn = deliveryEarningService.GetTotalEarning(uId, null);
            responseStatus.Data = new { totalEarning = earn };

            return Ok(responseStatus);

        }

        [HttpGet]
        [Route("delivery/earning-with-penalty")]
        [Authorize(Roles = "Deliver")]
        ///This API is for the dashboard page of the delivery app which
        ///shows the total earnings. It includes total earnings with penalty
        ///and incentives.
        public IHttpActionResult GetTotalEarningPenalty()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();

            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var earn = deliveryEarningService.GetTotalEarningWithPenalty(uId, null);
            responseStatus.Data = new { totalEarning = earn };

            return Ok(responseStatus);

        }


        [HttpPost]
        [Route("delivery/earning-history")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetEarningHistory(InputDateParam req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();

            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var totalearn = deliveryEarningService.GetTotalEarning(uId, req.Date);
            var earn = deliveryEarningService.GetEarningHistory(uId, req.Date);
            responseStatus.Data = new { totalEarning = totalearn, earning = earn };

            return Ok(responseStatus);

        }

        [HttpGet]
        [Route("delivery/earning/penalty")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetTotalEarningWithPenalty()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();

            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var earn = deliveryEarningService.GetTotalEarningWithPenalty(uId, null);
            responseStatus.Data = new { totalEarning = earn };

            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("delivery/earning-history-penalty")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetEarningHistoryWithPenalty(InputDateParam req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();

            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var totalearn = deliveryEarningService.GetTotalEarningWithPenalty(uId, req.Date);
            var earn = deliveryEarningService.GetEarningHistoryWithPenalty(uId, req.Date);
            responseStatus.Data = new { totalEarning = totalearn, earning = earn };

            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("delivery/earning-update")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult EarningUpdate(OrderInputParam req)
        {

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if(req == null || req.OrderId == 0)
            {
                responseStatus.Message = "Order Id should be greater than 0.";
                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            string uId = User.Identity.GetUserId();

            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var earn = deliveryEarningService.AddEarning(uId, req.OrderId);
            responseStatus.Data = new { earning = earn };

            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("new/delivery/earning-update")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult EarningUpdateNew(OrderInputParam req)
        {

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (req == null || req.OrderId == 0)
            {
                responseStatus.Message = "Order Id should be greater than 0.";
                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            string uId = User.Identity.GetUserId();
            
            var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
            var earn = deliveryEarningService.AddEarningNew(uId, req.OrderId);
            if (earn < 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    string fcm_title = default(string);
                    string fcm_msg = default(string);
                    
                    fcm_title = string.Format("Order {0} LATE DELIVERY PENALTY!", req.OrderId.ToString());
                    fcm_msg = string.Format("You just lost Rs. {0} because you delivered order {1} after 75 mins!!", earn.ToString(), req.OrderId.ToString());
                    var jsonValue = new
                    {
                        OrderId = req.OrderId,
                        Title = fcm_title,
                        Message = fcm_msg
                    };
                    var serializeJson = JsonConvert.SerializeObject(jsonValue);
                    var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                    var resp = client.PostAsync(ConfigurationManager.AppSettings["DeliveryAppGeneralNotifURL"], content).Result;
                    var ret = resp.Content.ReadAsStringAsync().Result;
                }
            }
            responseStatus.Data = new { earning = earn };

            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("delivery/logon/{onoff}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage OnAgent(int onoff)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    if (onoff < 2)
                    {
                        string uId = User.Identity.GetUserId();
                        RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                        db.Configuration.ProxyCreationEnabled = false;
                        var cdate = DateTime.Now;
                        var loginAgent = db.DeliveryAgentLogins.Where(o => o.DeliveryAgentId == regUser.DeliveryAgentId
                        && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day)?
                        .OrderByDescending(o => o.Id).FirstOrDefault();
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
                        && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day)?
                        .OrderByDescending(o => o.Id).FirstOrDefault();
                        }
                        else
                        {
                            if (onoff == 1)
                            {
                                loginAgent.OffDuty = DateTime.Now;
                            }
                            loginAgent.IsOnOff = (onoff == 0);
                            db.SaveChanges();
                            if (onoff == 0)
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
                            && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day)?
                            .OrderByDescending(o=>o.Id).FirstOrDefault();

                            }
                        }
                        responseStatus.Status = true;
                        responseStatus.Data = loginAgent;
                    }
                }
                catch (Exception ex)
                {
                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"Api_OnAgent: {ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_OnAgent: {ex.Message}", ex.StackTrace);

                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("walletscreen")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult DisplayWalletScreen()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            int pageId = (int)PageNameEnum.MYWALLET;
            string pageVersion = "1.6.2";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var pageImages = content.PageImages.Select(o => new { Key = o.ImageKey, Value = o.ImageUrl }).ToDictionary(o => o.Key, o => o.Value);
            var pageButtons = content.PageButtons.Select(o => new { Key = o.ButtonKey, Value = o.ButtonText }).ToDictionary(o => o.Key, o => o.Value);
            var ExpireInDays = SZIoc.GetSerivce<ISZConfiguration>();
             
            var ser = SZIoc.GetSerivce<IPromoCodeService>();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string userId = User.Identity.GetUserId();
                var data = ser.BalanceAndExpiryAmount(userId);
                if (data ==null)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
               var baldata = new
                {
                    balAmount = data.Wallet.FirstOrDefault().Balance,
                    expAmount = data.WalletOrder.FirstOrDefault().ExpireAmount,
                    expDate =Convert.ToInt32(ExpireInDays.GetConfigValue(ConfigEnums.ExpireInDays.ToString())),
                    IsReferralEnable =data.OrderExt.FirstOrDefault().IsReferralEnable
               };

                responseStatus.Data = new
                {
                    BalancedLbl = conCart[PageContentEnum.Text1.ToString()],
                    NoteLbl = conCart[PageContentEnum.Text2.ToString()],
                    ExpiryInDaysLbl = conCart[PageContentEnum.Text3.ToString()],
                    SpiritzoneCreditLbl = conCart[PageContentEnum.Text4.ToString()],
                    TranHistoryLbl = conCart[PageContentEnum.Text5.ToString()],
                    CreditAddLbl = conCart[PageContentEnum.Text6.ToString()],
                    CreditLeftLbl = conCart[PageContentEnum.Text7.ToString()],
                    WillExpireLbl = conCart[PageContentEnum.Text8.ToString()],
                    ReferralNotEnableMsg = conCart[PageContentEnum.Text27.ToString()],
                    CreditUsedLbl = conCart[PageContentEnum.Text9.ToString()],
                    Button1Lbl = pageButtons[PageButtonsEnum.Button1.ToString()],
                    Image1 = pageImages[PageImageEnum.Image1.ToString()],
                    Image2 = pageImages[PageImageEnum.Image2.ToString()],
                    BalData =baldata
                };
               

            } 
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("crediworksscreen")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult DisplayCreditWorksScreen()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            int pageId = (int)PageNameEnum.MYWALLET;
            string pageVersion = "1.6.2";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
                responseStatus.Data = new
                {
                        Title = conCart[PageContentEnum.Text4.ToString()],
                        Details = new object[]
                       {
                        new {
                            Title = conCart[PageContentEnum.Text10.ToString()],
                            SubTitle = conCart[PageContentEnum.Text11.ToString()]
                        },
                       
                        new {
                            Title = conCart[PageContentEnum.Text12.ToString()],
                            SubTitle = conCart[PageContentEnum.Text13.ToString()]

                        },
                        
                        new {
                            Title = conCart[PageContentEnum.Text14.ToString()],
                            SubTitle = conCart[PageContentEnum.Text15.ToString()]

                        },
                           new {
                            Title = conCart[PageContentEnum.Text16.ToString()],
                            SubTitle = conCart[PageContentEnum.Text17.ToString()]

                        },
                           new {
                            Title = conCart[PageContentEnum.Text30.ToString()],
                            SubTitle = conCart[PageContentEnum.Text31.ToString()]

                        },
                           new {
                            Title = conCart[PageContentEnum.Text32.ToString()],
                            SubTitle = conCart[PageContentEnum.Text33.ToString()]

                        },
                    },
                    
                };
                
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("transactionhistory")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult DisplayTransactionHistory()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            int pageId = (int)PageNameEnum.MYWALLET;
            string pageVersion = "1.6.2";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var ser = SZIoc.GetSerivce<IPromoCodeService>();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string userId = User.Identity.GetUserId();
                var data = ser.TransactionHistory(userId);
                var tranHistory = from d in data
                              select new
                              {
                                  CardId=d.CardId,
                                  Date=d.CreatedDate.ToString("dd MMM yyyy | hh:mm tt"),
                                  CreditAdded = d.CreditAdded,
                                  ExpiryDate = d.ExpiryDate.ToString("dd MMM yyyy | hh:mm tt"),
                                  TransactionType = d.TransactionType,
                                  SourceType = d.SourceType,
                                  CreditLeft=d.CreditLeft,
                                  CreditUsed=d.CreditUsed,
                                  ExpiredAmount=d.ExpiredAmount,
                                  WillExpireLbl = conCart[PageContentEnum.Text8.ToString()].Replace("{date}", d.ExpiryDate.ToString("dd MMMM yyyy")),
                                  IsExpired=System.DateTime.Now > d.ExpiryDate
                              };
                responseStatus.Data = tranHistory;


            }
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("referralandearn")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult DisplayReferralAndEarnScreen()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            int pageId = (int)PageNameEnum.MYWALLET;
            string pageVersion = "1.6.2";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var pageImages = content.PageImages.Select(o => new { Key = o.ImageKey, Value = o.ImageUrl }).ToDictionary(o => o.Key, o => o.Value);

            responseStatus.Data = new
            {
                Text1 = conCart[PageContentEnum.Text18.ToString()],
                Text2 = conCart[PageContentEnum.Text19.ToString()],
                Text3 = conCart[PageContentEnum.Text20.ToString()],
                Text4 = conCart[PageContentEnum.Text21.ToString()],
                Text5 = conCart[PageContentEnum.Text22.ToString()],
                ShareText = conCart[PageContentEnum.Text26.ToString()],
                Image1 = pageImages[PageImageEnum.Image3.ToString()],
                Howitworks = new
                {
                    Title = conCart[PageContentEnum.Text22.ToString()],
                    Details = new object[]
                    {
                        new {
                            Text = conCart[PageContentEnum.Text23.ToString()],
                            Default = 0
                        },
                        new
                        {
                            Text = conCart[PageContentEnum.Text24.ToString()],
                            Default = 0
                        },
                        new {
                            Text = conCart[PageContentEnum.Text25.ToString()],
                            Default = 0
                        }

                    },
                }
            };
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("cashback/{orderId}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetCashBack(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            
            var ser = SZIoc.GetSerivce<IPromoCodeService>();
            var cashBack = ser.CashBack(orderId);
            responseStatus.Data = cashBack;

            return Ok(responseStatus);

        }

        [HttpPost]
        [Route("payment-get-token")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage PaymentGetToken(CashFreeInput cashFreeInput)
        {
            HttpResponseMessage response = null;
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var db = new rainbowwineEntities();
            //var hostName = Request.RequestUri.GetLeftPart(UriPartial.Authority); 

            try
            {
                if (cashFreeInput == null)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Input object is null.";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
                else if (string.IsNullOrWhiteSpace(cashFreeInput.OrderNo))
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Input object order number is null.";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }

                Order order = null;
                decimal amt = 0;
                if (cashFreeInput.OrderNo.Contains("OG_"))
                {
                    var morder = db.Orders.Where(o => string.Compare(o.OrderGroupId, cashFreeInput.OrderNo, true) == 0);
                    if (morder != null && morder.Count() > 0)
                    {
                        foreach (var item in morder)
                        {
                            amt += item.OrderAmount;
                        }
                    }
                    int orderIdDecode = Convert.ToInt32(cashFreeInput.OrderNo.Replace("OG_", ""));
                    order = morder.Where(o => o.Id == orderIdDecode).FirstOrDefault();
                    if (order.WalletAmountUsed.HasValue && order.WalletAmountUsed > 0)
                    {
                        amt = amt - order.WalletAmountUsed.Value;
                    }
                    var promocode = db.PromoCodes.Where(o => o.PromoId == order.PromoId).FirstOrDefault();
                    var ser = SZIoc.GetSerivce<IPromoCodeService>();
                    if (order.PromoId.HasValue && order.PromoId > 0)
                    {
                        
                        var promodata = ser.PromoCodeApply(promocode.Code,(float)amt,order.Customer.UserId);
                        if (promodata.IsValid)
                        {
                            amt = amt - Convert.ToDecimal(promodata.DiscountAmount);

                        }
                    }
                }
                else
                {
                    order = db.Orders.Find(Convert.ToInt32(cashFreeInput.OrderNo));
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
               
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-client-id", ConfigurationManager.AppSettings["PayFreeId"]);
                    client.DefaultRequestHeaders.Add("x-client-secret", ConfigurationManager.AppSettings["PayKey"]);

                    var jsonValue = new
                    {
                        orderId = cashFreeInput.OrderNo,
                        orderAmount = amt,
                        orderCurrency = "INR"
                    };
                    var serializeJson = JsonConvert.SerializeObject(jsonValue);
                    var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                    var resp = client.PostAsync(ConfigurationManager.AppSettings["PayTokenUrl"], content).Result;
                    var ret = JsonConvert.DeserializeObject<object>(resp.Content.ReadAsStringAsync().Result);

                    response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Data = ret });
                }
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
        public HttpResponseMessage OrderPaymentCashFreeLog(CashFreePayment cashFreePayment)
        {
            if (cashFreePayment == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Data = cashFreePayment, Message = "Object is null.", Status = false });
            }
            else if (cashFreePayment.InputParam == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Data = cashFreePayment, Message = "Object input is null.", Status = false });
            }
            else if (cashFreePayment.VenderOutput == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Data = cashFreePayment, Message = "Object vender ouput is null.", Status = false });
            }
           
            HttpResponseMessage response = null;
            ResponseStatus responseStatus = new ResponseStatus();
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    CashFreePaymentResponse cashFreePaymentResponse = JsonConvert.DeserializeObject<CashFreePaymentResponse>(cashFreePayment.VenderOutput.Trim());
                    //CashFreeSetApproveResponse cashFreeSetApproveResponse = JsonConvert.DeserializeObject<CashFreeSetApproveResponse>(cashFreePayment.VenderOutput);
                    PaymentCashFreeLog paymentCashFreeLog = new PaymentCashFreeLog
                    {
                        InputParameters = cashFreePayment.InputParam,
                        VenderOutPut = cashFreePayment.VenderOutput,
                        CreatedDate = DateTime.Now,
                        OrderIdCF = cashFreePaymentResponse.orderId,
                        OrderAmount = cashFreePaymentResponse.orderAmount,
                        ReferenceId = cashFreePaymentResponse.referenceId,
                        Msg = cashFreePaymentResponse.txMsg,
                        PaymentMode = cashFreePaymentResponse.paymentMode,
                        Status = cashFreePaymentResponse.txStatus,
                        TxtTime = cashFreePaymentResponse.txTime,
                        Signature = cashFreePaymentResponse.signature,
                        MachineName = System.Environment.MachineName
                    };

                    int orderIdDecode = Convert.ToInt32(cashFreePaymentResponse.orderId.Replace("OG_", ""));
                    paymentCashFreeLog.OrderId = orderIdDecode;
                    db.PaymentCashFreeLogs.Add(paymentCashFreeLog);
                    db.SaveChanges();

                    var cashFreeSetApprove = new CashFreeSetApproveResponse
                    {
                        OrderId = cashFreePaymentResponse.orderId,
                        OrderAmount = cashFreePaymentResponse.orderAmount,
                        ReferenceId = cashFreePaymentResponse.referenceId,
                        Status = cashFreePaymentResponse.txStatus,
                        PaymentMode = cashFreePaymentResponse.paymentMode,
                        Msg = cashFreePaymentResponse.txMsg,
                        TxtTime = cashFreePaymentResponse.txTime,
                        Signature = cashFreePaymentResponse.signature
                    };
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = paymentLinkLogsService.UpdateWalletOrderToApprove(cashFreeSetApprove, cashFreePayment.VenderOutput);
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

        [HttpGet]
        [Route("usewalletamountonly/{orderId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage WhenUseWalletAmountOnly(string orderId)
        {
            HttpResponseMessage response = null;
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
            var ser = SZIoc.GetSerivce<IPromoCodeService>();
            var wallet = ser.WhenUseWalletAmountOnly(orderId);
            responseStatus.Data = wallet;
            
            if (wallet==1)
            {
                int orderIdDecode = 0;
                if (orderId.Contains("OG_"))
                {
                    var db = new rainbowwineEntities();
                    IList<Order> groupOrders = new List<Order>();
                    groupOrders = db.Orders.Where(o => string.Compare(o.OrderGroupId, orderId, true) == 0)?.ToList();
                    groupOrders.ForEach((o) =>
                    {
                        paymentLinkLogsService.InventoryUpdate(o.Id);
                        paymentLinkLogsService.InventoryMixerUpdate(o.Id);
                    });

                }
                else
                {
                    orderIdDecode = Convert.ToInt32(orderId);
                    paymentLinkLogsService.InventoryUpdate(orderIdDecode);
                    paymentLinkLogsService.InventoryMixerUpdate(orderIdDecode);
                }
                responseStatus.Status = true;
                response = Request.CreateResponse(HttpStatusCode.OK, responseStatus);
                

            }
            else
            {
                responseStatus.Status = false;
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
            }
            
            return response;
        }


        [HttpGet]
        [Route("walletnotify")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult WalletNotify()
        {

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var data = c.WalletNotify(Id);
                responseStatus.Data = data;

            }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("unregisterexistinguser/{mobilenumber}")]
        public HttpResponseMessage UnRegisterExistingUser(string mobileNumber)
        {
            HttpResponseMessage response = null;
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            var ser = SZIoc.GetSerivce<IPromoCodeService>();
            var wallet = ser.UnRegisterUser(mobileNumber);
            responseStatus.Data = wallet;

            if (wallet == 1)
            {
                responseStatus.Status = true;
                responseStatus.Message = "Successfully UnRegister";
                response = Request.CreateResponse(HttpStatusCode.OK, responseStatus);


            }
            else
            {
                responseStatus.Status = false;
                responseStatus.Message = "This is new user";
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
            }

            return response;
        }

        [HttpGet]
        [Route("referralcodeverification/{code}")]
        [AllowAnonymous]
        public IHttpActionResult ReferralCodeVerification( string code)
        {

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (code != null && code != "")
            {
                var c = SZIoc.GetSerivce<IPromoCodeService>();
                var referralCode = c.IsSignUpCode(code);
                if (referralCode.ValidCode == 1 || referralCode.ValidCode == 2 || referralCode.ValidCode == 3)
                {


                    responseStatus.Data = new
                    {
                        Message = "This is valid Code.",
                        IsValid = true
                    };
                }
                else
                {
                    responseStatus.Data = new
                    {
                        Message = "This is invalid/expired Code.",
                        IsValid = false
                       
                    };

                }
                }


            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("promopopupdetails")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetPromoPopupDetails()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var data = c.PromoPopupDetails(Id);
                responseStatus.Data = data;

            }

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("promopopupuserbinding/{promoPopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult PromoPopupUserBindings(int promoPopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var data = c.PromoPopupUserBindings(Id,promoPopId);
                responseStatus.Data = data;

            }

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("add-schedule-delivery")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddScheduleDelivery(ScheduleParameters scheduleParameters)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                if (scheduleParameters.DeliveryAgentId > 0)
                {
                    scheduleParameters.CustomerId = 0;
                }
                else
                {
                    scheduleParameters.CustomerId = custId.Id;
                }
                
                scheduleDeliveryDBO.AddScheduleDelivery(scheduleParameters);
                responseStatus.Message = "Your order has scheduled successfully.";
                responseStatus.Data = true;
            }
            return Ok(responseStatus);
        }


        [HttpGet]
        [Route("getinitconfig")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetInitConfig()
        {
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var DeliveryReachedDistCheckFlag = c.GetConfigValue(ConfigEnums.DeliveryReachedDistCheckFlag.ToString());
            var MinDistDeliveryReached = c.GetConfigValue(ConfigEnums.MinDistDeliveryReached.ToString());
            var AirtelIQUsername = c.GetConfigValue(ConfigEnums.AirtelIQUsername.ToString());
            var AirtelIQPassword = c.GetConfigValue(ConfigEnums.AirtelIQPassword.ToString());
            var Penalty_75minsAbove = c.GetConfigValue(ConfigEnums.Penalty_75minsAbove.ToString());

            
            var Data = new
            {
                DeliveryReachedDistCheckFlag,
                MinDistDeliveryReached,
                AirtelIQUsername,
                AirtelIQPassword,
                Penalty_75minsAbove
            };
            return Ok(Data);

        }

        [HttpGet]
        [Route("checkuser/{contactno}")]
        public IHttpActionResult CkeckCredibleCustomer(string contactNo)
        {
            CustomerDBO customerDBO = new CustomerDBO();
            var data= customerDBO.CkeckCredibleCustomer(contactNo);
            
            return Ok(data);

        }

        [HttpGet]
        [Route("addszcredittocustomer/{amount}/{contactno}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddSZCreditToCustomer(int amount, string contactNo)
        {
            CustomerDBO customerDBO = new CustomerDBO();
            var data = customerDBO.AddSZCreditToCustomer(amount,contactNo);

            return Ok(data);

        }

        [HttpPost]
        [Route("addcustomercontact")]
        public IHttpActionResult AddCustomerContact(CustomerContactDO customerContactDO)
        {
            var re =  Request;
            var headers = re.Headers;
            var token = headers.GetValues("X-SZ-Signature");
            var headerValue = token.FirstOrDefault();
            var secretKey = ConfigurationManager.AppSettings["CustContactSecretKey"].ToString();
            var payload = JsonConvert.SerializeObject(customerContactDO);
            var signature = HmacSHA256(payload, secretKey);
            string message = string.Empty;
            if (headerValue == signature)
            {
                CustomerDBO customerDBO = new CustomerDBO();
                var data = customerDBO.AddCustomerContactDetails(customerContactDO);
                if (data == 1)
                {
                    message = "Data inserted successfully";
                }
                else
                {
                    message="failed";
                }
            }
           


            return Ok(message);

        }

        [HttpPost]
        [Route("servicable-shop-zone/{servicabletype}")]
        public IHttpActionResult ServicableShopAndZone(string servicableType)
        {
            var re = Request;
            var headers = re.Headers;
            var message = string.Empty;
            var secretKey = ConfigurationManager.AppSettings["CustContactSecretKey"].ToString();
            if (servicableType == "Flag")
            {
                var token = headers.GetValues("X-SZ-Flag-Signature");
                var headerValue = token.FirstOrDefault();
                var c = SZIoc.GetSerivce<ISZConfiguration>();
                var serviceableShopChFlag = c.GetConfigValue(ConfigEnums.ServiceableShopChFlag.ToString());
                var serviceableZoneChFlag = c.GetConfigValue(ConfigEnums.ServiceableZoneChFlag.ToString());
                var signature = HmacSHA256(servicableType, secretKey);
                if (headerValue == signature)
                {
                   var data = new
                    {
                        ServiceableShopChFlag = serviceableShopChFlag,
                        ServiceableZoneChFlag = serviceableZoneChFlag

                    };
                    return Ok(data);
                }
                else
                {
                    message = "Flag Signature is mismatch";
                    return Ok(message); 
                }
            }
            else if (servicableType == "Shop")
            {
                var token = headers.GetValues("X-SZ-Shop-Signature");
                var headerValue = token.FirstOrDefault();
                var signature = HmacSHA256(servicableType, secretKey);
                if (headerValue == signature)
                {
                    CustomerDBO customerDBO = new CustomerDBO();
                    var data = customerDBO.SevicableShopAndZone(servicableType);
                    if (data != null)
                    {
                        var result = data.Select(a => new
                        {
                            Id = a.ShopID,
                            ShopName = a.ShopName,
                            Address = a.Address,
                            OperationalFlag = a.OperationalFlag
                        });
                        return Ok(result);
                    }
                   
                }
                else
                {
                    message = "Shop Signature is mismatch";
                    return Ok(message);
                }
            }
            else if (servicableType == "Zone")
            {
                var token = headers.GetValues("X-SZ-Zone-Signature");
                var headerValue = token.FirstOrDefault();
                var signature = HmacSHA256(servicableType, secretKey);
                if (headerValue == signature)
                {
                    CustomerDBO customerDBO = new CustomerDBO();
                    var data = customerDBO.SevicableShopAndZone(servicableType);
                    if (data != null)
                    {
                        var result = data.Select(a => new
                        {
                            ZoneID = a.ZoneID,
                            ZoneName = a.ZoneName,
                            ShopID = a.ShopID,
                            OperationalFlag = a.OperationalFlag
                        });
                        return Ok(result);
                    }

                }
                else
                {
                    message = "Zone Signature is mismatch";
                    return Ok(message);
                }
            }

            return Ok("Servicable type is not found");

        }


        [HttpPost]
        [Route("updconfigmstflag-shop-zone/{requestkey}")]
        public IHttpActionResult UpdateConfigMstFlagShopAndZone(string requestkey)
        {
            var re = Request;
            var headers = re.Headers;
            var message = string.Empty;
            var secretKey = ConfigurationManager.AppSettings["CustContactSecretKey"].ToString();
            CustomerDBO customerDBO = new CustomerDBO();
            if (requestkey == "Shop")
            {
                var token = headers.GetValues("X-SZ-Shop-Signature");
                var headerValue = token.FirstOrDefault();
                var signature = HmacSHA256(requestkey, secretKey);
                if (headerValue == signature)
                {
                    var data = customerDBO.UpdateConfigMstFlagShopAndZone(requestkey);
                    if (data == 1)
                    {
                        message = "ServiceableShopChFlag is deactivated successfully ";
                        return Ok(message);
                    }
                    else
                    {
                        message = "Alredy deactivated successfully ";
                        return Ok(message);
                    }
                }
                else
                {
                    message = "Flag Signature is mismatch";
                    return Ok(message);
                }
            }
            else if (requestkey == "Zone")
            {
                var token = headers.GetValues("X-SZ-Zone-Signature");
                var headerValue = token.FirstOrDefault();
                var signature = HmacSHA256(requestkey, secretKey);
                if (headerValue == signature)
                {
                  
                    var data = customerDBO.UpdateConfigMstFlagShopAndZone(requestkey);
                    if (data == 1)
                    {
                        message = "ServiceableZoneChFlag is deactivated successfully ";
                        return Ok(message);
                    }
                    else
                    {
                        message = "Alredy deactivated successfully ";
                        return Ok(message);
                    }
                   
                }
                else
                {
                    message = "Zone Signature is mismatch";
                    return Ok(message);
                }
            }

            return Ok("Request key is not found");

        }

        [HttpPost]
        [Route("add-goody/{goodyid}/{shopid}")]
        public IHttpActionResult AddGoodyInventory(int goodyid, int shopid)
        {
            string message = string.Empty;
            InventoryDBO inventoryDBO = new InventoryDBO();
            var res= inventoryDBO.AddGoodyInventory(goodyid, shopid);
            if (res == 1)
            {
                message = "Goody Added Successfully";
                return Ok(message);
            }
            else
            {
                message = "Failed";
                return Ok(message);
            }
           
        }

        [HttpPost]
        [Route("update-goody/{goodyid}/{shopid}")]
        public IHttpActionResult UpdateGoodyInventory(int goodyid, int shopid)
        {
            string message = string.Empty;
            InventoryDBO inventoryDBO = new InventoryDBO();
            var res = inventoryDBO.UpdateGoodyInventory(goodyid, shopid);
            if (res == 1)
            {
                message = "Goody Updated Successfully";
                return Ok(message);
            }
            else
            {
                message = "Failed";
                return Ok(message);
            }

        }

        [HttpPost]
        [Route("callfirestore/{orderId}")]
        public IHttpActionResult CallFireStore(int orderId)
        {
            using (var db = new rainbowwineEntities())
            {
                var c = SZIoc.GetSerivce<ISZConfiguration>();
                var trackToFirestore = c.GetConfigValue(ConfigEnums.TrackToFirestore.ToString());
                var isFirebaseCustAppOn = c.GetConfigValue(ConfigEnums.IsFirebaseCustAppOn.ToString());
                var isHyperTrackOn = c.GetConfigValue(ConfigEnums.IsHyperTrackOn.ToString());
                var isReferAndEarnHook = c.GetConfigValue(ConfigEnums.ReferAndEarnHook.ToString());
                if (trackToFirestore == "1")
                {
                    bool isStart = false;
                    bool canTrackAgentLoc = false;
                    bool ShowPrebookConfetti = false;

                    var routePlan = db.RoutePlans.Where(x => x.OrderID == orderId).OrderByDescending(a => a.id).FirstOrDefault();
                    if (routePlan != null && routePlan.IsOrderStart == true)
                    {
                        //isStart = true;
                        isStart = false;
                        canTrackAgentLoc = true;

                    }
                    //////Live Tracking ///
                    OrderDBO orderDBO = new OrderDBO();
                    LiveOrderTracking liveOrderTracking = new LiveOrderTracking();
                    liveOrderTracking = orderDBO.GetOrderDetailWithLocation(orderId);
                    List<OrderTrack> tracks = db.OrderTracks.Where(x => x.OrderId == orderId).ToList();
                    List<StatusChange> statusChange = new List<StatusChange>();
                    statusChange = orderDBO.GetManipulatedTracks(tracks, liveOrderTracking.OrderStatusId);
                    GiftBagDBO giftBagDBO = new GiftBagDBO();
                    HookDetails res = null;
                    if (isReferAndEarnHook == "1")
                    {
                        var app = orderDBO.GetAppVersion(orderId);
                        var numberOfOrder = c.GetConfigValue(ConfigEnums.NumberOfOrder.ToString());
                        var ordCount = db.Orders.Where(o => o.CustomerId == liveOrderTracking.CustomerId && o.OrderStatusId == 5);
                        bool isShowHook = false;
                        bool isAppVersionExist = false;
                        var appv = orderDBO.VerifyAppVersion(app.AppVersion, app.AppPlatForm);
                        if (ordCount.Count() > Convert.ToInt32(numberOfOrder))
                        {
                            isShowHook = true;

                        }
                        if (appv == 1)
                        {
                            isAppVersionExist = true;
                        }


                        if ((isAppVersionExist && isShowHook) || (isAppVersionExist && isShowHook))
                        {
                            var custAddId = db.Orders.Where(a => a.Id == orderId).FirstOrDefault().CustomerAddressId;
                            var zoneId = db.CustomerAddresses.Where(x => x.CustomerAddressId == custAddId).FirstOrDefault().ZoneId;
                            var hook = giftBagDBO.GetFeatureHookDetails(liveOrderTracking.ShopId, zoneId.Value, "ReferHook", "TrackingPage");
                            res = new HookDetails()
                            {
                                HookID = hook.FeatureHookId,
                                Title = hook.Title,
                                SubText = hook.SubText,
                                Icon = hook.Icon,
                                Payload = hook.Payload,
                                BgImage = hook.BgImage,
                                BgColors = new List<string>() { "#8e0033", "#dc004f" },
                                NeedCartClear=hook.NeedCartClear

                            };
                        }
                    }
                    if (liveOrderTracking.Prebook)
                    {
                        ShowPrebookConfetti = true;
                    }
                    if (liveOrderTracking.DeliveryAgentId > 0)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            DeliveryAgentLocation deliveryAgentLocation = new DeliveryAgentLocation();
                            var resp = client.GetAsync(ConfigurationManager.AppSettings["AgentLocation"] + "/" + liveOrderTracking.DeliveryAgentId).Result;
                            deliveryAgentLocation = JsonConvert.DeserializeObject<DeliveryAgentLocation>(resp.Content.ReadAsStringAsync().Result);

                            if (deliveryAgentLocation != null)
                            {
                                liveOrderTracking.Longitude = deliveryAgentLocation.Longitude;
                                liveOrderTracking.Latitude = deliveryAgentLocation.Latitude;
                            }
                        }

                    }
                    bool IsCompleted = false;
                    if (liveOrderTracking.StatusName == "Delivered" || liveOrderTracking.StatusName == "IssueRefunded" || liveOrderTracking.StatusName == "WalletRefunded" || liveOrderTracking.StatusName == "CustomerCancelled")
                    {
                        IsCompleted = true;
                    }
                    OrderFireStore orderFireStore = new OrderFireStore();
                    orderFireStore.OrderId = orderId;
                    orderFireStore.OrderStatus = liveOrderTracking.StatusName;
                    orderFireStore.ShopID = liveOrderTracking.ShopId;
                    orderFireStore.ShopLoc = new GeoPoint(longitude: liveOrderTracking.ShopLongitude, latitude: liveOrderTracking.ShopLatitude);
                    orderFireStore.CustomerId = liveOrderTracking.CustomerId;
                    orderFireStore.CustomerLoc = new GeoPoint(longitude: liveOrderTracking.CustLongitude, latitude: liveOrderTracking.CustLatitude);
                    orderFireStore.AgentId = liveOrderTracking.DeliveryAgentId;
                    //orderFireStore.AgentLoc = new GeoPoint(longitude: liveOrderTracking.Longitude.Value, latitude: liveOrderTracking.Latitude.Value);
                    orderFireStore.AgentLoc =
                            new GeoPoint(liveOrderTracking.Latitude.HasValue ? liveOrderTracking.Latitude.Value : 0,
                            liveOrderTracking.Longitude.HasValue ? liveOrderTracking.Longitude.Value : 0);
                    orderFireStore.Timestamp = Timestamp.GetCurrentTimestamp();
                    orderFireStore.CompletedFlag = IsCompleted;
                    orderFireStore.isOrderStart = isStart;
                    orderFireStore.OrderAmount = liveOrderTracking.OrderAmount;
                    orderFireStore.AgentContactNo = liveOrderTracking.AgentContactNo;
                    orderFireStore.PaymentTypeId = liveOrderTracking.PaymentTypeId;
                    orderFireStore.OrderStatusId = liveOrderTracking.OrderStatusId;
                    orderFireStore.IsFirebaseCustAppOn = isFirebaseCustAppOn == "1" ? true : false;
                    orderFireStore.IsHyperTrackOn = isHyperTrackOn == "1" ? true : false;
                    orderFireStore.HyperTrackDeviceId = !string.IsNullOrEmpty(liveOrderTracking.HyperTrackDeviceId) ? liveOrderTracking.HyperTrackDeviceId : string.Empty;
                    orderFireStore.TripId = !string.IsNullOrEmpty(liveOrderTracking.TripId) ? liveOrderTracking.TripId : string.Empty;
                    orderFireStore.CanTrackAgentLoc = canTrackAgentLoc;
                    orderFireStore.ShowPrebookConfetti = ShowPrebookConfetti;
                    orderFireStore.PrebookConfettiTitle = ShowPrebookConfetti == true ? ConfigurationManager.AppSettings["PrebookConfettiTitle"] : "";
                    orderFireStore.PrebookConfettiBody = ShowPrebookConfetti == true ? ConfigurationManager.AppSettings["PrebookConfettiBody"] : "";
                    orderFireStore.TotalItemQty = liveOrderTracking.TotalItemQty + " " + "item(s)";
                    orderFireStore.IsScheduledOrder = liveOrderTracking.IsScheduledOrder;
                    orderFireStore.SchDeliveryText = $"{ConfigurationManager.AppSettings["SchDeliveryText"]} {liveOrderTracking.DeliveryScheduledStartDate.ToString("MMM dd")} {"between"} {liveOrderTracking.DeliveryScheduledStartDate.ToString("hh:mm tt")} {"to"} {liveOrderTracking.DeliveryScheduledEndDate.ToString("hh:mm tt")}";
                    //orderFireStore.FeatureHook = res;
                    FireStoreAccess fireStoreAccess = new FireStoreAccess();
                    fireStoreAccess.AddToFireStore(orderFireStore, statusChange, res);
                }
                return Ok();
            }
        }

        #region Non Action
        public static void AddToFireStore(int orderId)
        {
            using (var db = new rainbowwineEntities())
            {
                var c = SZIoc.GetSerivce<ISZConfiguration>();
                var trackToFirestore = c.GetConfigValue(ConfigEnums.TrackToFirestore.ToString());
                var isFirebaseCustAppOn = c.GetConfigValue(ConfigEnums.IsFirebaseCustAppOn.ToString());
                var isHyperTrackOn = c.GetConfigValue(ConfigEnums.IsHyperTrackOn.ToString());
                var isReferAndEarnHook = c.GetConfigValue(ConfigEnums.ReferAndEarnHook.ToString());
                if (trackToFirestore == "1")
                {
                    bool isStart = false;
                    bool canTrackAgentLoc = false;
                    bool ShowPrebookConfetti = false;
                   
                    var routePlan = db.RoutePlans.Where(x => x.OrderID == orderId).OrderByDescending(a =>a.id).FirstOrDefault();
                    if (routePlan !=null && routePlan.IsOrderStart==true)
                    {
                        //isStart = true;
                        isStart = false;
                        canTrackAgentLoc = true;

                    }
                    //////Live Tracking ///
                    OrderDBO orderDBO = new OrderDBO();
                    LiveOrderTracking liveOrderTracking = new LiveOrderTracking();
                    liveOrderTracking = orderDBO.GetOrderDetailWithLocation(orderId);
                    List<OrderTrack> tracks = db.OrderTracks.Where(x => x.OrderId == orderId).ToList();
                    List<StatusChange> statusChange = new List<StatusChange>();
                    statusChange = orderDBO.GetManipulatedTracks(tracks, liveOrderTracking.OrderStatusId);
                    GiftBagDBO giftBagDBO = new GiftBagDBO();
                    HookDetails res =null ;
                    if (isReferAndEarnHook == "1")
                    {
                        var app = orderDBO.GetAppVersion(orderId);
                        var numberOfOrder = c.GetConfigValue(ConfigEnums.NumberOfOrder.ToString());
                        var ordCount = db.Orders.Where(o => o.CustomerId == liveOrderTracking.CustomerId && o.OrderStatusId == 5);
                        bool isShowHook = false;
                        bool isAppVersionExist = false;
                        var appv = orderDBO.VerifyAppVersion(app.AppVersion ,app.AppPlatForm);
                        if (ordCount.Count() > Convert.ToInt32(numberOfOrder))
                        {
                            isShowHook = true;

                        }
                        if (appv == 1)
                        {
                            isAppVersionExist = true;
                        }


                        if ((isAppVersionExist && isShowHook) || (isAppVersionExist && isShowHook))
                        {
                            var custAddId = db.Orders.Where(a => a.Id == orderId).FirstOrDefault().CustomerAddressId;
                            var zoneId = db.CustomerAddresses.Where(x => x.CustomerAddressId == custAddId).FirstOrDefault().ZoneId;
                            var hook = giftBagDBO.GetFeatureHookDetails(liveOrderTracking.ShopId, zoneId.Value, "ReferHook", "TrackingPage");
                            res = new HookDetails()
                            {
                                HookID = hook.FeatureHookId,
                                Title = hook.Title,
                                SubText = hook.SubText,
                                Icon = hook.Icon,
                                Payload = hook.Payload,
                                BgImage = hook.BgImage,
                                BgColors = new List<string>() { "#8e0033", "#dc004f" },
                                NeedCartClear = hook.NeedCartClear
                            };
                        }
                    }
                    if (liveOrderTracking.Prebook)
                    {
                        ShowPrebookConfetti = true;
                    }
                    if (liveOrderTracking.DeliveryAgentId > 0)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            DeliveryAgentLocation deliveryAgentLocation = new DeliveryAgentLocation();
                            var resp = client.GetAsync(ConfigurationManager.AppSettings["AgentLocation"] + "/" + liveOrderTracking.DeliveryAgentId).Result;
                            deliveryAgentLocation = JsonConvert.DeserializeObject<DeliveryAgentLocation>(resp.Content.ReadAsStringAsync().Result);

                            if (deliveryAgentLocation != null)
                            {
                                liveOrderTracking.Longitude = deliveryAgentLocation.Longitude;
                                liveOrderTracking.Latitude = deliveryAgentLocation.Latitude;
                            }
                        }

                    }
                    bool IsCompleted = false;
                    if (liveOrderTracking.StatusName == "Delivered" || liveOrderTracking.StatusName == "IssueRefunded" || liveOrderTracking.StatusName == "WalletRefunded" || liveOrderTracking.StatusName == "CustomerCancelled")
                    {
                        IsCompleted = true;
                    }
                    OrderFireStore orderFireStore = new OrderFireStore();
                    orderFireStore.OrderId = orderId;
                    orderFireStore.OrderStatus = liveOrderTracking.StatusName;
                    orderFireStore.ShopID = liveOrderTracking.ShopId;
                    orderFireStore.ShopLoc = new GeoPoint(longitude: liveOrderTracking.ShopLongitude, latitude: liveOrderTracking.ShopLatitude);
                    orderFireStore.CustomerId = liveOrderTracking.CustomerId;
                    orderFireStore.CustomerLoc = new GeoPoint(longitude: liveOrderTracking.CustLongitude, latitude: liveOrderTracking.CustLatitude);
                    orderFireStore.AgentId = liveOrderTracking.DeliveryAgentId;
                    //orderFireStore.AgentLoc = new GeoPoint(longitude: liveOrderTracking.Longitude.Value, latitude: liveOrderTracking.Latitude.Value);
                    orderFireStore.AgentLoc =
                            new GeoPoint(liveOrderTracking.Latitude.HasValue ? liveOrderTracking.Latitude.Value : 0,
                            liveOrderTracking.Longitude.HasValue ? liveOrderTracking.Longitude.Value : 0);
                    orderFireStore.Timestamp = Timestamp.GetCurrentTimestamp();
                    orderFireStore.CompletedFlag = IsCompleted;
                    orderFireStore.isOrderStart = isStart;
                    orderFireStore.OrderAmount = liveOrderTracking.OrderAmount;
                    orderFireStore.AgentContactNo = liveOrderTracking.AgentContactNo;
                    orderFireStore.PaymentTypeId = liveOrderTracking.PaymentTypeId;
                    orderFireStore.OrderStatusId = liveOrderTracking.OrderStatusId;
                    orderFireStore.IsFirebaseCustAppOn = isFirebaseCustAppOn == "1" ? true : false;
                    orderFireStore.IsHyperTrackOn =isHyperTrackOn == "1" ? true : false;
                    orderFireStore.HyperTrackDeviceId =!string.IsNullOrEmpty(liveOrderTracking.HyperTrackDeviceId) ? liveOrderTracking.HyperTrackDeviceId : string.Empty;
                    orderFireStore.TripId = !string.IsNullOrEmpty(liveOrderTracking.TripId) ? liveOrderTracking.TripId : string.Empty;
                    orderFireStore.CanTrackAgentLoc = canTrackAgentLoc;
                    orderFireStore.ShowPrebookConfetti = ShowPrebookConfetti;
                    orderFireStore.PrebookConfettiTitle = ShowPrebookConfetti == true ? ConfigurationManager.AppSettings["PrebookConfettiTitle"] : "";
                    orderFireStore.PrebookConfettiBody = ShowPrebookConfetti == true ? ConfigurationManager.AppSettings["PrebookConfettiBody"]  : "";
                    orderFireStore.TotalItemQty = liveOrderTracking.TotalItemQty + " " + "item(s)";
                    orderFireStore.IsScheduledOrder = liveOrderTracking.IsScheduledOrder;
                    orderFireStore.SchDeliveryText = $"{ConfigurationManager.AppSettings["SchDeliveryText"]} {liveOrderTracking.DeliveryScheduledStartDate.ToString("MMM dd")} {"between"} {liveOrderTracking.DeliveryScheduledStartDate.ToString("hh:mm tt")} {"to"} {liveOrderTracking.DeliveryScheduledEndDate.ToString("hh:mm tt")}";
                    //orderFireStore.FeatureHook = res;
                    FireStoreAccess fireStoreAccess = new FireStoreAccess();
                    fireStoreAccess.AddToFireStore(orderFireStore, statusChange ,res);
                }
            }
        }

        public static void DeleteToFireStore(int orderId)
        {
            using (var db = new rainbowwineEntities())
            {
                var c = SZIoc.GetSerivce<ISZConfiguration>();
                var trackToFirestore = c.GetConfigValue(ConfigEnums.TrackToFirestore.ToString());
                if (trackToFirestore == "1")
                {
                    //////Live Tracking ///
                    OrderDBO orderDBO = new OrderDBO();
                    LiveOrderTracking liveOrderTracking = new LiveOrderTracking();
                    liveOrderTracking = orderDBO.GetOrderDetailWithLocation(orderId);
                    FireStoreAccess fireStoreAccess = new FireStoreAccess();
                    if (liveOrderTracking.StatusName == "Delivered" || liveOrderTracking.StatusName == "IssueRefunded" || liveOrderTracking.StatusName == "WalletRefunded" || liveOrderTracking.StatusName == "CustomerCancelled")
                    {
                        fireStoreAccess.DeleteDocfromFireStore(orderId);
                    }
                }
            }
        }

        public static string HmacSHA256(string payload, string secretKey)
        {
            string hash;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] code = encoding.GetBytes(secretKey);
            using (HMACSHA256 hmac = new HMACSHA256())
            {
                hmac.Key = code;
                Byte[] hmBytes = hmac.ComputeHash(encoding.GetBytes(payload));
                hash = ToHexString(hmBytes);
            }
            return hash;
        }

        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
        #endregion
    }
}
