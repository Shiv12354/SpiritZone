using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services.DBO;
using System.Data.Entity;
using RainbowWine.Services;
using System.ComponentModel;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Configuration;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/v0/web")]
    [DisplayName("WebApis")]
    [EnableCors("*", "*", "*")]
    public class LeafletController : ApiController
    {
        [HttpPost]
        [Route("delivery/orders/{Id}")]
        public HttpResponseMessage DeliveryOrders(int? Id)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (Id == null)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order is null";
                return Request.CreateResponse(HttpStatusCode.BadRequest, responseStatus);
            }
            if (Id <= 0)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order Id must be greater than 0.";
                return Request.CreateResponse(HttpStatusCode.BadRequest, responseStatus);
            }
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    RoutePlan rporder = db.RoutePlans.Where(o => o.OrderID == Id)?.FirstOrDefault();
                    db.Configuration.ProxyCreationEnabled = false;
                    RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                    var routePlans = routePlanDBO.DevlieryOrders(rporder.DeliveryAgentId ?? 0, 6, rporder.JobId);

                    List<object> list = new List<object>();
                    foreach (var item in routePlans)
                    {
                        //var custAddress = db.CustomerAddresses.Find(item.Order.CustomerAddressId);
                        var orderWine = db.Orders.Include(o => o.WineShop).Include(o => o.Customer).Where(o => o.Id == item.OrderID).FirstOrDefault();
                        var custAddress = db.CustomerAddresses.Find(orderWine.CustomerAddressId);
                        list.Add(new
                        {
                            AgentId = rporder.DeliveryAgentId,
                            JobId = item.JobId,
                            OrderId = item.OrderID,
                            CustomerId = orderWine.Customer.Id,
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
                    responseStatus.Data = new { orderTrackAddress = list };
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
                finally
                {
                    db.Dispose();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("deliveryagent/{Id}")]
        public HttpResponseMessage DeliveryAgentShops(string Id)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (string.IsNullOrWhiteSpace(Id))
            {
                responseStatus.Status = false;
                responseStatus.Message = "Delivery Agent Id can not be blank.";
                return Request.CreateResponse(HttpStatusCode.BadRequest, responseStatus);
            }
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var shops = db.DelManageShops.Where(o => o.rUserId == Id).Select(o => o.ShopID);
                    var winshop = db.WineShops.Include(o => o.DeliveryAgents).Where(o => shops.Contains(o.Id)).ToList();
                    var oSerialize = JsonConvert.SerializeObject(winshop, Formatting.None,
                       new JsonSerializerSettings()
                       {
                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                       });
                    var deSerialize = JsonConvert.DeserializeObject<List<WineShop>>(oSerialize);
                    responseStatus.Data = deSerialize;
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryAgentShops: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
                finally
                {
                    db.Dispose();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("deliveryagents")]
        public HttpResponseMessage DeliveryAgentsShops()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
           
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var winshop = db.WineShops.Include(o => o.DeliveryAgents).ToList();
                    var oSerialize = JsonConvert.SerializeObject(winshop, Formatting.None,
                       new JsonSerializerSettings()
                       {
                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                       });
                    var deSerialize = JsonConvert.DeserializeObject<List<WineShop>>(oSerialize);
                    responseStatus.Data = deSerialize;
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryAgentShops: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
                finally
                {
                    db.Dispose();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("Customer-contact")]
        public HttpResponseMessage CustomerContact()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            try
            {
                CustomerDBO customerDBO = new CustomerDBO();
                var cust = customerDBO.GetCustomerContact();
                responseStatus.Data = cust;
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CustomerContact: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("order-zone")]
        public HttpResponseMessage OrderZone()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            try
            {
                OrderDBO customerDBO = new OrderDBO();
                var cust = customerDBO.GetCustomerContact();
                responseStatus.Data = cust;
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CustomerContact: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("newsletter")]
        public HttpResponseMessage NewsLetter(LoginViewModel loginViewModel)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            try
            {
                using (var db = new rainbowwineEntities())
                {
                    db.NewsLetters.Add(new NewsLetter { Email = loginViewModel.Email });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CustomerContact: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }
       
        [HttpPost]
        [Route("signup-mobile")]
        public HttpResponseMessage MobileSignup()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            try
            {

                CustomerDBO customerDBO = new CustomerDBO();
                var cust = customerDBO.SignUpComstomer();
                responseStatus.Data = cust;
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CustomerContact: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }


        [HttpPost]
        [Route("cashfree/return")]
        public HttpResponseMessage CashFreeReturn()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            try
            {
                Stream req = HttpContext.Current.Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                try
                {
                    string pecodeValue = new StreamReader(req).ReadToEnd();
                    //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                    string pdecode = HttpContext.Current.Server.UrlDecode(pecodeValue);
                    string pdecodevalue = HttpContext.Current.Server.UrlDecode(pdecode);
                    //Write to file

                    CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = paymentLinkLogsService.UpdateOrderIssueToApprove(decodevlaue, pdecode);

                    //Colsed the issue
                    var ord2 = decodevlaue.OrderId2.Split('_');
                    if (ord2.Length == 4)
                    {
                        var issueId = ord2[3];
                        using (var db = new rainbowwineEntities())
                        {

                            var issue = db.OrderIssues.Find(issueId);
                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            string uId = u.Id;

                            int issueClosed = (int)IssueType.Closed;
                            issue.OrderStatusId = issueClosed;
                            db.SaveChanges();

                            var issetrack = new OrderIssueTrack
                            {
                                OrderId = issue.OrderId,
                                OrderIssueId = issue.OrderIssueId,
                                Remark = "Payment Succesfull",
                                OrderStatusId = 6,
                                TrackDate = DateTime.Now,
                                UserId = uId,
                            };
                            db.OrderIssueTracks.Add(issetrack);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CashFreeReturn: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("cashfree/notify")]
        public HttpResponseMessage CashFreeNotify()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            try
            {
                Stream req = HttpContext.Current.Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                try
                {
                    string pecodeValue = new StreamReader(req).ReadToEnd();
                    //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                    string pdecode = HttpContext.Current.Server.UrlDecode(pecodeValue);
                    string pdecodevalue = HttpContext.Current.Server.UrlDecode(pdecode);
                    //Write to file

                    CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = paymentLinkLogsService.UpdateOrderIssueToApprove(decodevlaue, pdecode);

                    //Colsed the issue
                    var ord2 = decodevlaue.OrderId2.Split('_');
                    if (ord2.Length == 4)
                    {
                        var issueId = ord2[3];
                        using (var db = new rainbowwineEntities()) {

                            var issue = db.OrderIssues.Find(issueId);
                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            string uId = u.Id;

                            int issueClosed = (int)IssueType.Closed;
                            issue.OrderStatusId = issueClosed;
                            db.SaveChanges();

                            var issetrack = new OrderIssueTrack
                            {
                                OrderId = issue.OrderId,
                                OrderIssueId = issue.OrderIssueId,
                                Remark = "Payment Succesfull",
                                OrderStatusId = 6,
                                TrackDate = DateTime.Now,
                                UserId = uId,
                            };
                            db.OrderIssueTracks.Add(issetrack);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CashFreeReturn: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("cashfreemodify/return")]
        public HttpResponseMessage CashFreeOrderModifyReturn()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            try
            {
                Stream req = HttpContext.Current.Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                try
                {
                    string pecodeValue = new StreamReader(req).ReadToEnd();
                    //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                    string pdecode = HttpContext.Current.Server.UrlDecode(pecodeValue);
                    string pdecodevalue = HttpContext.Current.Server.UrlDecode(pdecode);
                    //Write to file

                    CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = paymentLinkLogsService.UpdateOrderModifyToApprove(decodevlaue, pdecode);

                    //Colsed the issue
                    var ord2 = decodevlaue.OrderId2.Split('_');
                    if (ord2.Length == 4)
                    {
                        var modifyId = ord2[3];
                        using (var db = new rainbowwineEntities())
                        {

                            var modify = db.OrderModifies.Find(modifyId);
                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            string uId = u.Id;

                            int issueClosed = (int)IssueType.Closed;
                            modify.StatusId = issueClosed;
                            db.SaveChanges();

                            var modifytrack = new OrderModifyTrack
                            {
                                OrderId = modify.OrderId,
                                OrderModifyId = modify.Id,
                                Remark = "Payment Succesfull",
                                OrderStatusId = issueClosed,
                                TrackDate = DateTime.Now,
                                UserId = uId,
                            };
                            db.OrderModifyTracks.Add(modifytrack);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CashFreeReturn: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("cashfreemodify/notify")]
        public HttpResponseMessage CashFreeOrderModifyNotify()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            try
            {
                Stream req = HttpContext.Current.Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                try
                {
                    string pecodeValue = new StreamReader(req).ReadToEnd();
                    //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                    string pdecode = HttpContext.Current.Server.UrlDecode(pecodeValue);
                    string pdecodevalue = HttpContext.Current.Server.UrlDecode(pdecode);
                    //Write to file

                    CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = paymentLinkLogsService.UpdateOrderModifyToApprove(decodevlaue, pdecode);

                    //Colsed the issue
                    var ord2 = decodevlaue.OrderId2.Split('_');
                    if (ord2.Length == 4)
                    {
                        var modifyId = ord2[3];
                        using (var db = new rainbowwineEntities())
                        {

                            var modify = db.OrderModifies.Find(modifyId);
                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            string uId = u.Id;

                            int issueClosed = (int)IssueType.Closed;
                            modify.StatusId = issueClosed;
                            db.SaveChanges();

                            var modifytrack = new OrderModifyTrack
                            {
                                OrderId = modify.OrderId,
                                OrderModifyId = modify.Id,
                                Remark = "Payment Succesfull",
                                OrderStatusId = issueClosed,
                                TrackDate = DateTime.Now,
                                UserId = uId,
                            };
                            db.OrderModifyTracks.Add(modifytrack);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CashFreeReturn: {ex.Message}", ex.StackTrace);
                responseStatus.Status = false;
                responseStatus.Message = $"{ex.Message}";
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }
    }
}
