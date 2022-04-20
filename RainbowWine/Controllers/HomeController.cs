using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services;
using RainbowWine.Services.Gateway;
using RainbowWine.Services.PaytmService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SZData;
using SZData.Interfaces;
using SZInfrastructure;
using SZInfrastructure.ConfigurationManage.Interfaces;

namespace RainbowWine.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult AppleFile()
        {
            byte[] bytes;
            using (System.IO.FileStream stream = new System.IO.FileStream(Server.MapPath("/.well-known/apple-app-site-association.json"), System.IO.FileMode.Open))
            {
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                //Response.AppendHeader("content-disposition", "attachment;filename=somefile");
                //return new FileContentResult(bytes, "text/plain");
            }
            return File(bytes, "application/file", "file");
            //string path = Server.MapPath("/.well-known/apple-app-site-association.json");
            //if (System.IO.File.Exists(path))
            //{
            //    Response.AppendHeader("content-disposition", "attachment;filename=apple-app-site-association");
            //    System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open); // don't use using keyword!
            //    return new FileStreamResult(stream, "application/file"); // the constructor will fire Dispose() when done
            //}
            //else
            //    return null;

        }
        public ActionResult Index()
        {
            //using (var db = new rainbowwineEntities()) {
            //    var order = db.Orders.Find(52585);
            //    PaymentStrategy pay = new PaymentStrategy(new TransactionCashFree());
            //    var paym = pay.MakePayment(new CashFreeModel
            //    {
            //        OrderId = order.Id.ToString(),
            //        OrderAmount = order.OrderAmount.ToString(),
            //        CustomerEmail = "subham@rainail.com",
            //        CustomerPhone = order.OrderTo
            //    });
            //}
            //using (HttpClient client = new HttpClient())
            //{
            //    var json = JsonConvert.SerializeObject(new { Email="neha@rainmail.com", Password="Test@123" });
            //    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            //    var response = client.PostAsync("https://crm.spiritzone.in/api/user/login", stringContent).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var x = response.Content.ReadAsStringAsync();
            //        //var User = await ApiServices.Instance.GetUser(ResponseModel.Data.AccessToken);

            //        //if (!User.Status)
            //        //{
            //        //    ResponseModel.IsError = true;
            //        //    ResponseModel.Msg = "Sorry!!! Something went wrong please check after some time or check your internet connectivity and try agian.";

            //        //}
            //    }
            //    else
            //    {
            //    }
            //}
            //throw new Exception("Applciation error");
            //PaytmPayment pay = new PaytmPayment();
            //pay.PaytmAPiCall(131);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}