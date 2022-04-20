using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class InventoryMixerDO:MixerDO
    {
        public int InventoryMixerId { get; set; }
        public int MixerDetailsId { get; set; }
        public int Qty { get; set; }
        public int ShopId { get; set; }
        public Mixer Mixer { get; set; }
        public int SupplierId { get; set; }

    }
}