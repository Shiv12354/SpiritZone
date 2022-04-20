using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class TransactionHistory
    {
        public int CardId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal CreditAdded { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string TransactionType { get; set; }
        public string SourceType { get; set; }
        public decimal CreditLeft { get; set; }
        public decimal CreditUsed { get; set; }
        public decimal ExpiredAmount { get; set; }
    }
}
