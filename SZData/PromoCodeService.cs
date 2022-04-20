using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;
using SZModels;

namespace SZData
{
    public class PromoCodeService : IPromoCodeService
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

        IPromoCodeRepo _repo;
        public PromoCodeService(IPromoCodeRepo repo)
        {
            _repo = repo;
        }
        public IList<SZPromoCode> PromoCode()
        {
            return _repo.PromoCode();
        }

        public bool ValidatePromoCodeByProduct(int productId)
        {
            return _repo.ValidatePromoCodeByProduct(productId);
        }

        public bool ValidatePromoCodeByShop(int shopId)
        {
            return _repo.ValidatePromoCodeByShop(shopId);
        }

        public IList<SZPromoType> PromoType()
        {
            return _repo.PromoType();
        }

        public PromoApplyOutput PromoCodeApply(string promoCode, float totalAmount, string userId)
        {
            return _repo.PromoCodeApply(promoCode, totalAmount,userId);
        }

        public  object PromoCodeCalculation(string promoCode, int totalAmount, int productId, int qty, int price)
        {
            return _repo.PromoCodeCalculation(promoCode, totalAmount, productId, qty, price);
        }
        public ReferralCode ValidCode(string code)
        {
            return _repo.ValidCode(code);
        }

        public ReferralCode UniqueReferralCode(string userId)
        {
            return _repo.UniqueReferralCode(userId);
        }

        public string SignReferralCal(string code,string userId)
        {
            return _repo.SignReferralCal(code,userId);
        }

        public ReferralCode IsSignUpCode(string code)
        {
            return _repo.IsSignUpCode(code);
        }

        public string ReferralTypeCodeCal(string code,string userId)
        {
            return _repo.ReferralTypeCodeCal(code,userId);
        }

        public string ReferralCal(string code,string userId)
        {
            return _repo.ReferralCal(code,userId);
        }

        public BalanceAndExpiryAmount BalanceAndExpiryAmount(string userId)
        {
            return _repo.BalanceAndExpiryAmount(userId);
        }

        public IList<TransactionHistory> TransactionHistory(string userId)
        {
            return _repo.TransactionHistory(userId);
        }

        public CashBackModel GetDiscountOnFirstOrder(string userId)
        {
            return _repo.GetDiscountOnFirstOrder(userId);
        }

        public int UsesTransactionAmount(string userId, decimal totalAmount, int orderId)
        {
            return _repo.UsesTransactionAmount(userId,totalAmount,orderId);
        }

        public CashBackModel CashBack(int orderId)
        {
            return _repo.CashBack(orderId);
        }

       public CashBackModel CheckAmountAfterWalletUse(int orderId, decimal totalAmount)
        {
            return _repo.CheckAmountAfterWalletUse(orderId,totalAmount);
        }

        public int UpdateWalletAmountForOrder(string userId, int orderId)
        {
            return _repo.UpdateWalletAmountForOrder(userId, orderId);
        }

        public int UpdatePromoIdForOrder(int promoId, int orderId, float discountAmount)
        {
            return _repo.UpdatePromoIdForOrder(promoId, orderId,discountAmount);
        }

        public int PromoCodeCashBack(int orderId)
        {
            return _repo.PromoCodeCashBack(orderId);
        }

        public int WhenUseWalletAmountOnly(string orderId)
        {
            return _repo.WhenUseWalletAmountOnly(orderId);
        }

        public int UnRegisterUser(string mobileNumber)
        {
            return _repo.UnRegisterUser(mobileNumber);
        }

        public int WalletNotify(string userId)
        {
            return _repo.WalletNotify(userId);
        }

        public PromoPopups PromoPopupDetails(string userId)
        {
            return _repo.PromoPopupDetails(userId);
        }

        public int PromoPopupUserBindings(string userId ,int promoPopId)
        {
            return _repo.PromoPopupUserBindings(userId,promoPopId);
        }
    }
}
