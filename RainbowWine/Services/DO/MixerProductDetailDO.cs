using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class MixerProductDetailDO
    {
        public int ProductId { get; set; }
        public int ProductRefID { get; set; }
        public string ProductName { get; set; }
        public string ProductThumbImage { get; set; }
        public string ProductImage { get; set; }
        public string BrandName { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public string Capacity { get; set; }
        public int AvailableQty { get; set; }
        public bool IsCustomerRated { get; set; }
        public int ProductRating { get; set; }
        public int RatingCount { get; set; }
        public int AverageRating { get; set; }
        public int IsFavourite { get; set; }
        

    }
}