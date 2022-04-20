using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZModels
{
    public class DeliveryEarningWithPenaltyModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DeliveryAgentId { get; set; }
        public int OrderId { get; set; }
        public System.DateTime DeliveredDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Earning { get; set; }
        public string Descriptions { get; set; }
        public bool IsPenalty { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
