//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RainbowWine.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class PageImage
    {
        public int PageImageId { get; set; }
        public Nullable<int> PageVersionId { get; set; }
        public string ImageKey { get; set; }
        public string ImageUrl { get; set; }
        public string ClickRedirect { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}