using Microsoft.AspNet.Identity;
using RainbowWine.Data;
using RainbowWine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Configuration;
using RainbowWine.Services;
using RainbowWine.Services.PaytmService;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using RainbowWine.Services.DBO;
using System.Text;
using System.Web.Http.Cors;
using RainbowWine.Services.DO;
using Microsoft.Ajax.Utilities;
using RainbowWine.Providers;
using SZData.Interfaces;
using static RainbowWine.Services.FireStoreService;
using Google.Cloud.Firestore;
using SZInfrastructure;
using System.Threading.Tasks;


namespace RainbowWine.Providers
{
    public class HyperTracking
    {
        NewDelAppDBO newDelAppDBO = new NewDelAppDBO();
        public async Task<ResponseStatus> HyperTrackTripCompleted(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = false };
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var isHyperTrackOn = c.GetConfigValue(ConfigEnums.IsHyperTrackOn.ToString());
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    if (isHyperTrackOn == "1")
                    {
                        using (HttpClient client = new HttpClient())
                        {

                            var hyperTrackBasicAuthToken = c.GetConfigValue(ConfigEnums.HyperTrackBasicAuthToken.ToString());
                            var hyperTrackTripCompletedUrl = ConfigurationManager.AppSettings["HyperTrackTripCompletedUrl"].ToString();
                            var tripResponse = newDelAppDBO.GetHyperTrackTripId(orderId);
                            if (tripResponse != null)
                            {
                                if (!string.IsNullOrEmpty(tripResponse.TripId))
                                {
                                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + hyperTrackBasicAuthToken);
                                    var resp = await client.PostAsync(hyperTrackTripCompletedUrl.Replace("{tripid}", tripResponse.TripId.ToString()), null);

                                    if (resp.IsSuccessStatusCode)
                                    {
                                        var res = newDelAppDBO.DeleteHyperTrackTripResponse(orderId);

                                        responseStatus.Message = "Hyper Tracking trip Completed Successfully";
                                        responseStatus.Status = true;
                                        return responseStatus;
                                    }
                                    else
                                    {
                                        responseStatus.Message = "External Api Is Not Respond";
                                        responseStatus.Status = false;
                                        return responseStatus;

                                    }
                                }
                                else
                                {
                                    responseStatus.Message = "Trip Id Is Not Found";
                                    responseStatus.Status = false;
                                    return responseStatus;

                                }
                            }
                        }
                    }
                    else
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "Hyper Tracking Is Not Active";
                        return responseStatus;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return responseStatus;
        }
    }
}