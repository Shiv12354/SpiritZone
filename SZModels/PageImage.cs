using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class PageImage
    {
        public int PageImageId { get; set; }
        public int PageVersionId { get; set; }
        public string ImageKey { get; set; }
        public string ImageUrl { get; set; }
        public string ClickRedirect { get; set; }
        public bool IsActive { get; set; }
    }
}
