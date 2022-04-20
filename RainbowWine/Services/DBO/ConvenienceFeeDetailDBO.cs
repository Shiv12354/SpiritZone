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
    public class ConvenienceFeeDetailDBO
    {
        public ConvenienceFeeDetailDO GetConvenienceFeeDetails(int orderId,string bankName)
        {
            ConvenienceFeeDetailDO convenienceFeeDetailDO = new ConvenienceFeeDetailDO();

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("BankName",bankName);
                    convenienceFeeDetailDO = dbQuery.Query<ConvenienceFeeDetailDO>("CalculateOnlineRefundAmount_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return convenienceFeeDetailDO;
        }

    }
}