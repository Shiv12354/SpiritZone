using Dapper;
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
    public class FiltersDBO
    {
        public FiltersDO Filters()
        {

            FiltersDO filtersDO = new FiltersDO();
            var cDictionary = new Dictionary<int, ProductCategoryExtDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    var results = dbQuery.QueryMultiple("Filters",
                    commandType: CommandType.StoredProcedure);

                    var pricerange = results.Read<PricerangeExtDO>().ToList();
                    var category = results.Read<ProductCategoryExtDO, ProductCategoryExtDO, ProductCategoryExtDO>(
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
                               Cat.SubProductCategory.Add(PC1);
                           }
                           return PC;
                       },
                         splitOn: "SubCategoryId"
                        ).ToList();

                    var brand = results.Read<ProductBrandExtDO>().ToList();
                    var capacity = results.Read<ProductSizeExtDO>().ToList();
                    var region = results.Read<RegionExtDO>().ToList();

                    var dic = cDictionary.Values;
                    var catValue = dic.ToList();
                    filtersDO.PricerangeExtDO = pricerange;
                    filtersDO.ProductCategoryExtDO = catValue;
                    filtersDO.ProductBrandExtDO = brand;
                    filtersDO.ProductSizeExtDO = capacity;
                    filtersDO.RegionExtDO = region;
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return filtersDO;
        }
    }
}