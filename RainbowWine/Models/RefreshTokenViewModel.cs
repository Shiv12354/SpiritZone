using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class RefreshTokenViewModel
    {
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
        public string Token { get; set; }
    }
}