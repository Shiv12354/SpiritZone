using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class PageVersion
    {
        public PageVersion()
        {
            PageContents = new List<PageContent>();
            PageImages = new List<PageImage>();
            PageButtons = new List<PageButton>();
        }
        public int PageVersionId { get; set; }
        public int PageId { get; set; }
        public string VersionNo { get; set; }

        public Page Page { get; set; }
        public IList<PageContent> PageContents { get; set; }
        public IList<PageImage> PageImages { get; set; }
        public IList<PageButton> PageButtons { get; set; }
    }
}
