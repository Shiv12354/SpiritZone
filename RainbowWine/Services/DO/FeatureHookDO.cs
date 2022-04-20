using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class FeatureHookDO
    {
        public int FeatureHookId { get; set; }
        public string FeatureHookName { get; set; }
        public string Title { get; set; }
        public string SubText { get; set; }
        public bool IsStory { get; set; }
        public string Icon { get; set; }
        public string Payload { get; set; }
        public Int32 SortOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool ForCredibleUser { get; set; }
        public int BrandId { get; set; }
        public string ProductRefIds { get; set; }
        public string ShopIds { get; set; }
        public bool IsGift { get; set; }
        public string BgImage { get; set; }
        public bool NeedCartClear { get; set; }
    }
}