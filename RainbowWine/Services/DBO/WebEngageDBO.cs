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
    public class WebEngageDBO
    {
        public WebEngageDO GetWebEngageOrderDetails(int orderId,int custId)
        {
            WebEngageDO webEngageDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@custid", custId);
                    webEngageDO = dbQuery.Query<WebEngageDO>("WebEngage_OrderDetails_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return webEngageDO;
        }
    }
}