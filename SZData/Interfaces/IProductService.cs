using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZModels;

namespace SZData.Interfaces
{
    public interface IProductService : IDisposable
    {
        ProductRefDataModel GetProductRefData(int[] ProductIdForPurchase, int[] ProductIdForLocation, int shopId, int customerId);

        List<ProductDTO> GetProductDetailByPromoCode(int shopId, int customerId, string promoCode);
    }
}
