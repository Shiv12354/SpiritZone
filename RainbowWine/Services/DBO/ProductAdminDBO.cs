using Dapper;
using RainbowWine.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DBO
{
    public class ProductAdminDBO
    {
        public IList<ProductAdmin> ProductList(int productId)
        {
            IList<ProductAdmin> productAdminDBOs = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", productId);
                    productAdminDBOs = dbQuery.Query<ProductAdmin>("ProductAdmin_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return productAdminDBOs;
        }
        public void UpdateProduct(ProductAdmin input)
        {
            //int outvalue = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", input.ProductID);
                    para.Add("@ProductName", input.ProductName);
                    para.Add("@PackingSize", input.PackingSize);
                    para.Add("@Price", input.Price);
                    para.Add("@Category", input.Category);
                    para.Add("@ProductSizeId", input.ProductSizeId);
                    para.Add("@IsDelete", input.IsDelete);
                    para.Add("@IsFeature", input.IsFeature);
                    para.Add("@IsShowcaseView", input.IsShowcaseView);
                    para.Add("@IsShowcasePremiumView",input.IsShowcasePremiumView);
                    para.Add("@IsPremium",input.IsPremium);
                    para.Add("@Barcode", input.Barcode);
                    para.Add("@ProductImage", input.ProductImage);
                    para.Add("@ProductThumbImage", input.ProductThumbImage);
                    para.Add("@User", input.UserName);
                    //para.Add("@Output", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                    dbQuery.Execute("ProductAdmin_InsUpd",
                        param: para, commandType: CommandType.StoredProcedure);
                     //outvalue = para.Get<int>("@Output");
                   
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
                //return outvalue;
            }
        }

        public void AddProduct(ProductAdmin input)
        {
            //int outvalue = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", 0);
                    para.Add("@ProductName", input.ProductName);
                    para.Add("@PackingSize", input.PackingSize);
                    para.Add("@Price", input.Price);
                    para.Add("@Category", input.Category);
                    para.Add("@ProductSizeId", input.ProductSizeId);
                    para.Add("@IsDelete", input.IsDelete);
                    para.Add("@IsFeature", input.IsFeature);
                    para.Add("@IsShowcaseView", input.IsShowcaseView);
                    para.Add("@IsShowcasePremiumView", input.IsShowcasePremiumView);
                    para.Add("@IsPremium", input.IsPremium);
                    para.Add("@ProductImage", input.ProductImage);
                    para.Add("@ProductThumbImage", input.ProductThumbImage);
                    para.Add("@Barcode", input.Barcode);
                    para.Add("@User", input.UserName);
                    //para.Add("@Output", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                    dbQuery.Execute("ProductAdmin_InsUpd",
                        param: para, commandType: CommandType.StoredProcedure);
                     //outvalue = para.Get<int>("@Output");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
                //return outvalue;
            }
        }

        public int DeleteProduct(int productId)
        {
            int outvalue = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", productId);
                    para.Add("@Output", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                    dbQuery.Execute("ProductAdmin_Del",
                        param: para, commandType: CommandType.StoredProcedure);
                     outvalue = para.Get<int>("@Output");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
                return outvalue;
            }
        }

        public ProductAdmin ProductDetails(int productId)
        {
           ProductAdmin productAdminDBOs = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ProductId", productId);
                    productAdminDBOs = dbQuery.Query<ProductAdmin>("ProductAdmin_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return productAdminDBOs;
        }

        public IList<ProductAdminSize> ProductSizeList()
        {
            IList<ProductAdminSize> productAdminSize = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    productAdminSize = dbQuery.Query<ProductAdminSize>("ProductAdmin_ProductSize_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return productAdminSize;
        }

        public IList<ProductCategories> ProductCategoryList()
        {
            IList<ProductCategories> productAdminCat = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    productAdminCat = dbQuery.Query<ProductCategories>("ProductAdminCategory_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return productAdminCat;
        }
    }
}