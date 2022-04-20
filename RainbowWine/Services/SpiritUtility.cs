using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Models.Zoho;
using RainbowWine.Services.PaytmService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RainbowWine.Services
{
    public static class SpiritUtility
    {
        public static string GenerateToken()
        {
            Random generator = new Random();
            return generator.Next(0, 999999).ToString("D6");
        }

        public static string GenerateToken4D()
        {
            Random generator = new Random();
            return generator.Next(0, 9999).ToString("D4");
        }

        public static void OrderTrackingLog(int orderId, string userId, string userName, int orderStatus)
        {
            using (var db = new rainbowwineEntities())
            {
                var orderTrack = new OrderTrack
                {
                    LogUserName = userName,
                    LogUserId = userId,
                    OrderId = orderId,
                    StatusId = orderStatus,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();
            }
        }
        public static void AppLogging(string errormsg, string stackTrace)
        {
            using (var db = new rainbowwineEntities())
            {
                db.AppLogApis.Add(new AppLogApi
                {
                    CreateDatetime = DateTime.Now,
                    Error = errormsg,
                    Message = stackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();
                db.Dispose();
            }
        }
        public static void Logging(string errormsg, string stackTrace)
        {
            using (var db = new rainbowwineEntities())
            {
                db.AppLogs.Add(new AppLog
                {
                    CreateDatetime = DateTime.Now,
                    Error = errormsg,
                    Message = stackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();
                db.Dispose();
            }
        }
        public static CashFreeSetApproveResponse GetCashFreeHooksDeserialize(string hookInput)
        {
            try
            {   //  For testing
                var objdata = HttpUtility.ParseQueryString(hookInput);
                var decodevlaue = new CashFreeSetApproveResponse
                {
                    OrderId = objdata["orderId"],
                    OrderId2 = objdata["orderId"],
                    OrderAmount = objdata["orderAmount"],
                    ReferenceId = objdata["referenceId"],
                    Status = objdata["txStatus"],
                    PaymentMode = objdata["paymentMode"],
                    Msg = objdata["txMsg"],
                    TxtTime = objdata["txTime"],
                    Signature = objdata["signature"]
                };
                var order2 = decodevlaue.OrderId.Split('_');
                if (order2.Count() > 1) { decodevlaue.OrderId = order2[0]; }
                return decodevlaue;
            }
            catch (Exception ex)
            {
                var objdata = JsonConvert.DeserializeObject<hookResponse>(hookInput);
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(objdata.VenderOutput);
                var decodevlaue = new CashFreeSetApproveResponse
                {
                    OrderId = values["orderId"],
                    OrderId2 = values["orderId"],
                    OrderAmount = values["orderAmount"],
                    ReferenceId = values["referenceId"],
                    Status = values["txStatus"],
                    PaymentMode = values["paymentMode"],
                    Msg = values["txMsg"],
                    TxtTime = values["txTime"],
                    Signature = values["signature"]
                };
                var order2 = decodevlaue.OrderId.Split('_');
                if (order2.Count() > 1) { decodevlaue.OrderId = order2[0]; }
                return decodevlaue;

            }
           

           
        }
        public static PaytmSetApproveResponse GetHooksDeserialize(string hookInput)
        {
            var objdata = HttpUtility.ParseQueryString(hookInput);
            var decodevlaue = new PaytmSetApproveResponse
            {
                CHECKSUMHASH = objdata["CHECKSUMHASH"],
                LINKNOTES = objdata["LINKNOTES"],
                MERC_UNQ_REF = objdata["MERC_UNQ_REF"],
                STATUS = objdata["STATUS"],
                TXNAMOUNT = objdata["TXNAMOUNT"],
                ORDERID = objdata["ORDERID"],
                TXNID = objdata["TXNID"],
                RESPMSG = objdata["RESPMSG"]
            };
            return decodevlaue;
        }
        public static string UploadSpiritFile(HttpPostedFileBase imagefile,string folderPath, string imageType)
        {
            string qrImagePtath = $"{folderPath}"; //$" /Content/images/product";
            if (imagefile.ContentLength > 0)
            {
                
                string path = HttpContext.Current.Server.MapPath($"~{qrImagePtath}");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Guid newguid = Guid.NewGuid();
                string fname = imagefile.FileName;
                string extention = fname.Substring(fname.LastIndexOf("."));
                qrImagePtath = $"{qrImagePtath}/{imageType}-{newguid.ToString()}{extention}";
                path = HttpContext.Current.Server.MapPath($"~{qrImagePtath}");
                imagefile.SaveAs(path);
            }
            return qrImagePtath;
        }

        public static void GenerateZohoTikect(int orderId, int orderIssueId)
        {
            InputAPIModel zohoAPIModel = null;
            var inputContent = default(string);
            string outputContent = default(string);
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var order = db.Orders.Find(orderId);
                    zohoAPIModel = new InputAPIModel
                    {
                        SubCategory = "Sub General",
                        Subject = $"Issue at Store {order.WineShop.ShopName}",
                        DueDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                        Cf = new Models.Zoho.Cf { CfModelname = "F3 2017", CfSeveritypercentage = "0.0" },
                        ContactId = "26909000000149179",
                        ProductId = "",
                        DepartmentId = "26909000000010772",
                        Description = $"Product Related Issue at Store. Order ID: {orderId}, Issue ID: {orderIssueId}",
                        Priority = "High",
                        Phone = $"{order.WineShop.ShopPhoneNo}",
                        Status = "Open",
                        Category = "general"
                    };
                    using (var client = new HttpClient())
                    {
                        var sUrl = ConfigurationManager.AppSettings["ZohoUrl"];
                        var sAuth = ConfigurationManager.AppSettings["ZohoAuth"];
                        var sOrgId = ConfigurationManager.AppSettings["ZohoOrg"];

                        client.DefaultRequestHeaders.Add("Authorization", sAuth);
                        client.DefaultRequestHeaders.Add("orgId", sOrgId);
                        inputContent = JsonConvert.SerializeObject(zohoAPIModel);

                        var strRequest = new StringContent(inputContent, Encoding.UTF8, "application/json");
                        //var request = new HttpRequestMessage
                        //{
                        //    Method = HttpMethod.Get,
                        //    RequestUri = new Uri(sUrl),
                        //    Content = new StringContent(inputContent, Encoding.UTF8, "application/json"),
                        //};
                        //request.Headers.Add("Authorization", sAuth);
                        //request.Headers.Add("orgId", sOrgId);
                        HttpResponseMessage response = client.PostAsync(sUrl, strRequest).Result;
                        outputContent = response.Content.ReadAsStringAsync().Result;


                    }
                }
                catch (Exception ex)
                {
                    db.AppLogs.Add(new AppLog
                    {
                        CreateDatetime = DateTime.Now,
                        Error = ex.Message,
                        Message = ex.StackTrace,
                        MachineName = System.Environment.MachineName
                    });
                    db.SaveChanges();
                }
                finally
                {
                    db.OrderIssueTickets.Add(new OrderIssueTicket
                    {
                        OrderIssueId = orderIssueId,
                        OrderId = orderId,
                        ZohoInput = inputContent,
                        ZohoOutput = outputContent,
                        CreatedDate = DateTime.Now
                    });
                    db.SaveChanges();
                    db.Dispose();
                }

            }
        }
        public static void CloseZohoTikect(int orderId, int orderIssueId)
        {
            var inputContent = default(string);
            string outputContent = default(string);
            using (var db = new rainbowwineEntities())
            {
                var issueticket = db.OrderIssueTickets.Where(o => o.OrderId == orderId && o.OrderIssueId == orderIssueId)?.FirstOrDefault();
                if (issueticket != null)
                {
                    try
                    {

                        var zohoOutAPIModel = JsonConvert.DeserializeObject<OutputAPIModel>(issueticket.ZohoOutput);

                        using (var client = new HttpClient())
                        {
                            var sUrl = ConfigurationManager.AppSettings["ZohoCloseUrl"];
                            var sAuth = ConfigurationManager.AppSettings["ZohoAuth"];
                            var sOrgId = ConfigurationManager.AppSettings["ZohoOrg"];

                            client.DefaultRequestHeaders.Add("Authorization", sAuth);
                            client.DefaultRequestHeaders.Add("orgId", sOrgId);
                            inputContent = JsonConvert.SerializeObject(new { ids = zohoOutAPIModel.id });

                            var strRequest = new StringContent(inputContent, Encoding.UTF8, "application/json");

                            HttpResponseMessage response = client.PostAsync(sUrl, strRequest).Result;
                            outputContent = response.Content.ReadAsStringAsync().Result;


                        }
                    }
                    catch (Exception ex)
                    {
                        db.AppLogs.Add(new AppLog
                        {
                            CreateDatetime = DateTime.Now,
                            Error = ex.Message,
                            Message = ex.StackTrace,
                            MachineName = System.Environment.MachineName
                        });
                        db.SaveChanges();
                    }
                    finally
                    {
                        issueticket.CloseZohoInput = inputContent;
                        issueticket.CloseZohoOutput = outputContent;
                        issueticket.ModifiedDate = DateTime.Now;

                        db.SaveChanges();
                        db.Dispose();
                    }
                }
            }
        }


        public static string UniqueAlphaNumeric(int length)
        {
            const string src = "abcdefghijklmnopqrstuvwxyz0123456789";

            var sb = new StringBuilder();
            Random RNG = new Random();
            for (var i = 0; i < length; i++)
            {
                var c = src[RNG.Next(0, src.Length)];
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string CreateTokenForCashFree(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static decimal CalculateOverAllDiscount(int orderId, decimal amt = 0)
        {
            decimal totalAmt = amt > 0 ? amt : 0;
            if (orderId > 0)
            {
                using (var db = new rainbowwineEntities())
                {
                    var order = db.Orders.Find(orderId);
                    var overAllDiscount = order.DiscountUnit;
                    if (overAllDiscount > 0)
                    {
                        var discountTotal = (overAllDiscount / 100) * Convert.ToDouble(totalAmt);
                        totalAmt -= Convert.ToDecimal(discountTotal);
                        totalAmt = Math.Round(totalAmt);
                    }
                }
            }
            return totalAmt;
        }
    }
}