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
    
    public partial class InventoryDetail
    {
        public int InventoryId { get; set; }
        public int ProductID { get; set; }
        public int ShopID { get; set; }
        public int QtyAvailable { get; set; }
        public Nullable<double> MRP { get; set; }
        public Nullable<int> packing_size { get; set; }
        public System.DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public string ShopItemId { get; set; }
    
        public virtual WineShop WineShop { get; set; }
    }
}
