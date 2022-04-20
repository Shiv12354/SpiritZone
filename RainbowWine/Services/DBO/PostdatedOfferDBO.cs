using Dapper;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SZModels;

namespace RainbowWine.Services.DBO
{
    public class PostdatedOfferDBO
    {
        public List<PreBookingPromoOfferProductLink> GetPreBookingPromoOfferProductLink(string productIds)
        {

            List<PreBookingPromoOfferProductLink> detail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductDetailIds", productIds);
                    detail = dbQuery.Query<PreBookingPromoOfferProductLink>("CoreV2_2_Get_PreBookingPromoOfferProductLink", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return detail;

        }

        public List<Order_OrderDetailsDO> GetOrderOrderDetails(int orderId)
        {

            List<Order_OrderDetailsDO> detail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    detail = dbQuery.Query<Order_OrderDetailsDO>("CoreV2_2_Get_OrderOrderDetails", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return detail;

        }

        public PromoApplyOutput PromoCodeApplyValidate(string promoCode, float totalAmount, string userId,int orderId)
        {

            PromoApplyOutput detail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add(SZParameters.PromoCode, promoCode);
                    para.Add(SZParameters.TotalAmount, totalAmount);
                    para.Add(SZParameters.UserId, userId);
                    para.Add("@OrderId", orderId);
                    detail = dbQuery.Query<PromoApplyOutput>("PromoCode_Apply_Validate", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return detail;

        }

        public int AddPostdatedOffer(PostdatedOfferDO postdatedOfferDO)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@PromoId", postdatedOfferDO.PromoId);
                    para.Add("@UserId", postdatedOfferDO.UserId);
                    para.Add("@OrderId", postdatedOfferDO.OrderId);
                    para.Add("@OfferStartDate", postdatedOfferDO.OfferStartDate);
                    para.Add("@OfferEndDate", postdatedOfferDO.OfferEndDate);
                    para.Add("@Title", postdatedOfferDO.Title);
                    para.Add("@SubTitle", postdatedOfferDO.SubTitle);
                    result = dbQuery.Query<int>("PostdatedOffer_Ins", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
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