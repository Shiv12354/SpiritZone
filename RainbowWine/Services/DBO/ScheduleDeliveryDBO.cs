using Dapper;
using RainbowWine.Models;
using RainbowWine.Models.Packers;
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
    public class ScheduleDeliveryDBO
    {
        public void AddScheduleDelivery(ScheduleParameters scheduleParameters)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", scheduleParameters.OrderId);
                    para.Add("@DeliveryAgentId", scheduleParameters.DeliveryAgentId);
                    para.Add("@CustomerId", scheduleParameters.CustomerId);
                    para.Add("@ShopId", scheduleParameters.ShopId);
                    para.Add("@ScheduledStart", scheduleParameters.ScheduleStart);
                    para.Add("@ScheduledEnd", scheduleParameters.ScheduleEnd);
                    dbQuery.Query<int>("ScheduleDelivery_InsUpd",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public List<ScheduledDeliveriesDO> GetScheduledDeliveries(int soid, string shopname , string custno )
        {

            List<ScheduledDeliveriesDO> schDelDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", soid.ToString());
                    para.Add("@ShopName", shopname);
                    para.Add("@CustNo", custno);
                    schDelDetail = dbQuery.Query<ScheduledDeliveriesDO>("ScheduledDeliveryNextDay_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return schDelDetail;
        }

        public List<ScheduledDeliveriesDO> GetAllScheduledDeliveries(int soid, string shopname, string custno)
        {

            List<ScheduledDeliveriesDO> schDelDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", soid.ToString());
                    para.Add("@ShopName", shopname);
                    para.Add("@CustNo", custno);
                    schDelDetail = dbQuery.Query<ScheduledDeliveriesDO>("AllScheduledDelivery_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return schDelDetail;
        }

        public List<ScheduledDeliveriesDO> GetPackerAllScheduledDeliveries(int soid, string shopname, string custno)
        {

            List<ScheduledDeliveriesDO> schDelDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", soid.ToString());
                    para.Add("@ShopName", shopname);
                    para.Add("@CustNo", custno);
                    schDelDetail = dbQuery.Query<ScheduledDeliveriesDO>("AllScheduledDeliveryPacker_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return schDelDetail;
        }

        public int UpdateScheduleDeliveryRelease(int orderId)
        {
            

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                   int res = dbQuery.Query<int>("ScheduleDelivery_Release_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return res;

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public DataTable GetFileDetails()
        {
            DataTable dtData = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString);
            con.Open();
            SqlCommand command = new SqlCommand("Select * From FileUploadDownloadDetails", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtData);
            con.Close();
            return dtData;
        }

        public bool SaveFile(FileUpload model)
        {
            string strQry = "INSERT INTO FileUploadDownloadDetails (FileName,FileUrl) VALUES('" +
                model.FileName + "','" + model.FileUrl + "')";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString);
            con.Open();
            SqlCommand command = new SqlCommand(strQry, con);
            int numResult = command.ExecuteNonQuery();
            con.Close();
            if (numResult > 0)
                return true;
            else
                return false;
        }

        public DataTable GetFiles(int fileID)
        {
            DataTable dtData = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString);
            con.Open();
            SqlCommand command = new SqlCommand("Select * From FileUploadDownloadDetails where Id='" +
               fileID + "'", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtData);
            con.Close();
            return dtData;
        }

        public List<ScheduledDeliveriesDO> GetScheduledOrderListForShop(int shopId)
        {

            List<ScheduledDeliveriesDO> schDelDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    schDelDetail = dbQuery.Query<ScheduledDeliveriesDO>("ScheduleOrderShopWise_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return schDelDetail;
        }
    }
}