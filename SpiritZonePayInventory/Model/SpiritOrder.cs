using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritZonePayInventory.Model
{
    public class SpiritOrder
    {
        public int Id { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string OrderPlacedBy { get; set; }
        public string OrderTo { get; set; }
        public decimal OrderAmount { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int ShopID { get; set; }
    }
}
