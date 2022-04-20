using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZModels;

namespace SZData.Interfaces
{
    public interface ICartService : IDisposable
    {
        CartItemModel GetCartData(int cartId, int shopId, int customerId);
    }
}
