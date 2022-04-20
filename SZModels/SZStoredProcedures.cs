using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class SZStoredProcedures
    {
        public static readonly string ConfigMasterByKeySel = "ConfigMaster_ByKey_Sel";
        public static readonly string ConfigMasterSel = "ConfigMaster_Sel";

        public static readonly string PageSel = "Page_Sel";

        public static readonly string PromoCodeSel = "PromoCode_Sel";
        public static readonly string PromoCodeByProduct = "PromoCodeBy_Product";
        public static readonly string PromoCodeByShop = "PromoCodeBy_Shop";
        public static readonly string PromoTypeSel = "PromoType_Sel";
        public static readonly string PromoCodeApply = "PromoCode_Apply";
        public static readonly string CheckPromoCode = "CheckPromoCode"; 
        public static readonly string PromoCodeCal = "PromoCode_Cal";
        public static readonly string GenerateUniqueReferralCode = "GenerateUniqueReferralCode";
        public static readonly string SignUpReferralCal = "SignUpReferral_Cal";
        public static readonly string ReferralTypeCodeCheck = "ReferralTypeCode_Check";
        public static readonly string ReferralCodeCal = "ReferralCode_Cal";
        public static readonly string ReferralTypeCodeCal = "ReferralTypeCodeCal";
        public static readonly string TransactionHistorySel = "TransactionHistory_Sel";
        public static readonly string FirstOrderCredit = "FirstOrderCredit";
        public static readonly string UsesTransactionAmount = "UsesTransactionAmount";
        public static readonly string CashBackSel = "CashBack_Sel";
        public static readonly string CheckAmountAfterUseWallet = "CheckAmountAfterUseWallet";
        public static readonly string UpdateWalletAmountOrder = "UpdateWalletAmount_Order";
        public static readonly string UpdatePromoIdInOrder = "UpdatePromoIdInOrder";
        public static readonly string PromoCashbackSel = "PromoCashback_Sel";
        public static readonly string WhenUseAllAmountFromWalletSel = "WhenUseAllAmountFromWallet_Sel";
        public static readonly string UnRegisterUser = "UnRegister_User";
        public static readonly string UpdateWalletNofify = "UpdateWalletNofify";
        public static readonly string PopupsDetailsSel = "PopupsDetails_Sel";
        public static readonly string PromoPopupUserBindingsInsUpd = "PromoPopupUserBindings_Ins_Upd";
        
        public static readonly string DeliveryEarningsTotal = "DeliveryEarnings_Total";
        public static readonly string DeliveryEarningsHistory = "DeliveryEarnings_History";
        public static readonly string DeliveryEarningIns = "DeliveryEarning_Ins";
        public static readonly string NewDeliveryEarningIns = "NewDeliveryEarning_Ins";
        public static readonly string ProductRefData = "SP_ProductRefData_Sel";
        public static readonly string CartDataAndPaymentType = "CartDataAndPaymentType_Sel";
        public static readonly string ProductByPromoCode = "ProductBy_PromoCode_Sel";

        public static readonly string DeliveryEarningsPenaltyTotal = "DeliveryEarnings_Penalty_Total";
        public static readonly string DeliveryEarningsHistoryWith_Penalty = "DeliveryEarnings_History_With_Penalty";
        public static readonly string DeliveryBoy_Rating_Incentive_Ins = "OrderRating_Incentive_Ins";


    }
}
