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
    
    public partial class Core_GetCollectionOrStories_Result
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubText { get; set; }
        public Nullable<bool> IsStory { get; set; }
        public string FilePath { get; set; }
        public string Payload { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
