using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class RatingProductDO: RatingProductReview
    {
        public Customer Customer { get; set; }

        public Product Product { get; set; }

        public RatingStar RatingStar { get; set; }
        public int AverageRating { get; set; }
        public int TotalRating { get; set; }
        public int ProductRefId { get; set; }
    }
}