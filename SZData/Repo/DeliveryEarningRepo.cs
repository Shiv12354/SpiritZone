using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;
using SZModels;

namespace SZData.Repo
{
    public class DeliveryEarningRepo :
    BaseRepo, IDeliveryEarningRepo
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

        public double GetTotalEarning(string userId, DateTime? fDate)
        {

            double result = 0;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                if(fDate!=null)
                    param.Add(SZParameters.CreatedDate, fDate);
                result = db.Query<double>(SZStoredProcedures.DeliveryEarningsTotal, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return result;
        }
        public IList<DeliveryEarningModel> GetEarningHistory(string userId, DateTime? fDate)
        {

            IList<DeliveryEarningModel> result = new List<DeliveryEarningModel>();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                if (fDate != null)
                    param.Add(SZParameters.CreatedDate, fDate);

                result = db.Query<DeliveryEarningModel, OrderModel, DeliveryEarningModel>(SZStoredProcedures.DeliveryEarningsHistory,
                    (d, o) => {
                        d.Order = o;
                        return d;
                    },
                    param: param, commandType: CommandType.StoredProcedure,
                    splitOn: "Id").ToList();
            }
            return result;
        }
        public int AddEarning(string userId, int orderId)
        {

            int result = 0;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                param.Add(SZParameters.OrderId, orderId);
                result = db.Query<int>(SZStoredProcedures.DeliveryEarningIns,
                    param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return result;
        }
        public int AddEarningNew(string userId, int orderId)
        {

            int result = 0;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                param.Add(SZParameters.OrderId, orderId);
                result = db.Query<int>(SZStoredProcedures.NewDeliveryEarningIns,
                    param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return result;
        }

        public double GetTotalEarningWithPenalty(string userId, DateTime? fDate)
        {

            double result = 0;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                if (fDate != null)
                    param.Add(SZParameters.CreatedDate, fDate);
                result = db.Query<double>(SZStoredProcedures.DeliveryEarningsPenaltyTotal, param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return result;
        }

        public IList<DeliveryEarningWithPenaltyModel> GetEarningHistoryWithPenalty(string userId, DateTime? fDate)
        {

            IList<DeliveryEarningWithPenaltyModel> result = new List<DeliveryEarningWithPenaltyModel>();
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.UserId, userId);
                if (fDate != null)
                    param.Add(SZParameters.CreatedDate, fDate);
                  result = db.Query<DeliveryEarningWithPenaltyModel>(SZStoredProcedures.DeliveryEarningsHistoryWith_Penalty,                    
                    param: param, commandType: CommandType.StoredProcedure
                  ).ToList();
            }
            return result;
        }

        public int InsertPenaltyTransaction(int OrderId, int rating)
        {

            var result = default(int);
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();                
                param.Add("@Rating", rating);
                param.Add("@OrderId", OrderId);
                result = db.Query<int>(SZStoredProcedures.DeliveryBoy_Rating_Incentive_Ins,
                    param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return result;
        }
    }
}
