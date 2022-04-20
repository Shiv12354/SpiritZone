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
    public class PageRepo : BaseRepo, IPageRepo
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

        public PageVersion GetPageContent(int pageId, string version)
        {
            PageVersion pVersion = null;
            using (var db = new SqlConnection(ConnectionText))
            {
                var param = new DynamicParameters();
                param.Add(SZParameters.pPageId, pageId);
                param.Add(SZParameters.pVersionNo, version);
                var result = db.QueryMultiple(SZStoredProcedures.PageSel, param: param, commandType: CommandType.StoredProcedure);
                
                var dicVersion = new Dictionary<int, PageVersion>();
                var pageContent = result.Read<PageVersion, Page, PageContent, PageVersion>(
                    (pv,p,vc) =>
                    {
                        //ver = null;
                        if (!dicVersion.TryGetValue(pv.PageVersionId, out var ver))
                        {
                            ver = pv;
                            ver.Page = p;
                            ver.PageContents.Add(vc);
                            dicVersion.Add(ver.PageVersionId,ver);
                        }
                        else
                        {
                            ver = dicVersion[pv.PageVersionId];
                            ver.PageContents.Add(vc);
                        }
                        return pv;
                    }
                    , splitOn: "PageId,PageContentId").FirstOrDefault();
                var images = result.Read<PageImage>().ToList();
                var buttons = result.Read<PageButton>().ToList();

                pVersion = dicVersion.Values?.FirstOrDefault();
                if (pVersion != null)
                {
                    pVersion.PageImages = images;
                    pVersion.PageButtons = buttons;
                }
            }
            return pVersion;
        }
    }
}
