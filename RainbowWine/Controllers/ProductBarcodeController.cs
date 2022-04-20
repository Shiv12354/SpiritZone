using RainbowWine.Data;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using RainbowWine.Services.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Controllers
{
    [AuthorizeSpirit]
    public class ProductBarcodeController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();
        // GET: ProductBarcode

        [AuthorizeSpirit(Roles = "Shopper,ProductAdmin")]
        public ActionResult Index(int pdId = 0, string prodname = "", string barId = "", string barname = "")
        {
            var pd = new List<ProductDetailsExtDO>();
            ProductDBO productDBO = new ProductDBO();
            if (pdId > 0)
            {
                pd = productDBO.ProductBarcodeList(pdId, null);
            }
            if (!string.IsNullOrWhiteSpace(barId)) { }

            var prodlist = db.ProductDetails.Where(o => o.IsDelete == false);
            var shop = db.WineShops;

            ViewBag.ProductDetailId = new SelectList(prodlist, "ProductID", "ProductName", pdId);
            ViewBag.ShopId = new SelectList(shop, "Id", "ShopName");

            ViewBag.ProductName = prodname;
            ViewBag.BarName = barname;
            return View(pd);
        }

        [AuthorizeSpirit(Roles = "Shopper,ProductAdmin")]        
        public ActionResult Barcode(string barname = "")
        {
            var pd = new List<ProductDetailsExtDO>();
            ProductDBO productDBO = new ProductDBO();

            if (!string.IsNullOrWhiteSpace(barname))
            {
                pd=productDBO.ProductBarcodeList(0, barname);
            }

            var shop = db.WineShops;

            ViewBag.ShopId = new SelectList(shop, "Id", "ShopName");
            
            ViewBag.BarName = barname;
            return View(pd);
        }

        [HttpPost]        
        public ActionResult UpdateBarcode(int bId = 0, string barId = "")
        {

            try { 
            ProductDBO productDBO = new ProductDBO();
            productDBO.ProductBarcodeUpdate(bId, barId);
            }
            catch (Exception ex){
                return Json(new { status = false, msg = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status=true,msg="udpate" },JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult AddBarcode(string barId, int shopId, int prodId)
        {

            try
            {
                ProductDBO productDBO = new ProductDBO();
                productDBO.ProductBarcodeAdd(barId,prodId,shopId);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "udpate" }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult DeleteBarcode(int bId = 0)
        {

            try
            {
                ProductDBO productDBO = new ProductDBO();
                productDBO.ProductBarcodeDelete(bId);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "udpate" }, JsonRequestBehavior.AllowGet);
        }

    }
}