using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RainbowWine.Services.Msg91
{
    public static class Msg91Service
    {
        public static async Task<bool> SendFlowSms(string msgTemplateId, string mobile, Dictionary<string, string> pairs)
        {
            var url = $"{ConfigurationManager.AppSettings["Msg91BaseUrl"]}/api/v5/flow/";

            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("flow_id", msgTemplateId);
            keyValuePairs.Add("sender", ConfigurationManager.AppSettings["Msg91SenderID"]);
            keyValuePairs.Add("mobiles", $"+91{mobile}");

            foreach (var item in pairs)
            {
                keyValuePairs.Add(item.Key,item.Value);
            }
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authky = ConfigurationManager.AppSettings["Msg91Key"];
                    httpClient.DefaultRequestHeaders.Add("authkey", authky);
                    httpClient.DefaultRequestHeaders.Accept
                              .Add(new MediaTypeWithQualityHeaderValue("application/JSON"));

                    var json = JsonConvert.SerializeObject(keyValuePairs);
                    var cont = new StringContent(json, Encoding.Default, "application/JSON");
                    var httpResponse = await httpClient.PostAsync(url, cont);

                    var jsonRes = await httpResponse.Content.ReadAsStringAsync();

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {

                }

            }

            return false;
        }

        public static async Task<bool> SendRecipientFlowSms(string msgTemplateId, string mobile, Dictionary<string, string> pairs)
        {
            var url = $"{ConfigurationManager.AppSettings["Msg91BaseUrl"]}/api/v5/flow/";

            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("flow_id", msgTemplateId);
            keyValuePairs.Add("sender", ConfigurationManager.AppSettings["Msg91SenderIDRecipient"]);
            keyValuePairs.Add("mobiles", $"+91{mobile}");

            foreach (var item in pairs)
            {
                keyValuePairs.Add(item.Key, item.Value);
            }
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authky = ConfigurationManager.AppSettings["Msg91Key"];
                    httpClient.DefaultRequestHeaders.Add("authkey", authky);
                    httpClient.DefaultRequestHeaders.Accept
                              .Add(new MediaTypeWithQualityHeaderValue("application/JSON"));

                    var json = JsonConvert.SerializeObject(keyValuePairs);
                    var cont = new StringContent(json, Encoding.Default, "application/JSON");
                    var httpResponse = await httpClient.PostAsync(url, cont);

                    var jsonRes = await httpResponse.Content.ReadAsStringAsync();

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {

                }

            }

            return false;
        }
    }

    public class OTPService
    {
        public async Task<bool> SendOTP(string mobile, string otp)
        {
            var authKey = ConfigurationManager.AppSettings["Msg91Key"];
            var template_id = ConfigurationManager.AppSettings["Msg91TempSendOTP"];
            var mobilenumber = $"{mobile}";
            var otpTosend = $"otp={otp}";

            //var url = $"{_config["Msg91:BaseUrl"]}/api/v5/otp?{authKey}&{template_id}&{mobilenumber}&{otpTosend}";
            var url2 = $"{ConfigurationManager.AppSettings["Msg91BaseUrl"]}/api/v5/otp?template_id={template_id}&mobile=+91{mobile}&authkey={authKey}&otp={otp}&otp_expiry=4";
            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(url2);

                var json = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> VerifyOTP(string mobile, string OTP)
        {
            var authKey = ConfigurationManager.AppSettings["Msg91Key"];
            var otp = $"otp={OTP}";
            var mobilenumber = $"mobile=+91{mobile}";

            var url = $"{ConfigurationManager.AppSettings["Msg91BaseUrl"]}/api/v5/otp/verify?{mobilenumber}&{otp}&{mobilenumber}";

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsync(url, null);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ResendOTP(string mobile)
        {
            var authKey = ConfigurationManager.AppSettings["Msg91Key"];

            //var url = $"{_config["Msg91:BaseUrl"]}/api/v5/otp/retry?{authKey}&{mobilenumber}";
            var url2 = $"https://api.msg91.com/api/v5/otp/retry?authkey={authKey}&retrytype=text&mobile=+91{mobile}";
            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsync(url2, null);
                var jsonop = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }
    }
}