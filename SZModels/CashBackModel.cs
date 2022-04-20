using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
   public class CashBackModel
    {
        public int CashBackId { get; set; }
        public decimal CashBackAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public int CreditAmount { get; set; }
        public string UserId { get; set; }
        public int CustomerId { get; set; }
    }
}
