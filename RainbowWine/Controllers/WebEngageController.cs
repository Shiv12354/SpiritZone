using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using SZInfrastructure;

namespace RainbowWine.Controllers
{
    
    [EnableCors("*", "*", "*")]
    public class WebEngageController : ApiController
    {
        CustomerDBO customerDBO = new CustomerDBO();
        [HttpPost]
        [Route("webengagecall/{orderid}/{status}")]
        public IHttpActionResult WebEngageCall(int orderid,string status)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //string uId = User.Identity.GetUserId();
                    //var aspUser = db.AspNetUsers.Find(uId);
                    var c = SZIoc.GetSerivce<ISZConfiguration>();
                    var webEngageEnable = c.GetConfigValue(ConfigEnums.WebEngageEnable.ToString());
                    if (webEngageEnable == "1")
                    {
                        Order order = db.Orders.Find(orderid);
                        var baseUrl = ConfigurationManager.AppSettings["Giftnotiurl"];
                        WebEngageDBO webEngageDBO = new WebEngageDBO();
                        var ordDetails = webEngageDBO.GetWebEngageOrderDetails(orderid, order.CustomerId);
                        string remarks = string.Empty;
                        if (ordDetails != null)
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                HttpContext httpContext = HttpContext.Current;
                                //string authHeader = httpContext.Request.Headers["Authorization"];

                                var data = customerDBO.CkeckCredibleCustomer(order.OrderTo);
                                bool isCredible = false;
                                if (data.Customer == "Credible Customer")
                                {
                                    isCredible = true;
                                    var jsonValue = new
                                    {
                                        userId = order.CustomerId,
                                        eventName = status, //db.OrderStatus.Where(a => a.Id == order.OrderStatusId).FirstOrDefault().OrderStatusName,
                                                            //eventTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss-08000"),//"2021-12-01T18:20:00-0800",
                                        eventData = new
                                        {
                                            OrderId = ordDetails.OrderId,
                                            OrderAmount = ordDetails.OrderAmount,
                                            isCredible,
                                            ProductDetails = ordDetails.LineItems.Select(x => new
                                            {
                                                ProductID = x.ProductId,
                                                ProductName = x.ProductName,
                                                Category= x.Category,
                                                SubCategory=x.SubCategory,
                                                ProductImage = x.ProductImage,
                                                IsMixer = x.IsMixer,
                                                IsGoody = x.IsGoody
                                            }),
                                            ProductNames = string.Join(",", ordDetails.LineItems.Select(y => y.ProductName))
                                        }
                                    };
                                    var serializeJson = JsonConvert.SerializeObject(jsonValue);
                                    var content1 = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["WebEngageToken"]);
                                    var resp = client.PostAsync(ConfigurationManager.AppSettings["WebEngageUrl"], content1).Result;
                                    var ret = resp.Content.ReadAsStringAsync().Result;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_WebEngage: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";

                }
            }

            return Ok(responseStatus);

        }


        public async Task<ResponseStatus> WebEngageStatusCall(int orderid ,string status)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            using (var db = new rainbowwineEntities())
            {
                try
                {
                    //string uId = User.Identity.GetUserId();
                    //var aspUser = db.AspNetUsers.Find(uId);
                    var c = SZIoc.GetSerivce<ISZConfiguration>();
                    var webEngageEnable = c.GetConfigValue(ConfigEnums.WebEngageEnable.ToString());
                    if (webEngageEnable == "1")
                    {
                        Order order = db.Orders.Find(orderid);

                        var baseUrl = ConfigurationManager.AppSettings["PtmSpiritUrl"];
                        WebEngageDBO webEngageDBO = new WebEngageDBO();
                        var ordDetails = webEngageDBO.GetWebEngageOrderDetails(orderid, order.CustomerId);

                        string remarks = string.Empty;
                        if (ordDetails != null)
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                HttpContext httpContext = HttpContext.Current;
                                //string authHeader = httpContext.Request.Headers["Authorization"];
                                CustomerDBO customerDBO = new CustomerDBO();
                                var data = customerDBO.CkeckCredibleCustomer(order.OrderTo);
                                bool isCredible = false;
                                if (data.Customer == "Credible Customer")
                                {
                                    isCredible = true;
                                }
                                var jsonValue = new
                                {
                                    userId = order.CustomerId,
                                    eventName = status, //db.OrderStatus.Where(a => a.Id == order.OrderStatusId).FirstOrDefault().OrderStatusName,
                                                        //eventTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss-08000"),//"2021-12-01T18:20:00-0800",

                                    eventData = new
                                    {
                                        OrderId = ordDetails.OrderId,
                                        OrderAmount = ordDetails.OrderAmount,
                                        isCredible,
                                        ProductDetails = ordDetails.LineItems.Select(x => new
                                        {
                                            ProductID = x.ProductId,
                                            ProductName = x.ProductName,
                                            Category = x.Category,
                                            SubCategory = x.SubCategory,
                                            ProductImage = baseUrl + x.ProductImage,
                                            IsMixer = x.IsMixer,
                                            IsGoody = x.IsGoody
                                        }),
                                        ProductNames=string.Join(",",ordDetails.LineItems.Select(y =>y.ProductName))
                                    }
                                };
                                var serializeJson = JsonConvert.SerializeObject(jsonValue);
                                var content1 = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["WebEngageToken"]);
                                var resp = client.PostAsync(ConfigurationManager.AppSettings["WebEngageUrl"], content1).Result;
                                var ret = resp.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_WebEngage: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";

                }
            }

            return responseStatus;

        }
    }
}
