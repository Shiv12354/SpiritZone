using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritZonePayInventory
{
    public class PaytmOrder
    {
        public DataTable GetOrderList()
        {
            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString; 
            DataSet ds = new DataSet();
            DataTable dataTable = null;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("PaymentLinkLog_Sel", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds,"Order");
                    }                        
                }
            }
            if (ds.Tables.Count > 0)
            {
                dataTable = ds.Tables[0];
            }
            return dataTable;
        }

        public DataTable GetOrderListCashFree()
        {
            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            DataSet ds = new DataSet();
            DataTable dataTable = null;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("PaymentLinkLog_Sel1", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds, "Order");
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                dataTable = ds.Tables[0];
            }
            return dataTable;
        }


        public void UpdateInventory(int orderId)
        {

            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("InventoryOrder_Upd", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@OrderId", orderId));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();
                }
            }
        }

        public DataTable GetPaytmRefundInitiatedList()
        {
            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            DataSet ds = new DataSet();
            DataTable dataTable = null;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //using (SqlCommand cmd = new SqlCommand("PaymentLinkLog_Refund_Sel", con))
                using (SqlCommand cmd = new SqlCommand("PaymentRefund_Refund_Sel", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds, "PaymentRefund");
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                dataTable = ds.Tables[0];
            }
            return dataTable;
        }


        public DataTable GettestingList()
        {
            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            DataSet ds = new DataSet();
            DataTable dataTable = null;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("select * from AppLogsPaytmHook where PtmRespMsg is null", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds, "AppLogsPaytmHook");
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                dataTable = ds.Tables[0];
            }
            return dataTable;
        }


        public void updattestingList(int id, string linkid, string ORDERID, string STATUS, string CHECKSUMHASH,
                    string TXNAMOUNT, string TXNID, string LINKNOTES, string RESPMSG)
        {
            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand($"update AppLogsPaytmHook "
                    +$" set "
                    //+$" PtmLinkId={linkid}, "
                    //+ $" LinkOrderId=(select top 1 OrderId from PaymentLinkLog where PtmLinkId={linkid}), "
                    //+ $" PtmOrderId='{ORDERID}', "
                    //+ $" PtmStatus='{STATUS}', "
                    //+ $" CheckSumHash='{CHECKSUMHASH}', "
                    //+ $" TxnAmount='{TXNAMOUNT}', "
                    //+ $" TxnId='{TXNID}', "
                    //+ $" LinkNotes='{LINKNOTES}', "
                    +$" PtmRespMsg='{RESPMSG}'"
                    + $" where AppLogsPaytmHookId={id}", con))
                {
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }

    }
}
