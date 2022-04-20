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
    public class RatingProductDBO
    {
        public List<RatingProductDO> GetProductRatingsDetails(int CustomerId)
        {
            List<RatingProductDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId",CustomerId);
                    prodDetail = dbQuery.Query<RatingProductDO,Product,Customer,RatingStar,RatingProductDO>("RatingProductReview_Sel",
                      (RP, P,C,RS) =>
                      {
                          RP.Product = P;
                          RP.Customer = C;
                          RP.RatingStar = RS;
                          return RP;
                      },
                        splitOn: "ProductId,CustomerId,RatingStartId",
                   param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<RatingProductDO> GetProductRatingsDetailsByProduct(int ProductId)
        {
            List<RatingProductDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", ProductId);
                    prodDetail = dbQuery.Query<RatingProductDO>("RatingProductReview_ByProductId_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

            //using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        var para = new DynamicParameters();
            //        para.Add("@ProductId",ProductId);
            //        prodDetail = dbQuery.Query<RatingProductDO,Product,Customer,RatingStar,RatingProductDO>("RatingProductReview_ByProductId_Sel",
            //        (RP, P, C, RS) =>
            //        {
            //            RP.Product = P;
            //            RP.Customer = C;
            //            RP.RatingStar = RS;
            //            return RP;
            //        },
            //        splitOn: "ProductId,CustomerId,RatingStartId",
            //        param: para, commandType: CommandType.StoredProcedure).ToList();

            //    }
            //    finally
            //    {
            //        dbQuery.Close();
            //    }
            //}
            //return prodDetail;
        }

        public List<RatingStarttDO> ProductAllRating(int CustomerId)
        {

            List<RatingStarttDO> prodDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", CustomerId);
                    prodDetail = dbQuery.Query<RatingStarttDO>("ProductAllRating_Sel",param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailExtDO> ProductDetailsRating(int index, int size, int CustomerId,int rating)
        {

            List<ProductDetailExtDO> prodDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@index", index);
                    para.Add("@size", size);
                    para.Add("@CustomerId", CustomerId); 
                    para.Add("@Rating", rating); 
                     prodDetail = dbQuery.Query<ProductDetailExtDO>("ProductDetailsRating_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                   
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
        public void ProductRatingsAdd(int CustomerId,int ProductId, int Rating )
        {
           
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId",CustomerId);
                    para.Add("@ProductId",ProductId);
                    para.Add("@Rating",Rating);
                    dbQuery.Query<int>("RatingProductReview_Ins",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }
           
        }

        public void ProductRatingsUpdate(int CustomerId, int ProductId, int Rating)
        {
            
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", CustomerId);
                    para.Add("@ProductId", ProductId);
                    para.Add("@Rating", Rating);
                    dbQuery.Query<int>("RatingProductReview_Upd",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }
           
        }

        public void ProductRatingsDelete(int ProductId)
        {
            
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", ProductId);
                    dbQuery.Query<int>("RatingProductReview_Del",
                    param: para, commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }
            }
           
        }

        public List<WineShopExtDO> GetShopAddress(int deliveryAgentId)
        {
            List<WineShopExtDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", deliveryAgentId);
                    prodDetail = dbQuery.Query<WineShopExtDO>("WineShop_Address_Sel",
                    param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
    }
}