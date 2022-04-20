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
    public class ManufacturerDBO
    {
        public List<ManufacturerDO> GetManufacturer()
        {
            List<ManufacturerDO> manufacturerDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ManufacturerId", 0);
                    manufacturerDetail = dbQuery.Query<ManufacturerDO>("Manufacturer_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return manufacturerDetail;
        }

        public int ManufacturerAdd(string manufacturerName, string manufacturerAbbreviated, string region, string collabSince)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ManufacturerId", 0);
                    para.Add("@ManufacturerName", manufacturerName);
                    para.Add("@ManufacturerAbbreviated", manufacturerAbbreviated);
                    para.Add("@Region", region);
                    para.Add("@CollabSince", collabSince);
                    result = dbQuery.Query<int>("Manufacturer_InsUpd",param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    //var outvalue= para.Get<int>("@Output");
                    
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;
        }
        public int ManufacturerUpdate(int manufacturerId, string manufacturerName, string manufacturerAbbreviated, string region, string collabSince)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ManufacturerId", manufacturerId);
                    para.Add("@ManufacturerName", manufacturerName);
                    para.Add("@ManufacturerAbbreviated", manufacturerAbbreviated);
                    para.Add("@Region", region);
                    para.Add("@CollabSince", collabSince);
                    result= dbQuery.Query<int>("Manufacturer_InsUpd",param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;
        }
        public void ManufacturerDelete(int manufacturerId)
        {
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ManufacturerId", manufacturerId);
                    dbQuery.Execute("Manufacturer_Del",
                        param: para, commandType: CommandType.StoredProcedure);
                    //var outvalue= para.Get<int>("@Output");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }

        public List<BrandManufacturerDO> GetBrandManufacturerDetail(int manufacturerId)
        {

            List<BrandManufacturerDO> details = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ManufacturerId", manufacturerId);
                    details = dbQuery.Query<BrandManufacturerDO>("BrandManufacturer_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return details;

        }

        public List<BrandManufacturerDO> GetBrandManufacturerById(int manufacturerId)
        {

            List<BrandManufacturerDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ManufacturerId", manufacturerId);
                    prodDetail = dbQuery.Query<BrandManufacturerDO>("BrandManufacturerByManufacturerId_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

        }

        public int AddAndUpdateBrandManufacturer(int manufacturerId, string brandIds, string email, string action)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ManufacturerId", manufacturerId);
                    para.Add("@BrandIds", brandIds);
                    para.Add("@Email", email);
                    para.Add("@Action", action);
                    result = dbQuery.Query<int>("BrandManufacturer_InsUpd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;
        }
    }
}