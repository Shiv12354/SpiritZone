using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace RainbowWine.Services.DBO
{
    public class IssueDBO
    {
        public int DeliveryManagerTrackAgent(int orderId, string userId, string oDetailIds)
        {
            int ret = 1;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@UserId", userId);
                    para.Add("@odetailIds", oDetailIds);
                    ret = dbQuery.Query<int>("Issue_ByPacker_Ins",
                        param: para, commandType: CommandType.StoredProcedure).Single();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return ret;
        }
    }
}