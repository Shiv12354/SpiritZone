using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class CommittedDateDO
    {
        public DateTime CommittedStartDate { get; set; }
        public DateTime CommittedEndDate { get; set; }
        public string OrderGroup { get; set; }
        public int SupplierId { get; set; }
        public string MixerType{ get; set; }

    }
}