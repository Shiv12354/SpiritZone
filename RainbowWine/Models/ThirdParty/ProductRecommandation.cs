using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.ThirdParty
{
    public class ProductRecommandation
    {
        public Request Request { get; set; }
        public List<Response> Response { get; set; }
        public string Timestamp { get; set; }
        public string Type { get; set; }
    }
    public class Request
    {
        public int CustomerId { get; set; }
        public int Num { get; set; }
        public int ProductId { get; set; }
        public int ShopID { get; set; }
    }

    public class Details
    {
        public double AlcoholContent { get; set; }
        public string CategoryName { get; set; }
        public int Cluster { get; set; }
        public int NumSold { get; set; }
        public double PopularityAmongBrand { get; set; }
        public double Price { get; set; }
        public int ProductID { get; set; }
        public string Region { get; set; }
        public double Score { get; set; }
        public string Size { get; set; }

    }

    public class Response
    {
        public int Collaborative { get; set; }
        public Details Details { get; set; }
        public string DisplayName { get; set; }
        public int Prev_purchased { get; set; }
        public string ProductName { get; set; }
    }


}