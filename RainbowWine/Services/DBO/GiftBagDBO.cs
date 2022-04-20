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
    public class GiftBagDBO
    {
        public List<GiftBagSearchDO> SearchGiftBag(int shopId, string searchText)
        {
            List<GiftBagSearchDO> giftBagSearch = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@SearchText", searchText);
                    giftBagSearch = dbQuery.Query<GiftBagSearchDO>("GiftBagSearch_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return giftBagSearch;
        }

        public GiftBagSearchDO GiftBagSelection(int shopId, int giftBagDetailId)
        {
            GiftBagSearchDO giftBagSearchDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@GiftBagDetailId", giftBagDetailId);
                    giftBagSearchDO = dbQuery.Query<GiftBagSearchDO>("GiftBagDetailSelection", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return giftBagSearchDO;
        }

        public int AddGiftBagOrderItemIssueDetail(GiftBagOrderItemIssueDetail giftIssueDetail)
        {
           int giftBag =0 ;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderIssueId", giftIssueDetail.OrderIssueId);
                    para.Add("@OrderDetailId", giftIssueDetail.OrderDetailId);
                    para.Add("@OrderId", giftIssueDetail.OrderId);
                    para.Add("@GiftBagDetailId", giftIssueDetail.GiftBagDetailId);
                    para.Add("@ItemQty", giftIssueDetail.ItemQty);
                    para.Add("@Price", giftIssueDetail.Price);
                    para.Add("@ShopId", giftIssueDetail.ShopId);
                    para.Add("@Issue", giftIssueDetail.Issue);
                    giftBag = dbQuery.Query<int>("GiftBagOrderItemIssueDetail_Ins", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return giftBag;
        }

        public GiftBagSearchDO GiftBagTotalAmountOrderwise(int orderId)
        {
            GiftBagSearchDO giftBagSearchDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    giftBagSearchDO = dbQuery.Query<GiftBagSearchDO>("GetgiftBagOrderTotalAmount", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return giftBagSearchDO;
        }

        public IList<GiftBagSearchDO> GetGiftBagDeliveryAgentWise(int deliveryAgentId)
        {
           IList<GiftBagSearchDO> giftBagSearchDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", deliveryAgentId);
                    giftBagSearchDO = dbQuery.Query<GiftBagSearchDO>("NewDelV3_GiftOrderDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return giftBagSearchDO;
        }

        public IList<GiftBagOrderItem> GetGiftBagOrderItems(int orderId)
        {
            IList<GiftBagOrderItem> giftBagOrderItems = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    giftBagOrderItems = dbQuery.Query<GiftBagOrderItem>("V3_GiftBagOrderItems_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return giftBagOrderItems;
        }

        public UserRecipientOrderDO GetUserRecipientOrder(string contactNo)
        {
            UserRecipientOrderDO userRecipientOrderDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@ContactNo", contactNo);
                    userRecipientOrderDO = dbQuery.Query<UserRecipientOrderDO>("UserRecipientOrder_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return userRecipientOrderDO;
        }

        public UserRecipientOrderDO GetUserRecipientOrderDetails(int orderId)
        {
            UserRecipientOrderDO userRecipientOrderDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    userRecipientOrderDO = dbQuery.Query<UserRecipientOrderDO>("UserRecipientOrderDetails_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return userRecipientOrderDO;
        }

        public int GetRecipient(int orderId ,string contactNo)
        {
            int recipient = 0; 
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@ContatcNo", contactNo);
                    recipient = dbQuery.Query<int>("Recipient_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return recipient;
        }

        public FeatureHookDO GetFeatureHookDetails(int shopId,int zoneId, string hookType,string pageName)
        {
            FeatureHookDO featureHookDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ZoneId",zoneId);
                    para.Add("@HookType", hookType);
                    para.Add("@PageName",pageName);
                    featureHookDO = dbQuery.Query<FeatureHookDO>("CoreV3_1_FeatureHook_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return featureHookDO;
        }

        public int UpdateUserRecipientOrder(int orderId ,string tinyUrl)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@TinyUrl",tinyUrl);
                    res = dbQuery.Query<int>("UserRecipientOrder_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public OTPCount GetOtpCount(string contactNo)
        {
            OTPCount oTPCount = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@MobileNo", contactNo);
                    oTPCount = dbQuery.Query<OTPCount>("Cal_OTPCount", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return oTPCount;
        }

        public UserRecipient GetUserRecipientDetails(int orderId)
        {
            UserRecipient userRecipient = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {

                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    userRecipient = dbQuery.Query<UserRecipient>("GetUserRecepientDetails", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return userRecipient;
        }

    }
}