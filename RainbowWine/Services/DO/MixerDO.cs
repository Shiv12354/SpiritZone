using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class MixerDO:Mixer
    {
        public MixerDetail MixerDetail { get; set; }
        public MixerSize MixerSize { get; set; }
    }

    public class MixerDetailsDO:MixerDetail
    {
        public InventoryMixerDO InventoryMixerDO { get; set; }
       
    }
}