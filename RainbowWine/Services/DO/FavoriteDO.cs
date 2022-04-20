using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class FavoriteDO:Favorite
    {
        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}