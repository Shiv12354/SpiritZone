using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class APIOrder
    {
        public APIOrder()
        {
            OrderItems = new List<APIOrderDetails>();
            MixerItems = new List<APIMixerItem>();
        }

        [Required]
        public int AddressId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PremitNo { get; set; }
        public IList<APIOrderDetails> OrderItems { get; set; }
        public IList<APIMixerItem> MixerItems { get; set; }
        public int? PaymentTypeId { get; set; }
        public int ShopId { get; set; }
        public bool IsUsingWalletBalance { get; set; } = false;
        public int WalletBalanceUsed { get; set; } = 0;
        public bool IsPromoApplied { get; set; }
        public string PromoCode { get; set; }
    }
    public class APIOrderDetails
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int ShopID { get; set; }
        public string Message { get; set; }
        public string OrderGroupId { get; set; }
        public DateTime CommittedStartDate { get; set; }
        public DateTime CommittedEndDate { get; set; }
        public string Capacity { get; set; }

    }
    public class APIMixerItem
    {
        public int MixerDetailId { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public int ShopID { get; set; }
        public string Message { get; set; }
        public int  SupplierId { get; set; }
        public string OrderGroupId { get; set; }
        public DateTime CommittedStartDate { get; set; }
        public DateTime CommittedEndDate { get; set; }
        public string Capacity { get; set; }
        public string MixerType { get; set; }

    }
}