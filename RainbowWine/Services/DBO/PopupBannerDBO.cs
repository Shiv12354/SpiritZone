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
    public class PopupBannerDBO
    {
        public PopupBannerDO GetPagePopupBanner(string pageName,string userid, int shopid, int? id = 0)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                PopupBannerDO popupBannerDO;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Userid", userid);
                    para.Add("@ShopId", shopid);
                    para.Add("@PageName", pageName);
                    para.Add("@otherid", id);
                    popupBannerDO = dbQuery.Query<PopupBannerDO>("CoreV2_1_Get_PagePopupBanners", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return popupBannerDO;

                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }

        public bool IsGoodies(int orderId)
        {
            bool Isgoodies = false;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    Isgoodies = dbQuery.Query<bool>("CheckGoodiesOrder_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return Isgoodies;
        }
    }
}