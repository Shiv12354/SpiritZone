using Dapper;
using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DBO
{
    public class DeliveryAgentDBO
    {
        public DeliveryAgent GetAgent(string agentId)
        {
            DeliveryAgent deliveryAgent = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    deliveryAgent = dbQuery.Query<DeliveryAgent>($"select * from DeliveryAgents where id = (select top 1 DeliveryAgentId from RUser where rUserId='{agentId}')").FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return deliveryAgent;
        }
    }
}