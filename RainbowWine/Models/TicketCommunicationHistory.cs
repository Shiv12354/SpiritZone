using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class TicketCommunicationHistory
    {
        public long Id { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}