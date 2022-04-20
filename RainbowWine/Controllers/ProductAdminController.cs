using PagedList;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using RainbowWine.Services.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Controllers
{
    public class ProductAdminController : Controller
    {
        // GET: ProductAdmin
        private rainbowwineEntities db = new rainbowwineEntities();
        ProductAdminDBO productAdminDBO = new ProductAdminDBO();
        [AuthorizeSpirit(Roles = "ProductAdmin")]
        // GET: ProductDetails
        public ActionResult Index(int? page,int ProductID = 0)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var prodlist = db.ProductDetails.Where(o => o.IsDelete == false);
            ViewBag.ProductDetailId = new SelectList(prodlist, "ProductID", "ProductName", ProductID);
            return View(productAdminDBO.ProductList(ProductID).ToPagedList(pageNumber,pageSize));
        }

        [AuthorizeSpirit(Roles = "ProductAdmin")]
        // GET: ProductDetails/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productDetail = productAdminDBO.ProductDetails(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.HostName = Request.Url.GetLeftPart(UriPartial.Authority);
            return View(productDetail);
        }

        [AuthorizeSpirit(Roles = "ProductAdmin")]
        // GET: ProductDetails/Create
        public ActionResult Create()
        {
            var ddl = productAdminDBO.ProductSizeList();
            ViewBag.ProductSize = new SelectList(ddl, "ProductSizeID", "Capacity");
            var ddlCat = productAdminDBO.ProductCategoryList();
            ViewBag.ProductCategory = new SelectList(ddlCat, "Category", "Category");
            return View();
        }

        [AuthorizeSpirit(Roles = "ProductAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductAdmin productAdmin)
        {

            var ddl = productAdminDBO.ProductSizeList();
            ViewBag.ProductSize = new SelectList(ddl, "ProductSizeID", "Capacity");
            var ddlCat = productAdminDBO.ProductCategoryList();
            ViewBag.ProductCategory = new SelectList(ddlCat, "Category", "Category");
            if (ModelState.IsValid)
            {
                var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                productAdmin.UserName = u.Email;
                productAdminDBO.AddProduct(productAdmin);
              
                return RedirectToAction("Index");
            }

            return View(productAdmin);
        }


        [AuthorizeSpirit(Roles = "ProductAdmin")]
        // GET: ProductDetails/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ddl = productAdminDBO.ProductSizeList();
            ViewBag.ProductSize = new SelectList(ddl, "ProductSizeID", "Capacity");
            var ddlCat = productAdminDBO.ProductCategoryList();
            ViewBag.ProductCategory = new SelectList(ddlCat, "Category", "Category");
            var productDetail = productAdminDBO.ProductDetails(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

       
        [AuthorizeSpirit(Roles = "ProductAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( ProductAdmin productAdmin)
        {
            var ddl = productAdminDBO.ProductSizeList();
            ViewBag.ProductSize = new SelectList(ddl, "ProductSizeID", "Capacity");
            var ddlCat = productAdminDBO.ProductCategoryList();
            ViewBag.ProductCategory = new SelectList(ddlCat, "Category", "Category");
            if (ModelState.IsValid)
            {
                var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                productAdmin.UserName = u.Email;
                productAdminDBO.UpdateProduct(productAdmin);
                return RedirectToAction("Index");
            }
           
            return View(productAdmin);
        }


        [AuthorizeSpirit(Roles = "ProductAdmin")]
        // GET: ProductDetails/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productDetail = productAdminDBO.DeleteProduct(id);
            if (productDetail == 0)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

    }
}