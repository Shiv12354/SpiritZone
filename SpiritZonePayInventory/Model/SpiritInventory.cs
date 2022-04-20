using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritZonePayInventory.Model
{
    public class SpiritInventory
    {
        public int InventoryId { get; set; }
        public int ProductID { get; set; }
        public int ShopID { get; set; }
        public int QtyAvailable { get; set; }
    }
}
