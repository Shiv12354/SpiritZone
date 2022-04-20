using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RainbowWine.Data;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using RainbowWine.Services.Filters;

namespace RainbowWine.Controllers
{
    [AuthorizeSpirit]
    public class ProductDetailsController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();

        [AuthorizeSpirit(Roles = "Shopper")]
        // GET: ProductDetails
        public ActionResult Index()
        {
            ViewBag.HostName = Request.Url.GetLeftPart(UriPartial.Authority);
            return View(db.ProductDetails.ToList());
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        // GET: ProductDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.HostName = Request.Url.GetLeftPart(UriPartial.Authority);
            return View(productDetail);
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        // GET: ProductDetails/Create
        public ActionResult Create()
        {
            return View();
        }



        [AuthorizeSpirit(Roles = "Shopper")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,Size,Category,ProductType,Price,ShopItemId,CreatedDate,ModifiedDate,IsDelete")] ProductDetail productDetail, HttpPostedFileBase Image, HttpPostedFileBase ThumbImage)
        {
            if (ModelState.IsValid)
            {
                productDetail.CreatedDate = DateTime.Now;
                productDetail.ModifiedDate = DateTime.Now;
                productDetail.IsDelete = productDetail.IsDelete;

                if (Image.ContentLength > 0) productDetail.ProductImage = SpiritUtility.UploadSpiritFile(Image, "/Content/images/product", "prod-image");
                if (ThumbImage.ContentLength > 0) productDetail.ProductThumbImage = SpiritUtility.UploadSpiritFile(ThumbImage, "/Content/images/product", "prod-thumb");

                db.ProductDetails.Add(productDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productDetail);
        }


        [AuthorizeSpirit(Roles = "Shopper")]
        // GET: ProductDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

        // POST: ProductDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [AuthorizeSpirit(Roles = "Shopper")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,Size,Category,ProductType,Price,ShopItemId,IsDelete")] ProductDetail productDetail, HttpPostedFileBase Image, HttpPostedFileBase ThumbImage)
        {
            if (ModelState.IsValid)
            {
                ProductDetail pd = db.ProductDetails.Find(productDetail.ProductID);
                pd.ProductName = productDetail.ProductName;
                pd.Size = productDetail.Size;
                pd.Category = productDetail.Category;
                pd.ProductType = productDetail.ProductType;
                pd.Price = productDetail.Price;
                pd.ShopItemId = productDetail.ShopItemId;
                pd.ModifiedDate = DateTime.Now;
                pd.IsDelete = productDetail.IsDelete;

                if (Image != null && Image.ContentLength > 0) pd.ProductImage = SpiritUtility.UploadSpiritFile(Image, "/Content/images/product", "prod-image");
                if (ThumbImage != null && ThumbImage.ContentLength > 0) pd.ProductThumbImage = SpiritUtility.UploadSpiritFile(ThumbImage, "/Content/images/product", "prod-thumb");

                db.Entry(pd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productDetail);
        }


        [AuthorizeSpirit(Roles = "Shopper")]
        // GET: ProductDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        // POST: ProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductDetail productDetail = db.ProductDetails.Find(id);
            db.ProductDetails.Remove(productDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "SalesManager, Shopper")]
        public ActionResult Notify(bool chkPremium=true)
        {
            NotifyDBO notifyDBO = new NotifyDBO();
            var notify=notifyDBO.Notifies(chkPremium);
            ViewBag.IsPremium = chkPremium;
            return View(notify);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "SalesManager, Shopper")]
        public ActionResult NotifyCalled(NotifyHandle notifyHandle)
        {
            if (notifyHandle.NotifyId <= 0)
            {
                return Json(new { msg="Notify is not selected.", status=false}, JsonRequestBehavior.AllowGet);
            }

            var nHandle = new NotifyHandle
            {
                CreatedDate = DateTime.Now,
                NotifyId = notifyHandle.NotifyId,
                Remark = notifyHandle.Remark,
                UserId = User.Identity.GetUserId()
            };
            db.NotifyHandles.Add(nHandle);
            var notify = db.Notifies.Find(notifyHandle.NotifyId);
            notify.ModifiedDate = DateTime.Now;
            notify.IsNotified = true;
            db.SaveChanges();

            return Json(new { msg = "Notify updated successfully.", status = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
