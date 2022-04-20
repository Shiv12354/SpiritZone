using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class ResponseStatus
    {
        public object Data { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
    }
}