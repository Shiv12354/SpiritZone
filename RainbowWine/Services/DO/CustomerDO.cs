using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class CustomerDO:Customer
    {
        public WineShop Shop { get; set; }
    }

    public class CheckCredibleUser
    {
        public double CustScore { get; set; }
        public double CustScoreThreshoValue { get; set; }
        public string Customer { get; set; }
    }
}