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
using System.Threading.Tasks;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/sp2")]
    [Authorize]
    [DisplayName("Operational")]
    [EnableCors("*", "*", "*")]
    public class UserCustomerV2Controller : ApiController
    {

        [HttpGet]
        [Route("shop")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetShop()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                string custId = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var shop = db.WineShops.ToList();
                responseStatus.Data = shop;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("shop/{id}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetShop(int id)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var shop = db.WineShops.Where(o => o.Id == id).ToList();
                responseStatus.Data = shop;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("product/{id}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetProduct(int id)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var prod = db.ProductDetails.Where(o => o.ProductID == id).ToList();
                responseStatus.Data = prod;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("product-by-shop/{shopId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetProductByShop(int shopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                //var prod = db.Inventories.Where(o => o.ShopID == shopId).Select(o => o.ProductDetail).ToList();
                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetProductDetails(shopId);
                responseStatus.Data = prod;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("product-search")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetProductSearch(ProductSearchViewModel search)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //using (var db = new rainbowwineEntities())
            //{
            //    db.Configuration.ProxyCreationEnabled = false;
            //var prod = db.Inventories.Where(o => o.ShopID == search.ShopId).Select(o => o.ProductDetail);
            ProductDBO productDBO = new ProductDBO();
            var prod = productDBO.GetProductDetails(search.ShopId);
            if (search.CategoryId > 0)
            {
                var prod1 = prod.Where(o => o.ProductCategoryID == search.CategoryId).ToList();
                prod = prod1;
            }
            if (search.BrandId > 0)
            {
                var prod1 = prod.Where(o => o.ProductBrandID == search.BrandId).ToList();
                prod = prod1;
            }
            if (!string.IsNullOrWhiteSpace(search.ProductName))
            {
                var prod1 = prod.Where(o => o.ProductName.Contains(search.ProductName)).ToList();
                prod = prod1;
            }
            responseStatus.Data = prod.ToList();
            //}
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("product-filter")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetFilteredProductSearch(ProductSearchViewModel search)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (search == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Required object is null." });
            }
            if (search.ShopId < 1 || search.CategoryIds.Count() == 0 || search.PriceStart < 0 || search.PriceEnd < 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Required object parameter are null." });
            }
            ProductDBO productDBO = new ProductDBO();
            var prod = productDBO.GetFilteredProductDetails(search.ShopId, search.CategoryIds, search.PriceStart, search.PriceEnd);
            responseStatus.Data = prod.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("product-filter-start")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetFilteredStartProductSearch(ProductSearchViewModel search)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (search == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Required object is null." });
            }
            if (search.ShopId < 1)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Required object parameter are null." });
            }
            ProductDBO productDBO = new ProductDBO();
            var prod = productDBO.GetFilteredStart(search.ShopId, search.PriceStart, search.PriceEnd);
            responseStatus.Data = prod;
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("product-all-search")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetAllsearchProductSearch(ProductSearchViewModel search)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (search == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Required object is null." });
            }
            if (search.ShopId < 1 || (string.IsNullOrWhiteSpace(search.SearchText)))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Required object parameter are null." });
            }
            ProductDBO productDBO = new ProductDBO();
            var prod = productDBO.GetAllSearchProductDetails(search.ShopId, search.SearchText);
            responseStatus.Data = prod.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("product-search-volume/shop/{shopId}/product/{productRefId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetProductSearch(int shopId, int productRefId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            ProductDBO productDBO = new ProductDBO();
            var prod = productDBO.GetProductvolumnById(shopId, productRefId);

            responseStatus.Data = prod.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("get-all-brands")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetAllBrand()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var brands = db.ProductBrands.Where(o => !o.IsDeleted);
                responseStatus.Data = brands.ToList();
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }


        [HttpGet]
        [Route("get-popular-brands")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetPopularBrand()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var brands = db.ProductBrands.Where(o => !o.IsDeleted && o.IsPopular).OrderBy(o => o.OrderBy);
                responseStatus.Data = brands.ToList();
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }


        [HttpGet]
        [Route("get-categories")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetCategories()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var cats = db.ProductCategories.Where(o => o.IsForFilter);
                responseStatus.Data = cats.ToList();
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("product/recommended/{shopId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetRecommendedProduct(int shopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                ProductDBO productDBO = new ProductDBO();
                var recom = productDBO.GetRecomProductDetails(shopId);
                responseStatus.Data = recom.ToList();
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("stock/shop/{shopId}/product/{prodId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetProduct(int shopId, int prodId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //using (var db = new rainbowwineEntities())
            //{
            //    db.Configuration.ProxyCreationEnabled = false;
            //    var invent = db.Inventories.Where(o => o.ProductID == prodId && o.ShopID == shopId).ToList();
            //    responseStatus.Data = invent;
            //}
            ProductDBO productDBO = new ProductDBO();
            var prod = productDBO.GetProductDetailsById(shopId, prodId);
            responseStatus.Data = prod;
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("stock/shop-product")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetFavoriteProduct(StockFavorite stockFavorite)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //using (var db = new rainbowwineEntities())
            //{
            //    List<Inve>
            //    db.Configuration.ProxyCreationEnabled = false;
            //    var invent = db.Inventories.Where(o => o.ProductID == prodId && o.ShopID == shopId).ToList();
            //    responseStatus.Data = invent;
            //}
            ProductDBO productDBO = new ProductDBO();
            var allProducts = new List<ProductDetailDO>();
            foreach (var item in stockFavorite.Favorites)
            {
                var prod = productDBO.GetProductDetailsById(item.ShopId, item.ProductId);
                if (prod.Count > 0) allProducts = allProducts.Concat(prod).ToList();
            }
            responseStatus.Data = allProducts;
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("user-order")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetCustomerOrder()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                //var order = db.Orders.Where(o => o.Customer.UserId == uId).ToList();
                var cust = db.Customers.Where(o => o.UserId == uId).FirstOrDefault();
                CustomerDBO customerDBO = new CustomerDBO();
                var order = customerDBO.GetOrders(cust.Id);
                responseStatus.Data = order;

            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        public static APIOrder InventoryCheck(IList<APIOrderDetails> items)
        {
            APIOrder aPIOrder = new APIOrder();

            decimal totalAmt = 0;
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    foreach (var item in items)
                    {
                        int q = item.Qty;
                        decimal p = item.Price;
                        decimal t = q * p;
                        totalAmt += t;

                        var invent = db.Inventories.Where(o => (o.ProductID == item.ProductId) && (o.ShopID == item.ShopID))?.FirstOrDefault();
                        if (invent != null)
                        {
                            if (invent.QtyAvailable < item.Qty)
                            {
                                aPIOrder.OrderItems.Add(new APIOrderDetails { ProductId = item.ProductId, ShopID = item.ShopID, Qty = invent.QtyAvailable });
                            }
                        }
                        else
                        {
                            aPIOrder.OrderItems.Add(new APIOrderDetails { ProductId = item.ProductId, ShopID = item.ShopID, Message = "Inventory not present for the selected product." });
                        }
                    }
                    aPIOrder.TotalAmount = totalAmt;
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_InventoryCheck: {ex.Message}", ex.StackTrace);
                }
                finally
                {
                    db.Dispose();
                }
            }
            return aPIOrder;
        }
        public static APIOrder InventoryMixerCheck(IList<APIMixerItem> items)
        {
            var c = SZIoc.GetSerivce<IPromoCodeService>();
            
            APIOrder aPIOrder = new APIOrder();

            double totalAmt = 0;
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    foreach (var item in items)
                    {
                        var data = c.ValidatePromoCodeByProduct(item.MixerDetailId);
                        int q = item.Qty;
                        double p = item.Price;
                        double t = q * p;
                        totalAmt += t;

                        var invent = db.InventoryMixers.Where(o => (o.MixerDetailId == item.MixerDetailId) && (o.ShopId == item.ShopID))?.FirstOrDefault();
                        if (invent == null)
                        {
                            invent = db.InventoryMixers.Where(o => (o.MixerDetailId == item.MixerDetailId))?.FirstOrDefault();
                        }

                        if (invent != null)
                        {
                            if (invent.Qty < item.Qty)
                            {
                                aPIOrder.MixerItems.Add(new APIMixerItem { MixerDetailId = item.MixerDetailId, SupplierId = item.SupplierId, Qty = invent.Qty??0 });
                            }
                        }
                        else
                        {
                            aPIOrder.MixerItems.Add(new APIMixerItem { MixerDetailId = item.MixerDetailId, SupplierId = item.SupplierId, Message = "Inventory not present for the selected product." });
                        }
                    }
                    aPIOrder.TotalAmount = Convert.ToDecimal(totalAmt);
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_MixerInventoryCheck: {ex.Message}", ex.StackTrace);
                }
                finally
                {
                    db.Dispose();
                }
            }
            return aPIOrder;
        }

        [HttpPost]
        [Route("add-order")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage AddOrderByLoginUserV2(APIOrder model)
        {
            if (model == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Failed to add order. Please try again.", Data = model });
            else if (model.AddressId <= 0)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Address field cannot be blank.", Data = model });
            else if (model.OrderItems == null || model.OrderItems.Count <= 0)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Your cart is empty", Data = model });

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            decimal intconfigAmtTotal = Convert.ToDecimal(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);

            var inventItem = InventoryCheck(model.OrderItems);
            if (inventItem.TotalAmount < intconfigAmtTotal)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = $"Minimum total amount of order is {intconfigAmtTotal} rs." });
            }
            if (inventItem.OrderItems.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Inventory issue.", ErrorType = "inventory", Data = new { orderId = 0, rejectedItems = inventItem } });
            }

            using (var db = new rainbowwineEntities())
            {
                int custAdress = model.AddressId;
                var aspUser = db.AspNetUsers.Where(o => o.Id == uId)?.FirstOrDefault();
                Customer customer = db.Customers.Where(o => o.UserId == uId)?.FirstOrDefault();
                string userName = aspUser.Email;
                CustomerAddress customerAddress = db.CustomerAddresses.Where(o => o.CustomerId == customer.Id && o.CustomerAddressId == custAdress)?.FirstOrDefault();

                bool operationFlag = db.WineShops.Find(customerAddress.ShopId)?.OperationFlag ?? false;

                if (!operationFlag)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Unfortunately, the selected shop is not operating at the moment. Please choose another shop.", Data = model });

                bool testorder = (ConfigurationManager.AppSettings["TestOrder"] == "1") ? true : false;

                db.Configuration.ProxyCreationEnabled = false;
                Order order = null;
                OrderTrack orderTrack = null;
                //using (var transaction = db.Database.BeginTransaction())
                //{
                int stPayid = (int)OrderPaymentType.POO;
                try
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
                        PaymentTypeId = stPayid
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
                    decimal totalAmt = 0;
                    foreach (var item in model.OrderItems)
                    {
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
                    //OrderTrackingLog(order.Id, uId, User.Identity.Name, 2);
                    int configPremitValue = Convert.ToInt32(ConfigurationManager.AppSettings["PremitValue"]);
                    int premitVlaue = (string.IsNullOrWhiteSpace(model.PremitNo)) ?
                        configPremitValue : 0;

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

                    //transaction.Commit();
                }
                catch (Exception ex)
                {
                    //transaction.Rollback();
                    //transaction.Dispose();
                    responseStatus.Status = false;
                    responseStatus.Message = ex.Message;

                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"ApiError_add-order_Transaction:{ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();
                    db.Dispose();
                    SpiritUtility.AppLogging($"Api_add-order_Transaction: {ex.Message}", ex.StackTrace);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
                if (order != null && order.Id > 0)
                {
                    try
                    {
                        responseStatus.Data = new { orderId = order.Id, rejectedItems = new List<APIOrderDetails>() };
                        //WSendSMS wsms = new WSendSMS();
                        //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSSubmitted"], order.Id.ToString());
                        //wsms.SendMessage(textmsg, customer.ContactNo);


                        //Update orderid to Customer ETA 
                        var orderETAobject = db.CustomerEtas.Where(i => i.CustomerId == customer.Id);
                        if (orderETAobject != null && orderETAobject.Count() > 0)
                        {
                            var orderETA = orderETAobject.OrderByDescending(j => j.Id).Take(1).FirstOrDefault();
                            orderETA.OrderId = order.Id;
                            db.SaveChanges();
                        }

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
                }
                else
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Your order hasn’t been placed yet. Please try again.";
                }
            }

            //}
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("order/{orderId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetOrder(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //db.Configuration.ProxyCreationEnabled = false;
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var order = db.Orders.Include(o => o.OrderDetails).Where(o => o.Id == orderId).ToList();
                var oSerialize = JsonConvert.SerializeObject(order, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                var deSerialize = JsonConvert.DeserializeObject<List<Order>>(oSerialize);
                responseStatus.Data = deSerialize;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }


        [HttpPost]
        [Route("order-status/{orderId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetOrderStatus(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //db.Configuration.ProxyCreationEnabled = false;
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var track = db.OrderTracks.Include(o => o.OrderStatu).Where(o => o.OrderId == orderId).ToList();
                var oSerialize = JsonConvert.SerializeObject(track, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                var deSerialize = JsonConvert.DeserializeObject<List<OrderTrack>>(oSerialize);
                responseStatus.Data = deSerialize;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("orderdetail/{orderId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetOrderDetail(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var orderdetail = db.OrderDetails.Where(o => o.OrderId == orderId).ToList();
                responseStatus.Data = orderdetail;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("feature-product")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetFeatureProduct()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //db.Configuration.ProxyCreationEnabled = false;
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var order = db.ProductDetails.Where(o => (o.IsFeature ?? false)).ToList();
                if (order == null)
                    Request.CreateResponse(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "There is no features product." });
                responseStatus.Data = order;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }


        [HttpPost]
        [Route("orders-track/{orderId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage OrderTrack(int orderId)
        {
            if (orderId <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = false, Message = "Order id should be greater than 0." });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                List<object> list = new List<object>();
                string uId = User.Identity.GetUserId();
                var order = db.Orders.Find(orderId); //db.Orders.Include(o => o.WineShop).Where(o => o.Customer.UserId == uId && o.OrderStatusId==6).FirstOrDefault();
                var routeOrder = db.RoutePlans.Where(o => o.OrderID == order.Id).FirstOrDefault();
                if (routeOrder != null)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                    var routePlans = routePlanDBO.DevlieryOrders(routeOrder.DeliveryAgentId ?? 0, 6);
                    var delAgent = db.DeliveryAgents.Find(routeOrder.DeliveryAgentId);

                    foreach (var item in routePlans)
                    {
                        var custAddress = db.CustomerAddresses.Find(item.Order.CustomerAddressId);
                        var orderWine = db.Orders.Include(o => o.Customer).Include(o => o.WineShop).Where(o => o.Id == item.OrderID).FirstOrDefault();
                        string cNumber = orderWine.Customer?.ContactNo ?? orderWine.OrderTo;
                        list.Add(new
                        {
                            AgentId = routeOrder.DeliveryAgentId,
                            AgentNo = delAgent.Contact,
                            JobId = item.JobId,
                            OrderId = item.OrderID,
                            CustomerId = orderWine.Customer.Id,
                            CustomerNo = orderWine.Customer.ContactNo,
                            AddressId = custAddress.CustomerAddressId,
                            Latitude = custAddress.Latitude,
                            Longitude = custAddress.Longitude,
                            Address = custAddress.Address,
                            FormattedAddress = custAddress.FormattedAddress,
                            Flat = custAddress.Flat,
                            Landmark = custAddress.Landmark,
                            ShopId = orderWine.WineShop.Id,
                            ShopName = orderWine.WineShop.ShopName,
                            ShopLatitude = orderWine.WineShop.Latitude,
                            ShopLongitude = orderWine.WineShop.Longitude
                        });
                    }
                }
                responseStatus.Data = new { orderTrackAddress = list };
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
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
            //else if (cashFreePayment.OrderId <= 0)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus { Data = cashFreePayment, Message = "Object order id is null.", Status = false });
            //}
            HttpResponseMessage response = null;
            ResponseStatus responseStatus = new ResponseStatus();
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    CashFreePaymentResponse cashFreePaymentResponse = JsonConvert.DeserializeObject<CashFreePaymentResponse>(cashFreePayment.VenderOutput);
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

                    int orderIdDecode = Convert.ToInt32(cashFreePaymentResponse.orderId.Replace("OG_",""));
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
                    var ret = paymentLinkLogsService.UpdateOrderToApprove(cashFreeSetApprove, cashFreePayment.VenderOutput);
                    responseStatus.Data = ret;
                    responseStatus.Status = true;
                    response = Request.CreateResponse(HttpStatusCode.OK, responseStatus);
                    //if (string.Compare(paymentCashFreeLog.Status, "SUCCESS", true) == 0)
                    //{
                    //    AppLogsCashFreeHook appLogsCashFreeHook = new AppLogsCashFreeHook
                    //    {
                    //        CreatedDate = DateTime.Now,
                    //        VenderInput = cashFreePayment.VenderOutput,
                    //        OrderId = cashFreePaymentResponse.orderId,
                    //        OrderAmount = cashFreePaymentResponse.orderAmount,
                    //        ReferenceId = cashFreePaymentResponse.referenceId,
                    //        Status = cashFreePaymentResponse.txStatus,
                    //        PaymentMode = cashFreePaymentResponse.paymentMode,
                    //        Msg = cashFreePaymentResponse.txMsg,
                    //        TxtTime = cashFreePaymentResponse.txTime,
                    //        Signature = cashFreePaymentResponse.signature,
                    //        MachineName = System.Environment.MachineName
                    //    };
                    //    db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
                    //    db.SaveChanges();
                    //    Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();
                    //    decimal amt = Convert.ToDecimal(paymentCashFreeLog.OrderAmount);

                    //    if (order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
                    //    {
                    //        order.OrderStatusId = 3;
                    //        db.SaveChanges();

                    //        var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                    //        OrderTrack orderTrack = new OrderTrack
                    //        {
                    //            LogUserName = u.Email,
                    //            LogUserId = u.Id,
                    //            OrderId = order.Id,
                    //            StatusId = order.OrderStatusId,
                    //            TrackDate = DateTime.Now
                    //        };
                    //        db.OrderTracks.Add(orderTrack);
                    //        db.SaveChanges();

                    //        paymentCashFreeLog.SendStatus = "Approved";
                    //        appLogsCashFreeHook.SendStatus = "Approved";
                    //        db.SaveChanges();

                    //        WSendSMS wsms = new WSendSMS();
                    //        //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                    //        string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                    //        wsms.SendMessage(textmsg, order.Customer.ContactNo);

                    //        responseStatus.Status = true;
                    //        responseStatus.Message = $"Order is Approved";
                    //        //responseStatus.Data = order;
                    //        response = Request.CreateResponse(HttpStatusCode.OK, responseStatus);
                    //    }
                    //    else
                    //    {
                    //        if (order.OrderAmount == amt)
                    //        {
                    //            paymentCashFreeLog.SendStatus = "AmtNotMatch";
                    //            appLogsCashFreeHook.SendStatus = "AmtNotMatch";
                    //        }
                    //        else if (order.OrderStatusId == 2 || order.OrderStatusId == 16)
                    //        {
                    //            paymentCashFreeLog.SendStatus = $"OrderNotStatus";
                    //            appLogsCashFreeHook.SendStatus = $"OrderNotStatus";
                    //        }
                    //        else
                    //        {
                    //            paymentCashFreeLog.SendStatus = "ShouldCheck";
                    //            appLogsCashFreeHook.SendStatus = "ShouldCheck";
                    //        }


                    //        db.SaveChanges();
                    //        PaytmPayment paytm = new PaytmPayment();
                    //        int reStatus = paytm.PaytmRefundApiCall(order.Id, true);

                    //        response = Request.CreateResponse(HttpStatusCode.OK, responseStatus);
                    //    }
                    //}
                    //else
                    //{
                    //    paymentCashFreeLog.SendStatus = $"Cashfree St {paymentCashFreeLog.Status}";
                    //    db.SaveChanges();
                    //    response = Request.CreateResponse(HttpStatusCode.OK, responseStatus);
                    //}


                }
                catch (Exception ex)
                {
                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"Api_OrderPaymentCashFreeLog: {ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();

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
                //db.AppLogs.Add(new AppLog
                //{
                //    CreateDatetime = DateTime.Now,
                //    Error = $"Api_PaymentGetToken: {ex.Message}",
                //    Message = ex.StackTrace,
                //    MachineName = System.Environment.MachineName
                //});
                //db.SaveChanges();

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
                        OrderDetails= deSerialize,
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
                //var route = db.RoutePlans.Where(o => o.DeliveryAgentId == regUser.DeliveryAgentId
                //&& o.Order.OrderStatusId == 6 && (o.Order.TestOrder == testorder)).OrderBy(o => o.DeliveryStart);
                //if(route==null || route.Count() <= 0)
                //{
                //    responseStatus.Message = "Currently there is no orders for delivery.";
                //}
                //route.ToList().ForEach((o) =>
                //{
                //    var order = db.Orders.Where(j => j.Id == o.OrderID)?.FirstOrDefault();
                //    var orderdetail = db.OrderDetails.Where(j => j.OrderId == order.Id)?.ToList();
                //    order.OrderDetails = orderdetail;
                //    o.Order = order;
                //});
                //var oSerialize = JsonConvert.SerializeObject(route, Formatting.None,
                //          new JsonSerializerSettings()
                //          {
                //              ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //          });
                //var deSerialize = JsonConvert.DeserializeObject<List<RoutePlan>>(oSerialize);
                responseStatus.Data = new { orderPlan = routePlans, orderTrackAddress = list };
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("delivery/orders-new/{index}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage NewDeliveryOrders(int index=1)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                string uId = User.Identity.GetUserId();
                RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                db.Configuration.ProxyCreationEnabled = false;
                RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                var routePlans = routePlanDBO.NewDevlieryOrders(regUser.DeliveryAgentId ?? 0,index);

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
                //var route = db.RoutePlans.Where(o => o.DeliveryAgentId == regUser.DeliveryAgentId
                //&& o.Order.OrderStatusId == 6 && (o.Order.TestOrder == testorder)).OrderBy(o => o.DeliveryStart);
                //if(route==null || route.Count() <= 0)
                //{
                //    responseStatus.Message = "Currently there is no orders for delivery.";
                //}
                //route.ToList().ForEach((o) =>
                //{
                //    var order = db.Orders.Where(j => j.Id == o.OrderID)?.FirstOrDefault();
                //    var orderdetail = db.OrderDetails.Where(j => j.OrderId == order.Id)?.ToList();
                //    order.OrderDetails = orderdetail;
                //    o.Order = order;
                //});
                //var oSerialize = JsonConvert.SerializeObject(route, Formatting.None,
                //          new JsonSerializerSettings()
                //          {
                //              ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //          });
                //var deSerialize = JsonConvert.DeserializeObject<List<RoutePlan>>(oSerialize);
                responseStatus.Data = new { orderPlan = routePlans, orderTrackAddress = list };
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
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
                        && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day)?.FirstOrDefault();
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
                        && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day)?.FirstOrDefault();
                        }
                        else
                        {
                            if (onoff == 1)
                            {
                                loginAgent.OffDuty = DateTime.Now;
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
                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"Api_DeliveryOrdersStart: {ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrdersStart: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }
            return DeliveryOrders();
        }

        [HttpPost]
        [Route("delivery/ordercompleted")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage DeliveryOrderCompleted()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                string uId = User.Identity.GetUserId();
                RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                db.Configuration.ProxyCreationEnabled = false;

                RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                var routePlans = routePlanDBO.DevlieryOrders(regUser.DeliveryAgentId ?? 0, 5);

                responseStatus.Data = new { orderPlan = routePlans };
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("delivery/order-delivered")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage DeliveryOrderPlaced(int orderId)
        {
            if (orderId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
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
                        return Request.CreateResponse(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = $"This is no such OrderID {orderId}." });
                    }

                    db.Configuration.ProxyCreationEnabled = false;
                    order.OrderStatusId = 5;
                    order.DeliveryDate = DateTime.Now;
                    db.SaveChanges();

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

                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSDelivered"], order.Id.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                    //wsms.SendMessage(textmsg, order.Customer.ContactNo);

                    //Flow SMS
                    var dicti = new Dictionary<string, string>();
                    dicti.Add("ORDERID", order.Id.ToString());
                    dicti.Add("DATETIME", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));

                    var templeteid = ConfigurationManager.AppSettings["SMSDeliveredFlowId"];

                    Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.Customer.ContactNo, dicti));
                    //End Flow
                }
                catch (Exception ex)
                {
                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"Api_DeliveryOrderPlaced: {ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

            return DeliveryOrders();
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
                    var statusCancel = (int)OrderStatusEnum.CancelByCustomer;
                    if (order.OrderStatusId == statusCancel)
                    {
                        responseStatus.Status = true;
                        responseStatus.Message = "Order cancel by customer.";
                        return Request.CreateResponse(HttpStatusCode.NotFound,responseStatus);
                    }

                    db.Configuration.ProxyCreationEnabled = false;
                    order.OrderStatusId = 27;
                    order.DeliveryDate = DateTime.Now;
                    db.SaveChanges();

                    //Live Tracking FireStore
                    CustomerApi2Controller.AddToFireStore(order.Id);
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

                    //inventory revert 
                    //string txtcancel = ConfigurationManager.AppSettings["DelCancelOrderTxt"];
                    //if (string.Compare(remark, txtcancel, true) == 0)
                    //{
                    //    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    //    paymentLinkLogsService.RevertInventory(order.Id);
                    //}

                    //inventory revert
                    //var remarks = new[] { "Deliver request for tomorrow", "Customer Cancelled", "Customer not available", "Return due to breakage", "Denied" };
                    var remarksInvenRevert = new[] { "Customer Cancelled","Denied" };
                    if (remarksInvenRevert.Contains(backToStoreStatus.Remark))
                    {
                        PaymentLinkLogsService serv = new PaymentLinkLogsService();
                        serv.RevertInventory(order.Id);
                        serv.RevertMixerInventory(order.Id);
                    }
                    //Added for Back to store for packer
                    var backtostoreExists = db.DeliveryBackToStores.Where(o => o.OrderId == orderId)?.FirstOrDefault();
                    if (backtostoreExists == null)
                    {
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
                        if (remarksInvenRevert.Contains(backToStoreStatus.Remark) == false)
                        {
                            db.RoutePlans.Remove(routeOrder);
                            db.SaveChanges();
                        }
                        db.SaveChanges();
                    }

                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSDelivered"], order.Id.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                    //wsms.SendMessage(textmsg, order.Customer.ContactNo);
                }
                catch (Exception ex)
                {
                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"Api_DeliveryOrderPlaced: {ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

            return DeliveryOrders();
        }

    }
}
