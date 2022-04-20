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
    public class ConfigMasterDBO
    {
        public List<ConfigMasterDO> GetConfigMasterDetails(int configMasterId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                List<ConfigMasterDO> configMasterDO;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ConfigMasterId", 0);
                    para.Add("@ValueText",0);
                    para.Add("@Description", null);
                    para.Add("@Email", null);
                    configMasterDO = dbQuery.Query<ConfigMasterDO>("ConfigMaster_Upd", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return configMasterDO;

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public int UpdateConfigMasterDetails(int configMasterId,bool valueText,string description,string email)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                int result = 0;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ConfigMasterId", configMasterId);
                    para.Add("@ValueText", valueText);
                    para.Add("@Description", description);
                    para.Add("@Email",email);
                    result = dbQuery.Query<int>("ConfigMaster_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return result;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public ConfigMasterDO GetConfigMasterDetail(int configMasterId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                ConfigMasterDO configMasterDO;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ConfigMasterId", configMasterId);
                    configMasterDO = dbQuery.Query<ConfigMasterDO>("ConfigMasterbyId_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return configMasterDO;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }
    }
}