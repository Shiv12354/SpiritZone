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
    public class OrderIssueDBO
    {
        public void AddPendingPay(OrderIssue orderIssue)
        {
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderIssue.OrderId);
                    para.Add("@OrderIssueId", orderIssue.OrderIssueId);
                    //para.Add("@OrderIssueTypeId", orderIssuePendingPay.OrderIssueTypeId);
                    //para.Add("@AdjustAmt", orderIssuePendingPay.AdjustAmt);
                    var prodDetail = dbQuery.Query<int>("OrderIssue_Cash_Ins", param: para, commandType: CommandType.StoredProcedure);
                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }
        public OrderIssue GetPendingPay(int orderId)
        {
            OrderIssue issue = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    issue = dbQuery.Query<OrderIssue>("OrderIssue_Cash_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return issue;
        }
        public int FullRefundToWallet(int orderId,float refAmount,int orderIssueId)
        {
            int issue = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@RefundAmount", refAmount);
                    para.Add("@OrderIssueId", orderIssueId);
                    issue = dbQuery.Query<int>("FullRefundToWallet_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return issue;
        }

        public bool AmountAddedInWallet(string orderId, int orderIssueId)
        {
            bool issue = false;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@OrderIssueId", orderIssueId);
                    int result = dbQuery.Query<int>("AmountAddedTowallet_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (result > 0)
                    {
                        issue = true;
                    }
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return issue;
        }

        public void RevertAddedWalletRefundAmount(int orderId)
        {
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    var prodDetail = dbQuery.Query<int>("RevertAddedWalletRefundAmount", param: para, commandType: CommandType.StoredProcedure);
                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }
    }
}