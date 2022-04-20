using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;
using SZModels;

namespace SZData
{
    public class ProductService : IProductService
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
                    _repo.Dispose();
                    _repo = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion

        IProductRepo _repo;
        public ProductService(IProductRepo product)
        {
            _repo = product;
        }
        public ProductRefDataModel GetProductRefData(int[] ProductIdForPurchase, int[] ProductIdForLocation, int shopId, int customerId)
        {
            return _repo.GetProductRefData(ProductIdForPurchase, ProductIdForLocation, shopId, customerId);
        }

        public List<ProductDTO> GetProductDetailByPromoCode(int shopId, int customerId, string promoCode)
        {
            return _repo.GetProductDetailByPromoCode(shopId, customerId, promoCode);
        }
    }
}
