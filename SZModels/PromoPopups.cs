using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class PromoPopups
    {
        public int PromoPopId { get; set; }
        public string Filepath { get; set; }
        public string OtherText { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ClickAction { get; set; }
        public string OtherParameters { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
