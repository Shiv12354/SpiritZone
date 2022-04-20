using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class MixerDetailItem
    {
        public int MixerPrice { get; set; }
        public int MixerItemQty { get; set; }
        public string MixerName { get; set; }
        public string MixerCapacity { get; set; }
        public int MixerOrderItemId { get; set; }
        public int MixerDetailId { get; set; }
    }
}