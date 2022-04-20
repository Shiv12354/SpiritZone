using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class OrderDetailSel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> ItemQty { get; set; }

        public decimal Price { get; set; }
        public Nullable<int> ShopID { get; set; }
        public Nullable<bool> Issue { get; set; }
        public Nullable<int> DiscountProductId { get; set; }
        public Nullable<double> DiscountUnit { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<int> ProductID1 { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> Size { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Category { get; set; }
        public string ProductType { get; set; }
        public Nullable<double> Price1 { get; set; }
        public string ShopItemId { get; set; }
        public string ProductImage { get; set; }
        public string ProductThumbImage { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<bool> IsFeature { get; set; }
        public Nullable<int> ProductRefID { get; set; }
        public Nullable<int> ProductSizeID { get; set; }
        public string ProductHomeImage { get; set; }
        public Nullable<bool> IsShowcaseView { get; set; }
        public Nullable<bool> IsShowcasePremiumView { get; set; }
        public Nullable<bool> IsPremium { get; set; }
        public Nullable<int> ProductSizeID1 { get; set; }
        public string Capacity { get; set; }
        public string SizeName { get; set; }
        public bool IsMixer { get; set; }
    }
}