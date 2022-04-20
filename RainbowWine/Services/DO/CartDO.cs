using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class CartDO
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public Double UnitPrice { get; set; }
        public int Qty { get; set; }
        public double SubTotal { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Text { get; set; }
        public string Remarks { get; set; }
        public bool IsMixer { get; set; }
        public int AvailableQty { get; set; }
        public int ProductRefID { get; set; }
        public string Capacity { get; set; }
        public int Current_Price { get; set; }
        public bool IsReserve { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OrderGroup { get; set; }
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

     public  class ReferBalance
    {
        public Decimal Balance { get; set; }
    }
}