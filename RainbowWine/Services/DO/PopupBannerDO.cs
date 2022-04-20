using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class PopupBannerDO
    {
            public int PopupBannerId { get; set; }
            public string FilePath { get; set; }
            public string OtherText { get; set; }
            public string Payload { get; set; }
            public string PageName { get; set; }
            public string SheduleOption { get; set; }
            public bool? ForCredibleUser { get; set; }
            public int SortOrder { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public bool IsDeleted { get; set; }
            public int NumberView { get; set; }
            public Nullable<int> BrandId { get; set; }


    }

    public class SilentNotification
    {
        public string UserID { get; set; }
        public int CustomerID { get; set; }
        public string BannerData { get; set; }
        public string PageName { get; set; }
    }
}