using PagedList;
using RainbowWine.Data;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Controllers
{
    [Authorize(Roles = "Shopper")]
    public class ManufacturerController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();
        ManufacturerDBO manufacturerDBO = new ManufacturerDBO();
        // GET: Manufacturer
        public ActionResult Index(int? page)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            var res= manufacturerDBO.GetManufacturer();

            return View(res.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult UpdateManufacturer(int manufacturerId, string manufacturerName, string manufacturerAbbreviated, string region, string collabSince)
        {
            int res = 0;
            try
            {
               res= manufacturerDBO.ManufacturerUpdate(manufacturerId, manufacturerName, manufacturerAbbreviated, region, collabSince);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { response = res, status = true, msg = "udpate" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddManufacturer(string manufacturerName, string manufacturerAbbreviated, string region, string collabSince)
        {
            int res = 0;
            try
            {
              res=  manufacturerDBO.ManufacturerAdd(manufacturerName, manufacturerAbbreviated, region, collabSince);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {response=res, status = true, msg = "udpate" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteManufacturer(int manufacturerId = 0)
        {

            try
            {
               
                manufacturerDBO.ManufacturerDelete(manufacturerId);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "udpate" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult BrandManufacturer(int id)
        {
            IList<BrandManufacturerDO> brandManufacturerDO = null;
            IList<BrandManufacturerDO> singleList = null;
            ManufacturerDBO manufacturerDBO = new ManufacturerDBO();
            brandManufacturerDO = manufacturerDBO.GetBrandManufacturerById(id);
            singleList = manufacturerDBO.GetBrandManufacturerDetail(id);
            ViewBag.ManufacturerName = singleList.FirstOrDefault().ManufacturerName;
            ViewBag.Id = id;
            ViewBag.AllBrandManufacturerList = new SelectList(singleList, "BrandId", "BrandName");
            ViewBag.SingleBrandManufacturer = new SelectList(singleList, "BrandId", "BrandName");
            return View(brandManufacturerDO);
        }

        [HttpGet]
        public ActionResult BrandManufacturerList()
        {
            ManufacturerDBO manufacturerDBO = new ManufacturerDBO();
            IList<BrandManufacturerDO> brandManufacturerDO = null;
            brandManufacturerDO = manufacturerDBO.GetBrandManufacturerDetail(0);
            IEnumerable<IGrouping<string, BrandManufacturerDO>> groups = brandManufacturerDO.GroupBy(x => x.ManufacturerName);
            var res = groups;
            ViewBag.ProductList = new SelectList(res, "BrandId", "BrandName");
            return View(groups);
        }

        [HttpPost]
        public JsonResult AddAndUpdateBrandManufacturer(int manufacturerId, string[] brandIds, string action)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            ManufacturerDBO manufacturerDBO = new ManufacturerDBO();
            string ids = String.Join(",", brandIds);
            var prods = manufacturerDBO.AddAndUpdateBrandManufacturer(manufacturerId, ids, u.Email, action);

            return Json(new { ComBrandManufacturer = prods });
        }

        [HttpPost]
        public JsonResult AddNewBrandManufacturer(string manufacturerId, string[] brandIds, string action)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            ManufacturerDBO manufacturerDBO = new ManufacturerDBO();
            string mId = String.Join(",", manufacturerId);
            string ids = String.Join(",", brandIds);
            var prods = manufacturerDBO.AddAndUpdateBrandManufacturer(Convert.ToInt32(mId), ids, u.Email, action);

            return Json(new { ComBrandManufacturer = prods });
        }

        [HttpGet]
        public ActionResult AddBrandManufacturer()
        {
            IList<BrandManufacturerDO> brandManufacturerDO = null;
            //IList<BrandManufacturerDO> allBrandManufacturerList = null;
            ManufacturerDBO manufacturerDBO = new ManufacturerDBO();
            brandManufacturerDO = manufacturerDBO.GetBrandManufacturerById(0);
            var allBrandList = db.ProductBrands.ToList();
            ViewBag.NotLinkBrandManufacturerList = new SelectList(brandManufacturerDO, "ManufacturerId", "ManufacturerName");
            ViewBag.AllBrandManufacturerList = new SelectList(allBrandList, "ProductBrandId", "BrandName");
            return View(brandManufacturerDO);
        }

        
    }

    
}