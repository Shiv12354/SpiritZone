using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZXing;

namespace RainbowWine.Services
{
    public class WSendSMS
    {
        public void SendMessage1(string message, string mobileno)
        {
            //string message = "Hello! Your order number 123 is packed and ready for dispatch. The delivery agent will pick it up shortly.";
            string url =ConfigurationManager.AppSettings["SMSTextCom"];
            string key = ConfigurationManager.AppSettings["SMSTextComKey"];
            mobileno = $"91{mobileno}";
            var finalUrl = $"?apikey={key}&numbers={mobileno}&message={message}&sender=SPIRIT";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    var response = client.GetAsync(finalUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var ret = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                rainbowwineEntities db = new rainbowwineEntities();
                db.AppLogs.Add(new AppLog
                {
                    CreateDatetime = DateTime.Now,
                    Error = ex.Message,
                    Message = ex.StackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();
            }
        }
        public void SendMessage(string text, string mobileno)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SMSUrl"]);

                request.Method = "POST";
                request.Accept = "application/json";
                request.Headers.Add("authkey", ConfigurationManager.AppSettings["SMSKey"]);

                List<string> smsto = new List<string>();
                smsto.Add(mobileno);
                List<Sm> smsobject = new List<Sm>();
                smsobject.Add(new Sm
                {
                    message = text,
                    to = smsto
                });

                SMSObject smsBodyObject = new SMSObject
                {
                    sender = "Spirit",
                    route = "4",
                    country = "91",
                    unicode = "1",
                    sms = smsobject
                };

                var smsBody = JsonConvert.SerializeObject(smsBodyObject);

                byte[] bodyBytes = Encoding.UTF8.GetBytes(smsBody);

                if (!string.IsNullOrEmpty(smsBody))
                {
                    request.ContentType = "application/json";
                    request.ContentLength = bodyBytes.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                    requestStream.Close();
                }

                // make the web request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string strResponse = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                rainbowwineEntities db = new rainbowwineEntities();
                db.AppLogs.Add(new AppLog
                {
                    CreateDatetime = DateTime.Now,
                    Error = ex.Message,
                    Message = ex.StackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();

                SendMessage1(text, mobileno);
            }
        }
        public void iOSPaymentLink(int orderId, bool bulk)
        {
            rainbowwineEntities db = new rainbowwineEntities();
            Order order = db.Orders.Find(orderId);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["PayUrl"]);

            request.Method = "POST";
            request.Accept = "application/json";
            request.Headers.Add("mid", ConfigurationManager.AppSettings["Paymid"]);
            string neworderno = bulk ? $"{order.Id}_{order.OrderTo}" : $"{order.Id}";
            //string msghash = $"{order.OrderAmount}|{ConfigurationManager.AppSettings["Paymid"]}|{order.Id}|{ConfigurationManager.AppSettings["PayStoreID"]}";
            string msghash = $"{order.OrderAmount}|{ConfigurationManager.AppSettings["Paymid"]}|{neworderno}|{ConfigurationManager.AppSettings["PayStoreID"]}";
            var hash = CreateToken(msghash, ConfigurationManager.AppSettings["PaySecret"]);
            request.Headers.Add("hash", hash);

            iOSLinkBody smsBodyObject = new iOSLinkBody
            {
                orderId = neworderno,//order.Id,
                amount = Convert.ToInt32(order.OrderAmount),
                mid = ConfigurationManager.AppSettings["Paymid"],
                storeId = Convert.ToInt32(ConfigurationManager.AppSettings["PayStoreID"])
            };

            var smsBody = JsonConvert.SerializeObject(smsBodyObject);

            byte[] bodyBytes = Encoding.UTF8.GetBytes(smsBody);

            if (!string.IsNullOrEmpty(smsBody))
            {
                request.ContentType = "application/json";
                request.ContentLength = bodyBytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                requestStream.Close();
            }

            // make the web request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string strResponse = reader.ReadToEnd();

            PaymentLinkLog paymentLinkLog = new PaymentLinkLog {
                OrderId=orderId,
                InputParam = smsBody,
                VendorOuput =strResponse 
            };
            
            db.PaymentLinkLogs.Add(paymentLinkLog);
            db.SaveChanges();

            iOSLinkResponse iOSLinkResponse = JsonConvert.DeserializeObject<iOSLinkResponse>(strResponse);
            //if (!string.IsNullOrWhiteSpace(iOSLinkResponse.upiString) && string.Compare(iOSLinkResponse.status, "ok", true) == 0)
            if (!string.IsNullOrWhiteSpace(iOSLinkResponse.upiString))
            {
                paymentLinkLog.UPIString = iOSLinkResponse.upiString;
                db.SaveChanges();
                string ordpayimg = GenerateMyQCCode(iOSLinkResponse.upiString, order.Id);

                if (bulk)
                    order.NewOrderId = neworderno;

                order.PaymentQRImage = ordpayimg;
                db.SaveChanges();
            }
        }

        private string CreateToken(string message, string secret)
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
            return "";
        }
        private string GenerateMyQCCode(string QCText, int orderId)
        {
            var QCwriter = new BarcodeWriter();
            QCwriter.Format = BarcodeFormat.QR_CODE;
            var result = QCwriter.Write(QCText);
            Guid newguid = Guid.NewGuid();
            string qrImagePtath = $"/Content/images/qrcodes/order-{Convert.ToString(orderId)}-{newguid.ToString()}.jpg";
            string path = HttpContext.Current.Server.MapPath($"~{qrImagePtath}");
            var barcodeBitmap = new Bitmap(result);

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path,
                   FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return qrImagePtath;
        }
    }
    public class iOSLinkBody
    {
        public string mid { get; set; }
        public int amount { get; set; }
        public string orderId { get; set; }
        public int storeId { get; set; }
    }
    public class iOSLinkResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public string status { get; set; }
        public string mid { get; set; }
        public int storeId { get; set; }
        public string bharatpeTxnId { get; set; }
        public string paymentStatus { get; set; }
        public string createdTimestamp { get; set; }
        public double amount { get; set; }
        public string orderId { get; set; }
        public string upiString { get; set; }
        public string paymentLink { get; set; }
    }
    public class Sm
    {
        public string message { get; set; }
        public IList<string> to { get; set; }
    }

    public class SMSObject
    {
        public string sender { get; set; }
        public string route { get; set; }
        public string country { get; set; }
        public string unicode { get; set; }
        public IList<Sm> sms { get; set; }
    }
}