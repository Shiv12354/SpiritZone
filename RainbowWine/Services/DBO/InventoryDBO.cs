using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services.DO;

namespace RainbowWine.Services.DBO
{
    public class InventoryDBO
    {
        public List<InventoryViewModel> GetInventory(string product, string shop)
        {
            List<InventoryViewModel> routePlans = new List<InventoryViewModel>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductName", product);
                    para.Add("@ShopName", shop);
                    routePlans = dbQuery.Query<InventoryViewModel, WineShop, Product, ProductDetail, ProductSize, InventoryViewModel>("Invertory_Sel",
                        (im, ws, p,pd,ps) =>
                        {
                            //im.ShopId = i.ShopID;
                            im.ShopName = ws.ShopName;
                            //im.ProductId = i.ProductID;
                            im.ProductName = pd.ProductName;
                            //im.InventoryId = i.InventoryId;
                            //im.PackingSize = i.packing_size ?? 0;
                            //im.MRP = i.MRP ?? 0;
                            //im.QtyAvailable = i.QtyAvailable;
                            im.Price = im.MRP ;
                            im.ProductSizeId = pd.ProductSizeID ?? 0;
                            im.Size = ps.Capacity;
                            return im;
                        },
                        param: para,
                        commandType: CommandType.StoredProcedure,
                        splitOn: "WineShopId,ProdId,ProdDetailId,ProdSizeId").ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public List<GlobalMrpChange> GetInventoryDetails(int  productRefId)
        {
            List<GlobalMrpChange> result = new List<GlobalMrpChange>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductRefId", productRefId);
                    result = dbQuery.Query<GlobalMrpChange>("GetInventoryDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;
        }

        public int UpdateInvFromThirdParty(ThirdPartyInventory thirdPartyInventoryModel)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ItemCode", thirdPartyInventoryModel.ItemCode);
                    para.Add("@ShopCode", thirdPartyInventoryModel.ShopCode);
                    para.Add("@QtyAvailable", thirdPartyInventoryModel.CLBalance);
                    result = dbQuery.Query<int>("SZInventory_From3P_Update", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;
        }

        public int UpdateGlobalInvetoryMrp(int productId,string shopIds,decimal mrp,string userId)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", productId);
                    para.Add("@ShopIds", shopIds);
                    para.Add("@Mrp", mrp);
                    para.Add("@UserId", userId);
                    result = dbQuery.Query<int>("GlobalInventoryMrp_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;
        }

        public int AddGoodyInventory(int goodyId, int shopId)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@GoodyId", goodyId);
                    para.Add("@ShopId", shopId);
                    result = dbQuery.Query<int>("GoodiesInventory_Ins", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;
        }

        public int UpdateGoodyInventory(int goodyId, int shopId)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@GoodyId", goodyId);
                    para.Add("@ShopId", shopId);
                    result = dbQuery.Query<int>("GoodiesInventory_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
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