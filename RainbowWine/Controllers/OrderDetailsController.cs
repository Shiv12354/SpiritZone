using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RainbowWine.Data;
using Microsoft.AspNet.Identity.Owin;
using RainbowWine.Services.Filters;

namespace RainbowWine.Controllers
{

    [AuthorizeSpirit(Roles = "Agent, Shopper, Deliver, Packer, DeliveryManager, Hub")]
    public class OrderDetailsController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public OrderDetailsController()
        {
        }

        public OrderDetailsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: OrderDetails
        public ActionResult Index()
        {
            var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.ProductDetail).Include(o => o.WineShop);
            return View(orderDetails.ToList());
        }

        [AuthorizeSpirit(Roles = "Agent, Shopper")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderId,ProductID,ItemQty,Price,ShopID")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy", orderDetail.OrderId);
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName", orderDetail.ProductID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", orderDetail.ShopID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy", orderDetail.OrderId);
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName", orderDetail.ProductID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", orderDetail.ShopID);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderId,ProductID,ItemQty,Price,ShopID")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy", orderDetail.OrderId);
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName", orderDetail.ProductID);
            ViewBag.ShopID = new SelectList(db.WineShops, "Id", "ShopName", orderDetail.ShopID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
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
