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
    public class NotifyDBO
    {
        public List<NotifyOrderDO> Notifies(bool IsPremium)
        {
            var dicNotify = new List<NotifyOrderDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@IsPremium", IsPremium);

                    dicNotify = dbQuery.Query<Notify, NotifyOrderDO, ProductDetail, WineShop, Customer, Inventory, NotifyOrderDO>("Notify_ByPremium_Sel",
                        (n, nord, p, ws, c, i) =>
                        {
                            if (n != null) nord.Notify = n;
                            if (p != null) nord.ProductDetail = p;
                            if (ws != null) nord.Shop = ws;
                            if (c != null) nord.Customer = c;
                            if (i != null) nord.Inventory = i;

                            return nord;
                        }, commandType: CommandType.StoredProcedure,param: para,
                        splitOn: "NotifyOrderId, ProductID, WinShopId, CustId, InventoryId").ToList();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return dicNotify;
        }
    }
}