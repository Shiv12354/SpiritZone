using Dapper;
using RainbowWine.Data;
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
    public class MixerDBO
    {
        public List<MixerExtDO> MixerDetails(int shopId)
        {

            List<MixerExtDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@shopId", shopId);
                    prodDetail = dbQuery.Query<MixerExtDO>("MixerDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
            //List<MixerDetail> mixDetails = null;
            //using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        var para = new DynamicParameters();
            //        para.Add("@ShopId", shopId);
            //        mixDetails = dbQuery.Query<MixerDetail, Mixer, MixerSize, MixerDetail>("MixerDetails_Sel",
            //           (MD, M, MS) =>
            //           {
            //               MD.Mixer = M;
            //               MD.MixerSize = MS;
            //               return MD;
            //           },
            //             splitOn: "MixerId,MixerSizeId",
            //        param: para, commandType: CommandType.StoredProcedure).ToList();

            //    }
            //    finally
            //    {
            //        dbQuery.Close();
            //    }
            //}
            //return mixDetails;
        }

        public List<MixerDetailsDO> MixerDetailsById(int mixerId, int shopId)
        {
            List<MixerDetailsDO> mixDetails = null;
            var quDictionary = new Dictionary<int, Mixer>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@mixerId", mixerId);
                    para.Add("@ShopId", shopId);
                    mixDetails = dbQuery.Query<MixerDetailsDO, InventoryMixerDO, Mixer, MixerSize, MixerDetailsDO>("MixerDetails_MixerId_Sel",
                      (MD, IM, M, MS) =>
                      {
                          MD.InventoryMixerDO = IM;
                          MD.Mixer = M;
                          MD.MixerSize = MS;
                          return MD;
                      },
                       splitOn: "InventoryMixerId,MixerId,MixerSizeId",
                   param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return mixDetails;

            //List<MixerDetail> mixDetails = null;
            //using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        var para = new DynamicParameters();
            //        para.Add("@MixerId",MixerId);
            //         mixDetails = dbQuery.Query<MixerDetail,Mixer, MixerSize, MixerDetail>("MixerDetails_MixerId_Sel",
            //          (MD, M, MS) =>
            //          {
            //              MD.Mixer = M;
            //              MD.MixerSize = MS;
            //              return MD;
            //          },
            //            splitOn: "MixerId,MixerSizeId",
            //       param: para, commandType: CommandType.StoredProcedure).ToList();

            //    }
            //    finally
            //    {
            //        dbQuery.Close();
            //    }
            //}
            //return mixDetails;
        }

        public List<MixerDetail> MixerDetailsByShowCase()
        {
            List<MixerDetail> mixDetails = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    mixDetails = dbQuery.Query<MixerDetail, Mixer, MixerSize, MixerDetail>("MixerDetails_ShowCase_Sel",
                      (MD, M, MS) =>
                      {
                          MD.Mixer = M;
                          MD.MixerSize = MS;
                          return MD;
                      },
                        splitOn: "MixerId,MixerSizeId",
                  param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return mixDetails;
        }

        public List<MixerExtDO> MixerDetailsBasedOnInvetory(int index, int size, int shopId)
        {

            List<MixerExtDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@index", index);
                    para.Add("@size", size);
                    para.Add("@shopId", shopId);
                    prodDetail = dbQuery.Query<MixerExtDO>("InventoryMixer_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
            //List<MixerDetail> mixDetails = null;
            //using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        var para = new DynamicParameters();

            //para.Add("@index", index);
            //para.Add("@size", size);
            //        para.Add("@ShopId", shopId);
            //        mixDetails = dbQuery.Query<MixerDetail, Mixer, MixerSize, MixerDetail>("InventoryMixer_Sel",
            //           (MD, M, MS) =>
            //           {
            //               MD.Mixer = M;
            //               MD.MixerSize = MS;
            //               return MD;
            //           },
            //             splitOn: "MixerId,MixerSizeId",
            //        param: para, commandType: CommandType.StoredProcedure).ToList();

            //    }
            //    finally
            //    {
            //        dbQuery.Close();
            //    }
            //}
            //return mixDetails;


        }

        public void MixerAdd(MixerDO mixerDO)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@MixerName", mixerDO.MixerName);
                    para.Add("@Description", mixerDO.Description);
                    para.Add("@MixerImage", mixerDO.MixerImage);
                    para.Add("@MixerThumbImage", mixerDO.MixerThumbImage);
                    dbQuery.Query<int>("Mixer_Ins",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public void MixerUpdate(MixerDO mixerDO)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@MixerId", mixerDO.MixerId);
                    para.Add("@MixerName", mixerDO.MixerName);
                    para.Add("@Description", mixerDO.Description);
                    para.Add("@MixerImage", mixerDO.MixerImage);
                    para.Add("@MixerThumbImage", mixerDO.MixerThumbImage);
                    dbQuery.Query<int>("Mixer_Upd",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public void MixerDelete(MixerDO mixerDO)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@MixerId", mixerDO.MixerId);
                    dbQuery.Query<int>("Mixer_Del",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public List<MixerExtDO> MixerDetailsByOrder(int orderId)
        {

            List<MixerExtDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    prodDetail = dbQuery.Query<MixerExtDO>("MixerDetailByOrder_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
            
        }

       

    }
}