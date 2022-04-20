using Dapper;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.OnlinePaymentService
{
    public class OnlinePaymentServiceDBO
    {
        public int AppLogsOnlinePaymentHookAdd(AppLogsOnlinePaymentHook appLogsOnlinePaymentHook)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@VenderInput", appLogsOnlinePaymentHook.VenderInput);
                    para.Add("@OrderId", appLogsOnlinePaymentHook.OrderId);
                    para.Add("@OrderIdPartial", appLogsOnlinePaymentHook.OrderIdPartial);
                    para.Add("@OrderAmount", appLogsOnlinePaymentHook.OrderAmount);
                    para.Add("@ReferenceId", appLogsOnlinePaymentHook.ReferenceId);
                    para.Add("@Status", appLogsOnlinePaymentHook.Status);
                    para.Add("@PaymentMode", appLogsOnlinePaymentHook.PaymentMode);
                    para.Add("@Msg", appLogsOnlinePaymentHook.Msg);
                    para.Add("@TxtTime", appLogsOnlinePaymentHook.TxtTime);
                    para.Add("@Signature", appLogsOnlinePaymentHook.Signature);
                    para.Add("@SendStatus", appLogsOnlinePaymentHook.SendStatus);
                    para.Add("@MachineName", appLogsOnlinePaymentHook.MachineName);
                    para.Add("@Error_Desc", appLogsOnlinePaymentHook.Error_Desc);
                    para.Add("@Currency", appLogsOnlinePaymentHook.Currency);
                    para.Add("@BankName", appLogsOnlinePaymentHook.BankName);
                    para.Add("@Country", appLogsOnlinePaymentHook.Country);
                    para.Add("@PaymentGateWayName", "AggrePay");
                    res = dbQuery.Query<int>("AppLogsOnlinePaymentHook_Ins",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public IList<AppLogsOnlinePaymentHook> GetAppLogsOnlinePaymentHook(string orderId)
        {
           
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
               IList<AppLogsOnlinePaymentHook> appLogsOnlinePaymentHook;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    appLogsOnlinePaymentHook = dbQuery.Query<AppLogsOnlinePaymentHook>("AppLogsOnlinePaymentHook_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return appLogsOnlinePaymentHook;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public OnlinePartialPayment GetOnlinePartialPaymentOrderId(string uniqueId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                OnlinePartialPayment onlinePartialPayment = null;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UniqueId", uniqueId);
                    onlinePartialPayment = dbQuery.Query<OnlinePartialPayment>("OnlinePartialPaymen_UniqueId_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return onlinePartialPayment;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public int OnlinePartialPaymentAdd(OnlinePartialPayment onlinePartialPayment)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@InputValue", onlinePartialPayment.InputValue);
                    para.Add("@VenderOutput", onlinePartialPayment.VenderOutput);
                    para.Add("@IssueId", onlinePartialPayment.IssueId);
                    para.Add("@OrderId", onlinePartialPayment.OrderId);
                    para.Add("@Amount", onlinePartialPayment.Amount);
                    para.Add("@PaymentGateWayName", "AggrePay");
                    para.Add("@UniqueId", onlinePartialPayment.UniqueId);
                    res = dbQuery.Query<int>("OnlinePartialPayment_Ins",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public IList<OnlinePartialPayment> GetOnlinePartialPayment(int orderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                IList<OnlinePartialPayment> onlinePartialPayment;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    onlinePartialPayment = dbQuery.Query<OnlinePartialPayment>("OnlinePartialPayment_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                    return onlinePartialPayment;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public int OnlinePaymentLogAdd(OnlinePaymentLog onlinePaymentLog)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", onlinePaymentLog.OrderId);
                    para.Add("@InputParameters", onlinePaymentLog.InputParameters);
                    para.Add("@VenderOutPut", onlinePaymentLog.VenderOutPut);
                    para.Add("@OrderIdCF", onlinePaymentLog.OrderIdCF);
                    para.Add("@OrderAmount", onlinePaymentLog.OrderAmount);
                    para.Add("@ReferenceId", onlinePaymentLog.ReferenceId);
                    para.Add("@Status", onlinePaymentLog.Status);
                    para.Add("@PaymentMode", onlinePaymentLog.PaymentMode);
                    para.Add("@Msg", onlinePaymentLog.Msg);
                    para.Add("@TxtTime", onlinePaymentLog.TxtTime);
                    para.Add("@Signature", onlinePaymentLog.Signature);
                    para.Add("@SendStatus", onlinePaymentLog.SendStatus);
                    para.Add("@MachineName", onlinePaymentLog.MachineName);
                    para.Add("@PaymentGateWayName", "AggrePay");
                    res = dbQuery.Query<int>("OnlinePaymentLog_Ins",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public OnlinePaymentLog GetOnlinePaymentLog(int orderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                OnlinePaymentLog onlinePaymentLog;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    onlinePaymentLog = dbQuery.Query<OnlinePaymentLog>("OnlinePaymentLog_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return onlinePaymentLog;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public int OnlineRefundAdd(OnlineRefund onlineRefund)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@IssueId", onlineRefund.IssueId);
                    para.Add("@OrderModifyId", onlineRefund.OrderModifyId);
                    para.Add("@OrderId", onlineRefund.OrderId);
                    para.Add("@InputParam", onlineRefund.InputParam);
                    para.Add("@VenderOutput", onlineRefund.VenderOutput);
                    para.Add("@Amount", onlineRefund.Amount);
                    para.Add("@PaymentGateWayName", "AggrePay");
                    para.Add("@Status", onlineRefund.Status);
                    res = dbQuery.Query<int>("OnlineRefund_Ins",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;
        }

        public IList<OnlineRefund> GetOnlineRefund(int orderId)
        {
            IList<OnlineRefund> onlineRefunds;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    onlineRefunds = dbQuery.Query<OnlineRefund>("OnlineRefund_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                    

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return onlineRefunds;

        }

        public int UpdateAppLogsOnlinePaymentHook(int appLogsOnlinePaymentHookId, string sendStatus,string convenienceFee)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
              int  appLogsOnlinePaymentHook = 0;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@AppLogsOnlinePaymentHookId", appLogsOnlinePaymentHookId);
                    para.Add("@SendStatus", sendStatus);
                    para.Add("@ConvenienceFee",convenienceFee);
                    appLogsOnlinePaymentHook = dbQuery.Query<int>("AppLogsOnlinePaymentHook_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return appLogsOnlinePaymentHook;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public PaymentGateWayType GetPaymentGateWayName(int orderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                PaymentGateWayType paymentGateWayType;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId.ToString());
                    paymentGateWayType = dbQuery.Query<PaymentGateWayType>("CheckPaymentGateWayName_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return paymentGateWayType;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public decimal GetWalletReturnedAmount(int orderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                decimal result =0;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId.ToString());
                    result = dbQuery.Query<decimal>("WalletReturnedAmount_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return result;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public decimal GetWalletExceededAmount(int orderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                decimal result = 0;

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId.ToString());
                    result = dbQuery.Query<decimal>("WalletExceededAmount_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return result;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public int UpdateOnlineRefund(int onlineRefundId,string refundReferenceNo)
        {
            using (IDbConnection dbQuery=new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                int result = 0;
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OnlineRefundId",onlineRefundId);
                    para.Add("@RefundReferenceNo", refundReferenceNo);
                    result = dbQuery.Query<int>("OnlineRefund_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                catch (Exception)
                {

                    throw;
                }
                return result;

            }
        }

        public ConvenienceFeeDetailDO GetAggrePayConvenienceFeeDetails(int orderId)
        {
            ConvenienceFeeDetailDO convenienceFeeDetailDO = new ConvenienceFeeDetailDO();

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    convenienceFeeDetailDO = dbQuery.Query<ConvenienceFeeDetailDO>("CalculateAggrePayConvenienceFree_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return convenienceFeeDetailDO;
        }
    }
}