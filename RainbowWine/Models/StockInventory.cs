using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class StockInventory
    {
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public bool IsMixer { get; set; }

    }
    public class StockFavorite
    {
        public StockFavorite()
        {
            Favorites = new List<StockInventory>();
            Items = new List<StockInventory>();
        }
        public List<StockInventory> Favorites { get; set; }

        public List<StockInventory> Items { get; set; }
    }
}