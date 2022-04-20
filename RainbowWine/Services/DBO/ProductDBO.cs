using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using Dapper;
using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services.DO;
using RestSharp;

namespace RainbowWine.Services.DBO
{
    public class ProductDBO
    {
        public List<ProductDetailDO> GetProductDetails(int shopId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
        public List<ProductDetailDO> GetProductDetailsPagination(ProductSearchViewModel productSearchViewModel,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Index", productSearchViewModel.Index);
                    para.Add("@Size", productSearchViewModel.Size);
                    para.Add("@ShopId", productSearchViewModel.ShopId);
                    para.Add("@IsAscending", productSearchViewModel.IsAscending);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetProductSearchPagination(ProductSearchViewModel productSearchViewModel ,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", productSearchViewModel.ShopId);
                    para.Add("@IsAscending", productSearchViewModel.IsAscending);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductSearch_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
        public List<ProductDetailDO> GetProductDetailsById(int shopId, int prodId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ProductID", prodId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_ById_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
        public List<ProductDetailDO> GetProductvolumnById(int shopId, int prodRefId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ProductID", prodRefId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_AllVolumn_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetProductvolumnByIds(int shopId, int prodRefId,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ProductID", prodRefId);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_AllVolumn_Sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetProductPremiumVolById(int shopId, int prodRefId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ProductID", prodRefId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_AllPremiumVolumn_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetProductPremiumVolByIds(int shopId, int prodRefId, int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ProductID", prodRefId);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_AllPremiumVolumn_Sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
        public List<ProductDetailDO> GetRecomProductDetails(int shopId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Recom_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetRecomProductDetail(int shopId, int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Recom_Sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetRecomProductDetailsByCust(int shopId, int custId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@CustomerId", custId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_RecomByCusotmer_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public ProductCatViewDO GetFilteredStart(int shopId, float priceStart=0, float priceEnd=0)
        {
            ProductCatViewDO prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);

                    if (priceStart > 0)
                        para.Add("@StartPrice", priceStart);
                    if (priceEnd > 0)
                        para.Add("@EndPrice", priceEnd);


                    var multpOutPut = dbQuery.QueryMultiple("ProductDetails_StartFilter_Sel", param: para, commandType: CommandType.StoredProcedure);

                    prodDetail = multpOutPut.Read<ProductCatViewDO>().FirstOrDefault();
                    prodDetail.ProductCategoryDOs = multpOutPut.Read<ProductCategoryDO>().ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetFilteredProductDetails(int shopId, int[] catIds, float priceStart, float priceEnd)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    string catId = string.Join(",", catIds);
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ProductCategoryID", catId);
                    para.Add("@PriceStart", priceStart);
                    para.Add("@PriceEnd", priceEnd);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Filter_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetFilteredProductDetailsPagination(ProductSearchViewModel productSearchViewModel,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    int[] catIds = productSearchViewModel.CategoryIds;
                    string catId = string.Join(",", catIds);
                    var para = new DynamicParameters();
                    para.Add("@Index", productSearchViewModel.Index);
                    para.Add("@Size", productSearchViewModel.Size);
                    para.Add("@ShopId", productSearchViewModel.ShopId);
                    para.Add("@ProductCategoryID", catId);
                    para.Add("@PriceStart", productSearchViewModel.PriceStart);
                    para.Add("@PriceEnd", productSearchViewModel.PriceEnd);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Filter_Sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
        public List<ProductDetailDO> GetAllSearchProductDetails(int shopId, string search)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@Search", search);
                    prodDetail = dbQuery.Query<ProductDetailDO>("vwProduct_Any_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetAllSearchProductDetail(ProductSearchViewModel productSearchViewModel)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Index", productSearchViewModel.Index);
                    para.Add("@Size", productSearchViewModel.Size);
                    para.Add("@ShopId", productSearchViewModel.ShopId);
                    para.Add("@Search", productSearchViewModel.SearchText);
                    prodDetail = dbQuery.Query<ProductDetailDO>("vwProduct_Any_Sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetAllPremiumProductDetails(int? shopId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetailsAllPremium_sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetAllPremiumProductDetail(int? shopId, int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetailsAllPremium_sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }


        public List<ProductDetailDO> GetRecomPremiumProductDetails(int? shopId, int? CustomerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@CustomerId", CustomerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("PremiumProductRecomm_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetRecomPremiumProductDetail(int? shopId, int? CustomerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@CustomerId", CustomerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("PremiumProductRecomm_Sel1", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }



        public List<ProductDetailsExtDO> ProductBarcodeList(int productId, string barcodeId)
        {
            var dicProduct = new Dictionary<int, ProductDetailsExtDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", productId);
                    para.Add("@BarcodeId", barcodeId);
                    var productEx = dbQuery.Query<ProductDetailsExtDO, ProductSize, ProductBarcodeLinkDO, WineShop, ProductDetailsExtDO>("BarcodeRegistration_Sel",
                        (pd, ps, bar ,ws) =>
                        {
                            if (!dicProduct.TryGetValue(pd.ProductID, out var prod))
                            {
                                prod = pd;
                                if (bar != null)
                                {
                                    bar.Shop = ws;
                                    prod.ProductBarcodeLinks.Add(bar);
                                }
                                prod.prdSize = ps;
                                dicProduct.Add(pd.ProductID, prod);
                            }
                            else
                            {
                                prod = dicProduct[pd.ProductID];

                                if (bar != null)
                                {
                                    bar.Shop = ws;
                                    prod.ProductBarcodeLinks.Add(bar);
                                }
                            }

                            return pd; 
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "SizeID, Id,WineShopId").FirstOrDefault();
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            var objCust = dicProduct?.Values;
            return objCust.ToList();
        }
        public void ProductBarcodeUpdate(int barId, string barcodeId)
        {
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Id", barId);
                    para.Add("@BarcodeId", barcodeId);
                    para.Add("@ProductID", 0);
                    para.Add("@ShopId", 0);
                    para.Add("@Output", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                    dbQuery.Execute("ProductBarcodeLink_InsUpd",
                        param: para, commandType: CommandType.StoredProcedure);
                    //var outvalue= para.Get<int>("@Output");
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }

        public void ProductBarcodeAdd(string barcodeId, int prodId, int shopId)
        {
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Id", 0);
                    para.Add("@BarcodeId", barcodeId);
                    para.Add("@ProductID", prodId);
                    para.Add("@ShopId", shopId);
                    para.Add("@Output", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                    dbQuery.Execute("ProductBarcodeLink_InsUpd",
                        param: para, commandType: CommandType.StoredProcedure);
                    //var outvalue= para.Get<int>("@Output");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }

        public void ProductBarcodeDelete(int barId)
        {
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Id", barId);
                    para.Add("@Output", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                    dbQuery.Execute("ProductBarcodeLink_Del",
                        param: para, commandType: CommandType.StoredProcedure);
                    //var outvalue= para.Get<int>("@Output");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
        }

        public List<ProductCategoryExtDO> ProductSubCategoryDetails()
        {
            List<ProductCategoryExtDO> prodDetail = null;
            var cDictionary = new Dictionary<int, ProductCategoryExtDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    //para.Add("@ShopId", shopId);
                    prodDetail = dbQuery.Query<ProductCategoryExtDO, ProductCategoryExtDO, ProductCategoryExtDO>("ProductSubCategoryDetails_Sel",
                      (PC, PC1) =>
                      {

                          if (!cDictionary.TryGetValue(PC.ProductCategoryID, out var Cat))
                          {
                              Cat = PC;
                              Cat.SubProductCategory.Add(PC1);
                              cDictionary.Add(PC.ProductCategoryID, Cat);
                          }
                          else
                          {
                              Cat = cDictionary[PC.ProductCategoryID];
                          }
                          return PC;
                      },
                         splitOn: "SubCategoryId",
                     commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            var objCust = cDictionary?.Values;
            return objCust.ToList();
        }

        public List<ProductDetailDO> ProductItems(ProductFilter productFilter ,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    //var para = new DynamicParameters();
                    //para.Add("@ShopID", productFilter.ShopId);
                    //para.Add("@categoryId", productFilter.CategoryId);
                    //para.Add("@BrandId", productFilter.BrandId);
                    //para.Add("@Valume", productFilter.Volume);
                    //para.Add("@ProductId", productFilter.ProductId);
                    //para.Add("@StartPrice", productFilter.PriceStart);
                    //para.Add("@EndPrice", productFilter.PriceEnd);
                    ////para.Add("@customerId", customerId);
                    //prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Sel2", param: para, commandType: CommandType.StoredProcedure).ToList();
                    string json = JsonConvert.SerializeObject(productFilter.Prices);
                    var dt = JsonConvert.DeserializeObject<DataTable>(json);
                    var sqlCommand = dbQuery.CreateCommand();
                    var parameter = new SqlParameter();

                    parameter.ParameterName = "PriceRange";
                    parameter.SqlDbType = SqlDbType.Structured;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        parameter.Value = dt;
                    }
                    else
                        parameter.Value = null;
                    sqlCommand.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "ShopID";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Value = productFilter.ShopId;
                    sqlCommand.Parameters.Add(parameter);

                    if (productFilter.CategoryId != null && productFilter.CategoryId.Count() > 0)
                    {
                        parameter = new SqlParameter();
                        parameter.ParameterName = "categoryId";
                        parameter.SqlDbType = SqlDbType.VarChar;
                        parameter.Value = string.Join(",", productFilter.CategoryId);
                        sqlCommand.Parameters.Add(parameter);
                    }
                    if (productFilter.BrandId != null && productFilter.BrandId.Count() > 0)
                    {
                        parameter = new SqlParameter();
                        parameter.ParameterName = "BrandId";
                        parameter.SqlDbType = SqlDbType.VarChar;
                        parameter.Value = string.Join(",", productFilter.BrandId);
                        sqlCommand.Parameters.Add(parameter);
                    }
                    if (productFilter.Volume != null && productFilter.Volume.Count() > 0)
                    {
                        parameter = new SqlParameter();
                        parameter.ParameterName = "Volume";
                        parameter.SqlDbType = SqlDbType.VarChar;
                        parameter.Value = string.Join(",", productFilter.Volume);
                        sqlCommand.Parameters.Add(parameter);
                    }
                    if (productFilter.ProductId > 0)
                    {
                        parameter = new SqlParameter();
                        parameter.ParameterName = "ProductId";
                        parameter.SqlDbType = SqlDbType.Int;
                        parameter.Value = productFilter.ProductId;
                        sqlCommand.Parameters.Add(parameter);
                    }
                    parameter = new SqlParameter();
                    parameter.ParameterName = "CustomerId";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Value = customerId;
                    sqlCommand.Parameters.Add(parameter);

                    if (productFilter.Region != null && productFilter.Region.Count() > 0)
                    {
                        parameter = new SqlParameter();
                        parameter.ParameterName = "Region";
                        parameter.SqlDbType = SqlDbType.VarChar;
                        parameter.Value = string.Join(",", productFilter.Region);
                        sqlCommand.Parameters.Add(parameter);
                    }

                    parameter = new SqlParameter();
                    parameter.ParameterName = "IsAscending";
                    parameter.SqlDbType = SqlDbType.Bit;
                    parameter.Value = productFilter.IsAscending;
                    sqlCommand.Parameters.Add(parameter);


                    sqlCommand.CommandText = "ProductDetails_Sel2"; // Assign Sql Text
                    sqlCommand.CommandType = CommandType.StoredProcedure; // Assign CommandType
                    sqlCommand.Connection.Open(); // Explicitly open connection to use it with SqlCommand object
                    var returnValue = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection); // Execute Query
                    prodDetail = DataReaderMapToList<ProductDetailDO>(returnValue);
                }

                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            var cols = dr.GetSchemaTable().Rows.Cast<DataRow>().Select(row => row["ColumnName"] as string).ToList();            

            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (cols.Contains(prop.Name))
                    {
                        if (!object.Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }


        public List<ProductDetailDO> GetPremiumProductDetail(int index,int size, int shopId,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@index", index);
                    para.Add("@size", size);
                    para.Add("@shopId", shopId);
                    para.Add("@customerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetails_Premium_ShowCase_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> GetProductDetailbyPurchase( int[] ProductId,int shopId,int customerId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    int[] prdIds = ProductId;
                    string prdId = string.Join(",", prdIds);
                    var para = new DynamicParameters();
                    
                    para.Add("@ProductId", prdId);
                    para.Add("@ShopId", shopId);
                    para.Add("@CustomerId", customerId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("ProductDetailsByPurchase_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<ProductDetailDO> CheckMixerInventory(int shopId,int productId)
        {
            List<ProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                   
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ProductId", productId);
                    prodDetail = dbQuery.Query<ProductDetailDO>("InventoryCheck", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<MixerProductDetailDO> MixerProductDetail(int cutomerId ,int mixerId, int shopId)
        {

            List<MixerProductDetailDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", cutomerId);
                    para.Add("@MixerId", mixerId);
                    para.Add("@ShopId", shopId);
                    prodDetail = dbQuery.Query<MixerProductDetailDO>("MixerProductDetail_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

        }

        public List<ProductCompetitorLinkDO> GetCompetitorProductDetail(int id)
        {

            List<ProductCompetitorLinkDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Id", id);
                    prodDetail = dbQuery.Query<ProductCompetitorLinkDO>("ProductCompetitorLink_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

        }

        public List<ProductCompetitorLinkDO> GetCompetitorProductDetailByProductName(int id)
        {

            List<ProductCompetitorLinkDO> prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Id", id);
                    prodDetail = dbQuery.Query<ProductCompetitorLinkDO>("ProductCompetitorLinkByProductname_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;

        }

        public int AddAndUpdateCompetitorProduct(int id, string competPrudts,string email,string action)
        {
            var result = default(int);
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Id", id);
                    para.Add("@CompetPrudts", competPrudts);
                    para.Add("@Email",email);
                    para.Add("@Action",action);
                    result = dbQuery.Query<int>("ProductCompetitorLink_InsUpd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
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