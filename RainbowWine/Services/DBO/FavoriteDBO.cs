using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using RainbowWine.Data;
using RainbowWine.Services.DO;

namespace RainbowWine.Services.DBO
{
    public class FavoriteDBO
    {
        public List<ProductDetailDO> GetFavoriteDetails(int index, int size,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@index", index);
                    para.Add("@size", size);
                    para.Add("@customerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("Favorite_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
            //List<FavoriteDO> favoDetail = null;
            //using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        var para = new DynamicParameters();
            //        para.Add("@index", index);
            //        para.Add("@size", size);
            //        para.Add("@CustomerId",customerId);                                
            //        favoDetail = dbQuery.Query<FavoriteDO, Product, Customer, FavoriteDO>("Favorite_Sel",
            //          (F, P, C) =>
            //          {
            //              F.Product = P;
            //              F.Customer = C;
            //              return F;
            //          },
            //            splitOn: "ProductId,CustomerId",
            //       param: para, commandType: CommandType.StoredProcedure).ToList();

            //    }
            //    finally
            //    {
            //        dbQuery.Close();
            //    }
            //}
            //return favoDetail;
        }
        public void FavoriteAdd(int ProductId, int CustomerId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", CustomerId);
                    para.Add("@ProductId", ProductId);
                    dbQuery.Query<int>("Favorite_Ins",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public void FavoriteUpdate(int ProductId, int CustomerId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", CustomerId);
                    para.Add("@ProductId", ProductId);
                    dbQuery.Query<int>("Favorite_Upd",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }

        public void FavoriteDelete(int productId, int customerId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", productId);
                    para.Add("@CustomerId", customerId);
                    dbQuery.Query<int>("Favorite_Del",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }

        }
    }
}