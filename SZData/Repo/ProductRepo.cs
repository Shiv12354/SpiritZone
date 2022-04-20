using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;
using SZModels;

namespace SZData.Repo
{
    public class ProductRepo : BaseRepo, IProductRepo
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~QonConfigRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion
        public ProductRefDataModel GetProductRefData(int[] ProductIdForPurchase, int[] ProductIdForLocation, int shopId, int customerId)
        {
            ProductRefDataModel result = new ProductRefDataModel();
            using (var db = new SqlConnection(ConnectionText))
            {
                int[] prdIdsPurchase = ProductIdForPurchase;
                string prdId = string.Join(",", prdIdsPurchase);

                int[] prdIdsLocation = ProductIdForLocation;
                string prdIdLocation = string.Join(",", prdIdsLocation);

                var param = new DynamicParameters();
                param.Add(SZParameters.productIdForPurchase, prdId);
                param.Add(SZParameters.shopId, shopId);
                param.Add(SZParameters.CustomerId, customerId);
                param.Add(SZParameters.productIdForLocation, prdIdLocation);
                
                var results = db.QueryMultiple(SZStoredProcedures.ProductRefData,
                     param: param,
                     commandType: CommandType.StoredProcedure);
                var purchaseProduct = results.Read<ProductDTO>().ToList();
                var locationProduct = results.Read<ProductDTO>().ToList();
                var recommendedProduct = results.Read<ProductDTO>().ToList();
                var preimuimProducts = results.Read<ProductDTO>().ToList();
                var mixer = results.Read<MixerProductDTO>().ToList();
                var brands = results.Read<ProductBrandDTO>().ToList();
                var category = results.Read<ProductCategoryDTO>().ToList();

                result.ProductByPurchase = purchaseProduct;
                result.ProductByLocation = locationProduct;
                result.RecommendedProducts = recommendedProduct;
                result.RecommendedPremiumProducts = preimuimProducts;
                result.MixerProducts = mixer;
                result.ProductBrands = brands;
                result.ProductCategories = category;
                if(result.MixerProducts != null && result.MixerProducts.Count > 0)
                {
                    for(int i =0; i< result.MixerProducts.Count; i++)
                    {
                        result.MixerProducts[i].MixerDetail = new MixerDetail
                        {
                            MixerDetailId = result.MixerProducts[i].MixerDetailId,
                            MixerId = result.MixerProducts[i].MixerId,
                            MixerSizeId = result.MixerProducts[i].MixerSizeId,
                            Price = result.MixerProducts[i].Price,
                            Capacity = result.MixerProducts[i].Capacity,
                            Qty = result.MixerProducts[i].Qty,
                            SupplierId = result.MixerProducts[i].SupplierId,
                            IsMixer = result.MixerProducts[i].IsMixer,
                        };
                    }
                }
            }
            return result;
        }

        public List<ProductDTO> GetProductDetailByPromoCode(int shopId, int customerId, string promoCode)
        {
            List<ProductDTO> products = new List<ProductDTO>();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.shopId, shopId);
                param.Add(SZParameters.CustomerId, customerId);
                param.Add(SZParameters.PromoCode, promoCode);
                var output = db.Query<ProductDTO>(SZStoredProcedures.ProductByPromoCode, param: param, commandType: CommandType.StoredProcedure).ToList();
                products = output;



            }
            return products;

        }

    }
}
