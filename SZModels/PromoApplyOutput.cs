using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
   public class PromoApplyOutput
    {
        public int PromoId { get; set; }
        public bool IsValid { get; set; }
        public float DiscountAmount { get; set; }
        public string Message { get; set; }
        public float TotalAmount { get; set; }
        public float TotalAmountAftDic { get; set; }
    }
}
