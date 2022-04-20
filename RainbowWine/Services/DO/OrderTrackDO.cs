using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class OrderTrackDO
    {
        public int OrderId { get; set; }
        public string TrackDate { get; set; }
        public bool IsApproved { get; set; }
        public string LogUserName { get; set; }
    }
}