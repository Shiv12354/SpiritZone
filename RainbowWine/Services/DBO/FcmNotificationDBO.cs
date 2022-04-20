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
    public class FcmNotificationDBO
    {
        public string AddToFcmNotification(FcmNotificationInput fcmNotificationInput, int customerId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", customerId);
                    para.Add("@Title", fcmNotificationInput.Title);
                    para.Add("@Body", fcmNotificationInput.Body);
                    para.Add("@Id", fcmNotificationInput.Id);
                    para.Add("@NotificationType", fcmNotificationInput.NotificationType);
                    para.Add("@NavigationPage", fcmNotificationInput.NavigationPage);
                    para.Add("@IconUrl", fcmNotificationInput.IconUrl);
                    para.Add("@BannerUrl", fcmNotificationInput.BannerUrl);
                    para.Add("@OtherParameters", fcmNotificationInput.OtherParameters);
                    para.Add("@FcmNotificationId", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);
                    dbQuery.Execute("FcmNotification_Ins",
                    param: para, commandType: CommandType.StoredProcedure);
                    var FcmNotificationId = para.Get<string>("@FcmNotificationId");
                    return FcmNotificationId;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }
        public void UpdateFcmNotification(int fcmNotificationId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@FcmNotificationId", fcmNotificationId);
                    dbQuery.Query<int>("FcmNotification_Upd",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public List<FcmNotificationDO> GetFcmNotification(int customerId)
        {
            List<FcmNotificationDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<FcmNotificationDO>("FcmNotification_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

        }

        public UnReadNotificationCount GetUnreadNotificationCount(int customerId)
        {
            UnReadNotificationCount unReadNotificationCount = new UnReadNotificationCount();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", customerId);
                    unReadNotificationCount = dbQuery.Query<UnReadNotificationCount>("UnreadNotificationCount_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return unReadNotificationCount;

        }

        public void DeleteFcmNotification(int fcmNotificationId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@FcmNotificationId", fcmNotificationId);
                    dbQuery.Query<int>("FcmNotification_Del",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }
    }
}