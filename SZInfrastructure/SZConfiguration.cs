using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData;
using SZData.Interfaces;
using SZModels;

namespace SZInfrastructure
{
    public class SZConfiguration : ISZConfiguration
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
                    _config.Dispose();
                    _config = null;
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

        IConfigService _config;
        public SZConfiguration(IConfigService config)
        {
            _config = config;
        }
        public string GetConfigValue(string key)
        {
            InMemoryCache cache = new InMemoryCache();
            var config = cache.GetOrSet<ConfigModel>("CacheConfig", () => _config.GetAll());
            var vConfig = config.Where(o => string.Compare(o.KeyText, key, true) == 0).FirstOrDefault();
            return vConfig?.ValueText;
        }
        public IList<ConfigModel> GetConfigValue()
        {
            InMemoryCache cache = new InMemoryCache();
            var config = cache.GetOrSet<ConfigModel>("CacheConfig", () => _config.GetAll());
            //var vConfig = config;
            return config;
        }

    }
    public interface ISZConfiguration:IDisposable
    {
        string GetConfigValue(string key);
        IList<ConfigModel> GetConfigValue();
    }
}
