using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class HyperTrackAppHookDO
    {
        public DateTime CreatedAt { get; set; }
        public string DeviceId { get; set; }
        public string TripId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime RecordedAt { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Activity { get; set; }
        public int? Distance { get; set; }
        public int? Duration { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string version { get; set; }

    }

}