using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Controllers
{
    public class OrderTrackController : Controller
    {
        // GET: OrderTrack
        public ActionResult Index()
        {
            return View();
        }
        //public static Task<List<FirebaseModel>> GetLatLongServices()
        //{
        //    Geolocation obj = new Geolocation();
        //    return obj.GetLatLng();
        //}
    }
}