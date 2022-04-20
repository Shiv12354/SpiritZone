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
    public class DiscountDBO
    {

        public DiscountProduct GetDiscountOnProduct(int productId)
        {
            DiscountProduct discounts = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@ProductID", productId);
                    discounts = dbQuery.Query<DiscountProduct>("DiscountProduct_ProductID_Sel", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return discounts;
        }

        public int UpdateUserSpecificOfferMapping(int customerId,int offerId,string offerType,int orderId,decimal rewardAmount)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@CustomerId", customerId);
                    para.Add("@OfferId", offerId);
                    para.Add("@OfferType", offerType);
                    para.Add("@OrderId", orderId);
                    para.Add("@RewardAmount", rewardAmount);
                    res = dbQuery.Query<int>("UserSpecificOfferMapping_Upd", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return res;
        }

        public int RevertNumberOfUseSpecificOffer(int customerId, int orderId)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@CustomerId", customerId);
                    para.Add("@OrderId", orderId);
                    res = dbQuery.Query<int>("RevertNumberOfUseUserSpecificOffer", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return res;
        }

        public UserSpecificOfferDO GetUserSpecificOffer(int customerId, int offerId, string offerType)
        {
            UserSpecificOfferDO userSpecificOfferDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@CustomerId", customerId);
                    para.Add("@OfferId", offerId);
                    para.Add("@OfferType", offerType);
                    userSpecificOfferDO = dbQuery.Query<UserSpecificOfferDO>("UserSpecificOfferMapping_Sel", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return userSpecificOfferDO;
        }

    }
}