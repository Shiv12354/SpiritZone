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
    public class PromoCodeRepo:BaseRepo,IPromoCodeRepo
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

        public IList<SZPromoCode> PromoCode()
        {
            IList<SZPromoCode> Promo = null;
            using (var db = new SqlConnection(ConnectionText))
            {
                
                Promo = db.Query<SZPromoCode>(SZStoredProcedures.PromoCodeSel).ToList();
                
            }
            return Promo;
        }
        public bool ValidatePromoCodeByProduct(int productId)
        {
            bool config = false;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.productId, productId);
                var ot = db.Query<int>(SZStoredProcedures.PromoCodeByProduct, param: param, commandType: CommandType.StoredProcedure);
                if (ot.Contains(1))
                {
                    config = true;
                }
               
            }
            return config;
        }
        public bool ValidatePromoCodeByShop(int shopId)
        {
            bool config = false;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.shopId, shopId);
                var ot =db.Query<int>(SZStoredProcedures.PromoCodeByShop, param: param, commandType: CommandType.StoredProcedure);

                if (ot.Contains(1) )
                {
                    config = true;
                }
            }
            return config;
        }

        public IList<SZPromoType> PromoType()
        {
            IList<SZPromoType> PromoType = null;
            using (var db = new SqlConnection(ConnectionText))
            {

                PromoType = db.Query<SZPromoType>(SZStoredProcedures.PromoTypeSel).ToList();

            }
            return PromoType;
        }

        public PromoApplyOutput PromoCodeApply(string promoCode, float totalAmount, string userId)
        {

            PromoApplyOutput promo = new PromoApplyOutput();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.PromoCode, promoCode);
                param.Add(SZParameters.TotalAmount, totalAmount);
                param.Add(SZParameters.UserId, userId);
                promo = db.Query<PromoApplyOutput>(SZStoredProcedures.PromoCodeApply, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                
            }
            return promo;
        }

        public object PromoCodeCalculation(string promoCode, int totalAmount, int productId, int qty, int price)
        {
            object promo;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.PromoCode, promoCode);
                param.Add(SZParameters.TotalAmount, totalAmount);
                param.Add(SZParameters.productId, productId);
                param.Add(SZParameters.Qty, qty);
                param.Add(SZParameters.Price, price);
                promo = db.Query(SZStoredProcedures.PromoCodeCal, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return promo;
        }

        public ReferralCode ValidCode(string code)
        {
            ReferralCode referralCode = new ReferralCode();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.PromoCode, code);
                referralCode = db.Query<ReferralCode>(SZStoredProcedures.CheckPromoCode, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return referralCode;
        }

        public ReferralCode UniqueReferralCode(string userId)
        {
            ReferralCode referralCode = new ReferralCode();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                referralCode = db.Query<ReferralCode>(SZStoredProcedures.GenerateUniqueReferralCode, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return referralCode;
        }

        public string SignReferralCal(string code,string userId)
        {
            string str = string.Empty;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.Code, code);
                param.Add(SZParameters.NewUserId, userId);
                str = db.Query<string>(SZStoredProcedures.SignUpReferralCal, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return str;
        }

        public ReferralCode IsSignUpCode(string code)
        {
            ReferralCode referralCode = new ReferralCode();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.Code, code);
                referralCode = db.Query<ReferralCode>(SZStoredProcedures.ReferralTypeCodeCheck, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return referralCode;
        }

        public string ReferralTypeCodeCal(string code,string userId)
        {
            string str = string.Empty;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.Code, code);
                param.Add(SZParameters.NewUserId, userId);
                str = db.Query<string>(SZStoredProcedures.ReferralTypeCodeCal, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return str;
        }

        public string ReferralCal(string code,string userId)
        {
            string str = string.Empty;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.Code, code);
                param.Add(SZParameters.NewUserId, userId);
                str = db.Query<string>(SZStoredProcedures.ReferralCodeCal, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return str;
        }

        public BalanceAndExpiryAmount BalanceAndExpiryAmount(string userId)
        {
            BalanceAndExpiryAmount balanceAndExpiryAmount = new BalanceAndExpiryAmount();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                var results = db.QueryMultiple("BalancedAnsExpireAmount",
                     param: param,
                     commandType: CommandType.StoredProcedure);
                var wallet = results.Read<WalletDO>().ToList();
                var walletOrd = results.Read<WalletOrderDO>().ToList();
                var isRefer = results.Read<OrderExt>().ToList();

                balanceAndExpiryAmount.Wallet = wallet;
                balanceAndExpiryAmount.WalletOrder = walletOrd;
                balanceAndExpiryAmount.OrderExt = isRefer;

            }
            return balanceAndExpiryAmount;
        }

        public IList<TransactionHistory> TransactionHistory(string userId)
        {
            IList<TransactionHistory> transactionHistory = null;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                transactionHistory = db.Query<TransactionHistory>(SZStoredProcedures.TransactionHistorySel, param: param, commandType: CommandType.StoredProcedure).ToList();

            }
            return transactionHistory;
        }

        public CashBackModel GetDiscountOnFirstOrder(string userId)
        {
            CashBackModel cashBackModel = new CashBackModel();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                cashBackModel = db.Query<CashBackModel>(SZStoredProcedures.FirstOrderCredit, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return cashBackModel;
        }

        public int UsesTransactionAmount(string userId,decimal totalAmount,int orderId)
        {
           
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                param.Add(SZParameters.TotalAmount, totalAmount);
                param.Add(SZParameters.OrderId, orderId);
                int str = db.Query<int>(SZStoredProcedures.UsesTransactionAmount, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return str;

            }
            
        }

        public CashBackModel CashBack(int orderId)
        {
            CashBackModel cashBackModel = new CashBackModel();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.OrderId, orderId);
                cashBackModel = db.Query<CashBackModel>(SZStoredProcedures.CashBackSel, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return cashBackModel;

        }
        public CashBackModel CheckAmountAfterWalletUse(int userId, decimal totalAmount)
        {
            CashBackModel cashBackModel = new CashBackModel();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                param.Add(SZParameters.TotalAmount, totalAmount);
                cashBackModel = db.Query<CashBackModel>(SZStoredProcedures.CheckAmountAfterUseWallet, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return cashBackModel;
        }

        public int UpdateWalletAmountForOrder(string userId, int orderId)
        {

            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                param.Add(SZParameters.OrderId, orderId);
                int str = db.Query<int>(SZStoredProcedures.UpdateWalletAmountOrder, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return str;

            }

        }

        public int UpdatePromoIdForOrder(int promoId, int orderId, float discountAmount)
        {

            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.PromoId, promoId);
                param.Add(SZParameters.OrderId, orderId);
                param.Add(SZParameters.DiscountAmount, discountAmount);
                int str = db.Query<int>(SZStoredProcedures.UpdatePromoIdInOrder, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return str;

            }

        }

        public int PromoCodeCashBack(int orderId)
        {

            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.OrderId, orderId);
                int str = db.Query<int>(SZStoredProcedures.PromoCashbackSel, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return str;

            }

        }

        public int WhenUseWalletAmountOnly(string orderId)
        {

            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.OrderId, orderId);
                int str = db.Query<int>(SZStoredProcedures.WhenUseAllAmountFromWalletSel, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return str;

            }

        }

        public int UnRegisterUser(string mobileNumber)
        {

            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.MobileNumber, mobileNumber);
                int str = db.Query<int>(SZStoredProcedures.UnRegisterUser, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return str;

            }

        }

        public int WalletNotify(string userId)
        {

            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                int str = db.Query<int>(SZStoredProcedures.UpdateWalletNofify, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return str;

            }

        }

        public PromoPopups PromoPopupDetails(string userId)
        {
          PromoPopups promoPopups =new PromoPopups();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                promoPopups = db.Query<PromoPopups>(SZStoredProcedures.PopupsDetailsSel, param: param, commandType: CommandType.StoredProcedure).LastOrDefault();
                return promoPopups;

            }

        }

        public int PromoPopupUserBindings(string userId,int promoPopId)
        {
            
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                param.Add(SZParameters.PromoPopId, promoPopId);
                int str = db.Query<int>(SZStoredProcedures.PromoPopupUserBindingsInsUpd, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return  str;

            }

        }
    }
}
