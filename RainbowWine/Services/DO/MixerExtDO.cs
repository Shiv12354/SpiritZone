using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class MixerExtDO:Mixer
    {
        public int MixerDetailId { get; set; }
        public int MixerSizeId { get; set; }
        public int Price { get; set; }
        public string Capacity { get; set; }
        public string MixerSizeName { get; set; }
        public int Qty { get; set; }
        public int SupplierId { get; set; }
        public bool IsMixer { get; set; }
    }
}