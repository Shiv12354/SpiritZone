using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Services;
using SZData.Interfaces;
using SZInfrastructure;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/1.5")]
    [EnableCors("*", "*", "*")]
    public class CustomerApi1Controller : ApiController
    {
        [HttpPost]
        [Route("addtocart")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddCart(CartInput cartInput)
        {
            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = cartDBO.AddToCart(cartInput, custId.Id);
                responseStatus.Message = "Item Is Added To Cart.";
                responseStatus.Data = data;

            }

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("updatecart")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult UpdateCard(CartInput cartInput)
        {
            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            cartDBO.UpdateCart(cartInput);
            responseStatus.Message = "Your Cart Is Updated";
            responseStatus.Data = true;
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("deletecart")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult DeleteCard(CartInput cartInput)
        {
            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            cartDBO.DeleteCart(cartInput);
            responseStatus.Message = "Item Is Deleted Successfully";
            responseStatus.Data = true;


            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("cartitem/{CartId}/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetCartItems(int cartId, int shopId)
        {
            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var objects = cartDBO.GetCartItems(cartId, shopId, custId.Id);
                var datalist = from d in objects
                               select new
                               {
                                   CartId = d.CartId,
                                   ProductId = d.ProductId,
                                   ProductName = d.ProductName,
                                   ProductImage = d.ProductImage,
                                   UnitPrice = d.UnitPrice,
                                   Qty = d.Qty,
                                   SubTotal = d.SubTotal,
                                   Text = d.Text,
                                   IsMixer=d.IsMixer,
                                   ProductRefID=d.ProductRefID,
                                   AvailableQty=d.AvailableQty,
                                   Capacity=d.Capacity,
                                   Current_Price=d.Current_Price,
                                   IsReserve=d.IsReserve,
                                   SupplierId=d.SupplierId,
                                   MixerType=d.MixerType,
                                   MixerRefId=d.MixerRefId
    };
               
                    responseStatus.Data = datalist;

                
                if (objects.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

            }


            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("carthistory")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetCartHistoryByCustomer()
        {
            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var objects = cartDBO.GetCartHistoryByCustomer(custId.Id);
                var datalist = from d in objects
                               select new
                               {
                                   CartId = d.CartId,
                                   ProductId = d.ProductId,
                                   ProductName = d.ProductName,
                                   ProductImage = d.ProductImage,
                                   UnitPrice = d.UnitPrice,
                                   Qty = d.Qty,
                                   CreatedDate = d.CreatedDate.ToString("dd/MM/yyyy HH:mm tt"),
                                   ModifiedDate = d.ModifiedDate.ToString("dd/MM/yyyy HH:mm tt"),
                                   Remarks = d.Remarks,
                                   IsMixer=d.IsMixer

                               };
                foreach (var item in datalist)
                {
                    responseStatus.Data = datalist;

                }
                if (objects.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

            }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("cartempty")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult CartEmptyByCustomer()
        {
            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                cartDBO.CartEmply(custId.Id);
                responseStatus.Message = "Your Order Has Been Placed Successfully, Your Cart Is Empty";
                responseStatus.Data = true;

            }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("checkpod/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult CheckPod(int shopId)
        {
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var miniOrd = c.GetConfigValue(ConfigEnums.MinLimitOrder.ToString());
            var miniOrdMsg = c.GetConfigValue(ConfigEnums.MinLimitOrderMsg.ToString());
            var upiEnable = c.GetConfigValue(ConfigEnums.UPIEnable.ToString());
            var podEnable = c.GetConfigValue(ConfigEnums.PODEnable.ToString()); 
            var maxiumPodAmount = c.GetConfigValue(ConfigEnums.MaxiumPodAmount.ToString());
            var isMixerPOD =c.GetConfigValue(ConfigEnums.IsMixerPOD.ToString());
            var deliveryCharges = c.GetConfigValue(ConfigEnums.DeliveryCharges.ToString());
            bool IsMixerPOD = false;
            if (isMixerPOD=="1")
            {
                IsMixerPOD = true;
            }
            else
            {
                IsMixerPOD = false;
            }
            int pageId = (int)PageNameEnum.CART;
            string pageVersion = "1.6";
            var cont = SZIoc.GetSerivce<IPageService>();
            var content = cont.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var pageImages = content.PageImages.Select(o => new { Key = o.ImageKey, Value = o.ImageUrl }).ToDictionary(o => o.Key, o => o.Value);
            var Title = conCart[PageContentEnum.Text2.ToString()];

            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = cartDBO.CheckPod(shopId, custId.Id);

                if (data == null)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Data = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

                if (data.WineShop.IsPOD == true && data.IsPOD == true)
                {
                    data.WineShop.IsPOD = true;

                }
                else if (data.WineShop.IsPOD == true && data.IsPOD == false)
                {
                    data.WineShop.IsPOD = true;

                }
                else if (data.WineShop.IsPOD == false)
                {
                    data.WineShop.IsPOD = true;
                }
                OrderDBO orderDBO = new OrderDBO();
                var payType = orderDBO.GetOrderPaymentType();
               
                if (payType == null)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "No Payment Type found.";
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                var payTypelist = new List<object>();
                foreach (var item in payType)
                {
                    if (item.PaymentTypeId == 1)
                    {
                        payTypelist.Add(new { item.PaymentTypeId ,Title = item.Description, Description = conCart[PageContentEnum.Text11.ToString()] });
                    }
                    if (item.PaymentTypeId == 2 && data.WineShop.IsPOD == true)
                    {
                        payTypelist.Add(new { item.PaymentTypeId,Title = item.Description, Description = conCart[PageContentEnum.Text13.ToString()] });
                    }

                }
                var result = new
                {
                    BillDetails = new {
                        Title = conCart[PageContentEnum.Text2.ToString()],
                        Details = new object[]
                    {
                       new  {
                        Text = conCart[PageContentEnum.Text3.ToString()],
                        Default = 0,
                        Key ="Permit_Cost"
                    },
                        new
                    {
                        Text = conCart[PageContentEnum.Text4.ToString()],
                        Default = 0,
                        Key ="Liquor_Cost"
                    },
                     new{
                        Text = conCart[PageContentEnum.Text5.ToString()],
                        Default = 0,
                        Key ="Mixer_Cost"
                    },
                      new {
                        Text =conCart[PageContentEnum.Text23.ToString()],
                        Default = deliveryCharges,
                        Key ="Delivery_Charges"
                    },
                      new {
                        Text =conCart[PageContentEnum.Text67.ToString()],
                        Default = 0,
                        Key ="Spiritzone_Credits"
                    },
                       new {
                        Text =conCart[PageContentEnum.Text56.ToString()],
                        Default = 0,
                        Key ="Discount"
                    },
                      new {
                        Text = conCart[PageContentEnum.Text6.ToString()],
                        Default = 0,
                         Key ="Total_Amount"
                    }
                    },
                    },
                    PaymentType = payTypelist,
                    IsMixerPOD,
                    IsOPD = data.WineShop.IsPOD,
                    MiniumOrder = miniOrd,
                    MiniumOrderMessage = miniOrdMsg,
                    UPIEnable = upiEnable,
                    MaxiumPodAmount=maxiumPodAmount,
                    DeliveryMessage = conCart[PageContentEnum.Text7.ToString()],
                    LeftInStkMsg= conCart[PageContentEnum.Text55.ToString()],
                    Image1 = pageImages[PageImageEnum.Image1.ToString()],
                    Image2 = pageImages[PageImageEnum.Image2.ToString()],
                    Image3 = pageImages[PageImageEnum.Image3.ToString()],
                    Image4 = pageImages[PageImageEnum.Image4.ToString()],
                    Image5 = pageImages[PageImageEnum.Image5.ToString()],
                    Text1 = conCart[PageContentEnum.Text63.ToString()],
                    Text2 = conCart[PageContentEnum.Text64.ToString()],
                    Text3 = conCart[PageContentEnum.Text65.ToString()],
                    Text4 = conCart[PageContentEnum.Text66.ToString()],
                    Balance=data.ReferBalance.Balance,
                    PromoAppliedChangeLineItem = conCart[PageContentEnum.Text68.ToString()],
                    SZCreditsCanUse =Convert.ToInt32(c.GetConfigValue(ConfigEnums.SZCreditsCanUse.ToString())),
                    IsWalletForPOD= Convert.ToInt32(c.GetConfigValue(ConfigEnums.IsWalletForPOD.ToString())) == 0 ? false :true,
                    CanWeUSeWalletForOrder = Convert.ToInt32(c.GetConfigValue(ConfigEnums.CanWeUSeWalletForOrder.ToString())) == 0 ? false : true
                };
                
                responseStatus.Status = true;

                responseStatus.Data = result;
               
                return Ok(responseStatus);
            }
        }

        [HttpGet]
        [Route("messages")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetMessages()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var miniOrd = c.GetConfigValue(ConfigEnums.MinLimitOrder.ToString());
            var miniOrdMsg = c.GetConfigValue(ConfigEnums.MinLimitOrderMsg.ToString());
            var upiEnable = c.GetConfigValue(ConfigEnums.UPIEnable.ToString());
            var podEnable = c.GetConfigValue(ConfigEnums.PODEnable.ToString());
            var ret = new
            {
                MiniumOrder=miniOrd,
                MiniumOrderMessage=miniOrdMsg,
                UPIEnable=upiEnable,
                podEnable=podEnable

            };
            responseStatus.Data = ret;
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("addfcmnotication")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddFcmNotification(FcmNotificationInput fcmNotificationInput)
        {
            FcmNotificationDBO fcmNotificationDBO = new FcmNotificationDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = fcmNotificationDBO.AddToFcmNotification(fcmNotificationInput, custId.Id);
                responseStatus.Message = "FcmNotification Added Successfully.";
                responseStatus.Data = data;

            }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("updatefcmnotication/{fcmnotificationid}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult UpdateFcmNotification(int fcmNotificationId)
        {
            FcmNotificationDBO fcmNotificationDBO = new FcmNotificationDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            fcmNotificationDBO.UpdateFcmNotification(fcmNotificationId);
            responseStatus.Message = "FcmNotification Is Updated";
            responseStatus.Data = true;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("fcmnotification")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetFcmNotification()
        {
            FcmNotificationDBO fcmNotificationDBO = new FcmNotificationDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var objects = fcmNotificationDBO.GetFcmNotification(custId.Id);
                responseStatus.Data = objects;


                if (objects.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

            }


            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("unreadcountnotification")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetUnreadNotificationCount()
        {
            FcmNotificationDBO fcmNotificationDBO = new FcmNotificationDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
               var ret= fcmNotificationDBO.GetUnreadNotificationCount(custId.Id);
                responseStatus.Data = ret;

            }

            return Ok(responseStatus);
        }


        [HttpPost]
        [Route("updatemultiplecarts")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult UpdateMultipleCarts(List<CartInput> cartInput)
        {
            CartDBO cartDBO = new CartDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            cartDBO.UpdateMultipleCarts(cartInput);
            responseStatus.Message = "Your Cart Is Updated";
            responseStatus.Data = true;
            return Ok(responseStatus);
        }


        [HttpGet]
        [Route("deletefcmnotication/{fcmnotificationid}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult DeleteFcmNotification(int fcmNotificationId)
        {
            FcmNotificationDBO fcmNotificationDBO = new FcmNotificationDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            fcmNotificationDBO.DeleteFcmNotification(fcmNotificationId);
            responseStatus.Message = "FcmNotification Is Deleted";
            responseStatus.Data = true;
            return Ok(responseStatus);
        }


        [HttpGet]
        [Route("configdata")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetConfigValues()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var data = c.GetConfigValue().Select(o => new { o.KeyText,  o.ValueText }).ToList();
            responseStatus.Data = data;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("configdatas")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetPageContentWithConfigValue()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var liveSite = c.GetConfigValue(ConfigEnums.LiveSite.ToString());
            var mixerEnable = c.GetConfigValue(ConfigEnums.MixerEnable.ToString());
            bool isMixer;
            if (mixerEnable == "1")
            {
                isMixer = true;
            }
            else
            {
                isMixer = false;
            }
            int pageId = (int)PageNameEnum.CART;
            string pageVersion = "1.6";
            var cont = SZIoc.GetSerivce<IPageService>();
            var content = cont.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var conBtn = content.PageButtons.Select(o => new { Key = o.ButtonKey, Value = o.ButtonText }).ToDictionary(o => o.Key, o => o.Value);
            var Title = conCart[PageContentEnum.Text2.ToString()];
            var Config = new
            {
                LiveSite = liveSite,
                IsMixer = isMixer,
                OnloadBannerUrl = conCart[PageContentEnum.Text39.ToString()],
                ShareText = conCart[PageContentEnum.Text38.ToString()]
            };
            var mixerDetail = new
            {
                MixerSubtitle = conCart[PageContentEnum.Text32.ToString()],
                CustomRequirementMessage = conCart[PageContentEnum.Text33.ToString()],
                CallusText = conCart[PageContentEnum.Text34.ToString()],
                CallCenterNumber = conCart[PageContentEnum.Text35.ToString()],
                ChooseGiftTitle = conCart[PageContentEnum.Text36.ToString()],
                ChooseGiftSubTitle = conCart[PageContentEnum.Text37.ToString()],
                BtnText = conBtn[PageButtonsEnum.Button3.ToString()]
               


            };


            responseStatus.Data = new { 
            Config=Config,
            MixerDetails=mixerDetail
            }
                ;
            return Ok(responseStatus);
        }

        
    }
}
