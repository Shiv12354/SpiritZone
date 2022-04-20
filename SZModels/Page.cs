using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class Page
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string PageKey { get; set; }
        public IList<PageVersion> PageVersions { get; set; }
    }
}
