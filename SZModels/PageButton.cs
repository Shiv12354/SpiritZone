using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class PageButton
    {
        public int PageContentId { get; set; }
        public int PageVersionId { get; set; }
        public string ContentKey { get; set; }
        public string ContentValue { get; set; }
        public bool IsActive { get; set; }
        public string ButtonKey { get; set; }
        public string ButtonText { get; set; }
    }
}
