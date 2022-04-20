using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class ReferralCode
    {
        public string UniqueReferralCode { get; set; }
        public int PromoTypeId { get; set; }
        public string TypeName { get; set; }
        public string Message { get; set; }
        public int ValidCode { get; set; }
    }
}
