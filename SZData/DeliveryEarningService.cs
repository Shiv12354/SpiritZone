using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;
using SZModels;

namespace SZData
{
    public class DeliveryEarningService : IDeliveryEarningService
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _repo.Dispose();
                    _repo = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~QonConfigRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        IDeliveryEarningRepo _repo;
        public DeliveryEarningService(IDeliveryEarningRepo earn)
        {
            _repo = earn;
        }
        public double GetTotalEarning(string userId, DateTime? fDate)
        {
            return _repo.GetTotalEarning(userId, fDate);
        }

        public IList<DeliveryEarningModel> GetEarningHistory(string userId, DateTime? fDate)
        {
            return _repo.GetEarningHistory(userId, fDate);
        }

        public int AddEarning(string userId, int orderId)
        {
            return _repo.AddEarning(userId, orderId);
        }

        public int AddEarningNew(string userId, int orderId)
        {
            return _repo.AddEarningNew(userId, orderId);
        }

        public double GetTotalEarningWithPenalty(string userId, DateTime? fDate)
        {
            return _repo.GetTotalEarningWithPenalty(userId, fDate);
        }

        public IList<DeliveryEarningWithPenaltyModel> GetEarningHistoryWithPenalty(string userId, DateTime? fDate)
        {
            return _repo.GetEarningHistoryWithPenalty(userId, fDate);
        }
    }
}
