using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        //[Route("products/{productRefId}/{isReserve}/{capacity}")]
        public ActionResult Index(string procode)
        {
            return View();
        }
    }
}