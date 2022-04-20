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
using Google.Cloud.Firestore;
using RainbowWine.Services.DO;
using System.Text;
using RainbowWine.Models.ThirdParty;
using SZInfrastructure;
using SZData.Interfaces;
using Microsoft.Ajax.Utilities;
using SZModels;
using static RainbowWine.Services.FireStoreService;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/1.6")]
    [EnableCors("*", "*", "*")]
    public class CustomerApiController : ApiController
    {
        rainbowwineEntities db = new rainbowwineEntities();

        [HttpGet]
        [Route("recom/showcase/{index}/{size}/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetRecommPremiumProductDetails(int index, int size, int shopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ProductDBO productDBO = new ProductDBO();
                var objects = productDBO.GetPremiumProductDetail(index, size, shopId, custId.Id);
                var datalist = from d in objects
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductRefID = d.ProductRefID,
                                   ProductName = d.ProductName,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating=d.ProductRating,
                                   RatingCount=d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

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
        [Route("recom/mixer/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetRecommMixer(int shopId)
        {
            MixerDBO mixerDBO = new MixerDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var objects = mixerDBO.MixerDetails(shopId);
            var datalist = from d in objects
                           select new
                           {
                               MixerId=d.MixerId,
                               MixerName = d.MixerName,
                               GiftPrice = Convert.ToInt32(d.GiftPrice),
                               GiftDescription=d.GiftDescription,
                               Description = d.Description,
                               MixerImage = d.MixerImage,
                               MixerThumbImage = d.MixerThumbImage,
                               MixerType=d.MixerType,
                               MixerDetail = new { MixerDetailId = d.MixerDetailId, 
                                   MixerId = d.MixerId, MixerSizeId = d.MixerSizeId, Price = d.Price, 
                                   Capacity = d.Capacity,Qty=d.Qty,
                                   SupplierId=d.SupplierId,
                                   IsMixer=d.IsMixer
                                   
                               },
                              
                           };
            responseStatus.Data = datalist;

            
            if (objects.Count() == 0)
            {
                responseStatus.Message = "Data Is Not Found";
                responseStatus.Status = false;
                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("bottomorder")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetBottomOrder()
        {

            OrderDBO orderDBO = new OrderDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = orderDBO.BottomOrder(custId.Id);
                var datalist = from d in data
                               
                               select new
                               {
                                   OrderId = d.OrderId,
                                   OrderStatus = d.OrderStatus,
                                   Message = d.Message,
                                   ButtonText = d.ButtonText,
                                   DeliveryDate = d.DeliveryDate
                               };
                if (data == null)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                responseStatus.Data = data;
            }


            return Ok(responseStatus);
        }


        [HttpPost]
        [Route("product-by-location")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductByLocation(RecommendedParamters recommendedParamters)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "af4d3392-dd8b-4b99-9046-cc490675fa28";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
               
                if (custId.Id > 0)
                {
                    var add = db.CustomerAddresses.Where(ca => ca.CustomerId == custId.Id && ca.ShopId == recommendedParamters.ShopId).FirstOrDefault();
                    recommendedParamters.CustomerId = custId.Id;
                    recommendedParamters.DOB = custId.DOB;
                    recommendedParamters.Address = add.FormattedAddress;
                }
                else
                {
                        responseStatus.Message = "CustomerId Is Not Found";
                        responseStatus.Status = false;
                        return Content(HttpStatusCode.NotFound, responseStatus);
                    
                }
            }
           
            var input = new
            {

                CustomerId =recommendedParamters.CustomerId,
                DOB = recommendedParamters.DOB,
                NUM =Convert.ToInt32(ConfigurationManager.AppSettings["ProductLocationNum"]),//recommendedParamters.NUM,
                ShopID=recommendedParamters.ShopId,
                Adress=recommendedParamters.Address
               
            };
           
            using (HttpClient client = new HttpClient())
            {
               
                var serializeJson = JsonConvert.SerializeObject(input);
                var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                var resp = client.PostAsync(ConfigurationManager.AppSettings["ProductLocation"], content).Result;
                var ret = JsonConvert.DeserializeObject<ProductRecommandation>(resp.Content.ReadAsStringAsync().Result);
                if (ret!=null) 
                {
                    List<int> li = new List<int>();
                    var data = from d in ret.Response
                               select new
                               {
                                   d.Details.ProductID
                               };
                    foreach (var item in data)
                    {
                        li.Add(item.ProductID);
                    }
                    recommendedParamters.ProductIds = li.ToArray(); 
                }
                else 
                {
                    responseStatus.Message = "External Api Is Not Respond";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetProductDetailbyPurchase(recommendedParamters.ProductIds, recommendedParamters.ShopId,recommendedParamters.CustomerId);
                if (prod.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
                var datalist = from d in prod
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
              
                    responseStatus.Data = datalist;

                
               
            }
                       
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("product-by-purchase")]
        [Authorize(Roles = "Customer")]
     
        public IHttpActionResult GetProductByPurchase(RecommendedParamters recommendedParamters)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "af4d3392-dd8b-4b99-9046-cc490675fa28";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                if (custId.Id > 0)
                {
                    recommendedParamters.CustomerId = custId.Id;
                }
                else
                {
                    responseStatus.Message = "CustomerId Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
               
            }
            
            var input = new
            {
                CustomerId =recommendedParamters.CustomerId,
                ShopID =recommendedParamters.ShopId,
                NUM = Convert.ToInt32(ConfigurationManager.AppSettings["UserProductNum"])
            };
            using (HttpClient client = new HttpClient())
            {
                var serializeJson = JsonConvert.SerializeObject(input);
                var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                var resp = client.PostAsync(ConfigurationManager.AppSettings["UserProduct"], content).Result;
                var ret = JsonConvert.DeserializeObject<ProductRecommandation>(resp.Content.ReadAsStringAsync().Result);
                if (ret !=null)
                {
                    List<int> li = new List<int>();

                    var data = from d in ret.Response
                               select new
                               {
                                   d.Details.ProductID
                               };

                    foreach (var item in data)
                    {
                        li.Add(item.ProductID);
                    }
                    recommendedParamters.ProductIds = li.ToArray(); 
                }
                else
                {
                    responseStatus.Message = "External Api Is Not Respond";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }

                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetProductDetailbyPurchase(recommendedParamters.ProductIds,recommendedParamters.ShopId, recommendedParamters.CustomerId);
                if (prod.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
                var datalist = from d in prod
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
                responseStatus.Data = datalist;

                
                

            }
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("similar-product")]
        [Authorize(Roles = "Customer")]
        
        public IHttpActionResult GetSimilarProduct(RecommendedParamters recommendedParamters)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "af4d3392-dd8b-4b99-9046-cc490675fa28";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                if (custId.Id > 0)
                {
                    recommendedParamters.CustomerId = custId.Id;
                }
                else
                {
                    responseStatus.Message = "CustomerId Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
               
            }
           
            var input = new
            {
                CustomerId =recommendedParamters.CustomerId,
                ProductId = recommendedParamters.ProductID,
                ShopID = recommendedParamters.ShopId,
                Num = Convert.ToInt32(ConfigurationManager.AppSettings["ProductProductNum"])
            };
            using (HttpClient client = new HttpClient())
            {
                var serializeJson = JsonConvert.SerializeObject(input);
                var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                var resp = client.PostAsync(ConfigurationManager.AppSettings["ProductProduct"], content).Result;
                var ret = JsonConvert.DeserializeObject<ProductRecommandation>(resp.Content.ReadAsStringAsync().Result);
                if (ret != null)
                {


                    List<int> li = new List<int>();
                    var data = from d in ret.Response
                               select new
                               {
                                   d.Details.ProductID
                               };

                    foreach (var item in data)
                    {
                        li.Add(item.ProductID);
                    }
                    recommendedParamters.ProductIds = li.ToArray();
                }
                else
                {
                    responseStatus.Message = "External Api Is Not Respond";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetProductDetailbyPurchase(recommendedParamters.ProductIds, recommendedParamters.ShopId, recommendedParamters.CustomerId);
                if (prod.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
                var datalist = from d in prod
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
               responseStatus.Data = datalist;

                
            }
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("mixer/{index}/{size}/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetMixer(int index, int size, int shopId)
        {
            MixerDBO mixerDBO = new MixerDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var objects = mixerDBO.MixerDetailsBasedOnInvetory(index, size, shopId);
            var datalist = from d in objects
                           select new 
                           {
                               MixerId = d.MixerId,
                               MixerName = d.MixerName,
                               GiftPrice = Convert.ToInt32(d.GiftPrice),
                               GiftDescription = d.GiftDescription,
                               Description = d.Description,
                               MixerImage = d.MixerImage,
                               MixerThumbImage = d.MixerThumbImage,
                               MixerType=d.MixerType,
                               MixerDetail = new { MixerDetailId = d.MixerDetailId, 
                                   MixerId = d.MixerId, MixerSizeId = d.MixerSizeId, Price = d.Price, Capacity = d.Capacity, 
                                   Qty = d.Qty,
                                   SupplierId=d.SupplierId,
                                   IsMixer=d.IsMixer
                               },
                           };

           responseStatus.Data = datalist;

            

            if (objects.Count() == 0)
            {
                responseStatus.Message = "Data Is Not Found";
                responseStatus.Status = false;
                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("mixer/variation/{mixerId}/{shopId}/{mixertype}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetVariationMixer(int mixerId, int shopId,string mixerType)
        {
            MixerDBO mixerDBO = new MixerDBO();
            ProductDBO productDBO = new ProductDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                int pageId = (int)PageNameEnum.CART;
                string pageVersion = "1.6";
                var cont = SZIoc.GetSerivce<IPageService>();
                var content = cont.GetPageContent(pageId, pageVersion);
                var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
                var conBtn = content.PageButtons.Select(o => new { Key = o.ButtonKey, Value = o.ButtonText }).ToDictionary(o => o.Key, o => o.Value);
                string MixerSubHeading = string.Empty;
                string Title = string.Empty;
                 string SubTitle=string.Empty;
                if (mixerType == "alcoholmix")
                {
                    MixerSubHeading = conCart[PageContentEnum.Text48.ToString()];
                    Title = conCart[PageContentEnum.Text49.ToString()];
                    SubTitle = conCart[PageContentEnum.Text50.ToString()];

                }
                else if (mixerType == "giftwrap")
                {
                    MixerSubHeading = conCart[PageContentEnum.Text51.ToString()];
                    Title = conCart[PageContentEnum.Text52.ToString()];
                    SubTitle = conCart[PageContentEnum.Text37.ToString()];
                }
                else if (mixerType == "beverage")
                {
                    MixerSubHeading = conCart[PageContentEnum.Text53.ToString()];
                    Title = conCart[PageContentEnum.Text36.ToString()];
                    SubTitle = conCart[PageContentEnum.Text37.ToString()];
                }

                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var prdDetail = productDBO.MixerProductDetail(custId.Id, mixerId, shopId);
                var objects = mixerDBO.MixerDetailsById(mixerId, shopId);

                MixerSubHeading = (objects != null && objects.Count > 0 && mixerType == "beverage")
                    ? objects[0].Mixer.Description
                    : MixerSubHeading;

                var datalist = from d in objects
                               select new
                               {
                                   MixerDetail = new
                                   {
                                       MixerDetailId = d.MixerDetailId,
                                       MixerId = d.MixerId,
                                       MixerSizeId = d.MixerSizeId,
                                       AvalableQty = d.InventoryMixerDO.Qty,
                                       Price = Convert.ToInt32(d.Price),
                                       SupplierId = d.InventoryMixerDO.SupplierId,
                                       MixerDetialName = d.MixerDetailName
                                   },
                                   Mixer = new
                                   {
                                       MixerId = d.Mixer.MixerId,
                                       MixerName = (string.IsNullOrWhiteSpace(d.MixerDetailName)) ?
                                   d.Mixer.MixerName
                                   : d.MixerDetailName,
                                       GiftPrice = Convert.ToInt32(d.Mixer.GiftPrice),
                                       GiftDescription = d.Mixer.GiftDescription,
                                       MixerType = d.Mixer.MixerType,
                                       Description = d.Mixer.Description,
                                       MixerImage = d.Mixer.MixerImage
                                   },
                                   MixerSize = new { MixerSizeId = d.MixerSize.MixerSizeId, Capacity = d.MixerSize.Capacity, MixerSizeName = d.MixerSize.MixerSizeName }

                               };

                responseStatus.Data = new
                {
                    datalist,
                    ProductDetail=prdDetail,
                    MixerSubHeading,
                    Title,
                    SubTitle,
                    BtnText = conBtn[PageButtonsEnum.Button3.ToString()]
                };


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
        [Route("favorite/{index}/{size}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetFavorite(int index, int size)
        {
            FavoriteDBO favoriteDBO = new FavoriteDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var objects = favoriteDBO.GetFavoriteDetails(index, size, custId.Id);
                var datalist = from d in objects
                               select new
                               {
                                   ProductId = d.ProductID,
                                   FavoriteId = d.FavoriteId,
                                   ProductRefID = d.ProductRefID,
                                   ProductName = d.ProductName,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,
                                   IsReserve = d.IsReserve

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

        [HttpPost]
        [Route("addtofavorite/{productId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddFavorite(int productId)
        {
            FavoriteDBO favoriteDBO = new FavoriteDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                favoriteDBO.FavoriteAdd(productId, custId.Id);
                responseStatus.Message = "Product favorite is Added.";
                responseStatus.Data = true;
            }

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("removefromfavorite/{productId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult RemoveFavorite(int productId)
        {
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                FavoriteDBO favoriteDBO = new FavoriteDBO();
                favoriteDBO.FavoriteDelete(productId,custId.Id);
                responseStatus.Message = "Product Is Deleted From Favorite.";
                responseStatus.Data = true;
            }
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("productallrating")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult ProductAllRating()
        {
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = ratingProductDBO.ProductAllRating(custId.Id);
                if (data.Count == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                responseStatus.Data = data;
            }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("productdetailsrating/{index}/{size}/{rating}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult ProductDetailsRating(int index, int size,int rating)
        {
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                ////Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = ratingProductDBO.ProductDetailsRating(index, size, custId.Id,rating);
                if (data.Count == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                responseStatus.Data = data;
            }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("orderallrating")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult OrderAllRating()
        {
            OrderDBO orderDBO = new OrderDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = orderDBO.OrderAllRating(custId.Id);
                if (data.Count == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                responseStatus.Data = data;
            }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("orderdetailsrating/{index}/{size}/{rating}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult OrderDetailsRating(int index, int size,int rating)
        {
            OrderDBO orderDBO = new OrderDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                ////Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var data = orderDBO.OrderDetailsRating(index, size, custId.Id,rating);
                if (data.Count == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                responseStatus.Data = data;
            }

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("add-product-rating/{productId}/{rating}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddProductRating(int ProductId, int Rating)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ratingProductDBO.ProductRatingsAdd(custId.Id, ProductId, Rating);
                responseStatus.Message = "Product Rating is Added.";
                responseStatus.Data = true;
            }
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("update-product-rating/{productId}/{rating}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult UpdateProductRating(int ProductId, int Rating)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ratingProductDBO.ProductRatingsUpdate(custId.Id, ProductId, Rating);
                responseStatus.Message = "Product Rating is Updated.";
                responseStatus.Data = true;
            }
            return Ok(responseStatus);
        }


        [HttpPost]
        [Route("Delete-product-rating/{productId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult RemoveProductRating(int productId)
        {
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            ratingProductDBO.ProductRatingsDelete(productId);
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            responseStatus.Message = "Product rating is Deleted.";
            responseStatus.Data = true;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("product-rating/{productId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductRating(int productId)
        {
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            var objects = ratingProductDBO.GetProductRatingsDetailsByProduct(productId);
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var datalist = from d in objects
                           select new
                           {
                               ProductRefId = d.ProductRefId,
                               AverageRating = d.AverageRating,
                               TotalRatings=d.TotalRating

                           };
           
                responseStatus.Data = datalist;

            
            if (objects.Count() == 0)
            {
                responseStatus.Message = "Data Is Not Found";
                responseStatus.Status = false;
                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("add-order")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddOrderByLoginUser(APIOrder model)
        {
            if (model == null)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Failed to add order. Please try again.", Data = model });
            else if (model.AddressId <= 0)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Address field cannot be blank.", Data = model });
            else if (model.OrderItems == null || model.OrderItems.Count <= 0)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Your cart is empty", Data = model });

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            decimal intconfigAmtTotal = Convert.ToDecimal(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);

            var inventItem = UserCustomerV2Controller.InventoryCheck(model.OrderItems);
            if (inventItem.TotalAmount < intconfigAmtTotal)
            {
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = $"Minimum order amount is {intconfigAmtTotal} rs." });
            }
            if (inventItem.OrderItems.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Inventory issue.", ErrorType = "inventory", Data = new { orderId = 0, rejectedItems = inventItem } });
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
                    return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Unfortunately, the selected shop is not operating at the moment. Please choose another shop.", Data = model });

                bool testorder = (ConfigurationManager.AppSettings["TestOrder"] == "1") ? true : false;

                db.Configuration.ProxyCreationEnabled = false;
                Order order = null;
                OrderTrack orderTrack = null;
                //using (var transaction = db.Database.BeginTransaction())
                //{
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

                    MixerOrderItem mixerOrderItem = null;
                    foreach (var item in model.MixerItems)
                    {
                        mixerOrderItem = new MixerOrderItem
                        {
                            OrderId = order.Id,
                            ItemQty = item.Qty,
                            MixerDetailId = item.MixerDetailId,
                            Price = item.Price,
                            ShopId = item.ShopID
                        };
                        db.MixerOrderItems.Add(mixerOrderItem);
                        int q = item.Qty;
                        decimal p = Convert.ToDecimal(item.Price);
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
                    responseStatus.Status = false;
                    responseStatus.Message = "Your order hasn’t been placed yet. Please try again";

                    db.Dispose();
                    SpiritUtility.AppLogging($"Api_add-order_Transaction: {ex.Message}", ex.StackTrace);
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
                if (order != null && order.Id > 0)
                {
                    try
                    {
                        responseStatus.Data = new { orderId = order.Id, rejectedItems = new List<APIOrderDetails>() };

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

        [HttpGet]
        [Route("order/{orderId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetOrder(int orderId)
        {
            MixerDetailExtDO MixerDetails = new MixerDetailExtDO();
            OrderDetailExtDO OrderDetails = new OrderDetailExtDO();
            OrderDBO orderDBO = new OrderDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            bool IsReached=false, IsOrderPaid= false, IsReorder= false, IsRetry= false, IsReviewd= false, IsCancelled= false, IsCancellable= false, IsTrack= false, IsOrderInProcess= false, IsDelivered= false;
            string DisplayStatus=string.Empty;
            using (var db = new rainbowwineEntities())
            {
          
                var objects = orderDBO.GetOrderMixerDetails(orderId);
                var ordData = objects.Order.FirstOrDefault();
               
                OrderStatusEnum oStatus = (OrderStatusEnum)ordData.OStatisId;
                OrderPaymentType pType = (OrderPaymentType)ordData.PaymentTypeId;
                switch (oStatus)
                {
                    case OrderStatusEnum.Approved:
                        {
                            IsReached = false;
                            IsOrderPaid = false;
                            IsReorder = false;
                            IsRetry = false;
                            IsCancelled = false;
                            IsCancellable = false;
                            IsTrack = true;
                            IsOrderInProcess = false;
                            DisplayStatus = "Order Received";
                        };
                        break;
                    case OrderStatusEnum.Submitted:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = true;
                                        IsReviewd = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Failed";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsReviewd = false;
                                        IsCancelled = false;
                                        IsCancellable = true;
                                        IsTrack = true;
                                        IsOrderInProcess = true;
                                        DisplayStatus = "Order Received";
                                    };
                                    break;
                            }

                        };
                        break;
                    case OrderStatusEnum.CancelByCustomer:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        DisplayStatus = "Cancelled";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = true;
                                        IsReviewd = false;
                                        IsCancelled = true;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Cancelled";
                                    };
                                    break;
                                
                            }

                        };
                        break;
                    case OrderStatusEnum.Delivered:
                        {
                            IsReached = false;
                            IsOrderPaid = false;
                            IsReorder = true;
                            IsRetry = false;
                            IsCancelled = false;
                            IsCancellable = false;
                            IsTrack = false;
                            IsOrderInProcess = false;
                            IsDelivered = true;
                            DisplayStatus = "Delivered";


                        };break;
                    case OrderStatusEnum.OutForDelivery:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = true;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Out For Delivery";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = true;
                                        IsTrack = true;
                                        IsOrderInProcess = true;
                                        DisplayStatus = "Out For Delivery";
                                    };
                                    break;
                                
                            }

                        };break;
                    case OrderStatusEnum.Packed:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = true;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Packed";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = true;
                                        IsTrack = true;
                                        IsOrderInProcess = true;
                                        DisplayStatus = "Packed";
                                    };
                                    break;
                               
                            }
                        };break;
                    case OrderStatusEnum.PODCashSelected:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Pay On Order-Cash Selected";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = true;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Pay On Delivery-Cash Selected";
                                    };
                                    break;
                               
                            }
                        };break;
                    case OrderStatusEnum.PODOnlineSelected:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Pay Online";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = true;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = true;
                                        DisplayStatus = "Pay Online";
                                    };
                                    break;
                                
                            }
                        };break;
                    case OrderStatusEnum.DeliveryReached:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = true;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Delivery Boy Reached";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = true;
                                        IsOrderPaid = false;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = true;
                                        IsOrderInProcess = true;
                                        DisplayStatus = "Delivery Boy Reached";
                                    };
                                    break;
                                
                            }

                        };break;
                    case OrderStatusEnum.PODPaymentSuccess:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = true;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Payment Success";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = true;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Payment Success";
                                    };
                                    break;
                               
                            }
                        };break;
                    case OrderStatusEnum.PODCashPaid:
                        {
                            switch (pType)
                            {
                                case OrderPaymentType.POO:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = true;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Payment Success";
                                    };
                                    break;
                                case OrderPaymentType.POD:
                                    {
                                        IsReached = false;
                                        IsOrderPaid = true;
                                        IsReorder = false;
                                        IsRetry = false;
                                        IsCancelled = false;
                                        IsCancellable = false;
                                        IsTrack = false;
                                        IsOrderInProcess = false;
                                        DisplayStatus = "Payment Success";
                                    };
                                    break;
                                
                            }
                        };break;
                    case OrderStatusEnum.BackToStore:
                        {
                            IsReached = false;
                            IsOrderPaid = false;
                            IsReorder = false;
                            IsRetry = false;
                            IsCancelled = false;
                            IsCancellable = false;
                            IsTrack = false;
                            IsOrderInProcess = false;
                            IsDelivered = false;
                            DisplayStatus = "Back To Store";


                        }; break;
                    default:
                        {
                            DisplayStatus = ordData.OrderStatusName;

                        }; break;
                }
                if (objects.MixerDetails.Count > 0)
                {
                    if (objects.MixerDetails.FirstOrDefault().SupplierId > 0)
                    {
                        IsTrack = false;
                    }
                }
                var datalist = new
                {
                    Id = ordData.Id,
                    OrderDate = ordData.OrderDate,
                    LicPermitNo = ordData.LicPermitNo,
                    ShopId = ordData.ShopId,
                    ZoneId = ordData.ZoneId,
                    OrderAmount = ordData.OrderAmount,
                    DeliveryDate = ordData.DeliveryDate,
                    PaymentTypeId = ordData.PaymentTypeId,
                    OrderStatu = new
                    {
                        OrderStatusName = ordData.OrderStatusName,
                        Id = ordData.OStatisId
                    },
                    Address = new
                    {
                        ordData.CustomerAddressId,
                        ordData.Address,
                        ordData.FormattedAddress,
                        ordData.PlaceId,
                        ordData.Flat,
                        ordData.Landmark,
                        ordData.ShopId,
                        ordData.Longitude,
                        ordData.Latitude,
                        ordData.ZoneId,
                        ordData.AddressType,
                        ordData.IsDeleted
                    },
                    Review = new
                    {
                        ordData.Rating,
                        ordData.Title,
                        ordData.Review,
                        ordData.Comment,
                        CreatedDate = ordData.ReviewDate
                    },
                    OrderDetails = objects.OrderDetails,
                    MixerDetails = objects.MixerDetails,
                    TotalItems=(objects.OrderDetails.Count() + objects.MixerDetails.Count()),
                    DisplayStatus ,
                    IsReached,
                    IsOrderPaid,
                    IsReorder,
                    IsRetry,
                    IsReviewd,
                    IsCancelled,
                    IsCancellable,
                    IsTrack,
                    IsOrderInProcess,
                    IsDelivered

            };
                responseStatus.Data = datalist;

                if (objects == null)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                return Ok(responseStatus);

            }
        }

        [HttpGet]
        [Route("filters")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetFliters()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            FiltersDBO filtersDBO = new FiltersDBO();
            var Data = filtersDBO.Filters();
           
            responseStatus.Data = Data;
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("products-by-filter")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductItems(ProductFilter productFilter)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            ProductDBO productDBO = new ProductDBO();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var objects = productDBO.ProductItems(productFilter,custId.Id);
                var datalist = from d in objects
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
               responseStatus.Data = datalist;

                
                if (objects.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }


                return Ok(responseStatus);
            }
        }
        [HttpPost]
        [Route("product-by-shop")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductByShop(ProductSearchViewModel search)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ProductDBO productDBO = new ProductDBO();
                var objects = productDBO.GetProductDetailsPagination(search,custId.Id);
                var datalist = from d in objects
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity=d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

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

        [HttpPost]
        [Route("product-search")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductSearch(ProductSearchViewModel search)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            int PageNumber = search.Index;
            int RecordsPerPage = search.Size;
            string str = search.SearchText;
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetProductSearchPagination(search,custId.Id);
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
                if (!string.IsNullOrWhiteSpace(search.SearchText))
                {
                    str = char.ToUpper(str[0]) + str.Substring(1);
                    var prod1 = prod.Where(o => o.ProductName.Contains(str) || o.Category.Contains(str) || o.BrandName.Contains(str)).ToList();
                    prod = prod1;
                }
                var dt = prod.Skip((PageNumber - 1) * RecordsPerPage)
                                        .Take(RecordsPerPage);
                var datalist = from d in dt
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
               responseStatus.Data = datalist;

                
                if (prod.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

                return Ok(responseStatus);
            }
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
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetFilteredProductDetailsPagination(search,custId.Id);
                var datalist = from d in prod
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
               responseStatus.Data = datalist;

                
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
            }
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
            var prod = productDBO.GetAllSearchProductDetail(search);
            responseStatus.Data = prod.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }


        [HttpGet]
        [Route("address/{index}/{size}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetCustomerAddress(int index, int size)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            int PageNumber = index;
            int RecordsPerPage = size;
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var address = db.CustomerAddresses.Where(o => o.Customer.UserId == uId).ToList();
                responseStatus.Data = address.Skip((PageNumber - 1) * RecordsPerPage)
                                    .Take(RecordsPerPage);
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
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetRecomProductDetail(shopId,custId.Id);
                var datalist = from d in prod
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
                responseStatus.Data = datalist;

                

            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("GetRecommPremiumProductDetails/{shopId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetRecommPremiumProductDetails(int shopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetRecomPremiumProductDetail(shopId, custId.Id);
                var datalist = from d in prod
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

                               };
                responseStatus.Data = datalist;

                

            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("AllPremiumProductDetails")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetAllPremiumProductDetails(GetAllPremiumProductDetails_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false; 
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                ProductDBO productDBO = new ProductDBO();
                var objects = productDBO.GetAllPremiumProductDetail(req.shopId,custId.Id);
                var datalist = from d in objects
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,

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
        [Route("shopaddress")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult GetShopAddress()
        {
            RatingProductDBO ratingProductDBO = new RatingProductDBO();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string uId = User.Identity.GetUserId();
                RUser regUser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
                var objects = ratingProductDBO.GetShopAddress(regUser.DeliveryAgentId ?? 0);
                ResponseStatus responseStatus = new ResponseStatus { Status = true };
                var datalist = from d in objects
                               select new
                               {
                                   Longitude = d.Longitude,
                                   Latitude = d.Latitude,
                                   MeterReachDistance=100

                               };

                responseStatus.Data = datalist;


                if (objects.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

                return Ok(responseStatus);
            }
        }

        [HttpPost]
        [Route("product-search-volume/shop/{shopId}/product/{productRefId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetProductSearch(int shopId, int productRefId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            ProductDBO productDBO = new ProductDBO();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var prod = productDBO.GetProductvolumnByIds(shopId, productRefId,custId.Id);

                responseStatus.Data = prod.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
            }
        }
        [HttpPost]
        [Route("premium-product-search-volume/shop/{shopId}/product/{productRefId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetPremiumProductSearch(int shopId, int productRefId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            ProductDBO productDBO = new ProductDBO();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var prod = productDBO.GetProductPremiumVolByIds(shopId, productRefId,custId.Id);

                responseStatus.Data = prod.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
            }
        }

        [HttpPost]
        [Route("delivery/track-order-start/{orderId}")]
        [Authorize]
        public IHttpActionResult UpdateTrackOrderStart(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (orderId <= 0)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order Id is not valid.";
                return Content(HttpStatusCode.BadRequest, responseStatus);
            }
            string Id = User.Identity.GetUserId();
            var ruser = db.RUsers.Where(o => o.rUserId == Id).FirstOrDefault();


            var agentRoute = db.RoutePlans.Where(o => o.OrderID == orderId && o.DeliveryAgentId == ruser.DeliveryAgentId)?.FirstOrDefault();
            if(agentRoute==null)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order does not found against the delivery boy.";
                return Content(HttpStatusCode.BadRequest, responseStatus);
            }
            if (agentRoute.IsOrderStart==false)
            {
                FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                Task.Run(() => fcmHelper.SendFirebaseNotification(orderId, FirebaseNotificationHelper.NotificationType.Order));
            }
            
            agentRoute.IsOrderStart = true;
            db.SaveChanges();
            // Live Tracking Changes

            CustomerApi2Controller.AddToFireStore(orderId);

            db.Dispose();

            

            responseStatus.Status = true;
            responseStatus.Message = "Order start for delivery by delivery boy.";

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("delivery/track-order-end/{orderId}")]
        [Authorize]
        public IHttpActionResult UpdateTrackOrderEnd(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            if (orderId <= 0)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order Id is not valid.";
                return Content(HttpStatusCode.BadRequest, responseStatus);
            }
            string Id = User.Identity.GetUserId();
            var ruser = db.RUsers.Where(o => o.rUserId == Id).FirstOrDefault();


            var agentRoute = db.RoutePlans.Where(o => o.OrderID == orderId && o.DeliveryAgentId == ruser.DeliveryAgentId)?.FirstOrDefault();
            if (agentRoute == null)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order does not found against the delivery boy.";
                return Content(HttpStatusCode.BadRequest, responseStatus);
            }

            agentRoute.IsOrderStart = false;
            db.SaveChanges();
            db.Dispose();
            CustomerApi2Controller.AddToFireStore(orderId);
            responseStatus.Status = true;
            responseStatus.Message = "Order stop for delivery by delivery boy.";

            return Ok(responseStatus);
        }


        [HttpPost]
        [Route("delivery/order-reached/{orderId}")]
        [Authorize]
        public IHttpActionResult DeliveryOrderReached(int orderId)
        {
            if (orderId == 0)
            {
                return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string uId = User.Identity.GetUserId();

                    var statusCancel = (int)OrderStatusEnum.CancelByCustomer;
                    var statusDelReached = (int)OrderStatusEnum.DeliveryReached;
                    var statusPODCashSel = (int)OrderStatusEnum.PODCashSelected;
                    var order = db.Orders.Find(orderId);
                    if (order.OrderStatusId == statusDelReached || order.OrderStatusId == statusPODCashSel)
                    {
                        responseStatus.Status = true;
                        responseStatus.Message = "Update order status as REACHED";
                       
                    }
                   else if (order.OrderStatusId == statusCancel)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "The customer has cancelled this order. Please proceed with the next order.";

                    }
                    
                    else
                    {
                        //Live Tracking FireStore
                        OrderDBO orderDBO = new OrderDBO();
                        orderDBO.UpdatedOrderStatus(orderId, uId, OrderStatusEnum.DeliveryReached.ToString());
                        CustomerApi2Controller.AddToFireStore(orderId);
                        OrderIssueDBO cashDBO = new OrderIssueDBO();
                        var issue = cashDBO.GetPendingPay(orderId);
                        //var issue = db.OrderIssues.Where(o => o.OrderId == orderId && (o.IsCashOnDelivery ?? false) == true)?.OrderByDescending(o => o.OrderIssueId).FirstOrDefault();


                        if (issue != null)
                        {
                            IssueType[] issueTypePay = new[] { IssueType.PartialPay, IssueType.PartialRefund };
                            int[] issueTypePayValue = issueTypePay.Cast<int>().ToArray();

                            string orderIssueTypePay = "";
                            if (issue.IsCashOnDelivery ?? false)
                            {
                                if (issueTypePayValue.Contains(issue.OrderIssueTypeId ?? 0))
                                {
                                    orderIssueTypePay = ((issue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialPay) ? "CashPay" : "CashRefund";
                                }
                            }

                            responseStatus.Data = new { AdjustAmt = issue.AdjustAmt, AmtType = orderIssueTypePay };
                        }

                        responseStatus.Status = true;
                        responseStatus.Message = "Update order status as REACHED.";
                    }
                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    Task.Run(async () => await fcmHelper.SendFirebaseNotification(orderId, FirebaseNotificationHelper.NotificationType.Order));
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_DeliveryOrderReached: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

        }

        [HttpPost]
        [Route("delivery/cash-on-delivery-confirm/{orderId}")]
        public IHttpActionResult CashOnDelivery(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    int stCod = (int)OrderPaymentType.COD;
                    int payType = (int)OrderPaymentType.OCOD;
                    int stOCODCashPaid = (int)OrderStatusEnum.OCODCashPaid;
                    int stPODCashPaid = (int)OrderStatusEnum.PODCashPaid;
                    int stApproved = (int)OrderStatusEnum.Approved;

                    db.Configuration.ProxyCreationEnabled = false;

                    var routeOrder = db.RoutePlans.Include(o => o.Order).Where(o => o.OrderID == orderId)?.FirstOrDefault();
                    var order = db.Orders.Find(orderId);


                    OrderIssueDBO cashDBO = new OrderIssueDBO();
                    var issue = cashDBO.GetPendingPay(orderId);
                    var delPayment = db.DeliveryPayments.Where(o => o.OrderId == orderId)?.FirstOrDefault();


                    int trackStatus;
                    if (order.PaymentTypeId == payType || delPayment ==null)
                    {
                        trackStatus = stOCODCashPaid;
                        delPayment = AgentCashOrder(orderId);
                    }
                    else
                    {
                        trackStatus = stPODCashPaid;
                        delPayment = db.DeliveryPayments.Where(o => o.OrderId == orderId)?.OrderByDescending(o => o.DeliveryPaymentId).FirstOrDefault(); // db.DeliveryPayments.Find(deliverypaymentid);
                        delPayment.DelPaymentConfirm = true;
                        db.SaveChanges();

                        //var order = db.Orders.Find(delPayment.OrderId);
                        order.OrderStatusId = stPODCashPaid;
                        db.SaveChanges();
                    }
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();

                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = stApproved,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = trackStatus,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();



                    if (issue != null)
                    {
                        var amountPaid = (issue != null && issue.AdjustAmt > 0) ?
                            Math.Round(issue.AdjustAmt ?? 0) :
                            Convert.ToDouble(order.OrderAmount);

                        //var deliveryPayment = db.DeliveryPayments.Where(o => o.OrderId == orderId)?.FirstOrDefault();


                        delPayment.AmountPaid = amountPaid;
                        db.SaveChanges();

                        int stTrack = 0;
                        int stPayRefund = (int)OrderStatusEnum.CashPartialRefundMade;
                        int stPayPay = (int)OrderStatusEnum.CashPartialPaymentCollected;

                        if ((issue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialPay)
                            stTrack = stPayPay;
                        if ((issue.OrderIssueTypeId ?? 0) == (int)IssueType.PartialRefund)
                            stTrack = stPayRefund;

                        orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = order.Id,
                            StatusId = stTrack,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();
                    }

                    var oSerializeAgent = JsonConvert.SerializeObject(delPayment, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryPayment>(oSerializeAgent);
                    responseStatus.Data = deSerializeAgent;
                    responseStatus.Status = true;
                    responseStatus.Message = "Cash collected.";
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
                    SpiritUtility.AppLogging($"Api_CashOnDelivery: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        private DeliveryPayment AgentCashOrder(int orderId)
        {
            int stCOODCashPaid = (int)OrderStatusEnum.OCODCashPaid;
            int stOcod = (int)OrderPaymentType.OCOD;

            var order = db.Orders.Find(orderId);
            var routeOrder = db.RoutePlans.Include(o => o.Order).Where(o => o.OrderID == orderId)?.FirstOrDefault();

            DeliveryPayment deliveryPayment = new DeliveryPayment
            {
                AmountPaid = Convert.ToDouble(order.OrderAmount),
                CreatedDate = DateTime.Now,
                DeliveryAgentId = routeOrder.DeliveryAgentId,
                DelPaymentConfirm = true,
                JobId = routeOrder.JobId,
                OrderId = order.Id,
                ShopId = order.ShopID,
                PaymentTypeId = stOcod,
                ShopAcknowledgement = false
            };
            db.DeliveryPayments.Add(deliveryPayment);
            db.SaveChanges();

            order.OrderStatusId = stCOODCashPaid;
            db.SaveChanges();

            return deliveryPayment;
        }


        [HttpGet]
        [Route("Ordertracking/{orderId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult Ordertracking(int orderId)
        {

            int pageId = (int)PageNameEnum.CART;
            string pageVersion = "1.6";
            var cont = SZIoc.GetSerivce<IPageService>();
            var content = cont.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var objects = routePlanDBO.Ordertracking(orderId);
            List<OrderTrackingDO> tstatus = new List<OrderTrackingDO>();
            int stSubmitted = (int)OrderStatusEnum.Submitted;
            int stPacked = (int)OrderStatusEnum.Packed;
            int stOutForDelivery = (int)OrderStatusEnum.OutForDelivery;
            int stDelivered = (int)OrderStatusEnum.Delivered;
            int stCancel = (int)OrderStatusEnum.Cancel;
            bool IsDelivered = false;
            bool IsCancelled = false;
            if (objects == null)
            {
                responseStatus.Message = "Data Is Not Found";
                responseStatus.Status = false;
                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            if (objects.OrderTrack != null)
            {
                
                tstatus.Add(new OrderTrackingDO
                {

                    Title = conCart[PageContentEnum.Text57.ToString()],
                    SubTitle = ""

                });


                tstatus.Add(new OrderTrackingDO
                {

                    Title = conCart[PageContentEnum.Text58.ToString()],
                });

                tstatus.Add(new OrderTrackingDO
                {

                    Title = conCart[PageContentEnum.Text59.ToString()],
                    SubTitle = "",
                });
                objects.OrderTrack.ToList().ForEach((o) =>
                {
                    if (o.OrderStatu.Id == stSubmitted)
                    {

                        tstatus[0].OrderStatusId = o.OrderStatu.Id;
                        tstatus[0].StatusName = o.OrderStatu.OrderStatusName;
                        tstatus[0].Time = o.TrackDate.ToString("HH:mm tt");
                        tstatus[0].Date = o.TrackDate.ToString("dd/MM/yyyy");
                        tstatus[0].IsCurrent = true;
                        tstatus[1].Title = conCart[PageContentEnum.Text60.ToString()];
                    }
                    if (o.OrderStatu.Id == stPacked)
                    {
                        tstatus[1].OrderStatusId = o.OrderStatu.Id;
                        tstatus[1].StatusName = o.OrderStatu.OrderStatusName;
                        tstatus[1].Time = o.TrackDate.ToString("HH:mm tt");
                        tstatus[1].Date = o.TrackDate.ToString("dd/MM/yyyy");
                        tstatus[1].IsCurrent = true;
                        tstatus[1].Title = conCart[PageContentEnum.Text58.ToString()];

                    }
                    if (o.OrderStatu.Id == stOutForDelivery)
                    {
                        tstatus[2].OrderStatusId = o.OrderStatu.Id;
                        tstatus[2].StatusName = o.OrderStatu.OrderStatusName;
                        tstatus[2].Time = o.TrackDate.ToString("HH:mm tt");
                        tstatus[2].Date = o.TrackDate.ToString("dd/MM/yyyy");
                        tstatus[2].IsCurrent = true;
                        tstatus[2].Title = conCart[PageContentEnum.Text61.ToString()];
                        tstatus[2].SubTitle = conCart[PageContentEnum.Text62.ToString()];
                        tstatus[1].Title = conCart[PageContentEnum.Text58.ToString()];
                    }
                    if (o.OrderStatu.Id == stDelivered)
                    {
                        IsDelivered = true;
                        
                    }
                    if (o.OrderStatu.Id == stCancel)
                    {
                        IsCancelled
                        = true;
                        
                    }

                });

               
            }


            var datalist = new
            {
                OrderId = objects.Order.Id,
                Title = conCart[PageContentEnum.Text69.ToString()],
                SubTitle = conCart[PageContentEnum.Text70.ToString()],
                IsDelivered,
                IsCancelled,
                ShopDetail = new
                {
                    ShopId = objects.WineShop.Id,
                    ShopName = objects.WineShop.ShopName,
                    Latitude = objects.WineShop.Latitude,
                    Longitude = objects.WineShop.Longitude,
                    IsOrderStart = objects.IsOrderStart
                },

                OrderDetail = from d in objects.Order.OrderDetails
                              select new
                              {
                                  OrderDetailsId = d.Id,
                                  OrderId = d.OrderId,
                                  TotalItems = d.ItemQty
                              },

                OrderStatuses = tstatus,

                DeliveryAgent = new
                {
                    AgentId = objects.DeliveryAgent.Id,
                    DeliveryBoyNo = objects.DeliveryAgent.Contact
                }
            };

            responseStatus.Data = datalist;
           


            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("customeraddress/{orderId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetCustomerAddress(int orderId)
        {
            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var data = routePlanDBO.GetCustomerAddress(orderId);
            responseStatus.Data = data;

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("checkoutfordelivery/{orderId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult CheckOutForDelivery(int orderId)
        {
            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var data = routePlanDBO.CheckOutForDelivery(orderId);
            responseStatus.Data = data;

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("dynamiccontent")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetDynamicPageContent()
        {
            int pageId = (int)PageNameEnum.CART;
            string pageVersion = "1.6";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId,pageVersion);

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            responseStatus.Data = content;
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
            var ordergrouName =default(string);
            DateTime orderCommittedStartDate = DateTime.Now;
            if (model.OrderItems.Count > 0)
            {
               var data= orderDBO.GetCommittedDate(currentDate,model.ShopId);
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

            if (model.MixerItems !=null && model.MixerItems.Count > 0)
            {
               
                foreach (var item in model.MixerItems)
                {
                    var result = orderDBO.GetMixerCommittedDate(currentDate, item.MixerDetailId, model.ShopId).FirstOrDefault();
                    item.CommittedEndDate = result.CommittedEndDate;
                    item.CommittedStartDate = result.CommittedStartDate;
                    item.SupplierId= result.SupplierId;
                    item.MixerType = result.MixerType;
                }
                var mixergroup=model.MixerItems.GroupBy(o => new { o.CommittedStartDate, o.SupplierId });
                foreach (var item in mixergroup)
                {
                    var gItem = item.ToList();
                    index += 1;
                    grouName += index.ToString();
                    if(gItem!=null)
                    {
                        foreach (var item2 in gItem)
                        {
                            if(item2.CommittedStartDate == orderCommittedStartDate && item2.SupplierId == 0)
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
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o=>o.Key,o=>o.Value);
            var Title = conCart[PageContentEnum.Text2.ToString()];

            var Od = model.OrderItems.Select(a => new OrderCartDetails()
            {
                ProductId = a.ProductId,
                ProductName=$"{a.ProductName} - Volume: {a.Capacity}",
                Quantity = a.Qty,
                OrderGroupId=a.OrderGroupId,
                IsMixer=false

            }).ToList();
            if (model.MixerItems.Count>0)
            {
                Od.AddRange(model.MixerItems.Select(a => new OrderCartDetails()
                {
                    ProductId = a.MixerDetailId,
                    ProductName= $"{a.ProductName} - Size: {a.Capacity}",
                    Quantity = a.Qty,
                    OrderGroupId = a.OrderGroupId,
                    SupplierId=a.SupplierId,
                    IsMixer = true

                }).ToList());
            }
            var groupData = model.OrderItems.GroupBy(x => x.OrderGroupId).Select(x => new ScheduleSlot()
            {
                OrderGroupId = x.FirstOrDefault().OrderGroupId,
                TimeSlot = $"{x.FirstOrDefault().CommittedStartDate.ToString("hh:mm tt")} - {x.FirstOrDefault().CommittedEndDate.ToString("hh:mm tt")}",
                Title = conCart[PageContentEnum.Text19.ToString()],
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
                Title = conCart[PageContentEnum.Text19.ToString()],
                Date = x.FirstOrDefault().CommittedStartDate.Day,
                FullDate = x.FirstOrDefault().CommittedStartDate.ToString("dd/MM/yyyy"),
                DisplayDay= x.FirstOrDefault().CommittedStartDate.ToString("dddd"),
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
           
                Description = payType.Where(o =>o.PaymentTypeId==model.PaymentTypeId).FirstOrDefault().Description;
            
            var dt = from d in groupData.DistinctBy(x=>x.OrderGroupId)
                     select new
                     {
                         SlotID=d.OrderGroupId,
                         Title = "Order Details",
                         OrderDetails = Od.Where(x =>x.OrderGroupId==d.OrderGroupId).ToList(),
                         SheduledSlot = d,
                         ModeOfPayment= Description,
                     };

            responseStatus.Data = new {
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

            using (var db = new rainbowwineEntities())
            {
                int custAdress = model.AddressId;
                var aspUser = db.AspNetUsers.Where(o => o.Id == uId)?.FirstOrDefault();
                Customer customer = db.Customers.Where(o => o.UserId == uId)?.FirstOrDefault();
                string userName = aspUser.Email;
                CustomerAddress customerAddress = db.CustomerAddresses.Where(o => o.CustomerId == customer.Id && o.CustomerAddressId == custAdress)?.FirstOrDefault();

                bool operationFlag = db.WineShops.Find(customerAddress.ShopId)?.OperationFlag ?? false;

                if (!operationFlag)
                    return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Unfortunately, the selected shop is not operating at the moment. Please choose another shop.", Data = model });

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
                //using (var transaction = db.Database.BeginTransaction())
                //{
                try
                {
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

                    if (mixerwithoutorder!=null)
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
                                    var mixerDetail =db.MixerDetails.Include(o=>o.Mixer).Where(o=>o.MixerDetailId== mixerDetailId)?.FirstOrDefault();
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

                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    Task.Run( () => fcmHelper.SendFirebaseNotification(order.Id, FirebaseNotificationHelper.NotificationType.Order));
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
        [Route("checkinventry/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult CheckInventory(int shopId, StockFavorite stockFavorite)
        {
            ProductDBO productDBO = new ProductDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

           
            var allProducts = new List<ProductDetailDO>();
             List<ProductDetailDO> prodDetail = null;
            foreach (var item in stockFavorite.Items)
            {
                if (item.IsMixer)
                {
                    prodDetail = productDBO.CheckMixerInventory(shopId, item.ProductId);
                }
                else
                {
                    prodDetail = productDBO.GetProductDetailsById(shopId, item.ProductId);
                    
                }
                if (prodDetail.Count > 0) allProducts = allProducts.Concat(prodDetail).ToList();
            }
            var data = from d in allProducts
                       select new
                       {
                           ProductId=d.ProductID,
                           AvailableQty=d.QtyAvailable,
                           IsMixer=d.IsMixer


                       };
            responseStatus.Data = data;


            if (allProducts.Count() == 0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("inventory-update")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult ImventoryUpdated(OrderDelivery_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            int orderId = Convert.ToInt32(req.OrderId.Replace("OG_", ""));

            var ord = db.Orders.Find(orderId);
            if (ord == null)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order not found.";

                return Content(HttpStatusCode.NotFound, responseStatus);
            }

            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
            if (req.OrderId.Contains("OG_"))
            {
                var groupOrders = db.Orders.Where(o => string.Compare(o.OrderGroupId, req.OrderId, true) == 0)?.ToList();
                foreach (var item in groupOrders)
                {
                    paymentLinkLogsService.InventoryMixerUpdate(item.Id);
                }
            }
            else
            {
                paymentLinkLogsService.InventoryUpdate(ord.Id);
            }

            //responseStatus.Data = ord;
            responseStatus.Status = true;
            responseStatus.Message = "Inventory updated successfully.";
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("success-content/{orderid}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult SuccessContent(string orderid)
        {
            OrderDBO orderDBO = new OrderDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var objects = orderDBO.GetOrders(orderid);
            var data= new
            {
                Order_No= objects.OrderIds,
                Title= "Your order will be delivered soon! \n Thank you for using our app.",
                //Description = "Our delivery window is between 10 am to 7 pm. Due to the Government's operational guidelines for COVID, the delivery schedules might be affected."
                Description="We deliver from 10 am to 10 pm. Some areas may experience delivery delays based on local conditions."
            };
            responseStatus.Data = data;


                if (objects.OrderIds.Count()==0)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("mixerproductdetail/{mixerid}/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetMixerProductDetail(int mixerId, int shopId)
        {
            ProductDBO productDBO = new ProductDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var objects = productDBO.MixerProductDetail(custId.Id, mixerId, shopId);
                

            responseStatus.Data = objects;


            if (objects == null)
            {
                responseStatus.Message = "Data Is Not Found";
                responseStatus.Status = false;
                return Content(HttpStatusCode.NotFound, responseStatus);
            }

        }


            return Ok(responseStatus);
        }


        [HttpGet]
        [Route("sharing-content/{productId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult SharingContent(string productId)
        {
            OrderDBO orderDBO = new OrderDBO();
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            int pageId = (int)PageNameEnum.CART;
            string pageVersion = "1.6";
            var cont = SZIoc.GetSerivce<IPageService>();
            var content = cont.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            responseStatus.Data = conCart[PageContentEnum.Text54.ToString()];

            return Ok(responseStatus);
        }

        #region API Consolidation
        [HttpPost]
        [Route("product-ref-data")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductRefData(RecommendedParamters recommendedParamters)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                //recommended
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                if (custId.Id > 0)
                {
                    var add = db.CustomerAddresses.Where(ca => ca.CustomerId == custId.Id && ca.ShopId == recommendedParamters.ShopId).FirstOrDefault();
                    recommendedParamters.CustomerId = custId.Id;
                    recommendedParamters.DOB = custId.DOB;
                    recommendedParamters.Address = add.FormattedAddress;
                }
                else
                {
                    responseStatus.Message = "CustomerId Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
            }

            // product id for purchase
            var purchaseInput = new
            {
                CustomerId = recommendedParamters.CustomerId,
                ShopID = recommendedParamters.ShopId,
                NUM = Convert.ToInt32(ConfigurationManager.AppSettings["UserProductNum"])
            };
            using (HttpClient client = new HttpClient())
            {
                var serializeJson = JsonConvert.SerializeObject(purchaseInput);
                var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                var resp = client.PostAsync(ConfigurationManager.AppSettings["UserProduct"], content).Result;
                var ret = JsonConvert.DeserializeObject<ProductRecommandation>(resp.Content.ReadAsStringAsync().Result);
                if (ret != null)
                {
                    List<int> li = new List<int>();

                    var data = from d in ret.Response
                               select new
                               {
                                   d.Details.ProductID
                               };

                    foreach (var item in data)
                    {
                        li.Add(item.ProductID);
                    }
                    recommendedParamters.ProductIdsForPurchase = li.ToArray();
                }
                else
                {
                    responseStatus.Message = "External Api Is Not Respond";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);

                }
            }

            //product id for location
            var locationInput = new
            {

                CustomerId = recommendedParamters.CustomerId,
                DOB = recommendedParamters.DOB,
                NUM = Convert.ToInt32(ConfigurationManager.AppSettings["ProductLocationNum"]),//recommendedParamters.NUM,
                ShopID = recommendedParamters.ShopId,
                Adress = recommendedParamters.Address

            };

            using (HttpClient client = new HttpClient())
            {

                var serializeJson = JsonConvert.SerializeObject(locationInput);
                var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                var resp = client.PostAsync(ConfigurationManager.AppSettings["ProductLocation"], content).Result;
                var ret = JsonConvert.DeserializeObject<ProductRecommandation>(resp.Content.ReadAsStringAsync().Result);
                if (ret != null)
                {
                    List<int> li = new List<int>();
                    var data = from d in ret.Response
                               select new
                               {
                                   d.Details.ProductID
                               };
                    foreach (var item in data)
                    {
                        li.Add(item.ProductID);
                    }
                    recommendedParamters.ProductIdsForlocation = li.ToArray();
                }
                else
                {
                    responseStatus.Message = "External Api Is Not Respond";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

            }

            var productService = SZIoc.GetSerivce<IProductService>();
            var response = productService.GetProductRefData(recommendedParamters.ProductIdsForPurchase, recommendedParamters.ProductIdsForlocation, recommendedParamters.ShopId, recommendedParamters.CustomerId);

            responseStatus.Data = response;
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("cartdata/{CartId}/{shopId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetCartData(int cartId, int shopId)
        {
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var miniOrd = c.GetConfigValue(ConfigEnums.MinLimitOrder.ToString());
            var miniOrdMsg = c.GetConfigValue(ConfigEnums.MinLimitOrderMsg.ToString());
            var upiEnable = c.GetConfigValue(ConfigEnums.UPIEnable.ToString());
            var podEnable = c.GetConfigValue(ConfigEnums.PODEnable.ToString());
            var isPODTempFixed = c.GetConfigValue(ConfigEnums.IsPODTempFixed.ToString());
            var maxiumPodAmount = c.GetConfigValue(ConfigEnums.MaxiumPodAmount.ToString());
            var isMixerPOD = c.GetConfigValue(ConfigEnums.IsMixerPOD.ToString());
            var deliveryCharges = c.GetConfigValue(ConfigEnums.DeliveryCharges.ToString());
            bool IsMixerPOD = false;
            if (isMixerPOD == "1")
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

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            CartContentResponse result = new CartContentResponse();

            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var cartService = SZIoc.GetSerivce<ICartService>();
                var response = cartService.GetCartData(cartId, shopId, custId.Id);

                if (response == null)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }

                if (response.customerExt != null)
                {
                    if (podEnable=="1") {
                        if (response.customerExt.WineShop.IsPOD == true && response.customerExt.IsPOD == true)
                        {
                            response.customerExt.WineShop.IsPOD = true;

                        }
                        else if (response.customerExt.WineShop.IsPOD == true && response.customerExt.IsPOD == false)
                        {
                            response.customerExt.WineShop.IsPOD = true;

                        }
                        else if (response.customerExt.WineShop.IsPOD == false)
                        {
                            response.customerExt.WineShop.IsPOD = true;
                        } 
                    }
                    else
                    {
                        response.customerExt.WineShop.IsPOD = false;
                    }
                }
                var payTypelist = new List<PaymentTypeResponse>();
                if (response.paymentTypes != null)
                {
                    foreach (var item in response.paymentTypes)
                    {
                        if (isPODTempFixed == "1")
                        {
                            if (item.PaymentTypeId == 1)
                            {
                                payTypelist.Add(new PaymentTypeResponse { PaymentTypeId = item.PaymentTypeId, Title = item.Description, Description = conCart[PageContentEnum.Text11.ToString()] });
                            }
                            if (item.PaymentTypeId == 2)
                            {
                                payTypelist.Add(new PaymentTypeResponse { PaymentTypeId = item.PaymentTypeId, Title = item.Description, Description = conCart[PageContentEnum.Text13.ToString()] });
                            }
                        }
                        else
                        {
                            if (item.PaymentTypeId == 1)
                            {
                                payTypelist.Add(new PaymentTypeResponse { PaymentTypeId = item.PaymentTypeId, Title = item.Description, Description = conCart[PageContentEnum.Text11.ToString()] });
                            }
                            if (item.PaymentTypeId == 2)
                            {
                                payTypelist.Add(new PaymentTypeResponse { PaymentTypeId = 1, Title = item.Description, Description = conCart[PageContentEnum.Text13.ToString()] });
                            }
                        }

                    }
                   
                }

                CartContent cartContent = new CartContent
                {
                    BillDetails = new BillDetails
                    {
                        Title = conCart[PageContentEnum.Text2.ToString()],
                        Details = new List<BillItem>
                    {
                       new BillItem {
                        Text = conCart[PageContentEnum.Text3.ToString()],
                        Default = 0.ToString(),
                        Key ="Permit_Cost"
                    },
                        new BillItem
                    {
                        Text = conCart[PageContentEnum.Text4.ToString()],
                        Default = 0.ToString(),
                        Key ="Liquor_Cost"
                    },
                     new BillItem{
                        Text = conCart[PageContentEnum.Text5.ToString()],
                        Default = 0.ToString(),
                        Key ="Mixer_Cost"
                    },
                      new BillItem{
                        Text =conCart[PageContentEnum.Text23.ToString()],
                        Default = deliveryCharges,
                        Key ="Delivery_Charges"
                    },
                      new BillItem {
                        Text =conCart[PageContentEnum.Text67.ToString()],
                        Default =0.ToString(),
                        Key ="Spiritzone_Credits"
                    },
                       new BillItem{
                        Text =conCart[PageContentEnum.Text56.ToString()],
                        Default = 0.ToString(),
                        Key ="Discount"
                    },
                      new BillItem{
                        Text = conCart[PageContentEnum.Text6.ToString()],
                        Default = 0.ToString(),
                        Key ="Total_Amount"
                    }
                    },
                    },
                    PaymentType = payTypelist,
                    IsMixerPOD = IsMixerPOD,
                    IsOPD = response.customerExt.WineShop.IsPOD,
                    MiniumOrder = Convert.ToInt32(miniOrd),
                    MiniumOrderMessage = miniOrdMsg,
                    UPIEnable = upiEnable,
                    MaxiumPodAmount = Convert.ToInt32(maxiumPodAmount),
                    DeliveryMessage = conCart[PageContentEnum.Text7.ToString()],
                    LeftInStkMsg = conCart[PageContentEnum.Text55.ToString()],
                    Image1 = pageImages[PageImageEnum.Image1.ToString()],
                    Image2 = pageImages[PageImageEnum.Image2.ToString()],
                    Image3 = pageImages[PageImageEnum.Image3.ToString()],
                    Image4 = pageImages[PageImageEnum.Image4.ToString()],
                    Image5 = pageImages[PageImageEnum.Image5.ToString()],
                    Text1 = conCart[PageContentEnum.Text63.ToString()],
                    Text2 = conCart[PageContentEnum.Text64.ToString()],
                    Text3 = conCart[PageContentEnum.Text65.ToString()],
                    Text4 = conCart[PageContentEnum.Text66.ToString()],
                    Balance = response.customerExt.ReferBalance.Balance,
                    PromoAppliedChangeLineItem= conCart[PageContentEnum.Text68.ToString()],
                    SZCreditsCanUse = Convert.ToInt32(c.GetConfigValue(ConfigEnums.SZCreditsCanUse.ToString())),
                    IsWalletForPOD = Convert.ToInt32(c.GetConfigValue(ConfigEnums.IsWalletForPOD.ToString())) == 0 ? false : true,
                    CanWeUSeWalletForOrder = Convert.ToInt32(c.GetConfigValue(ConfigEnums.CanWeUSeWalletForOrder.ToString())) == 0 ? false : true
                };
                result.CartContent = cartContent;
                result.cartItems = response.cartItems;
                responseStatus.Data = true;
                responseStatus.Data = result;
            }


            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("getproductdetail-with-offer/shop/{shopId}/promoCode/{promoCode}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductDetailWithOffer(int shopId, string promoCode)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                //Id = "a0e37c0b-d37e-4d69-814d-67b0b3a08d55";
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                var productService = SZIoc.GetSerivce<IProductService>();
                var objects = productService.GetProductDetailByPromoCode(shopId, custId.Id, promoCode);



                if (objects == null)
                {
                    responseStatus.Message = "Data Is Not Found";
                    responseStatus.Status = false;
                    return Content(HttpStatusCode.NotFound, responseStatus);
                }
                var datalist = from d in objects
                               select new
                               {
                                   ProductId = d.ProductID,
                                   ProductName = d.ProductName,
                                   ProductRefID = d.ProductRefID,
                                   ProductThumbImage = d.ProductThumbImage,
                                   ProductImage = d.ProductImage,
                                   BrandName = d.BrandName,
                                   Category = d.Category,
                                   Price = d.Price,
                                   Capacity = d.Capacity,
                                   IsCustomerRated = d.IsCustomerRated,
                                   ProductRating = d.ProductRating,
                                   RatingCount = d.RatingCount,
                                   AverageRating = d.AverageRating,
                                   IsFavourite = d.IsFavourite,



                               };
                responseStatus.Data = datalist;
            }
            return Ok(responseStatus);
        }
        #endregion

        #region THIRD PARTY INVENTORY UPDATE IN OUR SYSTEM
        [HttpPost]
        [Route("v1/third-party/inventory/update")]
        [Authorize(Roles = "Packer")]
        public ThirdPartyResponse UpdateInventoryFromThirdParty(ThirdPartyInventoryModel thirdPartyInventory)
        {
            var result = default(int);
            InventoryDBO inventoryDBO = new InventoryDBO();
            string item_codes = "";
            string status_msg = "";
            string item_codes_missing = "";
            foreach(var invModel in thirdPartyInventory.Data)
            {
                if (invModel.CLBalance < 0)
                    item_codes = item_codes + invModel.ItemCode + ", ";
                else
                {
                    result = inventoryDBO.UpdateInvFromThirdParty(invModel);
                    if (result == -1)
                        item_codes_missing = item_codes_missing + invModel.ItemCode + ", ";
                }
                    
            }
            item_codes_missing = item_codes_missing.Trim(',');
            item_codes = item_codes.Trim(',');
            if (item_codes == "" && item_codes_missing == "")
                status_msg = "All product stocks provided have been updated.";
            var responseObj = new ThirdPartyResponse();
            responseObj.NegativeStock = item_codes;
            responseObj.NonExistentItems = item_codes_missing;
            responseObj.Status_Msg = status_msg;
            responseObj.Timestamp = DateTime.Now;
            return responseObj;
        }
        #endregion

    }
}
