using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services
{
    public class CommonWebHook
    {
        public string device_id { get; set; }
        public string activity { get; set; }
        public int? DeliveryAgentId { get; set; }
        public int? expiry_time { get; set; }
        public string version { get; set; }
        public bool tracking { get; set; } = false;
        public double? longitude { get; set; }
        public double? latitude { get; set; }
        public string sdk_version { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public DateTime recorded_at { get; set; }
        public int? remaining_duration { get; set; }
        public int? remaining_distance { get; set; }
        public DateTime created_at { get; set; }
        public string trip_id { get; set; }
        public int? duration { get; set; }
        public int? distance { get; set; }
        public string Reason { get; set; }


        public float arrival_latitude { get; set; }
        public float arrival_longitude { get; set; }
        public string arrival_recorded_at { get; set; }
        public float exit_latitude { get; set; }
        public float exit_longitude { get; set; }
        public string exit_recorded_at { get; set; }
        public string geofence_id { get; set; }
        public string geofence_value { get; set; }
        public string geometry_latitude { get; set; }
        public string geometry_longitude { get; set; }
        public int route_to_distance { get; set; }
        public int route_to_duration { get; set; }
        public int route_to_idle_time { get; set; }
        public string route_to_started_at { get; set; }
    }

}