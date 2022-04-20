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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SZInfrastructure;

namespace RainbowWine.Controllers
{
    [RoutePrefix("delapi/v3.0")]
    [Authorize]
    [DisplayName("Operational")]
    [EnableCors("*", "*", "*")]
    public class DeliveryAgents3Controller : ApiController
    {
        NewDelAppDBO newDelAppDBO = new NewDelAppDBO();

        [HttpGet]
        [Route("homepagedetail/{index}/{size}")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage HomePageDetails(int index = 1, int size = 5)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string uId = User.Identity.GetUserId(); //"14b95b0f-00c7-4b0d-8fe1-afc64413eaf0";
                var c = SZIoc.GetSerivce<ISZConfiguration>();
                string airtelIQUsername = string.Empty;
                string airtelIQPassword = string.Empty;
                string loginDistCheck = string.Empty;
                string slotStartWOLogin = string.Empty;
                string firebaseTrack = string.Empty;
                string firebaseInterval = string.Empty;
                string slotStartedStatusList = string.Empty;
                string slotStartWithoutHandover = string.Empty;
                string isDelAppHyperTrackOn = string.Empty;
                string PODSucessStatus = string.Empty;
                HandOver handOver = new HandOver();
                EarningAndAttendance earningAndAttendance = new EarningAndAttendance();
                DeliveryAgentDetail deliveryAgentDetail = new DeliveryAgentDetail();
                if (index == 1)
                {
                    airtelIQUsername = c.GetConfigValue(ConfigEnums.AirtelIQUsername.ToString());
                    airtelIQPassword = c.GetConfigValue(ConfigEnums.AirtelIQPassword.ToString());
                    loginDistCheck = c.GetConfigValue(ConfigEnums.LoginDistCheck.ToString());
                    slotStartWOLogin = c.GetConfigValue(ConfigEnums.SlotStartWOLogin.ToString());
                    firebaseTrack = c.GetConfigValue(ConfigEnums.FirebaseTrack.ToString());
                    firebaseInterval = c.GetConfigValue(ConfigEnums.FirebaseInterval.ToString());
                    slotStartedStatusList = c.GetConfigValue(ConfigEnums.SlotStartedStatusList.ToString());
                    slotStartWithoutHandover = c.GetConfigValue(ConfigEnums.SlotStartWithoutHandover.ToString());
                    isDelAppHyperTrackOn = c.GetConfigValue(ConfigEnums.isDelAppHyperTrackOn.ToString());
                    PODSucessStatus = c.GetConfigValue(ConfigEnums.PODSucessStatus.ToString());
                    earningAndAttendance = newDelAppDBO.EarningWithAttendance(uId);
                    deliveryAgentDetail = newDelAppDBO.GetDeliveryAgentDetail(uId);
                    handOver = newDelAppDBO.HandOver(uId);
                }
                var condata = new
                {
                    airtelIQUsername,
                    airtelIQPassword,
                    loginDistCheck,
                    slotStartWOLogin,
                    firebaseTrack,
                    firebaseInterval,
                    slotStartedStatusList,
                    slotStartWithoutHandover,
                    isDelAppHyperTrackOn ,
                    PODSucessStatus
                };

                var oDetails = newDelAppDBO.DelV3OrderDetails(index, size, uId);
                var groupOrdDetails = oDetails.GroupBy(u => u.JobId).Select(grp => new { JobId = grp.Key, Orders = grp.OrderBy(a => a.AssignedDate).ToList() }).ToList();
                var giftorddetail = groupOrdDetails.Select(x => new
                {
                    x.JobId,
                   Orders= x.Orders.Select(a => new 
                   { 
                       a.OrderId,
                       a.OrderAmount,
                       a.OrderStatusId,
                       a.OrderStatusName,
                       a.PaymentType,
                       a.LicPermitNo,
                       a.CreationDate,
                       a.ShopID,
                       a.CustContactNo,
                       a.CustName,
                       a.CustLatitude,
                       a.CustLongitude,
                       a.Address,
                       a.Flat,
                       a.Landmark,
                       a.ShopLatitude,
                       a.ShopLongitude,
                       a.Deliveries_60,
                       a.Deliveries_90,
                       a.EarningAmount60,
                       a.EarningAmount90,
                       a.IsPickedUp,
                       a.PickedUpDate,
                       AssignedDate=a.AssignedDate.ToString("yyyy-MM-ddTHH:mm:ss.300"),
                       //AssignedDate=(Convert.ToString(a.AssignedDate)).Length==19 ? a.AssignedDate : Convert.ToDateTime(a.AssignedDate.ToString("yyyy-MM-dd hh:mm:ss.300")),
                       a.PaymentTypeId,
                       a.JobId,
                       a.IsGift,
                       ContactNos = new List<dynamic>() {
                       new
                       {
                           Number =a.RecipientContactNo ,
                           DisplayMsg=a.DisplayMsgRecipient,
                       },
                        new
                       {
                           Number= a.CustContactNo,
                           DisplayMsg=a.DisplayMsgCustomer
                       },
                       },
                       a.GiftIcon,
                       a.GiftMsg

                   })

                });
                responseStatus.Data = new
                {
                    AgentDetail = deliveryAgentDetail,
                    Earning = earningAndAttendance != null ? earningAndAttendance : new object(),
                    HandOver = handOver,
                    ConfigValues = condata,
                    OrderDetails = giftorddetail
                };
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);


            }
        }

        [HttpPost]
        [Route("orderdetail/{orderid}")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetOrderItemDetails(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            var orderItem = newDelAppDBO.GetV3OrderItemDetails(orderId);
            var data = ManupulateOrderData(orderItem);
            responseStatus.Data = new { OrderItems = data };

            return Ok(responseStatus);

        }


        [HttpPost]
        [Route("delivery-handover")]
        public IHttpActionResult DeliveryHandoverNew()
        {
            ResponseStatus responseStatus = new ResponseStatus();



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                using (var db = new rainbowwineEntities())
                {
                    try
                    {
                        string uId = User.Identity.GetUserId();
                        db.Configuration.ProxyCreationEnabled = false;
                        var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();
                        var delagent = db.DeliveryAgents.Where(o => o.Id == ruser.DeliveryAgentId).FirstOrDefault();
                        var aspUSer = db.AspNetUsers.Find(uId);

                        OrderDBO orderDBO = new OrderDBO();
                        GiftBagDBO giftBagDBO = new GiftBagDBO();
                        var delpay = orderDBO.GetDeliveryPaymentDetails(ruser.DeliveryAgentId ?? 0);
                        var delback = orderDBO.GetDelV3BacktoStoreDetailsNew(ruser.DeliveryAgentId ?? 0);
                        var giftdetails = giftBagDBO.GetGiftBagDeliveryAgentWise(ruser.DeliveryAgentId ?? 0);
                        var oSerializeAgent = JsonConvert.SerializeObject(delagent, Formatting.None,
                               new JsonSerializerSettings()
                               {
                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                               });
                        var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryAgent>(oSerializeAgent);

                        var cashDel = delpay.GroupBy(x => x.JobId).Select(y => new
                        {
                            JobId = y.Key,
                            Orders = y.Select(d => new
                            {
                                OrderId = d.Order.Id,
                                d.Order.OrderDate,
                                d.Order.OrderPlacedBy,
                                d.Order.OrderTo,
                                d.Order.OrderAmount,
                                d.Order.ShopID,
                                d.Order.DeliveryDate,
                                d.DeliveryPaymentId,
                                d.PaymentTypeId,
                                d.AmountPaid,
                                d.CreatedDate,
                                d.DelPaymentConfirm,
                                OrderDetails = d.Order.OrderDetails.Select(a => new {
                                    a.Id,
                                    a.OrderId,
                                    a.ItemQty,
                                    a.Price,
                                    a.ShopID,
                                    a.Issue,
                                    a.ProductID,
                                    ProductDetails = new
                                    {
                                        a.ProductDetail.ProductID,
                                        a.ProductDetail.ProductName,
                                        Volume = a.ProductDetail.Category,
                                        a.ProductDetail.Price,
                                        a.ProductDetail.ShopItemId,
                                        a.ProductDetail.ProductImage,
                                        a.ProductDetail.ProductType


                                    }
                                })

                            })
                        });
                        string abc = null;
                        var backToStoreDel = delback.GroupBy(x => x.JobId).Select(y => new
                        {
                            JobId = y.Key,
                            Orders = y.Select(b => new
                            {
                                OrderId = b.Order.Id,
                                b.Order.OrderDate,
                                b.Order.OrderPlacedBy,
                                b.Order.OrderTo,
                                b.Order.OrderAmount,
                                b.Order.ShopID,
                                b.Order.DeliveryDate,
                                b.CreatedDate,
                                PickedDate = b.Order.PickedUpDate,
                                Gift = b.Order.listItem.Select(s => new
                                {
                                    s.GiftBagOrderItemId,
                                    s.GiftBagName,
                                    s.BagImageUrl,
                                    s.Qty,
                                    s.GiftBagDetailId,
                                    s.Price,
                                    IsGift = true
                                }),
                                OrderDetails = b.Order.OrderDetails.Select(c => new
                                {
                                    Id = c.Id,
                                    OrderId = c.OrderId,
                                    ItemQty = c.ItemQty,
                                    Price = c.Price,
                                    ShopID = c.ShopID,
                                    Issue = c.Issue.HasValue ? c.Issue.Value : false,
                                    ProductID = c.ProductID,
                                    ProductDetails = new
                                    {
                                        ProductID = c.ProductDetail.ProductID,
                                        ProductName = c.ProductDetail.ProductName,
                                        Volume = c.ProductDetail.Category,
                                        ProductType = c.ProductDetail.ProductType,
                                        Price = c.ProductDetail.Price.HasValue ? c.ProductDetail.Price.Value : Convert.ToDouble(0.0),
                                        ShopItemId = c.ProductDetail.ShopItemId,
                                        ProductImage = c.ProductDetail.ProductImage,

                                    }
                                })
                                .AsQueryable().Union(b.Order.listItem.Select(x => new
                                {
                                    Id = x.GiftBagOrderItemId.Value,
                                    OrderId = 0,
                                    ItemQty = 1,
                                    Price = Convert.ToDecimal(x.Price * x.Qty),
                                    ShopID = 0,
                                    Issue = false,
                                    ProductID = x.GiftBagDetailId,
                                    ProductDetails = new
                                    {
                                        ProductID = 0,
                                        ProductName = x.GiftBagName,
                                        Volume = x.Qty.ToString() == "1" ? x.Qty.ToString() + " "+ "bag" : x.Qty.ToString() + " " + "bags",
                                        ProductType = abc,
                                        Price = Convert.ToDouble(x.Price * x.Qty),
                                        ShopItemId = abc,
                                        ProductImage = x.BagImageUrl,

                                    }
                                }).ToList())
                            })

                        });
                        responseStatus.Data = new { cash = cashDel, backtostore = backToStoreDel };
                        responseStatus.Status = true;
                        return Ok(responseStatus);
                    }
                    catch (Exception ex)
                    {



                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Some error occurred while processign the request"
                        };
                        SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                        db.Dispose();
                        return Content(HttpStatusCode.InternalServerError, responseStatus);



                    }
                    finally
                    {
                        db.Dispose();
                    }
                }

            }

        }

        #region Non Action
        public dynamic ManupulateOrderData(IEnumerable<OrderItem> data)
        {
            return data.Select(x => new
            {
                x.OrderId,
                x.OrderStatusId,
                x.DeliveryDate,
                x.OrderStatusName,
                x.OrderDate,
                x.OrderAmount,
                x.ShopID,
                x.LicPermitNo,
                x.PermitCost,
                x.PackedDate,
                x.PaymentTypeId,
                x.LineItems
            });
        }
        #endregion
    }
}
