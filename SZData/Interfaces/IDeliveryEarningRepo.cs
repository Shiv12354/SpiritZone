using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZModels;

namespace SZData.Interfaces
{
    public interface IDeliveryEarningRepo:IDisposable
    {
        double GetTotalEarning(string userId, DateTime? fDate);
        IList<DeliveryEarningModel> GetEarningHistory(string userId, DateTime? fDate);
        int AddEarning(string userId, int orderId);
        int AddEarningNew(string userId, int orderId);
        double GetTotalEarningWithPenalty(string userId, DateTime? fDate);
        IList<DeliveryEarningWithPenaltyModel> GetEarningHistoryWithPenalty(string userId, DateTime? fDate);
        int InsertPenaltyTransaction(int OrderId, int Rating);
    }
}

