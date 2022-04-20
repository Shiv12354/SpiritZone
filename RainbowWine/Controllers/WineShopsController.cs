using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RainbowWine.Data;
using RainbowWine.Services.Filters;

namespace RainbowWine.Controllers
{
    [AuthorizeSpirit(Roles = "Shopper")]
    public class WineShopsController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();

        // GET: WineShops
        public ActionResult Index()
        {
            return View(db.WineShops.ToList());
        }

        // GET: WineShops/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WineShop wineShop = db.WineShops.Find(id);
            if (wineShop == null)
            {
                return HttpNotFound();
            }
            return View(wineShop);
        }

        // GET: WineShops/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WineShops/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ShopName,Address,PhoneNo,ContactPerson,GoogelCode,AvailableAgent,Longitude,Latitude,PlaceId,OperationFlag,GST,VAT")] WineShop wineShop)
        {
            if (ModelState.IsValid)
            {
                db.WineShops.Add(wineShop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(wineShop);
        }

        // GET: WineShops/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WineShop wineShop = db.WineShops.Find(id);
            if (wineShop == null)
            {
                return HttpNotFound();
            }
            return View(wineShop);
        }

        // POST: WineShops/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ShopName,Address,PhoneNo,ContactPerson,GoogelCode,AvailableAgent,Longitude,Latitude,PlaceId,OperationFlag,GST,VAT")] WineShop wineShop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wineShop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wineShop);
        }

        // GET: WineShops/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WineShop wineShop = db.WineShops.Find(id);
            if (wineShop == null)
            {
                return HttpNotFound();
            }
            return View(wineShop);
        }

        // POST: WineShops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WineShop wineShop = db.WineShops.Find(id);
            db.WineShops.Remove(wineShop);
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
    }
}
