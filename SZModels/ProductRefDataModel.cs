using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class ProductRefDataModel
    {
        public List<ProductDTO> RecommendedProducts { get; set; }
        public List<ProductDTO> ProductByPurchase { get; set; }
        public List<ProductDTO> ProductByLocation { get; set; }
        public List<ProductCategoryDTO> ProductCategories { get; set; }
        public List<ProductDTO> RecommendedPremiumProducts { get; set; }
        public List<ProductBrandDTO> ProductBrands { get; set; }
        public List<MixerProductDTO> MixerProducts { get; set; }
    }

    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductRefID { get; set; }
        public string ProductThumbImage { get; set; }
        public string ProductImage { get; set; }
        public string BrandName { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public string Capacity { get; set; }
        public bool IsCustomerRated { get; set; }
        public int ProductRating { get; set; }
        public int RatingCount { get; set; }
        public int AverageRating { get; set; }
        public bool IsFavourite { get; set; }
    }
    public class ProductCategoryDTO
    {
        public int ProductCategoryID { get; set; }
        public string ProductCategory1 { get; set; }
        public int? ParentID { get; set; }
        public bool IsForFilter { get; set; }
        public string CategoryImage { get; set; }
    }
    public class ProductBrandDTO
    {
        public int ProductBrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandImage { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPopular { get; set; }
        public int? OrderBy { get; set; }
    }
    public class MixerProductDTO
    {
        public int MixerId { get; set; }
        public string MixerName { get; set; }
        public long GiftPrice { get; set; }
        public string GiftDescription { get; set; }
        public string Description { get; set; }
        public string MixerImage { get; set; }
        public string MixerThumbImage { get; set; }
        public string MixerType { get; set; }
        public MixerDetail MixerDetail { get; set; }
        public int MixerDetailId { get; set; }
        public int MixerSizeId { get; set; }
        public int Price { get; set; }
        public string Capacity { get; set; }
        public int Qty { get; set; }
        public int SupplierId { get; set; }
        public bool IsMixer { get; set; }
    }
    public class MixerDetail
    {
        public int MixerDetailId { get; set; }
        public int MixerId { get; set; }
        public int MixerSizeId { get; set; }
        public int Price { get; set; }
        public string Capacity { get; set; }
        public int Qty { get; set; }
        public int SupplierId { get; set; }
        public bool IsMixer { get; set; }
    }
}
