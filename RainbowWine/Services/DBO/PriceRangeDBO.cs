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
    public class PriceRangeDBO
    {
        public List<PriceRangeDO> GetPriceRange()
        {
            List<PriceRangeDO> priceDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    priceDetail = dbQuery.Query<PriceRangeDO>("PriceRange_Sel", commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return priceDetail;
        }

    }
}