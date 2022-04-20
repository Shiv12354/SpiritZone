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
    public class ConfigurableETADBO
    {
        public List<ConfigurableETADO> GetConfigurableETAList()
        {
            List<ConfigurableETADO> configurableETADO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ConfigurableETAId", 0);
                    configurableETADO = dbQuery.Query<ConfigurableETADO>("ConfigurableETA_Sel", param: para,
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return configurableETADO;
        }

        public ConfigurableETADO GetConfigurableETA(int configurableETAId)
        {
            ConfigurableETADO configurableETADO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ConfigurableETAId", configurableETAId);
                    configurableETADO = dbQuery.Query<ConfigurableETADO>("ConfigurableETA_Sel", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return configurableETADO;
        }

        public int UpdateConfigurableETA(ConfigurableETADO configurableETADO)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ConfigurableETAId", configurableETADO.ConfigurableETAId);
                    para.Add("@ShopId", configurableETADO.ShopId);
                    para.Add("@DeliveryStartHours", configurableETADO.DeliveryStartHours);
                    para.Add("@DeliveryEndHours", configurableETADO.DeliveryEndHours);
                    para.Add("@DryDay", configurableETADO.DryDay);
                    para.Add("@FirstDeliverySTInMin", configurableETADO.FirstDeliverySTInMin);
                    para.Add("@FirstDeliveryETInMin", configurableETADO.FirstDeliveryETInMin);
                    para.Add("@DelDeadLineStart", configurableETADO.DelDeadLineStart);
                    para.Add("@DelDeadLineEnd", configurableETADO.DelDeadLineEnd);
                    para.Add("@Remarks", configurableETADO.Remarks);
                    para.Add("@ModifiedBy", configurableETADO.ModifiedBy);
                    res = dbQuery.Query<int>("ConfigurableETA_InsUpd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return res;
        }

        public int UpdateDryDayForAllShops(DateTime dryDay,string userEmail)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DryDay", dryDay);
                    para.Add("@Email", userEmail);
                    res = dbQuery.Query<int>("UpdateDryDayAllShops", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

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