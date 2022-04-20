using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services.DBO;

namespace RainbowWine.Controllers
{
    [Authorize(Roles = "Shopper,SalesManager")]
    public class InventoriesController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();

        // GET: Inventories
        public ActionResult Index1()
        {
            var inventories = db.Inventories.Include(i => i.WineShop);
            return View(inventories.ToList());
        }

        // GET: Inventories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // GET: Inventories/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName");
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName");
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InventoryId,ProductID,ShopID,QtyAvailable,MRP,packing_size,ShopItemId")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.LastModified = DateTime.Now;
                inventory.LastModifiedBy = User.Identity.Name;
                db.Inventories.Add(inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var shop = db.WineShops.Where(o => o.Id == inventory.ShopID)?.FirstOrDefault().ShopName;
            var product = db.ProductDetails.Where(o => o.ProductID == inventory.ProductID)?.FirstOrDefault().ProductName;
            ViewBag.ShopName = shop;
            ViewBag.ProductName = product;
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName", inventory.ProductID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", inventory.ShopID);
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ShopName = inventory.WineShop.ShopName;
            ViewBag.ProductName = inventory.ProductDetail.ProductName;
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName", inventory.ProductID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", inventory.ShopID);
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InventoryId,ProductID,ShopID,QtyAvailable,MRP,packing_size,ShopItemId")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.LastModified = DateTime.Now;
                inventory.LastModifiedBy = User.Identity.Name;
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var invent = db.Inventories.Include(o => o.WineShop).Include(o => o.ProductDetail).Where(o=>o.InventoryId== inventory.InventoryId).FirstOrDefault();
            ViewBag.ShopName = invent.WineShop.ShopName;
            ViewBag.ProductName = invent.ProductDetail.ProductName;
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName", invent.ProductID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", invent.ShopID);
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventory inventory = db.Inventories.Find(id);
            db.Inventories.Remove(inventory);
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

        public ActionResult Index(string product ="", string shop="")
        {
            int shopId = 0;
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            if (user != null && user.ShopId > 0) shopId = user.ShopId ?? 0;

            InventoryDBO inventoryDBO = new InventoryDBO();
            var inventories = inventoryDBO.GetInventory(product, shop);
            //var inventories = db.Inventories.Include(p => p.ProductDetail).Include(i => i.WineShop).Select(o => new InventoryViewModel
            //{
            //    ShopId = o.ShopID,
            //    ShopName = o.WineShop.ShopName,
            //    ProductId = o.ProductID,
            //    ProductName = o.ProductDetail.ProductName,
            //    InventoryId = o.InventoryId,
            //    PackingSize = o.packing_size ?? 0,
            //    MRP = o.MRP ?? 0,
            //    QtyAvailable = o.QtyAvailable,
            //    Price = o.ProductDetail.Price ?? 0,
            //    ProductSizeId = o.ProductDetail.ProductSizeID ?? 0
            //});
            //inventories.ForEach(i => {
            //    var prodSize = db.ProductSizes.Where(o => o.ProductSizeID == i.ProductSizeId).FirstOrDefault();
            //    if (prodSize != null)
            //        i.Size = prodSize.Capacity;
            //});
            //if (!string.IsNullOrWhiteSpace(product))
            //{
            //    var p = inventories.Where(o => o.ProductName.StartsWith(product)).ToList();
            //    inventories = p;
            //}
            //if (!string.IsNullOrWhiteSpace(shop))
            //{
            //    var p = inventories.Where(o => o.ShopName.StartsWith(shop)).ToList();
            //    inventories = p;
            //}

            ViewBag.ShopName = shop;
            ViewBag.ProductName = product;
            return View(inventories.ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult SaveInventory(Inventory inventory)
        {
            var model = db.Inventories.Where(o => o.InventoryId == inventory.InventoryId)?.FirstOrDefault();
            if (model != null)
            {
                int changeSource = 12;
                if (inventory.QtyAvailable > model.QtyAvailable)
                {
                    changeSource = 11;
                }

                var invenTrack = new InventoryTrack
                {
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ProductID = model.ProductID,
                    ShopID = model.ShopID,
                    QtyAvailBefore = model.QtyAvailable,
                    QtyAvailAfter = inventory.QtyAvailable,
                    ChangeSource=changeSource
                };
                model.QtyAvailable = inventory.QtyAvailable;
                model.MRP = inventory.MRP;
                model.LastModified = DateTime.Now;
                model.LastModifiedBy= User.Identity.Name;
                db.SaveChanges();

                db.InventoryTracks.Add(invenTrack);
                db.SaveChanges();
            }
            return Json(new { InventoryId = model.InventoryId, QtyAvailable = model.QtyAvailable, MRP = model.MRP });
        }

        public ActionResult GlobalMrpChange(string product = "")
        {
            int productRefId = 0;
            if (product != "")
            {
                var prod = db.Products.Where(x => x.ProductName == product && x.IsDelete == false).FirstOrDefault();
                productRefId = prod.Id;    
            }
            var shops = db.WineShops.Where(x => x.OperationFlag == true).ToList();
            InventoryDBO inventoryDBO = new InventoryDBO();
            var inventories = inventoryDBO.GetInventoryDetails(productRefId);
            ViewBag.ProductName = product;
            ViewBag.ShopList = new SelectList(shops, "Id", "ShopName");
            return View(inventories.ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult UpdateGlobalMrp(int productId, string [] shopIds,decimal mrp)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            InventoryDBO inventoryDBO = new InventoryDBO();
            string shops = String.Join(",", shopIds);
            var inventories = inventoryDBO.UpdateGlobalInvetoryMrp(productId, shops, mrp,u.Email);

            return Json(new { InventoryId = inventories });
        }
    }
}
