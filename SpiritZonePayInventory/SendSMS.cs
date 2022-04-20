using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpiritZonePayInventory
{
    public class SendSMS
    {
        public void SendMessage1(string message, string mobileno)
        {
            //string message = "Hello! Your order number 123 is packed and ready for dispatch. The delivery agent will pick it up shortly.";
            string url = ConfigurationManager.AppSettings["SMSTextCom"];
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
                //rainbowwineEntities db = new rainbowwineEntities();
                //db.AppLogs.Add(new AppLog
                //{
                //    CreateDatetime = DateTime.Now,
                //    Error = ex.Message,
                //    Message = ex.StackTrace,
                //    MachineName = System.Environment.MachineName
                //});
                //db.SaveChanges();
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
            catch (Exception ex) {
                SendMessage1(text, mobileno);
            }
        }
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
