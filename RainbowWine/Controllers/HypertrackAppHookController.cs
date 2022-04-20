using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using SZInfrastructure;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/v1")]
    [EnableCors("*", "*", "*")]
    public class HypertrackAppHookController : ApiController
    {
        [HttpPost]
        [Route("hypertrackapphook")]
        public IHttpActionResult HyperTrackAppHookLog(CommonWebHook commonWebHook)
        {

            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            int result = 0;
            if (commonWebHook !=null)
            {
                if (commonWebHook.created_at ==default(DateTime))
                {
                    commonWebHook.created_at = DateTime.Now;
                }
                if (commonWebHook.recorded_at == default(DateTime))
                {
                    commonWebHook.recorded_at = DateTime.Now;
                }
                HyperTrackAppHookDBO hyperTrackAppHookDBO = new HyperTrackAppHookDBO();
                var c = SZIoc.GetSerivce<ISZConfiguration>();
                var notifyDeliveryManager = c.GetConfigValue(ConfigEnums.NotifyDeliveryManager.ToString());
                if (notifyDeliveryManager == "1")
                {
                    if (commonWebHook.type == "device_status" && (commonWebHook.value == "inactive" || commonWebHook.value == "disconnected") && commonWebHook.DeliveryAgentId > 0)
                    {
                        rainbowwineEntities db = new rainbowwineEntities();
                        var delAgent = db.DeliveryAgents.Where(x => x.Id == commonWebHook.DeliveryAgentId).FirstOrDefault();
                        SendEmail(delAgent.DeliveryExecName, commonWebHook.value);
                    }

                }
               
                if (!string.IsNullOrEmpty(commonWebHook.geofence_id))
                {
                    result = hyperTrackAppHookDBO.HyperTrackGeofenceLogAdd(commonWebHook);
                }
                else
                {
                    result = hyperTrackAppHookDBO.HyperTrackAppHookLogAdd(commonWebHook);
                }
                
                if (result == 1 || result == 2)
                {
                    responseStatus.Message = "Record Inserted Successfully";
                    responseStatus.Status = true;
                }
                else
                {
                    responseStatus.Message = "Failed";
                    responseStatus.Status = false;
                }

            }
            responseStatus.Data = result;
            return Content(HttpStatusCode.OK, responseStatus);

        }

        [HttpPost]
        [Route("add-ht-penaltydetails")]
        public IHttpActionResult HyperTrackPenalyDetail_Add(HTPenaltyDetailsDO hTPenaltyDetailsDO)
        {
           
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            PenaltyNotification penaltyNotification = new PenaltyNotification();
            if (hTPenaltyDetailsDO != null)
            {
                HyperTrackAppHookDBO hyperTrackAppHookDBO = new HyperTrackAppHookDBO();

                penaltyNotification = hyperTrackAppHookDBO.HyperTrackPenaltyDetailAdd(hTPenaltyDetailsDO);
                if (penaltyNotification != null && penaltyNotification.DeliveryAgentId > 0)
                {
                    responseStatus.Message = "Record Inserted Successfully";
                    responseStatus.Status = true;
                }
                else
                {
                    responseStatus.Message = "Failed";
                    responseStatus.Status = false;
                }
                PenaltyNotification(hTPenaltyDetailsDO.DeliveryAgentId, hTPenaltyDetailsDO.PenaltyTypeCount,penaltyNotification.PenaltyAmount,penaltyNotification.PenaltyDescription,penaltyNotification.Solution);
            }
            responseStatus.Data = penaltyNotification;
            return Content(HttpStatusCode.OK, responseStatus);

        }

        [HttpPost]
        [Route("update-shopstock-webhook")]
        public IHttpActionResult ShopStock_Update(ShopItemCodeDO shopItemCodeDO)
        {

            ResponseStatus responseStatus = new ResponseStatus { Status = false };

            HttpContext httpContext = HttpContext.Current;
            HyperTrackAppHookDBO hyperTrackAppHookDBO = new HyperTrackAppHookDBO();
            string authHeader = httpContext.Request.Headers["AuthKey"];
            var result = hyperTrackAppHookDBO.Verify3PSuppier(authHeader);
           

            if (shopItemCodeDO != null && result==1)
            {
                var reqJson = new
                {
                    shopItemCodeDO.ShopCode,
                    shopItemCodeDO.ShopItemCode,
                    shopItemCodeDO.Stock,
                    shopItemCodeDO.Rate,
                    shopItemCodeDO.UniversalItemCode,
                    shopItemCodeDO.TimeStamp,
                    shopItemCodeDO.ItemName,
                    shopItemCodeDO.Packing,
                    shopItemCodeDO.ML
                };


                int shopitem = hyperTrackAppHookDBO.UpdateShopStockbyWebHook(shopItemCodeDO,result, reqJson.ToString());
                if (shopitem == 1)
                {
                    responseStatus.Message = "Inventory Updated";
                    responseStatus.Status = true;
                }
                else
                {
                    responseStatus.Message = "Failed to update";
                    responseStatus.Status = false;
                }
                responseStatus.Data = shopitem;

            }
           
            return Content(HttpStatusCode.OK, responseStatus);

        }

        [HttpPost]
        [Route("orderassign/{shopid}/{orderid}")]
        public IHttpActionResult OrdersAssign(int shopId, int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            try
            {

                var url = ConfigurationManager.AppSettings["ExtOrderAssign_Url"];
                var authHeader = ConfigurationManager.AppSettings["ExtAuthHeader"];

                var input = new
                {
                    shop_id = shopId,
                    order_id = orderId
                };

                using (HttpClient client = new HttpClient())
                {
                    var serializeJson = JsonConvert.SerializeObject(input);
                    var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", authHeader);
                    var resp = client.PostAsync(url, content).Result;
                    var res = resp.Content.ReadAsStringAsync();
                    if (resp.IsSuccessStatusCode)
                    {
                        responseStatus.Status = true;
                        responseStatus.Message = "Order Assigned";
                       

                    }
                    else
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "External Api Failed";
                    }
                   

                }

            }
            catch (Exception)
            {

                throw;
            }
            return Content(HttpStatusCode.OK, responseStatus);
        }
        #region Non Actions
        #region HyperTrackApphook
        [HttpPost]
        [Route("3psupplierencryption/{text}")]
        public IHttpActionResult EncrytionText(string text)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            var res = Encrypt(text);
            responseStatus.Data =res;
            responseStatus.Status = false;
            return Content(HttpStatusCode.OK, responseStatus);

        }


        public int PenaltyNotification(int deliveryAgentId, int penaltyCount,double penaltyAmount,string penaltyDescription, string solution)
        {
            int res = 0;
            FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var thresoldCount = c.GetConfigValue(ConfigEnums.ThresholdPenaltyCount.ToString());
            var penaltyWarningTitle = c.GetConfigValue(ConfigEnums.PenaltyWarningTitle.ToString());
            var penaltyWarningBody = c.GetConfigValue(ConfigEnums.PenaltyWarningBody.ToString());
            var penaltyTitle = c.GetConfigValue(ConfigEnums.PenaltyTitle.ToString()); 
            var penaltyBody = c.GetConfigValue(ConfigEnums.PenaltyBody.ToString());
            var isPenalyApply = c.GetConfigValue(ConfigEnums.IsPenalyApply.ToString());
            if (penaltyCount >= Convert.ToInt32(thresoldCount) && isPenalyApply=="1")
            {
                PenaltyNotification penaltyNotification = new PenaltyNotification();
                penaltyNotification.DeliveryAgentId = deliveryAgentId;
                penaltyNotification.Title = penaltyTitle.Replace("{PenaltyDescription}", penaltyDescription);
                penaltyNotification.Body = penaltyBody.Replace("{Solution}",solution);
                Task.Run(async () => await fcmHelper.SendFirebaseNotificationPenalty(penaltyNotification));
                res = 1;
            }
            else if(penaltyCount >=1)
            {
                PenaltyNotification penaltyNotification = new PenaltyNotification();
                penaltyNotification.DeliveryAgentId = deliveryAgentId;
                penaltyNotification.Title = penaltyWarningTitle.Replace("{PenaltyDescription}", penaltyDescription);
                penaltyNotification.Body = penaltyWarningBody.Replace("{Solution}", solution); 
                Task.Run(async () => await fcmHelper.SendFirebaseNotificationPenalty(penaltyNotification));
                res = 2;
            }
            else
            {
                res = 3;
            }
            return res;
        }
        public static string hash_hmac(string signatureString, string secretKey)
        {
        //    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        //    byte[] keyByte = encoding.GetBytes(secretKey);

            var sh = SHA1.Create(secretKey);
            byte[] bytes = Encoding.UTF8.GetBytes(signatureString);
            byte[] b = sh.ComputeHash(bytes);
            //byte[] messageBytes = Encoding.UTF8.GetBytes(signatureString);
          
            //byte[] hashmessage = hmac.ComputeHash(messageBytes);


            return Convert.ToBase64String(b);
        }

        public static string HmacSHA1(string signatureString , string secretKey)
        {
            string hash;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] code = encoding.GetBytes(secretKey);
            using (HMACSHA1 hmac = new HMACSHA1(code))
            {
                Byte[] hmBytes = hmac.ComputeHash(encoding.GetBytes(signatureString));
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

        private void SendEmail(string DelAgentName,string status)
        {
            try
            {
                string MailSubject = $"Device Status Update:" +' '+ DelAgentName;
                string MailTo = ConfigurationManager.AppSettings["EmailIds"].ToString();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"].ToString());
                mail.From = new MailAddress(ConfigurationManager.AppSettings["from"].ToString());
                string[] emails = MailTo.Split(',');
                foreach (var mulEmailIds in emails)
                {
                    mail.To.Add(new MailAddress(mulEmailIds)); // Sending MailTo 
                }
                //List<string> li = new List<string>();
                //li.Add("shivk4999@gmail.com");
                //mail.CC.Add(string.Join<string>(",", li)); // Sending CC  
                //mail.Bcc.Add(string.Join<string>(",", li)); // Sending Bcc   
                mail.Subject = MailSubject; // Mail Subject  
                mail.Body =$"Hello! \n\n\nIt seems that Delivery Agent" + ' ' + DelAgentName + ' ' + "has had some problem with his device. It is now"+' ' + status+ ". Please look into this.\n\n\nRegards,\nDelivery Bot";
                string UserName = (ConfigurationManager.AppSettings["SMTPUsername"]).ToString();
                string Password = (ConfigurationManager.AppSettings["SMTPPassword"]).ToString();
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(UserName, Password, ConfigurationManager.AppSettings["SMTPServer"].ToString());
                SmtpServer.EnableSsl = true;
                SmtpServer.Port = 587;
                SmtpServer.Timeout = 600000;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                cipherText = cipherText.Replace(" ", "+");
                //cipherText = "HYtMQvIjw4/tqzqwJN1HQ==";
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] cipherBytes = Convert.FromBase64String(cipherText.Trim());
                using (Aes encryptor = Aes.Create())
                {

                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {


                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {

                return "123";
            }
        }

        #endregion



        #endregion
    }
}
