using Newtonsoft.Json;
using RainbowWine.Models;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RainbowWine.Providers
{
    public class FirebaseNotificationHelper
    {
        private string _baseUrl;
        public string BaseUrl
        {
            get => ConfigurationManager.AppSettings["ExtApi"].ToString();
        }

        public enum NotificationType
        {
            Order,
            Product,
            Wallet
        }

        public async Task SendFirebaseNotification(long id,NotificationType type)
        {
            using (var _httpClient = new HttpClient())
            {
                var url = string.Empty;

                if(type==NotificationType.Order)
                   url = $"{BaseUrl}/v1/fcm/send-order-notification/{id}";
                //else
                //   url = $"{BaseUrl}/fcm/send-product-notification/{id}/{shopID.Value}";

                await _httpClient.PostAsync(url, null);
            }
        }

        public async Task SendFirebaseNotificationForWallet(WalletNotificationRequest walletNotificationRequest, NotificationType type)
        {
            using (var _httpClient = new HttpClient())
            {
                try
                {
                    string BaseUrl = ConfigurationManager.AppSettings["ExtFCMApiCall"].ToString();

                    var url = string.Empty;

                if (type == NotificationType.Wallet)
                    url = $"{BaseUrl}/v1/fcm/send-wallet-notification";
                //else
                //   url = $"{BaseUrl}/fcm/send-product-notification/{id}/{shopID.Value}";
                var json = JsonConvert.SerializeObject(walletNotificationRequest);
                var str = new StringContent(json, Encoding.UTF8, "application/json");

                var res=  await _httpClient.PostAsync(url, str);
                if (res.IsSuccessStatusCode)
                {
                    var result = await res.Content.ReadAsStringAsync();
                }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task SendFirebaseNotificationSilent(SilentNotification silentNotiRequest)
        {
            using (var _httpClient = new HttpClient())
            {
                var url = $"{BaseUrl}/v2/fcm/send-notification-silent";
                var json = JsonConvert.SerializeObject(silentNotiRequest);
                var str = new StringContent(json, Encoding.UTF8, "application/json");
                var reas = await _httpClient.PostAsync(url, str);
                var datt = await reas.Content.ReadAsStringAsync();
            }
        }

        public async Task SendFirebaseNotificationPenalty(PenaltyNotification penaltyNotification)
        {
            using (var _httpClient = new HttpClient())
            {
                var url = $"{BaseUrl}/v2/fcm/send-notification-penalty";
                var json = JsonConvert.SerializeObject(penaltyNotification);
                var str = new StringContent(json, Encoding.UTF8, "application/json");
                var reas = await _httpClient.PostAsync(url, str);
                var datt = await reas.Content.ReadAsStringAsync();
            }
        }
    }
}