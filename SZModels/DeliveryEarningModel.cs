using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class DeliveryEarningModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DeliveryAgentId { get; set; }
        public int OrderId { get; set; }
        public System.DateTime DeliveredDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Earning { get; set; }
        public OrderModel Order { get; set; }
        public DateTime TrackDate { get; set; }
    }
}
