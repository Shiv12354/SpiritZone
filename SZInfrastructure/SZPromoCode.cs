using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;

namespace SZInfrastructure
{
    public class SZPromoCode : ISZPromoCode
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
                    _promo.Dispose();
                    _promo = null;
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
        IPromoCodeService _promo;
        public SZPromoCode(IPromoCodeService promo)
        {
            _promo = promo;
        }
        public IList<SZPromoCode> CheckPromoCode()
        {
            InMemoryCache cache = new InMemoryCache();
            var pro = cache.GetOrSet<SZPromoCode>("CacheConfig", () => (IList<SZPromoCode>)_promo.PromoCode());
            return pro;
        }

        public bool ValidatePromoCodeByProduct(int productId)
        {
            bool isProduct = false;
            //InMemoryCache cache = new InMemoryCache();
            //isProduct = cache.GetOrSet<bool>("CacheConfig", () =>Convert.ToBoolean( _promo.ValidatePromoCodeByProduct(productId)));
            return isProduct;
        }

        public bool ValidatePromoCodeByShop(int shopId)
        {
            bool isShop = false;
            //InMemoryCache cache = new InMemoryCache();
            //isShop = cache.GetOrSet<bool>("CacheConfig", () => _promo.ValidatePromoCodeByShop(shopId));
            return isShop;
        }
    }
    public interface ISZPromoCode:IDisposable
    {
        IList<SZPromoCode> CheckPromoCode();
        bool ValidatePromoCodeByProduct(int productId);
        bool ValidatePromoCodeByShop(int shopId);

    }
}
