using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class BalanceAndExpiryAmount
    {
        public ICollection<WalletDO> Wallet { get; set; }
        public ICollection<WalletOrderDO> WalletOrder { get; set; }

        public ICollection<OrderExt> OrderExt { get; set; }
    }

    public class WalletDO
    {
        public decimal Balance { get; set; }
    }

    public class WalletOrderDO
    {
        public decimal ExpireAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
    public class OrderExt
    {
        public bool IsReferralEnable { get; set; }

    }
}
