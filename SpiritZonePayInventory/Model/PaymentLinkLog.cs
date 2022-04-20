using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritZonePayInventory.Model
{
    public class PaymentLinkLog
    {
        public int PaymentLinkId { get; set; }
        public int OrderId { get; set; }
        public string UPIString { get; set; }
        public string VendorOuput { get; set; }
        public string PayUrlExpiry { get; set; }
        public string PtmType { get; set; }
        public string PtmStatus { get; set; }
    }
}
