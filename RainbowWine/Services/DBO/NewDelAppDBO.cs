using Dapper;
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
    public class NewDelAppDBO
    {
        public string OrderPickedUp(int orderId,int deliveryAgentId,string userId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@DeliveryAgentId", deliveryAgentId);
                    para.Add("@UserId", userId);
                    string res = dbQuery.Query<string>("OrderPickedUp_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return res;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public EarningAndAttendance EarningWithAttendance(string userId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                EarningAndAttendance earningAndAttendance;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    earningAndAttendance = dbQuery.Query<EarningAndAttendance>("NewDelEarningWithAttendance_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return earningAndAttendance;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public HandOver HandOver(string userId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                HandOver handOver;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    handOver = dbQuery.Query<HandOver>("NewDelHanOver_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return handOver;

                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }

        public List<NewDelOrderDetails> NewDelOrderDetails(int index, int size, string userId)
        {

            List<NewDelOrderDetails> newDelOrderDetails = new List<NewDelOrderDetails>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                 

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@index", index);
                    para.Add("@Size", size);
                    para.Add("@UserId", userId);
                    newDelOrderDetails = dbQuery.Query<NewDelOrderDetails>("NewDelOrderDetail_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                    

                }
                finally
                {
                    dbQuery.Close();
                }

            }

            return newDelOrderDetails;



        }

        public List<NewDelOrderDetails> DelV3OrderDetails(int index, int size, string userId)
        {

            List<NewDelOrderDetails> newDelOrderDetails = new List<NewDelOrderDetails>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {


                try
                {
                    var para = new DynamicParameters();
                    para.Add("@index", index);
                    para.Add("@Size", size);
                    para.Add("@UserId", userId);
                    newDelOrderDetails = dbQuery.Query<NewDelOrderDetails>("DelV3_NewDelOrderDetail_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }

            }

            return newDelOrderDetails;



        }

        public Earning Earning(string userId, DateTime startdate, DateTime endDate)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                Earning earning;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    para.Add("@StartDate", startdate);
                    para.Add("@EndDate", endDate);
                    earning = dbQuery.Query<Earning>("NewDelEarningDashboard_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return earning;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public List<LastSevendaysEarning> LastSevenDaysEarning(string userId, DateTime startdate, DateTime endDate)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                List<LastSevendaysEarning> lastSevendaysEarning;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    para.Add("@StartDate", startdate);
                    para.Add("@EndDate", endDate);
                    lastSevendaysEarning = dbQuery.Query<LastSevendaysEarning>("NewDelTotalEarningDashboardForLastSevendays_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return lastSevendaysEarning;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public OrdersList OrdersList(string userId, DateTime startdate, DateTime endDate)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                OrdersList ordersList ;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    para.Add("@StartDate", startdate);
                    para.Add("@EndDate", endDate);
                    ordersList = dbQuery.Query<OrdersList>("NewDelOrdersList_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return ordersList;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public List<DetailsView> GetDetailsViews(int index,int size,string type, string userId,DateTime startdate, DateTime endDate)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                List<DetailsView> detailsView;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Index", index);
                    para.Add("@Size", size);
                    para.Add("@Type", type);
                    para.Add("@UserId", userId);
                    para.Add("@StartDate", startdate);
                    para.Add("@EndDate", endDate);
                    detailsView = dbQuery.Query<DetailsView>("NewDelDetailsView", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return detailsView;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public int TotalEarning(string userId,DateTime startdate, DateTime endDate)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                   
                    para.Add("@UserId", userId);
                    para.Add("@StartDate", startdate);
                    para.Add("@EndDate", endDate);
                    int res = dbQuery.Query<int>("NewDelEarningTotal_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return res;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public List<DeliveryEarningWithPenalty> GetDeliveryEarningWithPenalty(string userId,int index,int size, DateTime startdate, DateTime endDate)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                List<DeliveryEarningWithPenalty> earningWithPenalties;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Index", index);
                    para.Add("@Size", size);
                    para.Add("@UserId", userId);
                    para.Add("@StartDate", startdate);
                    para.Add("@EndDate", endDate);
                    earningWithPenalties = dbQuery.Query<DeliveryEarningWithPenalty>("NewDelEarningWithPenallty", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return earningWithPenalties;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public List<OrderItem> GetOrderItemDetails(int orderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                List<OrderItem> orderItems;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    orderItems = dbQuery.Query<OrderItem>("NewDelGetOrderItemDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return orderItems;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public List<OrderItem> GetV3OrderItemDetails(int orderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                List<OrderItem> orderItems;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    orderItems = dbQuery.Query<OrderItem>("DelV3GetOrderItemDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return orderItems;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public DeliveryAgentDetail GetDeliveryAgentDetail(string userId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                DeliveryAgentDetail deliveryAgentDetail;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    deliveryAgentDetail = dbQuery.Query<DeliveryAgentDetail>("NewDelDeliveryAgentDetails_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return deliveryAgentDetail;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public bool GetDeliveryAgent(int delAgentId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                bool hTrackDisallow;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", delAgentId);
                    hTrackDisallow = dbQuery.Query<bool>("NewDelDeliveryAgent_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return hTrackDisallow;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public HypertrackDeliveryDevices GetHyperTrackDeviceId(string userId)
        {
            HypertrackDeliveryDevices hypertrackDeliveryDevices = new HypertrackDeliveryDevices();

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    hypertrackDeliveryDevices = dbQuery.Query<HypertrackDeliveryDevices>("HypertrackDeliveryDevice_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return hypertrackDeliveryDevices;
        }

        public int UpdateHyperTrackDevice(string userId,int isMetaSet)
        {
            HypertrackDeliveryDevices hypertrackDeliveryDevices = new HypertrackDeliveryDevices();

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    para.Add("@IsMetaSet", isMetaSet);
                    int res = dbQuery.Query<int>("HypertrackDeliveryDevice_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return res;

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            
        }

        public HypertrackTripDetail GetHypertrackTripDetail(int orderId)
        {
            HypertrackTripDetail hypertrackTripDetail = new HypertrackTripDetail();

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    hypertrackTripDetail = dbQuery.Query<HypertrackTripDetail>("NewDelTripDetailForHyperTrack_sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return hypertrackTripDetail;
        }

        public int HyperTrackTripResponseAdd(string tripId,DateTime startedAt, DateTime completedAt ,string sharedUrl, string deviceId,int deliveryAgentId,int orderId)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@TripId", tripId);
                    para.Add("@StartedAt", startedAt);
                    para.Add("@CompletedAt", completedAt);
                    para.Add("@SharedUrl", sharedUrl);
                    para.Add("@DeviceId", deviceId);
                    para.Add("@DeliveryAgentId", deliveryAgentId);
                    para.Add("@OrderId", orderId);
                   res=  dbQuery.Query<int>("HyperTrackTripResponse_Ins_Upd",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public int DeleteHyperTrackTripResponse(int orderId)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    res = dbQuery.Query<int>("HyperTrackTripResponse_Del", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;

        }

        public HyperTrackTripResponse GetHyperTrackTripId(int orderId)
        {
            HyperTrackTripResponse hyperTrackTripResponse = new HyperTrackTripResponse();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    hyperTrackTripResponse = dbQuery.Query<HyperTrackTripResponse>("HyperTrackTripResponse_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return hyperTrackTripResponse;

        }

        public int UpdateSlotStatus(string jobId)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@JobId", jobId);
                    res = dbQuery.Query<int>("UpdateSlotsStatus", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


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