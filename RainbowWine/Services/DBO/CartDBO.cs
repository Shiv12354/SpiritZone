using Dapper;
using RainbowWine.Models;
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
    public class CartDBO
    {
        public string AddToCart(CartInput cartInput, int CustomerId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", CustomerId);
                    para.Add("@ProductId", cartInput.ProductId);
                    para.Add("@ProductName", cartInput.ProductName);
                    para.Add("@ProductImage", cartInput.ProductImage);
                    para.Add("@UnitPrice", cartInput.UnitPrice);
                    para.Add("@Qty", cartInput.Qty);
                    para.Add("@IsMixer", cartInput.IsMixer);
                    para.Add("@IsReserve", cartInput.IsReserve);
                    para.Add("@MixerType", cartInput.MixerType);
                    para.Add("@MixerRefId", cartInput.MixerRefId);
                    para.Add("@CartId", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);
                    dbQuery.Execute("Cart_Ins",
                    param: para, commandType: CommandType.StoredProcedure);
                    var cartId = para.Get<string>("@CartId");
                    return cartId;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }
        public void UpdateCart(CartInput cartInput)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CartId", cartInput.CartId);
                    para.Add("@Qty", cartInput.Qty);
                    dbQuery.Query<int>("Cart_Upd_Del",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }
        public void DeleteCart(CartInput cartInput)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CartId", cartInput.CartId);
                    para.Add("@Qty", cartInput.Qty);
                    dbQuery.Query<int>("Cart_Upd_Del",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }
        public List<CartDO> GetCartItems(int cartId,int shopId ,int customerId)
        {
            List<CartDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CartId", cartId);
                    para.Add("@ShopId", shopId);
                    para.Add("@customerId", customerId);
                    prodDetail = dbQuery.Query<CartDO>("Cart_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

        }
        public List<CartDO> GetCartHistoryByCustomer(int customerId)
        {
            List<CartDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@customerId", customerId);
                    prodDetail = dbQuery.Query<CartDO>("Card_History", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

        }
        public void CartEmply(int customerId)
        {
            
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@customerId", customerId);
                   dbQuery.Query<CartDO>("CartEmpty", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            

        }
        public CustomerExt CheckPod(int shopId,int customerId)
        {
            CustomerExt customerExt = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@CustomerId", customerId);
                    var results = dbQuery.QueryMultiple("CheckPOD_Sel", param: para, commandType: CommandType.StoredProcedure);

                    var opdByCust = results.Read<CustomerExt>().FirstOrDefault();
                    var opdByShop = results.Read<WineShopExt>().FirstOrDefault();
                    var refBal = results.Read<ReferBalance>().FirstOrDefault();
                    customerExt = opdByCust;
                    customerExt.WineShop = opdByShop;
                    customerExt.ReferBalance = refBal;
                }
                finally
                {
                    dbQuery.Close();
                }

            }

            return customerExt;
        }

        public void UpdateMultipleCarts(List<CartInput> cartInput)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    for (int i = 0; i < cartInput.Count(); i++)
                    {
                        var para = new DynamicParameters();
                        para.Add("@CartId", cartInput[i].CartId);
                        para.Add("@Qty", cartInput[i].Qty);
                        dbQuery.Query<int>("MultipleCarts_Upd",
                        param: para, commandType: CommandType.StoredProcedure);
                    }
                    
                    

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }
    }
}