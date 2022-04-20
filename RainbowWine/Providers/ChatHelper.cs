using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using RestSharp.Authenticators;
using RestSharp;
using RainbowWine.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Configuration;

namespace RainbowWine.Providers
{
    public class ChatHelper
    {
        public string chatapi(ChatAPI objChatAPI)
        {
            string TicketID = default(string);
            var inputContent = default(string);
            string outputContent = default(string);
            //var client = new RestSharp.RestClient("https://desk.zoho.in/api/v1/tickets");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Authorization", "e0028f52c2de7c4d70e7bc818a80ff49");
            //request.AddHeader("orgId", "60004934587");
            //request.AddHeader("Content-Type", "application/json");

            //request.AddParameter("application/json", JsonConvert.SerializeObject(objChatAPI), ParameterType.RequestBody);
            //IRestResponse response1 = client.Execute(request);

            //var jObject = JObject.Parse(response1.Content);
            //string TicketID = jObject.GetValue("id").ToString();

            using (var clientZoho = new HttpClient())
            {
                var sUrl = ConfigurationManager.AppSettings["ZohoUrl"];
                var sAuth = ConfigurationManager.AppSettings["ZohoAuth"];
                var sOrgId = ConfigurationManager.AppSettings["ZohoOrg"];

                var redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];
                var scope = ConfigurationManager.AppSettings["scope"];
                var grant_type = ConfigurationManager.AppSettings["grant_type"];
                var client_secret = ConfigurationManager.AppSettings["client_secret"];
                var client_id = ConfigurationManager.AppSettings["client_id"];
                var ZohoRefreshUrl = ConfigurationManager.AppSettings["ZohoRefreshUrl"];
                var ZohoRefreshToken = ConfigurationManager.AppSettings["ZohoRefreshToken"];
                var url = ZohoRefreshUrl + ZohoRefreshToken + "&" + grant_type + "&" + client_id + "&" + client_secret + "&" + redirect_uri + "&" + scope;
                HttpResponseMessage response2 = clientZoho.PostAsync(url, null).Result;

                if (response2.IsSuccessStatusCode)
                {
                    var zohoResponse = response2.Content.ReadAsStringAsync().Result;
                    var refreshResponse = JsonConvert.DeserializeObject<ZohoModel>(zohoResponse);
                    sAuth = refreshResponse.access_token;

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Zoho-oauthtoken " + sAuth);
                        client.DefaultRequestHeaders.Add("orgId", sOrgId);
                        inputContent = JsonConvert.SerializeObject(objChatAPI);

                        var strRequest = new StringContent(inputContent, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = client.PostAsync(sUrl, strRequest).Result;
                        outputContent = response.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(outputContent);
                        TicketID = jObject.GetValue("id").ToString();

                    }
                }

            }
            return TicketID;
        }

        public void chatapiAttchmentPath(string fnm,string TicID)
        {
            String Filep = fnm;//@"D:\Quosphere\RainbowWine\Content\images\TicketDocuments\2020720202791.png"; 
            var client = new RestSharp.RestClient("https://desk.zoho.in/api/v1/tickets/" + TicID + "/attachments?isPublic=true");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "e0028f52c2de7c4d70e7bc818a80ff49");
            request.AddHeader("orgId", "60004934587");
            request.AddFile("file", @Filep);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}