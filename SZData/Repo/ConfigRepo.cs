using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;
using SZModels;

namespace SZData.Repo
{
    public class ConfigRepo : BaseRepo, IConfigRepo
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

        public string Get(string key)
        {
            string keyvalue = default(string);
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.pKeyText, key);
                var ot = db.Query<ConfigModel>(SZStoredProcedures.ConfigMasterByKeySel, param: param).FirstOrDefault();
                keyvalue = (ot == null) ? "" : ot.ValueText;
            }
            return keyvalue;
        }
        public IList<ConfigModel> GetAll()
        {
            IList<ConfigModel> config = null;
            using (var db = new SqlConnection(ConnectionText))
            {
                config = db.Query<ConfigModel>(SZStoredProcedures.ConfigMasterSel).ToList();
            }
            return config;
        }
    }
}
