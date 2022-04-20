using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class ProductDetailDO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string AlcoholContent { get; set; }
        public string Region { get; set; }
        public int Size { get; set; }
        public double Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Category { get; set; }
        public string ProductType { get; set; }
        public int OrderBy { get; set; }
        public string ShopItemId { get; set; }
        public string ProductImage { get; set; }
        public string ProductHomeImage { get; set; }
        public string ProductThumbImage { get; set; }
        public bool IsDelete { get; set; }
        public bool IsFeature { get; set; }
        public int ProductRefID { get; set; }
        public int ProductSizeID { get; set; }
        public int ProductBrandID { get; set; }
        public int ProductCategoryID { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int QtyAvailable { get; set; }
        public string Capacity { get; set; }
        public string SizeName { get; set; }
        public string Description { get; set; }
        public bool IsMixer { get; set; }
        public string MixerImage { get; set; }
        public bool IsCustomerRated { get; set; }
        public int AverageRating { get; set; }
        public bool IsFavourite { get; set; }
        public int FavoriteId { get; set; }
        public bool IsReserve { get; set; }
        public int ProductRating { get; set; }
        public int RatingCount { get; set; }

    }
}