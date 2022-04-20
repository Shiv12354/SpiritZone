using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class CustomerSignup
    {
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}