using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class CartItemModel
    {
        public List<CartItemDetail> cartItems { get; set; }
        public CustomerExt customerExt { get; set; }
        public List<PaymentTypeDTO> paymentTypes { get; set; }
    }
    public class CartContentResponse
    {
        public List<CartItemDetail> cartItems { get; set; }
        public CartContent CartContent { get; set; }
    }

    public class CartItemDetail
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public Double UnitPrice { get; set; }
        public int Qty { get; set; }
        public double SubTotal { get; set; }
        public string Text { get; set; }
        public bool IsMixer { get; set; }
        public int ProductRefID { get; set; }
        public int AvailableQty { get; set; }
        public string Capacity { get; set; }
        public int Current_Price { get; set; }
        public bool IsReserve { get; set; }
        public int SupplierId { get; set; }
        public string MixerType { get; set; }
        public int MixerRefId { get; set; }
    }
    public class CustomerExt
    {
        public int CustomerId { get; set; }
        public WineShopExt WineShop { get; set; }
        public ReferBalance ReferBalance { get; set; }
        public bool IsPOD { get; set; }
    }

    public class WineShopExt
    {
        public int ShopId { get; set; }
        public bool IsPOD { get; set; }
    }

    public class ReferBalance
    {
        public Decimal Balance { get; set; }
    }
    public class PaymentTypeDTO
    {
        public int PaymentTypeId { get; set; }
        public string PayType { get; set; }
        public string Description { get; set; }
    }
    public class CartContent
    {
        public List<PaymentTypeResponse> PaymentType { get; set; }
        public BillDetails BillDetails { get; set; }
        public bool IsOPD { get; set; }
        public bool IsMixerPOD { get; set; }
        public string UPIEnable { get; set; }
        public int MiniumOrder { get; set; }
        public string MiniumOrderMessage { get; set; }
        public int MaxiumPodAmount { get; set; }
        public string DeliveryMessage { get; set; }
        public string LeftInStkMsg { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public decimal Balance { get; set; }
        public int SZCreditsCanUse { get; set; }
        public bool IsWalletForPOD { get; set; }
        public bool CanWeUSeWalletForOrder { get; set; }
        public string PromoAppliedChangeLineItem { get; set; }
    }
    public class BillItem
    {
        public string Text { get; set; }
        public string Default { get; set; }
        public string Key { get; set; }
    }

    public class BillDetails
    {
        public string Title { get; set; }
        public List<BillItem> Details { get; set; }
    }
    public class PaymentTypeResponse
    {
        public int PaymentTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

    }
}
