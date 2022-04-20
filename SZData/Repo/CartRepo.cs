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
    public class CartRepo : BaseRepo, ICartRepo
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

        public CartItemModel GetCartData(int cartId, int shopId, int customerId)
        {
            CartItemModel result = new CartItemModel();
            using (var db = new SqlConnection(ConnectionText))
            {
                try
                {
                    var param = new DynamicParameters();
                    param.Add(SZParameters.CartId, cartId);
                    param.Add(SZParameters.shopId, shopId);
                    param.Add(SZParameters.CustomerId, customerId);

                    var results = db.QueryMultiple(SZStoredProcedures.CartDataAndPaymentType,
                         param: param,
                         commandType: CommandType.StoredProcedure);

                    var cartItem = results.Read<CartItemDetail>().ToList();
                    var opdByCust = results.Read<CustomerExt>().FirstOrDefault();
                    var opdByShop = results.Read<WineShopExt>().FirstOrDefault();
                    var refBal = results.Read<ReferBalance>().FirstOrDefault();
                    var paymentType = results.Read<PaymentTypeDTO>().ToList();


                    result.cartItems = cartItem;
                    result.customerExt = opdByCust != null ? opdByCust : new CustomerExt();
                    result.customerExt.WineShop = opdByShop;
                    result.customerExt.ReferBalance = refBal;
                    result.paymentTypes = paymentType;
                }
                finally
                {
                    db.Close();
                }
            }
            return result;
        }

    }
}
