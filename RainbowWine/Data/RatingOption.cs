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
    
    public partial class RatingOption
    {
        public int RatingOptionId { get; set; }
        public Nullable<int> RatingStarId { get; set; }
        public string StarOption { get; set; }
    
        public virtual RatingStar RatingStar { get; set; }
    }
}
