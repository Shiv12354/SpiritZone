using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZModels;

namespace SZData.Interfaces
{
    public interface IPromoCodeService:IDisposable
    {
        IList<SZPromoCode> PromoCode();
        bool ValidatePromoCodeByProduct(int productId);
        bool ValidatePromoCodeByShop(int shopId);
        IList<SZPromoType> PromoType();
        PromoApplyOutput PromoCodeApply(string promoCode,float totalAmount, string userId);

        object PromoCodeCalculation(string promoCode, int totalAmount,int productId,int qty,int price);

        ReferralCode ValidCode(string code);
        ReferralCode UniqueReferralCode(string userId);

        string SignReferralCal(string code,string userId);

        ReferralCode IsSignUpCode(string code);

        string ReferralTypeCodeCal(string Code, string userId);

        string ReferralCal(string code,string userId);

        BalanceAndExpiryAmount BalanceAndExpiryAmount(string userId);

        IList<TransactionHistory> TransactionHistory(string userId);

        CashBackModel GetDiscountOnFirstOrder(string userId);

        int UsesTransactionAmount(string userId, decimal totalAmount, int orderId);
        CashBackModel CashBack(int orderId);

        CashBackModel CheckAmountAfterWalletUse(int orderId, decimal totalAmount);

        int UpdateWalletAmountForOrder(string userId, int orderId);

        int UpdatePromoIdForOrder(int promoId, int orderId, float discountAmount);

        int PromoCodeCashBack(int orderId);

        int WhenUseWalletAmountOnly(string orderId);

        int UnRegisterUser(string mobileNumber);

        int WalletNotify(string userId);
        PromoPopups PromoPopupDetails(string userId);
        int PromoPopupUserBindings(string userId, int promoPopId);
    }
}
