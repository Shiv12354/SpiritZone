using Dapper;
using RainbowWine.Models;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DBO
{
    public class HyperTrackAppHookDBO
    {
        public int HyperTrackAppHookLogAdd(CommonWebHook commonWebHook)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CreatedAt", commonWebHook.created_at);
                    para.Add("@DeviceId", commonWebHook.device_id);
                    para.Add("@TripId", commonWebHook.trip_id);
                    para.Add("@DeliveryAgentId", commonWebHook.DeliveryAgentId);
                    para.Add("@RecordedAt", commonWebHook.recorded_at);
                    para.Add("@Type", commonWebHook.type);
                    para.Add("@Value", commonWebHook.value);
                    para.Add("@Activity", commonWebHook.activity);
                    para.Add("@Distance", commonWebHook.distance);
                    para.Add("@Duration", commonWebHook.duration);
                    para.Add("@RemainingDistance", commonWebHook.remaining_distance);
                    para.Add("@RemainingDuration", commonWebHook.remaining_duration);
                    para.Add("@Longitude", commonWebHook.longitude);
                    para.Add("@Latitude", commonWebHook.latitude);
                    para.Add("@SdkVersion", commonWebHook.sdk_version);
                    para.Add("@ExpiryTime", commonWebHook.expiry_time);
                    para.Add("@Tracking", commonWebHook.tracking);
                    para.Add("@Reason", commonWebHook.Reason);
                    para.Add("@Version", commonWebHook.version);
                    res = dbQuery.Query<int>("HyperTrackAppHookLog_Ins",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public int HyperTrackGeofenceLogAdd(CommonWebHook commonWebHook)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeviceId", commonWebHook.device_id);
                    para.Add("@RecordedAt", commonWebHook.recorded_at);
                    para.Add("@Type", commonWebHook.type);
                    para.Add("@Duration", commonWebHook.duration);
                    para.Add("@ArrivalLatitude", commonWebHook.arrival_latitude);
                    para.Add("@ArrivalLongitude", commonWebHook.arrival_longitude);
                    para.Add("@ArrivalRecordedAt", commonWebHook.arrival_recorded_at);
                    para.Add("@ExitLatitude", commonWebHook.exit_latitude);
                    para.Add("@ExitLongitude", commonWebHook.exit_longitude);
                    para.Add("@ExitRecordedAt", commonWebHook.exit_recorded_at);
                    para.Add("@GeofenceId", commonWebHook.geofence_id);
                    para.Add("@GeofenceValue", commonWebHook.geofence_value);
                    para.Add("@GeometryLatitude", commonWebHook.geometry_latitude);
                    para.Add("@GeometryLongitude", commonWebHook.geometry_longitude);
                    para.Add("@RouteToDistance", commonWebHook.route_to_distance);
                    para.Add("@RouteToDuration", commonWebHook.route_to_duration);
                    para.Add("@RouteToIdleTime", commonWebHook.route_to_idle_time);
                    para.Add("@RouteToStartedAt", commonWebHook.route_to_started_at);
                    para.Add("@Reason", commonWebHook.Reason);
                    para.Add("@Version", commonWebHook.version);
                    res = dbQuery.Query<int>("HyperTrackGeofenceLog_Ins",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public PenaltyNotification HyperTrackPenaltyDetailAdd(HTPenaltyDetailsDO hTPenaltyDetailsDO)
        {
            PenaltyNotification penaltyNotification = new PenaltyNotification();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", hTPenaltyDetailsDO.DeliveryAgentId);
                    para.Add("@DeliveryAgentName", hTPenaltyDetailsDO.DeliveryAgentName);
                    para.Add("@PenaltyType", hTPenaltyDetailsDO.PenaltyType);
                    para.Add("@PenaltyTypeCount", hTPenaltyDetailsDO.PenaltyTypeCount);
                    para.Add("@ModifiedDate", hTPenaltyDetailsDO.ModifiedDate);
                    penaltyNotification = dbQuery.Query<PenaltyNotification>("HyperTrackPenaltyDetails_Ins",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return penaltyNotification;
        }

        public int UpdateShopStockbyWebHook(ShopItemCodeDO shopItemCodeDO,int Id,string RequestJson)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopCode", shopItemCodeDO.ShopCode);
                    para.Add("@ShopItemCode", shopItemCodeDO.ShopItemCode);
                    para.Add("@Stock", shopItemCodeDO.Stock);
                    para.Add("@Rate",shopItemCodeDO.Rate);
                    para.Add("@UniversalItemCode",shopItemCodeDO.UniversalItemCode);
                    para.Add("@TimeStamp",shopItemCodeDO.TimeStamp);
                    para.Add("@3pSupplierId", Id);
                    para.Add("@ItemName",shopItemCodeDO.ItemName);
                    para.Add("@Packing",shopItemCodeDO.Packing);
                    para.Add("@ML",shopItemCodeDO.ML);
                    para.Add("@RequestJson", RequestJson);

                    res = dbQuery.Query<int>("InventoryStockUpdateBy_WebHook",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public int Verify3PSuppier(string AuthKey)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@AuthKey", AuthKey);
                    res = dbQuery.Query<int>("Validate3PSupplier",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

    }
}