using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class OTPVerifyRequestDO
    {
        public string Mobile { get; set; }
        public string OTP { get; set; }
    }
}