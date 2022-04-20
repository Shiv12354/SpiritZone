using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZData.Interfaces;
using SZModels;

namespace SZData
{
    public class PageService : IPageService
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

        IPageRepo _repo;
        public PageService(IPageRepo repo)
        {
            _repo = repo;
        }
        public PageVersion GetPageContent(int pageId, string version)
        {
            return _repo.GetPageContent(pageId, version);
        }

        public PageVersion GetPageContent(string pageKey, string version)
        {
            throw new NotImplementedException();
        }
    }
}
