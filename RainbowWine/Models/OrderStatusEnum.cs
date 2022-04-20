using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public enum OrderStatusEnum
    {
        Pending = 1,
        Submitted = 2,
        Approved = 3,
        Rejected = 4,
        Delivered = 5,
        Packed = 6,
        OutForDelivery = 9,
        Assigned = 14,
        PayLinkSend = 15,
        Cancel = 16,
        PayTmLinkSend = 17,
        PayTmBulkSend = 21,
        RefundInitiated = 22,
        Refunded = 23,
        Reassigned = 24,
        RefundMax = 25,
        RefundFailed = 26,
        BackToStore = 27,
        PartialRefund = 28,
        IssueInitiated = 29,
        IssueProcessed = 30,
        IssueApproved = 31,
        CashLinkSend = 35,
        IssueRefundInitiated = 36,
        IssueRefunded = 37,
        IssueRefundFailed = 38,
        IssuePaymentInitiated = 39,
        DeliveryReached = 40,
        PODPaymentFail = 41,
        PODPaymentSuccess = 42,
        PODCashSelected = 43,
        PODCashPaid = 44,
        CustGeneratePin = 45,
        DelPinVerified = 46,
        PODOnlineSelected = 47,
        CancelByCustomer = 48,
        DelCancelByCustomer = 49,
        PODOrderCancel = 50,
        OrderModifyProcess = 51,
        OrderModifyApproved = 52,
        OrderModifyRefundInitiated = 53,
        OrderModifyRefunded = 54,
        OrderModifyRefundFailed = 55,
        OrderModifyPaymentInitiated = 56,
        CashFreePayLinkSmsResent = 57,
        CashFreePayLinkEmailResent = 58,
        OCODCashPaid = 59,
        DelPinGenerated = 60,
        ShopPinVerified = 61,
        CashPartialPaymentSelected = 62,
        CashPartialRefundSelected = 63,
        CashPartialPaymentCollected = 64,
        CashPartialRefundMade = 65,
        EmailedToSupplier = 66 ,
        WalletPaymentRefundSelected = 67,
        OrderPickedUp = 71,
        WalletRefundSuccessful = 72,
        WalletRefundFailed = 73 ,
        WalletRefundSelected = 74 ,
    }

    public enum OrderPaymentType
    {
        POO = 1, //Pay On Order
        POD = 2, //Pay On Delivery
        COD = 3, //Cash On Delivery
        OOD = 4, //Online On Delivery
        OCOD = 5  //Order Cash On Delivery
    }
    public enum DiscountType
    {
        ProductItem = 1,
        OverAll = 2
    }
    public enum IssueType
    {
        Open = 1,
        PartialPay = 2,
        PartialRefund = 3,
        FullRefund = 4,
        BackToOrderList = 5,
        Closed = 6,
        Approved = 7,
        Settlement = 8,
        Cancel=9  ,
        WalletRefund= 10,
        WalletPartialRefund =11
    }

    public enum PageNameEnum
    {
        CART=1,
        MIXERALCOHOL=2,
        MYWALLET =3 ,
        GIFT = 4
    }

    public enum PageContentEnum
    {
        Text1 , 
        Text2 , 
        Text3 , 
        Text4 , 
        Text5 , 
        Text6 , 
        Text7 , 
        Text8 , 
        Text9 , 
        Text10 ,
        Text11 ,
        Text12 ,
        Text13 ,
        Text14 ,
        Text15 ,
        Text16 ,
        Text17 ,
        Text18 ,
        Text19 ,
        Text20 ,
        Text21 ,
        Text22 ,
        Text23 ,
        Text24 ,
        Text25 ,
        Text26 ,
        Text27 ,
        Text28 ,
        Text29 ,
        Text30 ,
        Text31 ,
        Text32 ,
        Text33 ,
        Text34 ,
        Text35 ,
        Text36 ,
        Text37 ,
        Text38 ,
        Text39 ,
        Text40 ,
        Text41 ,
        Text42 ,
        Text43 ,
        Text44 ,
        Text45 ,
        Text46 ,
        Text47 ,
        Text48 ,
        Text49 ,
        Text50 ,
        Text51 ,
        Text52 ,
        Text53 ,
        Text54 ,
        Text55 ,
        Text56 ,
        Text57 ,
        Text58 ,
        Text59 ,
        Text60 ,
        Text61 ,
        Text62 ,
        Text63 ,
        Text64 ,
        Text65 ,
        Text66 ,
        Text67 ,
        Text68 ,
        Text69,
        Text70
    }

    public enum PageImageEnum
    {
        Image1=1,
        Image2=2,
        Image3=3,
        Image4=4,
        Image5=5 ,
        BirthdayImg =15,
        AnniversaryImg = 16 ,
        WeddingImg =17 ,
        DefaultImg = 18
    }

    public enum PageButtonsEnum
    {
        Button1 = 1,
        Button2 = 2,
        Button3 = 3
    }

    public enum MixerType
    {
        beverage,
        giftwrap
    }

    public enum InventoryTrackEnum
    {
        OrderPlaced
    }

    public enum PromoTypeEnum
    {
        UniquePromoCode = 1,
        GroupPromoCode = 2,
        FlatPromoCode = 3
    }

}