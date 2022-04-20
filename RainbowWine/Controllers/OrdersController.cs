using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RainbowWine.Data;
using RainbowWine.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RainbowWine.Services;
using System.Configuration;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RainbowWine.Services.PaytmService;
using System.Data.SqlClient;
using RainbowWine.Services.Filters;
using RainbowWine.Services.DBO;
using Microsoft.AspNet.Identity;
using System.Security.Cryptography;
using System.Net.Http;
using RainbowWine.Services.Gateway;
using RainbowWine.Services.Email;
using RainbowWine.Providers;
using SZModels;
using SZInfrastructure;
using RainbowWine.Models.Packers;
using SZData.Interfaces;
using static RainbowWine.Services.FireStoreService;
using Google.Cloud.Firestore;
using RainbowWine.Services.DO;
using System.Threading.Tasks;
using RainbowWine.Services.OnlinePaymentService;
using PagedList;

namespace RainbowWine.Controllers
{
    [AuthorizeSpirit]
    public class OrdersController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private rainbowwineEntities db = new rainbowwineEntities();

        public OrdersController()
        {
        }

        public OrdersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public ActionResult GetPrice(int productid)
        {
            string ret = default(string);
            var p = db.ProductDetails.Where(o => o.ProductID == productid)?.FirstOrDefault();
            if (p != null) ret = p.Price.ToString();

            return Content(ret);
        }

        public ActionResult SearchBarcode(string text)
        {
            var p = db.ProductBarcodeLinks.Where(o => o.BarcodeID.Contains(text)).Select(o => new { Id = o.Id, Name = o.BarcodeID });

            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchProduct(string text,int shopId)
        {
            var p = db.ProductDetails.Join(db.Inventories, pd => pd.ProductID, inv => inv.ProductID, (pd, inv) => new { ProductDetails = pd, Inventories = inv }).Where(o => o.ProductDetails.ProductName.Contains(text) && (o.ProductDetails.IsDelete == false) && (o.Inventories.ShopID == shopId)).Select(o => new { Id = o.ProductDetails.ProductID, Name = o.ProductDetails.ProductName, price = o.Inventories.MRP, IsMixer = 0 });
            //var p = db.ProductDetails.Where(o => o.ProductName.Contains(text) && (o.IsDelete == false)).Select(o => new { Id = o.ProductID, Name = o.ProductName, price = o.Price });
            

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchProductMaster(string text)
        {
            var p = db.Products.Where(o => o.ProductName.Contains(text) && (o.IsDelete == false)).Select(o => new { Id = o.Id, Name = o.ProductName });

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SearchShop(string text)
        {
            var p = db.WineShops.Where(o => o.ShopName.Contains(text)).Select(o => new { Id = o.Id, Name = o.ShopName });

            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchStatus(string text)
        {
            var p = db.OrderStatus.Where(o => o.OrderStatusName.Contains(text)).Select(o => new { Id = o.Id, Name = o.OrderStatusName });

            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchIssueStatus(string text)
        {
            var p = db.OrderIssueTypes.Where(o => o.IssueTypeName.Contains(text)).Select(o => new { Id = o.OrderIssueTypeId, Name = o.IssueTypeName });

            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchOrder(string text)
        {
            var p = db.Orders.Where(o => o.Id.ToString().Contains(text)).Select(o => new { Id = o.Id, Name = o.Id });

            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchIssue(string text)
        {
            var p = db.OrderIssues.Where(o => o.OrderIssueId.ToString().Contains(text)).Select(o => new { Id = o.OrderIssueId, Name = o.OrderIssueId });

            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchCustNumber(string text)
        {
            var p = db.Customers.Where(o => o.ContactNo.Contains(text) && o.RegisterSource=="w").Select(o => new { Id = o.Id, Name = o.ContactNo });

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchDeliveryAgent(string text)
        {
            var p = db.DeliveryAgents.Where(o => o.DeliveryExecName.Contains(text)).Select(o => new { Id = o.Id, Name = o.DeliveryExecName });

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetShopById(int custId)
        {
            WineShop wineShops = null;
            //Add address table for new customer
            var custAddress = db.CustomerAddresses.Where(o => o.CustomerId == custId)?.ToList();
            if (custAddress.Count() > 0)
            {
                var sid = custAddress[0].ShopId;
                wineShops = db.WineShops.Where(o => o.Id == sid)?.FirstOrDefault();
                return Json(new { Status = "OK", ShopId = wineShops.Id, ShopName = wineShops.ShopName }, JsonRequestBehavior.AllowGet);
            }
            //
            var order = db.Orders.Include(o => o.WineShop).Where(o => o.Customer.Id == custId)
                .OrderByDescending(o=>o.Id)
                .Take(1).ToList();
            if (order.Count > 0)
            {
                var ord = order[0];
                wineShops = db.WineShops.Where(o => o.Id == ord.ShopID)?.FirstOrDefault();
                return Json(new { Status = "OK", ShopId = wineShops.Id, ShopName = wineShops.ShopName }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { Status="FAIL", ShopId="", ShopName="" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Status = "FAIL", ShopId = "", ShopName = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchProductSelection(int productId, int shopId)
        {
            var p = db.ProductDetails.Where(o => o.ProductID== productId)?.FirstOrDefault();
            var invent = db.Inventories.Where(o => o.ShopID == shopId).ToList();
            var i = invent?.Select(o => o.ProductID).ToArray();

            if (p != null && i != null)
            {
                if (!i.Contains(p.ProductID))
                {
                    return Json(new { msg=$"{p.ProductName} not available.", qty = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DiscountDBO discountDBO = new DiscountDBO();
                    var prodDiscout= discountDBO.GetDiscountOnProduct(productId);
                    double discount = 0;
                    if (prodDiscout != null)
                    {
                        discount = prodDiscout.DiscountUnit ?? 0;
                    }

                    int qty = 0;
                    var i1 = invent.Where(o => o.ProductID == p.ProductID).FirstOrDefault();
                    qty = i1?.QtyAvailable ?? 0;
                    return Json(new { msg = $"{p.ProductName} available qty {qty}.",qty= qty, discount= discount }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (p != null && i == null)
            {
                return Json(new { msg = $"There is no inventory for the selected shop.", qty = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { msg = $"", qty = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchProductWithMixer(string text,int shopId)
        {
            GiftBagDBO giftBagDBO = new GiftBagDBO();
            var pr = db.ProductDetails.Join(db.Inventories,pd =>pd.ProductID,inv =>inv.ProductID,(pd,inv) => new { ProductDetails = pd,Inventories = inv }).Where(o => o.ProductDetails.ProductName.Contains(text) && (o.ProductDetails.IsDelete == false) && (o.Inventories.ShopID == shopId)).Select(o => new { Id = o.ProductDetails.ProductID, Name = o.ProductDetails.ProductName, price = o.Inventories.MRP, IsMixer = 0 ,IsGift =0 });
            var m = db.Mixers.Include(o => o.MixerDetails).Where(o => o.MixerName.Contains(text) && (o.IsDelete == false)).Select(o => new { Id = o.MixerId, Name = o.MixerName, price = o.MixerDetails.FirstOrDefault().Price, IsMixer = 1, IsGift = 0 });
            var gift = giftBagDBO.SearchGiftBag(shopId,text);
            var g = gift.Select(x => new
            {
                    Id =x.GiftBagDetailId,
                    Name =x.GiftBagName,
                    price =x.Price,
                    IsGift =x.IsGift,
                    IsMixer = 0
            });
            var p = new List<object>();
            p.AddRange(pr);
            p.AddRange(m);
            p.AddRange(g);

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchMixerSelection(int mixerId, int shopId)
        {
            var m = db.Mixers.Where(o => o.MixerId == mixerId)?.FirstOrDefault();
            var md = db.MixerDetails.Where(o => o.MixerId == mixerId)?.FirstOrDefault();
            var invent = db.InventoryMixers.Where(o => o.ShopId == shopId).ToList();
            var i = invent?.Select(o => o.MixerDetailId).ToArray();

            if (md != null && i != null)
            {
                if (!i.Contains(md.MixerId))
                {
                    return Json(new { msg = $"{m.MixerName} not available.", qty = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DiscountDBO discountDBO = new DiscountDBO();
                    var prodDiscout = discountDBO.GetDiscountOnProduct(mixerId);
                    double discount = 0;
                    if (prodDiscout != null)
                    {
                        discount = prodDiscout.DiscountUnit ?? 0;
                    }

                    int qty = 0;
                    var i1 = invent.Where(o => o.MixerDetailId == md.MixerDetailId).FirstOrDefault();
                    qty = i1?.Qty ?? 0;
                    return Json(new { msg = $"{m.MixerName} available qty {qty}.", qty = qty, discount = discount }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (md != null && i == null)
            {
                return Json(new { msg = $"There is no inventory for the selected shop.", qty = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { msg = $"", qty = 0 ,isMixer = 1}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchGiftBagSelection(int giftBagDetailId, int shopId)
        {
            GiftBagDBO giftBagDBO = new GiftBagDBO();
            var gd = giftBagDBO.GiftBagSelection(shopId, giftBagDetailId);
            if (gd != null)
            {
                if (!(gd.GiftBagDetailId > 0))
                {
                    return Json(new { msg = $"{gd.GiftBagName} not available.", qty = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    
                    int qty = 0;
                    qty = gd.Qty;
                    return Json(new { msg = $"{gd.GiftBagName} available qty {qty}.", qty = qty, discount = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (gd==null)
            {
                return Json(new { msg = $"There is no inventory for the selected shop.", qty = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { msg = $"", qty = 0, isMixer = 0,isGift =1 }, JsonRequestBehavior.AllowGet);
        }

        private void LogProductShop(int productid, int shopId, int orderId, int custId)
        {
            var p = db.ProductDetails.Where(o => o.ProductID == productid)?.FirstOrDefault();
            var i = db.Inventories.Where(o => o.ShopID == shopId).Select(o => o.ProductID).ToArray();

            if (p != null && i != null)
            {
                if (!i.Contains(p.ProductID))
                {
                    db.ProductLogs.Add(new ProductLog { OrderId = orderId, ProductId = productid, ShopId = shopId, CustomerId = custId, CreatedDate = DateTime.Now });
                    db.SaveChanges();
                }
            }
            else if (p != null && i == null)
            {
                db.ProductLogs.Add(new ProductLog { OrderId = orderId, ProductId = productid, ShopId = shopId, CustomerId = custId, CreatedDate = DateTime.Now });
                db.SaveChanges();
            }

        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult OrderItemUpdate(int OrderDetailsId, int ItemQty)
        {
            OrderDetail orderDetail = db.OrderDetails.Where(o => o.Id == OrderDetailsId)?.FirstOrDefault();
            Order ordStatus = db.Orders.Find(orderDetail.OrderId);
            if (ordStatus.OrderStatusId == 1)
            {
                //Added for Discount
                decimal discountTotalAmt = 0;
                double discountAmt = 0;
                if ((orderDetail.DiscountProductId ??0) >0)
                {
                    double discount = orderDetail.DiscountUnit ?? 0;
                    int q = ItemQty;
                    decimal p = orderDetail.Price;
                    decimal t = q * p;
                    t = decimal.Round(t, 2, MidpointRounding.AwayFromZero);
                    discountAmt = Math.Round((discount / 100) * Convert.ToDouble(t),2);
                    orderDetail.DiscountAmount = discountAmt;
                    t -= Convert.ToDecimal(discountAmt);
                    discountTotalAmt = decimal.Round(t, 2, MidpointRounding.AwayFromZero);

                }
                orderDetail.ItemQty = ItemQty;
                db.SaveChanges();
                int totalItems = 0;

                decimal totalAmt = 0;
                foreach (var item in db.OrderDetails.Where(o => o.OrderId == orderDetail.OrderId)?.ToList())
                {
                    int q = item.ItemQty;
                    totalItems += q;
                    decimal p = item.Price;
                    decimal t = q * p;

                    if ((item.DiscountProductId ?? 0) > 0)
                    {
                        t -= Convert.ToDecimal(item.DiscountAmount);
                    }

                    totalAmt += t;
                }
                Order or1 = db.Orders.Find(orderDetail.OrderId);
                or1.OrderAmount = totalAmt;
                db.SaveChanges();
                return Json(new
                {
                    OrderDetailsId = OrderDetailsId,
                    ItemQty = ItemQty,
                    ItemPrice = ItemQty * orderDetail.Price,
                    OrderAmount = totalAmt,
                    TotalItem = totalItems,
                    discountTotalAmt = discountTotalAmt,
                    discountAmt = discountAmt,
                    status = 0,
                    msg = "Success"
                });
            }
            else
            {
                return Json(new { OrderDetailsId = OrderDetailsId, ItemQty = ItemQty, status=1, msg= $"You can not update as order status is {ordStatus.OrderStatu.OrderStatusName}. " });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PaymentLink(int orderId)
        {
            Order order = db.Orders.Find(orderId);
            ViewBag.Img = order.PaymentQRImage;

            return View();
        }


        private void writeoutput(string req)
        {
            string text = "First line" + Environment.NewLine;

            // Set a variable to the Documents path.
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Guid newguid = Guid.NewGuid();
            string qrImagePtath = $"/Content/images/qrcodes/";
            string path = Server.MapPath($"~{qrImagePtath}");

            // Write the text to a new file named "WriteFile.txt".
            System.IO.File.WriteAllText(Path.Combine(docPath, "SetApprovePtm-{newguid.ToString()}.txt"), req);

            // Create a string array with the additional lines of text
            //string[] lines = { "New line 1", "New line 2" };

            // Append new lines of text to the file
            //System.IO.File.AppendAllLines(Path.Combine(docPath, "WriteFile.txt"), req);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetApprovePaytmIssue()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string pecodeValue = new StreamReader(req).ReadToEnd();
            string pdecode = Server.UrlDecode(pecodeValue);
            string pdecodevalue = Server.UrlDecode(pdecode);
            //Write to file

            PaytmSetApproveResponse decodevlaue = SpiritUtility.GetHooksDeserialize(pdecodevalue);
            int ptmLinkId = Convert.ToInt32(decodevlaue.MERC_UNQ_REF.Replace("LI_", ""));
            var paylog = db.PaymentLinkLogs.Where(o => o.PtmLinkId == ptmLinkId)?.FirstOrDefault();

            AppLogsPaytmHook appLogsPaytmHook = new AppLogsPaytmHook
            {
                CreateDate = DateTime.Now,
                InputPaytm = pdecodevalue,
                PtmLinkId = ptmLinkId,
                LinkOrderId = paylog.OrderId,
                PtmOrderId = decodevlaue.ORDERID,
                PtmStatus = decodevlaue.STATUS,
                CheckSumHash = decodevlaue.CHECKSUMHASH,
                TxnAmount = decodevlaue.TXNAMOUNT,
                TxnId = decodevlaue.TXNID,
                LinkNotes = decodevlaue.LINKNOTES,
                PtmRespMsg = decodevlaue.RESPMSG,
                MachineName = System.Environment.MachineName
            };
            db.AppLogsPaytmHooks.Add(appLogsPaytmHook);
            db.SaveChanges();

            //var objdata = HttpUtility.ParseQueryString(pdecodevalue);

            ////var decodevlaue = JsonConvert.DeserializeObject<PaytmSetApproveResponse>(pdecodevalue);
            //var decodevlaue =new PaytmSetApproveResponse {
            //    CHECKSUMHASH = objdata["CHECKSUMHASH"],
            //    LINKNOTES= objdata["LINKNOTES"],
            //     MERC_UNQ_REF= objdata["MERC_UNQ_REF"],
            //     STATUS=objdata["STATUS"],
            //     TXNAMOUNT=objdata["TXNAMOUNT"]
            //};

            if (string.Compare(decodevlaue.STATUS, "TXN_SUCCESS", true) == 0)
            {
                if (string.IsNullOrEmpty(decodevlaue.TXNAMOUNT))
                {
                    return Json(new { message = "amount is null" }, JsonRequestBehavior.AllowGet);
                }

                var ordshop = decodevlaue.LINKNOTES.Split('_');

                int or = Convert.ToInt32(ordshop[0]);
                
                Order order = db.Orders.Where(o => o.Id == or)?.FirstOrDefault();
                appLogsPaytmHook.OrderId = order.Id;
                db.SaveChanges();
                decimal orderAmt = 0;
                int issueId = 0;
                
                if (ordshop.Length == 4)
                {
                    issueId = Convert.ToInt32(ordshop[3]);
                    var issue = db.OrderIssues.Find(issueId);
                    orderAmt = Convert.ToDecimal(issue.AdjustAmt);
                }
                else
                {
                    orderAmt = order.OrderAmount;
                }
                decimal  amt = Convert.ToDecimal(decodevlaue.TXNAMOUNT);
                if (orderAmt == amt)
                {
                    order.OrderStatusId = 3;
                    db.SaveChanges();

                    var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = order.OrderStatusId,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    appLogsPaytmHook.SendStatus = "Approved";
                    db.SaveChanges();

                    int issueClosed = (int)IssueType.Closed;
                    var issue = db.OrderIssues.Find(issueId);
                    issue.OrderStatusId = issueClosed;
                    db.SaveChanges();

                    var issetrack = new OrderIssueTrack
                    {
                        OrderId = issue.OrderId,
                        OrderIssueId = issue.OrderIssueId,
                        Remark = "Payment Succesfull",
                        OrderStatusId = issueClosed,
                        TrackDate = DateTime.Now,
                        UserId = u.Id,
                    };
                    db.OrderIssueTracks.Add(issetrack);
                    db.SaveChanges();

                    WSendSMS wsms = new WSendSMS();
                    string textmsg = string.Format(ConfigurationManager.AppSettings["CFPartialPay"], Math.Abs(Convert.ToDouble(issue.AdjustAmt)), order.Id.ToString(), "");
                    wsms.SendMessage(textmsg, order.Customer.ContactNo);

                    return Json(new { message = "Order is Approved" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    appLogsPaytmHook.SendStatus = "ToBeRefunded";
                    db.SaveChanges();
                    PaytmPayment paytm = new PaytmPayment();
                    int reStatus = paytm.PaytmRefundApiCall(order.Id, true);


                    return Json(new { message = "To Be Refunded." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                appLogsPaytmHook.SendStatus = $"Paytm St {decodevlaue.STATUS}";
                db.SaveChanges();
                return Json(new { message = $"Status is {decodevlaue.STATUS}" }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetApprovePaytm()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string pecodeValue = new StreamReader(req).ReadToEnd();
            string pdecode = Server.UrlDecode(pecodeValue);
            string pdecodevalue = Server.UrlDecode(pdecode);
            //Write to file

            PaytmSetApproveResponse decodevlaue = SpiritUtility.GetHooksDeserialize(pdecodevalue);
            int ptmLinkId = Convert.ToInt32(decodevlaue.MERC_UNQ_REF.Replace("LI_", ""));
            var paylog = db.PaymentLinkLogs.Where(o => o.PtmLinkId == ptmLinkId)?.FirstOrDefault();

            AppLogsPaytmHook appLogsPaytmHook = new AppLogsPaytmHook
            {
                CreateDate = DateTime.Now,
                InputPaytm = pdecodevalue,
                PtmLinkId = ptmLinkId,
                LinkOrderId = paylog.OrderId,
                PtmOrderId = decodevlaue.ORDERID,
                PtmStatus = decodevlaue.STATUS,
                CheckSumHash = decodevlaue.CHECKSUMHASH,
                TxnAmount = decodevlaue.TXNAMOUNT,
                TxnId = decodevlaue.TXNID,
                LinkNotes = decodevlaue.LINKNOTES,
                PtmRespMsg = decodevlaue.RESPMSG,
                MachineName = System.Environment.MachineName
            };
            db.AppLogsPaytmHooks.Add(appLogsPaytmHook);
            db.SaveChanges();

            //var objdata = HttpUtility.ParseQueryString(pdecodevalue);

            ////var decodevlaue = JsonConvert.DeserializeObject<PaytmSetApproveResponse>(pdecodevalue);
            //var decodevlaue =new PaytmSetApproveResponse {
            //    CHECKSUMHASH = objdata["CHECKSUMHASH"],
            //    LINKNOTES= objdata["LINKNOTES"],
            //     MERC_UNQ_REF= objdata["MERC_UNQ_REF"],
            //     STATUS=objdata["STATUS"],
            //     TXNAMOUNT=objdata["TXNAMOUNT"]
            //};

            if (string.Compare(decodevlaue.STATUS, "TXN_SUCCESS", true) == 0)
            {
                if (string.IsNullOrEmpty(decodevlaue.TXNAMOUNT))
                {
                    return Json(new { message = "amount is null" }, JsonRequestBehavior.AllowGet);
                }

                var ordshop = decodevlaue.LINKNOTES.Split('_');

                int or = Convert.ToInt32(ordshop[0]);

                Order order = db.Orders.Where(o => o.Id == or)?.FirstOrDefault();
                appLogsPaytmHook.OrderId = order.Id;
                db.SaveChanges();

                decimal amt = Convert.ToDecimal(decodevlaue.TXNAMOUNT);

                if (order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
                {
                    order.OrderStatusId = 3;
                    db.SaveChanges();

                    var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = order.OrderStatusId,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    appLogsPaytmHook.SendStatus = "Approved";
                    db.SaveChanges();

                    WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                    string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                    wsms.SendMessage(textmsg, order.Customer.ContactNo);

                    return Json(new { message = "Order is Approved" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    appLogsPaytmHook.SendStatus = "ToBeRefunded";
                    db.SaveChanges();
                    PaytmPayment paytm = new PaytmPayment();
                    int reStatus = paytm.PaytmRefundApiCall(order.Id, true);


                    return Json(new { message = "To Be Refunded." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                appLogsPaytmHook.SendStatus = $"Paytm St {decodevlaue.STATUS}";
                db.SaveChanges();
                return Json(new { message = $"Status is {decodevlaue.STATUS}" }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetApproveCashFree()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            try
            {
                string pecodeValue = new StreamReader(req).ReadToEnd();
                //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                string pdecode = Server.UrlDecode(pecodeValue);
                string pdecodevalue = Server.UrlDecode(pdecode);
                //Write to file

                CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                var ret = paymentLinkLogsService.UpdateOrderToApprove(decodevlaue, pdecode);
                return Json(new { message = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
            //var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodevlaue.OrderId)?.FirstOrDefault();
            //bool addupdate = false;
            //if (appLogsCashFreeHook != null)
            //{
            //    string secret = ConfigurationManager.AppSettings["PayKey"];
            //    string data = "";

            //    data = data + decodevlaue.OrderId;
            //    data = data + decodevlaue.OrderAmount;
            //    data = data + decodevlaue.ReferenceId;
            //    data = data + decodevlaue.Status;
            //    data = data + decodevlaue.PaymentMode;
            //    data = data + decodevlaue.Msg;
            //    data = data + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    string signature = CreateToken(data, secret);
            //    if (decodevlaue.Signature == signature)
            //    {
            //    }

            //    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
            //    {
            //        addupdate = false;
            //    }
            //    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) != 0)
            //    {
            //        addupdate = true;
            //    }
            //}
            //else
            //{
            //    appLogsCashFreeHook = new AppLogsCashFreeHook
            //    {
            //        CreatedDate = DateTime.Now,
            //        VenderInput = pdecodevalue,
            //        MachineName = System.Environment.MachineName
            //    };
            //    db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
            //    db.SaveChanges();



            //    appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
            //    appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
            //    appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
            //    appLogsCashFreeHook.Status = decodevlaue.Status;
            //    appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
            //    appLogsCashFreeHook.Msg = decodevlaue.Msg;
            //    appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
            //    appLogsCashFreeHook.Signature = decodevlaue.Signature;
            //    appLogsCashFreeHook.MachineName = System.Environment.MachineName;
            //    db.SaveChanges();
            //}
            //if (!addupdate)
            //{
            //    if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
            //    {

            //        Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();

            //        decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);

            //        if (order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
            //        {
            //            order.OrderStatusId = 3;
            //            db.SaveChanges();

            //            var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
            //            OrderTrack orderTrack = new OrderTrack
            //            {
            //                LogUserName = u.Email,
            //                LogUserId = u.Id,
            //                OrderId = order.Id,
            //                StatusId = order.OrderStatusId,
            //                TrackDate = DateTime.Now
            //            };
            //            db.OrderTracks.Add(orderTrack);
            //            db.SaveChanges();

            //            appLogsCashFreeHook.SendStatus = "Approved";
            //            db.SaveChanges();

            //            WSendSMS wsms = new WSendSMS();
            //            //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
            //            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
            //            wsms.SendMessage(textmsg, order.Customer.ContactNo);

            //            return Json(new { message = "Order is Approved" }, JsonRequestBehavior.AllowGet);
            //        }
            //        else
            //        {
            //            appLogsCashFreeHook.SendStatus = "ShouldCheck";
            //            db.SaveChanges();
            //            PaytmPayment paytm = new PaytmPayment();
            //            int reStatus = paytm.PaytmRefundApiCall(order.Id, true);


            //            return Json(new { message = "ShouldCheck" }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //    else
            //    {
            //        appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
            //        db.SaveChanges();
            //        return Json(new { message = $"Cashfree is {decodevlaue.Status}" }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("orderssss/SetApproveCashFree")]
        public ActionResult SetApprovedCashFree()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            try
            {
                string pecodeValue = new StreamReader(req).ReadToEnd();
                //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                string pdecode = Server.UrlDecode(pecodeValue);
                string pdecodevalue = Server.UrlDecode(pdecode);
                //Write to file

                CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                var ret = paymentLinkLogsService.UpdateWalletOrderToApprove(decodevlaue, pdecode);
                return Json(new { message = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
            //var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodevlaue.OrderId)?.FirstOrDefault();
            //bool addupdate = false;
            //if (appLogsCashFreeHook != null)
            //{
            //    string secret = ConfigurationManager.AppSettings["PayKey"];
            //    string data = "";

            //    data = data + decodevlaue.OrderId;
            //    data = data + decodevlaue.OrderAmount;
            //    data = data + decodevlaue.ReferenceId;
            //    data = data + decodevlaue.Status;
            //    data = data + decodevlaue.PaymentMode;
            //    data = data + decodevlaue.Msg;
            //    data = data + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    string signature = CreateToken(data, secret);
            //    if (decodevlaue.Signature == signature)
            //    {
            //    }

            //    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
            //    {
            //        addupdate = false;
            //    }
            //    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) != 0)
            //    {
            //        addupdate = true;
            //    }
            //}
            //else
            //{
            //    appLogsCashFreeHook = new AppLogsCashFreeHook
            //    {
            //        CreatedDate = DateTime.Now,
            //        VenderInput = pdecodevalue,
            //        MachineName = System.Environment.MachineName
            //    };
            //    db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
            //    db.SaveChanges();



            //    appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
            //    appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
            //    appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
            //    appLogsCashFreeHook.Status = decodevlaue.Status;
            //    appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
            //    appLogsCashFreeHook.Msg = decodevlaue.Msg;
            //    appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
            //    appLogsCashFreeHook.Signature = decodevlaue.Signature;
            //    appLogsCashFreeHook.MachineName = System.Environment.MachineName;
            //    db.SaveChanges();
            //}
            //if (!addupdate)
            //{
            //    if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
            //    {

            //        Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();

            //        decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);

            //        if (order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
            //        {
            //            order.OrderStatusId = 3;
            //            db.SaveChanges();

            //            var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
            //            OrderTrack orderTrack = new OrderTrack
            //            {
            //                LogUserName = u.Email,
            //                LogUserId = u.Id,
            //                OrderId = order.Id,
            //                StatusId = order.OrderStatusId,
            //                TrackDate = DateTime.Now
            //            };
            //            db.OrderTracks.Add(orderTrack);
            //            db.SaveChanges();

            //            appLogsCashFreeHook.SendStatus = "Approved";
            //            db.SaveChanges();

            //            WSendSMS wsms = new WSendSMS();
            //            //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
            //            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
            //            wsms.SendMessage(textmsg, order.Customer.ContactNo);

            //            return Json(new { message = "Order is Approved" }, JsonRequestBehavior.AllowGet);
            //        }
            //        else
            //        {
            //            appLogsCashFreeHook.SendStatus = "ShouldCheck";
            //            db.SaveChanges();
            //            PaytmPayment paytm = new PaytmPayment();
            //            int reStatus = paytm.PaytmRefundApiCall(order.Id, true);


            //            return Json(new { message = "ShouldCheck" }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //    else
            //    {
            //        appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
            //        db.SaveChanges();
            //        return Json(new { message = $"Cashfree is {decodevlaue.Status}" }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [Route("aggrepay/callback")]
        public ActionResult AggrePayCallBack(FormCollection form)
        {
            var req = Request.Form;
           
            if (req != null)
            {
                try
                {

                    CallBackWebHookResponse callBackWebHookResponse = new CallBackWebHookResponse();
                    callBackWebHookResponse.order_id = form["order_id"];
                    callBackWebHookResponse.amount = form["amount"];
                    callBackWebHookResponse.currency = form["currency"];
                    callBackWebHookResponse.description = form["description"];
                    callBackWebHookResponse.name = form["name"];
                    callBackWebHookResponse.email = form["email"];
                    callBackWebHookResponse.phone = form["phone"];
                    callBackWebHookResponse.address_line_1 = form["address_line_1"];
                    callBackWebHookResponse.address_line_2 = form["address_line_2"];
                    callBackWebHookResponse.city = form["city"];
                    callBackWebHookResponse.state = form["state"];
                    callBackWebHookResponse.country = form["country"];
                    callBackWebHookResponse.zip_code = form["zip_code"];
                    callBackWebHookResponse.udf1 = form["udf1"];
                    callBackWebHookResponse.udf2 = form["udf2"];
                    callBackWebHookResponse.udf3 = form["udf3"];
                    callBackWebHookResponse.udf4 = form["udf4"];
                    callBackWebHookResponse.udf5 = form["udf5"];
                    callBackWebHookResponse.transaction_id = form["transaction_id"];
                    callBackWebHookResponse.payment_mode = form["payment_mode"];
                    callBackWebHookResponse.payment_channel = form["payment_channel"];
                    callBackWebHookResponse.payment_datetime = form["payment_datetime"];
                    callBackWebHookResponse.response_code =Convert.ToInt32( form["response_code"]);
                    callBackWebHookResponse.response_message = form["response_message"];
                    callBackWebHookResponse.error_desc = form["error_desc"];
                    callBackWebHookResponse.cardmasked = form["cardmasked"];
                    callBackWebHookResponse.hash = form["hash"];
                    var serialiseValue = JsonConvert.SerializeObject(callBackWebHookResponse);
                    AggrePaymentController.LogResult(serialiseValue);
                    AggrePaymentController aggePaymentController = new AggrePaymentController();
                    if (!string.IsNullOrEmpty(callBackWebHookResponse.udf5))
                    {
                        aggePaymentController.UpdateOrderIssueToApprove(callBackWebHookResponse, serialiseValue, "AggrePay");
                    }
                    else
                    {
                        aggePaymentController.OnlineOrderToApprove(callBackWebHookResponse, serialiseValue, "AggrePay");
                    }

                }
                catch (Exception ex)
                {

                    AggrePaymentController.LogResult(ex.Message + " -" + ex.StackTrace);
                }
            }
            return Json("ok");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("orderss/SetApproveCashFree")]
        public ActionResult SetApprovedCashFreee()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            try
            {
                string pecodeValue = new StreamReader(req).ReadToEnd();
                //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                string pdecode = Server.UrlDecode(pecodeValue);
                string pdecodevalue = Server.UrlDecode(pdecode);
                //Write to file

                CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                var ret = paymentLinkLogsService.UpdateWalletOrderToApprove(decodevlaue, pdecode);
                return Json(new { message = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
            //var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodevlaue.OrderId)?.FirstOrDefault();
            //bool addupdate = false;
            //if (appLogsCashFreeHook != null)
            //{
            //    string secret = ConfigurationManager.AppSettings["PayKey"];
            //    string data = "";

            //    data = data + decodevlaue.OrderId;
            //    data = data + decodevlaue.OrderAmount;
            //    data = data + decodevlaue.ReferenceId;
            //    data = data + decodevlaue.Status;
            //    data = data + decodevlaue.PaymentMode;
            //    data = data + decodevlaue.Msg;
            //    data = data + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    string signature = CreateToken(data, secret);
            //    if (decodevlaue.Signature == signature)
            //    {
            //    }

            //    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
            //    {
            //        addupdate = false;
            //    }
            //    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) != 0)
            //    {
            //        addupdate = true;
            //    }
            //}
            //else
            //{
            //    appLogsCashFreeHook = new AppLogsCashFreeHook
            //    {
            //        CreatedDate = DateTime.Now,
            //        VenderInput = pdecodevalue,
            //        MachineName = System.Environment.MachineName
            //    };
            //    db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
            //    db.SaveChanges();



            //    appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
            //    appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
            //    appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
            //    appLogsCashFreeHook.Status = decodevlaue.Status;
            //    appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
            //    appLogsCashFreeHook.Msg = decodevlaue.Msg;
            //    appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
            //    appLogsCashFreeHook.Signature = decodevlaue.Signature;
            //    appLogsCashFreeHook.MachineName = System.Environment.MachineName;
            //    db.SaveChanges();
            //}
            //if (!addupdate)
            //{
            //    if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
            //    {

            //        Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();

            //        decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);

            //        if (order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
            //        {
            //            order.OrderStatusId = 3;
            //            db.SaveChanges();

            //            var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
            //            OrderTrack orderTrack = new OrderTrack
            //            {
            //                LogUserName = u.Email,
            //                LogUserId = u.Id,
            //                OrderId = order.Id,
            //                StatusId = order.OrderStatusId,
            //                TrackDate = DateTime.Now
            //            };
            //            db.OrderTracks.Add(orderTrack);
            //            db.SaveChanges();

            //            appLogsCashFreeHook.SendStatus = "Approved";
            //            db.SaveChanges();

            //            WSendSMS wsms = new WSendSMS();
            //            //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
            //            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
            //            wsms.SendMessage(textmsg, order.Customer.ContactNo);

            //            return Json(new { message = "Order is Approved" }, JsonRequestBehavior.AllowGet);
            //        }
            //        else
            //        {
            //            appLogsCashFreeHook.SendStatus = "ShouldCheck";
            //            db.SaveChanges();
            //            PaytmPayment paytm = new PaytmPayment();
            //            int reStatus = paytm.PaytmRefundApiCall(order.Id, true);


            //            return Json(new { message = "ShouldCheck" }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //    else
            //    {
            //        appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
            //        db.SaveChanges();
            //        return Json(new { message = $"Cashfree is {decodevlaue.Status}" }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ordersss/SetApproveCashFree")]
        public ActionResult SSetApprovedCashFreee()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            try
            {
                string pecodeValue = new StreamReader(req).ReadToEnd();
                //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                string pdecode = Server.UrlDecode(pecodeValue);
                string pdecodevalue = Server.UrlDecode(pdecode);
                //Write to file

                CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                var ret = paymentLinkLogsService.UpdateWalletOrderToApprove(decodevlaue, pdecode);
                return Json(new { message = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //int orderIdDecode = Convert.ToInt32(decodevlaue.OrderId);
            //var appLogsCashFreeHook = db.AppLogsCashFreeHooks.Where(o => o.OrderId == decodevlaue.OrderId)?.FirstOrDefault();
            //bool addupdate = false;
            //if (appLogsCashFreeHook != null)
            //{
            //    string secret = ConfigurationManager.AppSettings["PayKey"];
            //    string data = "";

            //    data = data + decodevlaue.OrderId;
            //    data = data + decodevlaue.OrderAmount;
            //    data = data + decodevlaue.ReferenceId;
            //    data = data + decodevlaue.Status;
            //    data = data + decodevlaue.PaymentMode;
            //    data = data + decodevlaue.Msg;
            //    data = data + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    string signature = CreateToken(data, secret);
            //    if (decodevlaue.Signature == signature)
            //    {
            //    }

            //    if (string.Compare(appLogsCashFreeHook.Status, "SUCCESS", true) == 0)
            //    {
            //        addupdate = false;
            //    }
            //    else if (string.Compare(decodevlaue.Status, "SUCCESS", true) != 0)
            //    {
            //        addupdate = true;
            //    }
            //}
            //else
            //{
            //    appLogsCashFreeHook = new AppLogsCashFreeHook
            //    {
            //        CreatedDate = DateTime.Now,
            //        VenderInput = pdecodevalue,
            //        MachineName = System.Environment.MachineName
            //    };
            //    db.AppLogsCashFreeHooks.Add(appLogsCashFreeHook);
            //    db.SaveChanges();



            //    appLogsCashFreeHook.OrderId = decodevlaue.OrderId;
            //    appLogsCashFreeHook.OrderAmount = decodevlaue.OrderAmount;
            //    appLogsCashFreeHook.ReferenceId = decodevlaue.ReferenceId;
            //    appLogsCashFreeHook.Status = decodevlaue.Status;
            //    appLogsCashFreeHook.PaymentMode = decodevlaue.PaymentMode;
            //    appLogsCashFreeHook.Msg = decodevlaue.Msg;
            //    appLogsCashFreeHook.TxtTime = decodevlaue.TxtTime;
            //    appLogsCashFreeHook.Signature = decodevlaue.Signature;
            //    appLogsCashFreeHook.MachineName = System.Environment.MachineName;
            //    db.SaveChanges();
            //}
            //if (!addupdate)
            //{
            //    if (string.Compare(decodevlaue.Status, "SUCCESS", true) == 0)
            //    {

            //        Order order = db.Orders.Where(o => o.Id == orderIdDecode)?.FirstOrDefault();

            //        decimal amt = Convert.ToDecimal(decodevlaue.OrderAmount);

            //        if (order.OrderAmount == amt && (order.OrderStatusId == 2 || order.OrderStatusId == 16))
            //        {
            //            order.OrderStatusId = 3;
            //            db.SaveChanges();

            //            var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
            //            OrderTrack orderTrack = new OrderTrack
            //            {
            //                LogUserName = u.Email,
            //                LogUserId = u.Id,
            //                OrderId = order.Id,
            //                StatusId = order.OrderStatusId,
            //                TrackDate = DateTime.Now
            //            };
            //            db.OrderTracks.Add(orderTrack);
            //            db.SaveChanges();

            //            appLogsCashFreeHook.SendStatus = "Approved";
            //            db.SaveChanges();

            //            WSendSMS wsms = new WSendSMS();
            //            //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
            //            string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
            //            wsms.SendMessage(textmsg, order.Customer.ContactNo);

            //            return Json(new { message = "Order is Approved" }, JsonRequestBehavior.AllowGet);
            //        }
            //        else
            //        {
            //            appLogsCashFreeHook.SendStatus = "ShouldCheck";
            //            db.SaveChanges();
            //            PaytmPayment paytm = new PaytmPayment();
            //            int reStatus = paytm.PaytmRefundApiCall(order.Id, true);


            //            return Json(new { message = "ShouldCheck" }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //    else
            //    {
            //        appLogsCashFreeHook.SendStatus = $"Cashfree St {decodevlaue.Status}";
            //        db.SaveChanges();
            //        return Json(new { message = $"Cashfree is {decodevlaue.Status}" }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReSendPaytmLink(int? id)
        {            
            if (id == null)
            {
                return Json(new { message = "Order id is null" }, JsonRequestBehavior.AllowGet);
            }
            Order ord1 = db.Orders.Where(o => o.Id == id)?.FirstOrDefault();
            if (ord1 == null)
            {
                return Json(new { message = "There is no such Order Id.", status="1" }, JsonRequestBehavior.AllowGet);
            }

            var paymentLinkLog = db.PaymentLinkLogs.Where(o => o.OrderId == ord1.Id)?.ToList();
            if (paymentLinkLog.Count<=0)
            {
                return Json(new { message = "Paytm url is not generate.", status = "2" }, JsonRequestBehavior.AllowGet);
            }

            int maxlimit =Convert.ToInt32(ConfigurationManager.AppSettings["PtmMaxLimitReSend"]);
            if(paymentLinkLog.Count >= maxlimit)
            {
                return Json(new { message = "Max limit cross resend link.", status = "3" }, JsonRequestBehavior.AllowGet);
            }
            PaytmPayment pay = new PaytmPayment();
            pay.PaytmAPiCall(ord1.Id, true);

            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ReSendBulkPaytmLink(string bulkDate)
        {
            if (string.IsNullOrWhiteSpace(bulkDate))
            {
                return Json(new { message = "Selected date is blank", status = "1" }, JsonRequestBehavior.AllowGet);
            }

            DateTime selDate = Convert.ToDateTime(bulkDate);

            if (selDate.Year <= 2018)
            {
                return Json(new { message = "Please select year greater than 2018.", status = "1" }, JsonRequestBehavior.AllowGet);
            }

            var order = db.Orders.Where(o => o.OrderStatusId == 2 && o.TestOrder == true && o.OrderDate == selDate);
            int maxlimit = Convert.ToInt32(ConfigurationManager.AppSettings["PtmMaxLimitReSend"]);
            foreach (var item in order)
            {
                var paymentLinkLog = db.PaymentLinkLogs.Where(o => o.OrderId == item.Id)?.ToList();

                if (paymentLinkLog.Count >= maxlimit)
                { }
                else
                {
                    PaytmPayment pay = new PaytmPayment();
                    pay.PaytmAPiCall(item.Id, true);
                }
            }

            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SendAllPaytmLink()
        {
            var order = db.Orders.Where(o => o.OrderStatusId == 2 && o.TestOrder == false);
            //var order = db.Orders.Where(o => o.Id == 131 || o.Id == 132);
            int maxlimit = Convert.ToInt32(ConfigurationManager.AppSettings["PtmMaxLimitReSend"]);
            foreach (var item in order)
            {
                bool operationFlag = item.WineShop.OperationFlag ?? false;
                if (operationFlag)
                {
                    PaytmPayment pay = new PaytmPayment();
                    pay.PaytmAPiCall(item.Id);

                    var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = item.Id,
                        StatusId =21,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                }
            }
            db.SaveChanges();

            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SendDefaulter()
        {
            WSendSMS wSendSMS = new WSendSMS();

            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            DataSet ds = new DataSet();
            DataTable dataTable = null;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string sqlstmmmt = "select * from orders where OrderStatusId=2 and TestOrder=0 and YEAR(OrderDate)=2020 and Month(OrderDate)=05 and day(OrderDate)=30";

                using (SqlCommand cmd = new SqlCommand(sqlstmmmt, con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds, "Order");
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                dataTable = ds.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    int oid = Convert.ToInt32(row["Id"]);
                    string txt = ConfigurationManager.AppSettings["SMSSubmitted"] + " Please ignore this message if you have already paid for the order.";
                    string str = string.Format(txt, row["Id"].ToString());
                    wSendSMS.SendMessage(str, row["OrderTo"].ToString());
                }
            }
            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SendDefaulter2()
        {
            WSendSMS wSendSMS = new WSendSMS();

            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
            DataSet ds = new DataSet();
            DataTable dataTable = null;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string sqlstnt = "select top 250 * from [dbo].[orders] where id in ("
                + " select orderid from [dbo].[PaymentLinkLog]"
                + " where YEAR(CreatedDate) = 2020"
                + " and Month(CreatedDate) = 05"
                + " and day(CreatedDate) = 30"
                + " and orderid not in (select orderid from [dbo].[AppLogsPaytmHook] where YEAR(CreateDate) = 2020"
                + " and Month(CreateDate) = 05"
                + " and day(CreateDate) = 30 and orderid is not null))";

                using (SqlCommand cmd = new SqlCommand(sqlstnt, con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds, "Order");
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                dataTable = ds.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    int oid = Convert.ToInt32(row["Id"]);
                    PaytmPayment pay = new PaytmPayment();
                    pay.PaytmAPiCall(oid);

                    var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = oid,
                        StatusId = 21,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);

                }
            }
            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }

//        private void bulkSQL()
//        {
//            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
//            DataSet ds = new DataSet();
//            DataTable dataTable = null;
//            using (SqlConnection con = new SqlConnection(constr))
//            {
//                string sqlstnt = "select top 250 * from orders where id in("
//+ "select orderid from[dbo].[PaymentLinkLog]"
//+ " where YEAR(CreatedDate) = 2020"
//+ " and Month(CreatedDate) = 05"
//+ " and day(CreatedDate) = 30"
//+ " and orderid not in (select orderid from AppLogsPaytmHook where YEAR(CreateDate) = 2020"
//+ " and Month(CreateDate) = 05"
//+ " and day(CreateDate) = 30 and orderid is not null))";

//                string sqlstmmmt = "select * from orders where OrderStatusId=2 and TestOrder=0 and YEAR(OrderDate)=2020 and Month(OrderDate)=05 and day(OrderDate)=30";

//                using (SqlCommand cmd = new SqlCommand(sqlstmmmt, con))
//                {
//                    using (var da = new SqlDataAdapter(cmd))
//                    {
//                        da.Fill(ds, "Order");
//                    }
//                }
//            }
//            if (ds.Tables.Count > 0)
//            {
//                WSendSMS wSendSMS = new WSendSMS();
//                dataTable = ds.Tables[0];
//                foreach (DataRow row in dataTable.Rows)
//                {
//                    int oid = Convert.ToInt32(row["Id"]);
//                    string txt = ConfigurationManager.AppSettings["SMSSubmitted"] + " Please ignore this message if you have already paid for the order.";
//                    string str = string.Format(txt, row["Id"].ToString());
//                    wSendSMS.SendMessage1(str, row["OrderTo"].ToString());

//                    //PaytmPayment pay = new PaytmPayment();
//                    //pay.PaytmAPiCall(oid);

//                    //var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
//                    //OrderTrack orderTrack = new OrderTrack
//                    //{
//                    //    LogUserName = User.Identity.Name,
//                    //    LogUserId = u.Id,
//                    //    OrderId = oid,
//                    //    StatusId = 21,
//                    //    TrackDate = DateTime.Now
//                    //};
//                    //db.OrderTracks.Add(orderTrack);

//                }
//            }

//        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SendPaytmLink(int id)
        {
            var order = db.Orders.Find(id);
            PaytmPayment pay = new PaytmPayment();
            pay.PaytmAPiCall(order.Id);
            return Json(new { message = "Success", status = "0" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetApprove(string orderId, string amount)
        {

            if (string.IsNullOrEmpty(orderId))
            {
                return Json(new { message = "Order is null" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(amount))
            {
                return Json(new { message = "amount is null" }, JsonRequestBehavior.AllowGet);
            }
            int or = 0;
            if (orderId.Contains("_"))
            {
                Order ord1 = db.Orders.Where(o => o.NewOrderId == orderId)?.FirstOrDefault();
                or = ord1.Id;
            }
            else
            {
                or = Convert.ToInt32(orderId);
            }
            decimal amt = Convert.ToDecimal(amount);
            Order order = db.Orders.Find(or);

            if (order.OrderAmount == amt && order.OrderStatusId==2)
            {
                order.OrderStatusId = 3;
                db.SaveChanges();

                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                OrderTrack orderTrack = new OrderTrack
                {
                    LogUserName = u.Email,
                    LogUserId = u.Id,
                    OrderId = order.Id,
                    StatusId = order.OrderStatusId,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();

                WSendSMS wsms = new WSendSMS();
                //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString(), DateBetweenText());
                string textmsg = string.Format(ConfigurationManager.AppSettings["SMSApproved"], order.Id.ToString());
                wsms.SendMessage(textmsg, order.Customer.ContactNo);

                return Json(new { message = "Order is Approved"}, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message="Amount not matched."}, JsonRequestBehavior.AllowGet);
        }
        private string DateBetweenText()
        {
            string dtTime = default(string);

            var nowTime = DateTime.Now;
            var nowHrs = nowTime.Hour;
            if (nowHrs >= 9 && nowHrs < 11)
            {
                dtTime = $"10 am to 01 pm";
            }
            else if (nowHrs >= 11 && nowHrs < 15)
            {
                dtTime = $"01 pm to 04 pm";
            }
            else if (nowHrs >= 15 && nowHrs < 17)
            {
                dtTime = $"04 pm to 06:30 pm";
            }
            else if (nowHrs >= 17 && nowHrs < 19)
            {
                dtTime = $"10 am to 01 pm tomorrow";
            }
            else
            {
                dtTime = $"10 am to 01 pm tomorrow";
            }
            return dtTime;
        }

        [AuthorizeSpirit(Roles = "Agent, Shopper, Packer, DeliveryManager, Hub, SalesManager")]
        [StopAction]
        public ActionResult Index(int shopId=0, string shopname="",  int statusId=0, string statusname="", int soid=0, string custno="")
        {

            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o=>o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            IList<Order> orders = new List<Order>();
            bool isSearch = false;


            if (shopId > 0)
            {
                orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                    .Where(o => o.ShopID == shopId && (!o.TestOrder) && (o.OrderGroupType == null || o.OrderGroupType == MixerType.beverage.ToString()))
                    .OrderByDescending(o => o.OrderDate)
                        .Take(300).ToList();
                isSearch = true;
            }
            if (statusId > 0)
            {
                if (orders.Count > 0) orders = orders.Where(o => o.OrderStatusId == statusId).ToList();
                else orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => o.OrderStatusId == statusId && (!o.TestOrder) && (o.OrderGroupType == null || o.OrderGroupType == MixerType.beverage.ToString()))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(200).ToList();
                isSearch = true;

            }
            if (soid > 0)
            {
                if (orders.Count > 0) orders = orders.Where(o => o.Id == soid).ToList();
                else orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => o.Id == soid && (o.OrderGroupType == null || o.OrderGroupType == MixerType.beverage.ToString())).OrderByDescending(o => o.OrderDate).ToList();
                isSearch = true;
            }
            if (!string.IsNullOrWhiteSpace(custno))
            {
                if (orders.Count > 0) orders = orders.Where(o => string.Compare(o.OrderTo, custno,true)==0).ToList();
                else orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o=>string.Compare(o.OrderTo, custno, true) == 0 && (o.OrderGroupType == null || o.OrderGroupType == MixerType.beverage.ToString())).OrderByDescending(o => o.OrderDate).ToList();
                isSearch = true;
            }

            if (user.UserType1.UserTypeName.ToLower() == "agent")
            {


                //if (orders.Count > 0) orders = orders.Where(o => o.OrderStatusId == 1 || o.OrderStatusId == 2).OrderByDescending(o => o.Id).ToList();
                //else if(!isSearch) orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                //        .Where(o => o.OrderStatusId == 1 || o.OrderStatusId == 2 && (!o.TestOrder))
                //        .OrderByDescending(o => o.Id)
                //        .Take(1000).ToList();

            }
            else if (user.UserType1.UserTypeName.ToLower() == "packer")
            {
                var defaultStatuid = (statusId == 3 || statusId == 6) ? statusId : 3;

                if (orders.Count > 0)
                {
                    orders = orders.Where(o => o.OrderStatusId == defaultStatuid && o.ShopID == user.ShopId)?.OrderBy(o => o.Id).ToList();
                }
                else if (!isSearch)
                {
                    orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => o.OrderStatusId == defaultStatuid && o.ShopID == user.ShopId && (!o.TestOrder) && (o.OrderGroupType == null || o.OrderGroupType == MixerType.beverage.ToString()))
                        .OrderBy(o => o.Id)
                        .Take(200).ToList();
                }
            }
            else if (user.UserType1.UserTypeName.ToLower() == "deliver")
            {
                return RedirectToAction("Delivery"); ;
            }
            else
            {
                if (orders.Count > 0) orders = orders.ToList();
                else if (!isSearch) orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => (!o.TestOrder) && (o.OrderGroupType == null || o.OrderGroupType == MixerType.beverage.ToString()))
                        .OrderByDescending(o=>o.Id)
                        .Take(200).ToList();
            }
            if (orders.Count > 0)
            {
                int ordId = orders.FirstOrDefault().Id;
                var etas = db.CustomerEtas.Where(o => o.OrderId == ordId);
                if (etas.Count() > 0)
                {
                    if (etas.FirstOrDefault().CommittedTime != null && etas.FirstOrDefault().CommittedTimeEnd != null)
                    {
                        orders.FirstOrDefault().EtaStartTime = etas.FirstOrDefault().CommittedTime;
                        orders.FirstOrDefault().EtaEndTime = etas.FirstOrDefault().CommittedTimeEnd;
                    }
                }
               
            }
            OrderDBO orderDBO = new OrderDBO();
            var internalUser = orderDBO.GetInterUserContactNo(u.Id);
            var shop = db.WineShops.ToList();
            var status = db.OrderStatus.ToList();
            ViewBag.ShopName = shopname;
            ViewBag.StatusName = statusname;
            ViewBag.soid = soid;
            ViewBag.CustNo = custno;
            ViewBag.InterUserContNo = internalUser;
            ViewBag.ShopId = new SelectList(shop, "Id", "ShopName", shopId);
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", statusId);
            ViewBag.UserType = user.UserType1.UserTypeName;
            if (TempData["msg"] != null)
            {
                ViewBag.Msg = TempData["msg"];
            }
            
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            if (TempData["MessageCall"] != null)
            {
                ViewBag.MessageCall = TempData["MessageCall"];
            }
            return View(orders);
        }

        [AuthorizeSpirit(Roles = "Agent, Shopper, DeliveryManager, DeliverySubManager, DeliverySupervisor, SalesManager,Packer")]
        [StopAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.ProductDetail).Include(o => o.WineShop).Where(o=>o.OrderId==id)?.ToList();
            var mixerDetails = db.MixerOrderItems.Include(o => o.Order).Include(o => o.MixerDetail).Include(o => o.MixerDetail.Mixer).Include(o=>o.WineShop).Include(o=>o.Supplier)
                .Where(o=>o.OrderId==id).ToList();
            OrderDBO orderDBO = new OrderDBO();
            GiftBagDBO giftBagDBO = new GiftBagDBO();
            var giftDetails = giftBagDBO.GetGiftBagOrderItems(id.Value);
            var recipient = giftBagDBO.GetUserRecipientDetails(id.Value);
            var result = orderDBO.GetGoodiesOrderDetails(id.Value);
            foreach (var item in result)
            {
                if (item.GName != null)
                {
                    orderDetails.Where(x => x.Id == item.OrderDetailId).FirstOrDefault().GName = item.GName;
                }
            }


            if (order == null)
            {
                return HttpNotFound();
            }

            var prepost = new[] {
                 new { Id=0,Text="PreOrder"},
                 new { Id=1,Text="Normal Order"}
             };
            Customer customer = null;
            if (order.CustomerId > 0)
            {
                customer = db.Customers.Find(order.CustomerId);
                if (customer.CustomerAddresses.Count() > 0)
                {
                    var address = db.CustomerAddresses.Find(order.CustomerAddressId);
                    //var address = customer.CustomerAddresses.Where(o=>o.ShopId==order.ShopID)?.FirstOrDefault();
                    if (address != null)
                    {
                        customer.Address = address.Address;
                        customer.FormattedAddress = address.FormattedAddress;
                        customer.Flat = address.Flat;
                        customer.Landmark = address.Landmark;
                    }
                }
                else customer = null;
            }
            if (customer == null)
            {
                customer = order.Customer;
            }
            ViewBag.PrePost = order.PreOrder == null ? 0 : order.PreOrder;
            ViewBag.delpickvalue = string.IsNullOrWhiteSpace(order.DeliveryPickup) ? "Delivery" : order.DeliveryPickup;
            ViewBag.mobileTypevalue = string.IsNullOrWhiteSpace(order.PaymentDevice) ? "Android" : order.PaymentDevice;
            ViewBag.overAllDiscount = (order.DiscountUnit==null) ? 0 : order.DiscountUnit;

            var shop = db.WineShops.Where(o => o.Id == order.ShopID).ToList();
            var i = db.Inventories.Where(o => o.ShopID == order.ShopID).Select(o => o.ProductID).ToArray();
            var prod = db.ProductDetails.Where(o => i.Contains(o.ProductID));
            var smsent = db.CustomerSMSSends.Where(o => o.OrderId == order.Id && o.CustomerId == order.CustomerId)?.FirstOrDefault();
            var emailsent = db.CustomerEmailSends.Where(o => o.OrderId == order.Id && o.CustomerId == order.CustomerId)?.OrderByDescending(o=>o.Id).FirstOrDefault();
            string lastEmailsentTo = emailsent != null ? emailsent.EmailId : "";

            ViewBag.PaySentEmail = lastEmailsentTo;
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy");
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName");

            int pay = (int)OrderPaymentType.POO;
            int payocod = (int)OrderPaymentType.OCOD;
            var selPayOption = (pay == order.PaymentTypeId) ? "online" : (payocod == order.PaymentTypeId) ? "cash" : "";

            ViewBag.delPayOption = selPayOption;
        
            var app = orderDBO.GetAppVersion(order.Id);
            if (app != null)
            {
                order.AppVersion = app.AppVersion;
                order.AppPlatForm = app.AppPlatForm;
            }
            if (recipient != null)
            {
                order.RecipientName = recipient.RecipientName;
                order.RecipientNumber = recipient.RecipientNumber;
            }
            var refund = orderDBO.GetRefundDetails(order.Id);

            if (refund != null)
            {
                order.ProcessedDate = refund.ProcessedDate;
                order.ARNNumber = refund.ARNNumber;
            }
            if (TempData["RefundSMS"] !=null)
            {
                ViewBag.RefundSMS = TempData["RefundSMS"];
            }
            ViewBag.PayOption = new SelectList(GetPayOptionList(),"Value","Text", selPayOption);
            ViewBag.ShopID = new SelectList(shop, "Id", "ShopName");
            return View(new OrderDetailsView { Ord = order, CurOrdDetail = orderDetails, MixerItems= mixerDetails, Cust = customer, SmsSent = smsent, GiftBagItems = giftDetails });
        }
        private List<SelectListItem>  GetPayOptionList()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="online",Text="Online"},
                 new SelectListItem{ Value="cash",Text="Cash"}
             };
            myList = data.ToList();
            return myList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSpirit(Roles = "Agent, Shopper, SalesManager, SalesManager")]
        [StopAction]
        public ActionResult Details(int id, OrderDetailsView order, string txtdelpick, string txtpayoption, double txtoverAllDiscount = 0)//, string txtmobileType, string txtorderPrePost)
        {
            string msg = default(string);
            string ostatus= default(string);
            if (ModelState.IsValid)
            {
                Order ordStatus = db.Orders.Find(id);
                ostatus = ordStatus.OrderStatu.OrderStatusName;
                if (ordStatus.OrderStatusId == 1)
                {
                    //Added for Discount
                    DiscountDBO discountDBO = new DiscountDBO();
                    var prodDiscout = discountDBO.GetDiscountOnProduct(order.OrdDetail.ProductID);
                    if (prodDiscout != null)
                    {
                        double discount = prodDiscout.DiscountUnit ?? 0;
                        order.OrdDetail.DiscountProductId = prodDiscout.DiscountProductId;
                        order.OrdDetail.DiscountUnit = discount;

                        int q = order.OrdDetail.ItemQty;
                        decimal p = order.OrdDetail.Price;
                        decimal t = q * p;
                        order.OrdDetail.DiscountAmount = (discount / 100) * Convert.ToDouble(t);
                    }

                    order.OrdDetail.OrderId = id;
                    db.OrderDetails.Add(order.OrdDetail);
                    db.SaveChanges();

                    decimal totalAmt = 0;
                    int disTypeID = 0;
                    foreach (var item in db.OrderDetails.Where(o => o.OrderId == id)?.ToList())
                    {
                        int q = item.ItemQty;
                        decimal p = item.Price;
                        decimal t = q * p;

                        if ((item.DiscountProductId ?? 0) > 0)
                        {
                            disTypeID = (int)DiscountType.ProductItem;
                            t -= Convert.ToDecimal(item.DiscountAmount);
                        }

                        totalAmt += t;
                    }
                    Order or1 = db.Orders.Find(id);
                    or1.OrderAmount = totalAmt;
                    if (disTypeID > 0) or1.DiscountTypeId = disTypeID;
                    //or1.PreOrder = string.IsNullOrWhiteSpace(txtorderPrePost) ? Convert.ToInt16(0) : Convert.ToInt16(txtorderPrePost);
                    or1.DeliveryPickup = string.IsNullOrWhiteSpace(txtdelpick) ? "Delivery" : txtdelpick;
                    //or1.PaymentDevice = string.IsNullOrWhiteSpace(txtmobileType) ? "Android" : txtmobileType;

                    int payPoo = (int)OrderPaymentType.POO;
                    int payOCOD = (int)OrderPaymentType.OCOD;
                    or1.PaymentTypeId = (string.Compare(txtpayoption, "cash", true) == 0) ? payOCOD : payPoo;

                    db.SaveChanges();

                    UpdateOverallDiscountItem(txtoverAllDiscount, or1.Id,false);

                    msg = "Item added";
                }
                else
                {
                    msg = $"You can not update as order status is {ostatus}. ";
                }
            }
            return RedirectToAction("Details", new { id = id, msg = msg });


            //var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.ProductDetail).Include(o => o.WineShop).Where(o => o.OrderId == id)?.ToList();

            //Order ord = db.Orders.Find(id);
            //order.Ord = ord;
            //order.CurOrdDetail = orderDetails;

            //var shop = db.WineShops.Where(o => o.Id == ord.ShopID).ToList();
            //ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy");
            //ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName");
            //ViewBag.ShopID = new SelectList(shop, "Id", "ShopName");

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSpirit(Roles = "Agent, Shopper, SalesManager")]
        [StopAction]
        public ActionResult SubmitOrder(int id, string delpick, string dppayoption, double overAllDiscount=0)//, string mobileType, string orderPrePost)
        {
            try
            {
                Order order = db.Orders.Find(id);
                var orderDetails = db.OrderDetails.Include(o => o.ProductDetail).Where(o => o.OrderId == id)?.ToList();
                List<string> prodNames = new List<string>();
                List<string> prodAval = new List<string>();
                List<int> prodAvalId = new List<int>();
                List<Inventory> inventories = new List<Inventory>();
                string stockvalue = default(string);
                string productAval = default(string);

                var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();

                var otrack = db.OrderTracks.Where(o => o.OrderId== id && o.StatusId == 2)?.FirstOrDefault();
                if (otrack == null)
                {
                    if (orderDetails.Count > 0)
                    {
                        bool operationFlag = order.WineShop.OperationFlag ?? false;
                        //if (string.Compare(orderPrePost, "1", true) == 0 && string.Compare(user.UserType1.UserTypeName,"shopper",true)!=0)
                        //if (string.Compare(orderPrePost, "1", true) == 0 || operationFlag)

                        if (operationFlag)
                        {
                            foreach (var item in db.OrderDetails.Where(o => o.OrderId == id)?.ToList())
                            {
                                var invent = db.Inventories.Where(o => (o.ProductID == item.ProductID) && (o.ShopID == item.ShopID))?.FirstOrDefault();
                                if (invent == null)
                                {
                                    prodAval.Add($"{item.ProductDetail.ProductName}");
                                    prodAvalId.Add(item.ProductDetail.ProductID);
                                }
                                else
                                {
                                    if (invent.QtyAvailable < item.ItemQty)
                                    {
                                        prodNames.Add($"{item.ProductDetail.ProductName}-{invent.QtyAvailable}");
                                    }
                                    else
                                    {
                                        if (string.Compare(dppayoption, "cash", true) == 0)
                                        {
                                            var beforeqty = invent.QtyAvailable;
                                            invent.QtyAvailable = invent.QtyAvailable - item.ItemQty;
                                            var afterqty = invent.QtyAvailable;
                                            db.InventoryTracks.Add(new InventoryTrack { ProductID = invent.ProductID, ShopID = invent.ShopID, CreatedDate = DateTime.Now, QtyAvailBefore = beforeqty, QtyAvailAfter = afterqty, ModifiedDate = DateTime.Now, ChangeSource = 12, Order_Id = id });
                                            invent.LastModified = System.DateTime.Now;
                                            invent.LastModifiedBy = u.Email;
                                            inventories.Add(invent);
                                        }
                                    }
                                }
                            }
                        }
                        if (prodNames.Count <= 0 && prodAval.Count <= 0)
                        {
                            //var amt = order.OrderAmount;
                            //if (overAllDiscount>0)
                            //{
                            //    int discountType = (int)DiscountType.OverAll;
                            //    order.DiscountTypeId = discountType;
                            //    order.DiscountUnit = overAllDiscount;
                            //    var discountTotal = OverallDiscountItem(Convert.ToDecimal(overAllDiscount), amt);
                            //    order.DiscountAmount = discountTotal;
                            //    order.OrderAmount = amt - Convert.ToDecimal(discountTotal);
                            //}
                            //else
                            //{
                            //    order.OrderAmount=amt;
                            //}

                            int payPoo = (int)OrderPaymentType.POO;
                            int payOCOD = (int)OrderPaymentType.OCOD;
                            order.OrderStatusId = 2;
                            order.PaymentTypeId = (string.Compare(dppayoption, "cash", true) == 0) ? payOCOD : payPoo;
                            //order.PreOrder = Convert.ToInt16(orderPrePost);
                            order.DeliveryPickup = string.IsNullOrWhiteSpace(delpick) ? "Delivery" : delpick;
                            //order.PaymentDevice = string.IsNullOrWhiteSpace(mobileType) ? "Android" : mobileType;
                            db.SaveChanges();

                            UpdateOverallDiscountItem(overAllDiscount, order.Id,true);

                            OrderTrack orderTrack = new OrderTrack
                            {
                                LogUserName = User.Identity.Name,
                                LogUserId = u.Id,
                                OrderId = order.Id,
                                StatusId = order.OrderStatusId,
                                TrackDate = DateTime.Now
                            };
                            db.OrderTracks.Add(orderTrack);
                            db.SaveChanges();

                            //if (string.Compare(orderPrePost, "0", true) == 0)
                            //{
                            //    WSendSMS wsms = new WSendSMS();
                            //    string textmsg = string.Format(ConfigurationManager.AppSettings["SMSPreOrderSubmit"], DateTime.Now.ToString(), order.Id.ToString());
                            //    wsms.SendMessage(textmsg, order.Customer.ContactNo);
                            //}
                            //else //if (string.Compare(orderPrePost, "1", true) == 0)
                            //{

                            //var msgsms = (string.Compare(dppayoption, "cash", true) == 0) ? 
                            //    string.Format(ConfigurationManager.AppSettings["SMSSubmittedForCash"], order.Id.ToString())
                            //    : string.Format(ConfigurationManager.AppSettings["SMSSubmitted"], order.Id.ToString());

                            //WSendSMS wsms = new WSendSMS();
                            //string textmsg = msgsms;
                            //wsms.SendMessage(textmsg, order.Customer.ContactNo);

                            //Flow SMS
                            var dicti = new Dictionary<string, string>();
                            dicti.Add("ORDERID", order.Id.ToString());

                            var templeteid = (string.Compare(dppayoption, "cash", true) == 0) ?
                             ConfigurationManager.AppSettings["SMSSubmittedForCashFlowId"]
                             : ConfigurationManager.AppSettings["SMSSubmittedFlowId"];

                            Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.Customer.ContactNo, dicti));
                            //End Flow SMS

                            //}

                            if (operationFlag)
                            {
                                //GeneratePayLinkAll(order.Id, false); //old bharatpay url link,
                                //Start: Paytm url
                                //PaytmPayment pay = new PaytmPayment();
                                //pay.PaytmAPiCall(order.Id);
                                //REmoved Paytm gateway and added Cashfree
                                if (string.Compare(dppayoption, "cash", true) == 0)
                                {
                                    //int stapproved = (int)OrderStatusEnum.Approved;

                                    //order.OrderStatusId = stapproved;
                                    //db.SaveChanges();

                                    //orderTrack = new OrderTrack
                                    //{
                                    //    LogUserName = User.Identity.Name,
                                    //    LogUserId = u.Id,
                                    //    OrderId = order.Id,
                                    //    StatusId = stapproved,
                                    //    TrackDate = DateTime.Now,
                                    //    Remark="Customer Paid Cash on Delivery."
                                    //};
                                    //db.OrderTracks.Add(orderTrack);
                                    //db.SaveChanges();
                                }
                                else
                                {
                                    PaymentStrategy pay = new PaymentStrategy(new TransactionCashFree());
                                    var paym = pay.MakePayment(new CashFreeModel
                                    {
                                        OrderId = order.Id.ToString(),
                                        OrderAmount = order.OrderAmount.ToString(),
                                        CustomerEmail = u.Email,
                                        CustomerPhone = order.OrderTo
                                    });
                                    //End: Paytm url
                                    //int stCashLink = (int)OrderStatusEnum.CashLinkSend;
                                    //orderTrack = new OrderTrack
                                    //{
                                    //    LogUserName = User.Identity.Name,
                                    //    LogUserId = u.Id,
                                    //    OrderId = order.Id,
                                    //    StatusId = stCashLink,
                                    //    TrackDate = DateTime.Now
                                    //};
                                    //db.OrderTracks.Add(orderTrack);
                                    //db.SaveChanges();
                                }
                            }

                            return RedirectToAction("Create", "CustomerOrder", new { msg = "Order Submitted successfully." });
                        }
                        else
                        {
                            if (prodNames.Count > 0)
                            {
                                stockvalue = string.Join(",", prodNames);
                                stockvalue = $"Stock Available - {stockvalue}";
                            }

                            if (prodAval.Count > 0)
                            {
                                //if (string.Compare(orderPrePost, "1", true) == 0)
                                //{
                                foreach (var item in prodAvalId)
                                {
                                    LogProductShop(item, order.ShopID ?? 0, order.Id, order.CustomerId);

                                }
                                //}
                                productAval = string.Join(",", prodAval);
                                productAval = $"Product not available at {productAval}";
                            }
                            return RedirectToAction("Details", new { id = id, error = $"{stockvalue} {productAval}" });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Details", new { id = id, error = $"Order already submitted." });
                }
            }
            catch (Exception ex)
            {
                db.AppLogs.Add(new AppLog
                {
                    CreateDatetime = DateTime.Now,
                    Error = "SubmitOrder Method: " + ex.Message,
                    Message = ex.StackTrace,
                    MachineName = System.Environment.MachineName
                });
                db.SaveChanges();
                throw ex;
            }
            return RedirectToAction("Details", new { id = id, msg = $"notvalid." });
            //var shop = db.WineShops.Where(o => o.Id == order.ShopID).ToList();
            //ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy");
            //ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName");
            //ViewBag.ShopID = new SelectList(shop, "Id", "ShopName");
            //OrderDetailsView orderview = new OrderDetailsView();
            //orderview.Ord = order;
            //orderview.CurOrdDetail = orderDetails;
            //return View("Details", orderview);
        }

        private void UpdateOverallDiscountItem(double overAllDiscount, int orderId, bool isUpdateOnDb)
        {
            var order = db.Orders.Find(orderId);
            if (overAllDiscount > 0)
            {
                int discountType = (int)DiscountType.OverAll;
                var amt = order.OrderAmount;
                order.DiscountTypeId = discountType;
                order.DiscountUnit = overAllDiscount;
                var discountTotal = (overAllDiscount / 100) * Convert.ToDouble(amt);
                order.DiscountAmount = discountTotal;
                if (isUpdateOnDb) order.OrderAmount =Math.Round(amt - Convert.ToDecimal(discountTotal));
            }
            else
            {
                order.DiscountTypeId = null;
                order.DiscountUnit = 0;
                order.DiscountAmount = 0;
            }
            db.SaveChanges();
        }

        private void GeneratePayLink(int orderId)
        {
            Order order = db.Orders.Find(orderId);
            bool operationFlag = order.WineShop.OperationFlag ?? false;
            if (operationFlag && string.IsNullOrWhiteSpace(order.PaymentQRImage))
            {
                if (string.Compare(order.PaymentDevice, "ios", true) == 0)
                {
                    WSendSMS wsms = new WSendSMS();
                    wsms.iOSPaymentLink(order.Id, false);
                    var hostName = ConfigurationManager.AppSettings["PtmSpiritUrl"];//Request.Url.GetLeftPart(UriPartial.Authority);
                    string payurl = $"{hostName}/orders/paymentlink?orderId={order.Id}";
                    string textmsg1 = string.Format(ConfigurationManager.AppSettings["SMSiOSPayLink"], order.Customer.CustomerName, order.OrderAmount.ToString(), order.Id.ToString(), payurl);
                    wsms.SendMessage(textmsg1, order.Customer.ContactNo);
                }
            }
        }

        private void GeneratePayLinkAll(int orderId, bool bulk)
        {
            Order order = db.Orders.Find(orderId);
            bool operationFlag = order.WineShop.OperationFlag ?? false;
            if (operationFlag)
            {
                WSendSMS wsms = new WSendSMS();
                wsms.iOSPaymentLink(order.Id, bulk);
                var hostName = ConfigurationManager.AppSettings["PtmSpiritUrl"];//Request.Url.GetLeftPart(UriPartial.Authority);
                string payurl = $"{hostName}/orders/paymentlink?orderId={order.Id}";
                string textmsg1 = string.Format(ConfigurationManager.AppSettings["SMSiOSPayLink"], order.Customer.CustomerName, order.OrderAmount.ToString(), order.Id.ToString(), payurl);
                wsms.SendMessage(textmsg1, order.Customer.ContactNo);
            }
        }

        public ActionResult SendPaymentLink(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            GeneratePayLinkAll(order.Id,true);
            ViewBag.Img = order.PaymentQRImage;

            return View();
        }
        public ActionResult SendPaymentLinkall(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            GeneratePayLinkAll(order.Id, true);
            ViewBag.Img = order.PaymentQRImage;

            return View();
        }

        public ActionResult SendPaymentLinkAuto()
        {
            //var allorders = db.Orders.Where(o => o.TestOrder == false && o.OrderStatusId == 2 && (o.PaymentQRImage == null)).ToList();
            var allorders = db.Orders.Where(o => o.TestOrder == false && o.OrderStatusId == 2 && (o.NewOrderId == null)).ToList();
            foreach (var item in allorders)
            {
                GeneratePayLinkAll(item.Id, true);
            }
            return View();
        }


        [HttpGet]
        public ActionResult RemoveItem(int id, int oid,double overalldiscount=0)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
            db.SaveChanges();

            decimal totalAmt = 0;
            foreach (var item in db.OrderDetails.Where(o => o.OrderId == oid)?.ToList())
            {
                int q = item.ItemQty;
                decimal p = item.Price;
                decimal t = q * p;
                totalAmt += t;
            }
            Order or1 = db.Orders.Find(oid);
            or1.OrderAmount = totalAmt;
            db.SaveChanges();
            UpdateOverallDiscountItem(overalldiscount, or1.Id,false);
            return RedirectToAction("Details", new { id = oid });
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "CustomerName");
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "OrderStatusName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderDate,OrderPlacedBy,OrderTo,OrderAmount,CustomerId,OrderStatusId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "CustomerName", order.CustomerId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "OrderStatusName", order.OrderStatusId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Order order = db.Orders.Find(id);
            OrderDBO orderDBO = new OrderDBO();
            var order = orderDBO.GetOrderEditDetails(id.Value);
            if (order == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "CustomerName", order.CustomerId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "OrderStatusName", order.OrderStatusId);
            return View(order);
        }

        public ActionResult Call(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            //var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            OrderDBO orderDBO = new OrderDBO();
            var contactDeatails = orderDBO.GetContactDetails(id.Value, u.Id);
            if (contactDeatails != null )
            {
                if (string.IsNullOrEmpty(contactDeatails.InterUserContactNo))
                {
                   TempData["MessageCall"] = "Internal user contact number is not available in Ruser";
                    return RedirectToAction("Index");
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {

                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["CallToken"]);
                        //var resp = client.GetAsync(ConfigurationManager.AppSettings["CallingUrl"] + contactDeatails.CustContactNo + "/" + contactDeatails.InterUserContactNo).Result;
                        //var ret = resp.Content.ReadAsStringAsync().Result;
                        var callr = new CallRequestDO();
                        callr.callFlowConfiguration.initiateCall_1.participants[0].participantAddress = contactDeatails.InterUserContactNo;
                        callr.callFlowConfiguration.initiateCall_1.participants[0].participantName = "Agent Name";
                        callr.callFlowConfiguration.addParticipant_1.participants[0].participantAddress = contactDeatails.CustContactNo;
                        var json = JsonConvert.SerializeObject(callr);

                        var c = SZIoc.GetSerivce<ISZConfiguration>();

                        var AirtelIQUsername = c.GetConfigValue(ConfigEnums.AirtelIQUsername.ToString());
                        var AirtelIQPassword = c.GetConfigValue(ConfigEnums.AirtelIQPassword.ToString());

                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        var byteArray = Encoding.ASCII.GetBytes($"{AirtelIQUsername}:{AirtelIQPassword}");
                        var base64Auth = Convert.ToBase64String(byteArray);
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

                        var response = client.PostAsync("https://openapi.airtel.in/gateway/airtel-iq/v2/execute/workflow", stringContent).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            //return Json(new { Data = ret }, JsonRequestBehavior.AllowGet);
                            TempData["MessageCall"] = "You will recieve a call shortly";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["MessageCall"] = "External api call failed";
                            return RedirectToAction("Index");
                        }
                        
                    }

                }
                
            }
            return Json(new { Data = contactDeatails, message = "Contact Details" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CallOptionOrderIssue(int? id,int issueId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var issueOrders = db.OrderIssues.Include(o => o.Order).Where(o => o.OrderIssueId == id)?.FirstOrDefault();
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            //var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            OrderDBO orderDBO = new OrderDBO();
            var contactDeatails = orderDBO.GetContactDetails(id.Value, u.Id);
            if (contactDeatails != null)
            {
                if (string.IsNullOrEmpty(contactDeatails.InterUserContactNo))
                {
                    TempData["MessageCall"] = "Internal user contact number is not available in Ruser";
                    return RedirectToAction("IssueDetail", "CustomerOrder", new { id = issueId });
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {

                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["CallToken"]);
                        //var resp = client.GetAsync(ConfigurationManager.AppSettings["CallingUrl"] + contactDeatails.CustContactNo + "/" + contactDeatails.InterUserContactNo).Result;
                        var callr = new CallRequestDO();
                        callr.callFlowConfiguration.initiateCall_1.participants[0].participantAddress = contactDeatails.InterUserContactNo;
                        callr.callFlowConfiguration.initiateCall_1.participants[0].participantName = "Agent Name";
                        callr.callFlowConfiguration.addParticipant_1.participants[0].participantAddress = contactDeatails.CustContactNo;
                        var json = JsonConvert.SerializeObject(callr);

                        var c = SZIoc.GetSerivce<ISZConfiguration>();

                        var AirtelIQUsername = c.GetConfigValue(ConfigEnums.AirtelIQUsername.ToString());
                        var AirtelIQPassword = c.GetConfigValue(ConfigEnums.AirtelIQPassword.ToString());

                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        var byteArray = Encoding.ASCII.GetBytes($"{AirtelIQUsername}:{AirtelIQPassword}");
                        var base64Auth = Convert.ToBase64String(byteArray);
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

                        var response = client.PostAsync("https://openapi.airtel.in/gateway/airtel-iq/v2/execute/workflow", stringContent).Result;
                        //var ret = response./*Content*/.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //return Json(new { Data = ret }, JsonRequestBehavior.AllowGet);
                            TempData["MessageCall"] = "You will recieve a call shortly";
                            return RedirectToAction("IssueDetail","CustomerOrder",new { id= issueId });
                        }
                        else
                        {
                            TempData["MessageCall"] = "External api call failed";
                            return RedirectToAction("IssueDetail", "CustomerOrder", new { id = issueId });
                        }

                    }

                }

            }
            return Json(new { Data = contactDeatails, message = "Contact Details" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Order order = db.Orders.Find(id);
            OrderDBO orderDBO = new OrderDBO();
            var order = orderDBO.GetOrderEditDetails(id.Value);
            if (order == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "CustomerName", order.CustomerId);
            //ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "OrderStatusName", order.OrderStatusId);
            ViewBag.ShopID = new SelectList(db.WineShops.Where(x =>x.OperationFlag == true), "Id", "ShopName", order.ShopID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderDate,OrderPlacedBy,OrderTo,OrderAmount,CustomerId,OrderStatusId")] Order order)
        {
            if (ModelState.IsValid)
            {
                var ordStatusInventoryRevert = new[] { OrderStatusEnum.Cancel,
                    OrderStatusEnum.Refunded, OrderStatusEnum.CancelByCustomer, OrderStatusEnum.PODOrderCancel, OrderStatusEnum.DelCancelByCustomer,
                    OrderStatusEnum.OrderModifyRefunded
                    };
               
                //var arryRevert = Array.ConvertAll<OrderStatusEnum, int>(ordStatusInventoryRevert, (v) => (int)v);
                var arryRevert = ordStatusInventoryRevert.Cast<int>().ToArray();

                if (arryRevert.Contains(order.OrderStatusId))
                {
                    PaymentLinkLogsService payService = new PaymentLinkLogsService();
                    payService.RevertInventory(order.Id);
                    payService.RevertMixerInventory(order.Id);
                }

                Order ord = db.Orders.Find(order.Id);
                var cust = db.Customers.Where(x => x.Id == ord.CustomerId).FirstOrDefault();
             
                ord.OrderStatusId = order.OrderStatusId;
                db.SaveChanges();
               
                var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                OrderTrack orderTrack = new OrderTrack
                {
                    LogUserName = User.Identity.Name,
                    LogUserId = u.Id,
                    OrderId = order.Id,
                    StatusId = order.OrderStatusId,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();
                var delOrderCount = db.OrderTracks.Where(x => x.StatusId == 5 && x.OrderId ==order.Id).ToList();
                if (delOrderCount.Count() > 1)
                {

                    TempData["Message"] = $"Already your order has been delivered OrderId {order.Id}";
                    return RedirectToAction("Index");
                }
                FireStoreAccess fireStoreAccess = new FireStoreAccess();
                OrderDBO orderDBO = new OrderDBO();

                if (order.OrderStatusId == 5 || order.OrderStatusId == 37 || order.OrderStatusId == 48 || order.OrderStatusId == 68 || order.OrderStatusId == 3 || (order.OrderStatusId == 2 && order.PaymentTypeId == 2) || (order.OrderStatusId == 6))
                {
                    //HyperTracking Complted
                    HyperTracking hyperTracking = new HyperTracking();
                    Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(order.Id));
                   

                }
                if (order.OrderStatusId == 3 || (order.OrderStatusId == 2 && order.PaymentTypeId == 2) || (order.OrderStatusId == 6) || (order.OrderStatusId == 9))
                {
                    //Live Tracking FireStore
                    CustomerApi2Controller.AddToFireStore(order.Id);


                }
               
                if (order.OrderStatusId == 6)
                {
                    WebEngageController webEngageController = new WebEngageController();
                    Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Order Packed"));

                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    Task.Run(async () => await fcmHelper.SendFirebaseNotification(order.Id, FirebaseNotificationHelper.NotificationType.Order));
                }

                if (order.OrderStatusId == 5 || order.OrderStatusId == 37 || order.OrderStatusId == 48 || order.OrderStatusId == 68  || (order.OrderStatusId == 27))
                {
                    //Live Tracking FireStore
                    CustomerApi2Controller.DeleteToFireStore(order.Id);
                    orderDBO.UpdatedScheduledOrder(order.Id);
                }

                if (order.OrderStatusId == (int)OrderStatusEnum.Delivered)
                {
                    ord.DeliveryDate = DateTime.Now;
                    db.SaveChanges();
                    var routePlan = db.RoutePlans.Where(a => a.OrderID == order.Id).FirstOrDefault();
                    if (routePlan != null)
                    {
                        var rUser = db.RUsers.Where(b => b.DeliveryAgentId == routePlan.DeliveryAgentId && b.UserType == 5).FirstOrDefault();
                        var deliveryEarningService = SZIoc.GetSerivce<IDeliveryEarningService>();
                   
                        var earn = deliveryEarningService.AddEarningNew(rUser.rUserId, order.Id);
                    }
                    if (ord.IsGift.HasValue && ord.IsGift.Value)
                    {

                        //SMS Flow
                        //var cust = db.Customers.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                        GiftBagDBO giftBagDBO = new GiftBagDBO();
                        var reci = giftBagDBO.GetUserRecipientOrderDetails(order.Id);
                        DeliveryAgents2Controller deliveryAgents2Controller = new DeliveryAgents2Controller();
                        if ((reci != null) && (!string.IsNullOrEmpty(reci.Occasion) || !string.IsNullOrEmpty(reci.SplMessage)))
                        {
                            var smsGiftUrl = ConfigurationManager.AppSettings["GiftSmsurl"].ToString();
                            
                            smsGiftUrl = smsGiftUrl + "?mobile=" + reci.ContactNo + "&token=" + GiftRecipientController.Encrypt(reci.ContactNo) + "&orderId=" + order.Id;
                            var smsUrl = deliveryAgents2Controller.ShrinkURL(smsGiftUrl);
                            var res = giftBagDBO.UpdateUserRecipientOrder(order.Id, smsUrl);
                            //var link1 = smsUrl.Substring(0, 28);
                            //var link2 = smsUrl.Substring(28, smsUrl.Length - 28);
                            var dictii = new Dictionary<string, string>();
                            dictii.Add("ORDERID", order.Id.ToString());
                            dictii.Add("DATE", DateTime.Now.ToString("dd-MM-yyyy HH:MM"));
                            dictii.Add("SENDER", reci.CustomerName);
                            dictii.Add("LINK", smsUrl);
                            //dict.Add("LINK1", link2);

                            var tempid = ConfigurationManager.AppSettings["SMSGiftRecipientFlowId"];

                            Task.Run(async () => await Services.Msg91.Msg91Service.SendRecipientFlowSms(tempid, reci.ContactNo, dictii));
                            //END SMS Flow
                        }
                        else
                        {
                            var smsGiftUrl = ConfigurationManager.AppSettings["GiftSmsurl"].ToString();
                            var appdownloadlink = ConfigurationManager.AppSettings["Appdownloadlink"].ToString();
                            //smsGiftUrl = smsGiftUrl + "?mobile=" + reci.ContactNo + "&token=" + GiftRecipientController.Encrypt(reci.ContactNo) + "&orderId=" + order.Id;
                            var smsUrl = deliveryAgents2Controller.ShrinkURL(appdownloadlink);
                            var res = giftBagDBO.UpdateUserRecipientOrder(order.Id, smsUrl);
                            var dict = new Dictionary<string, string>();
                            dict.Add("OrderID", order.Id.ToString());
                            dict.Add("Date", DateTime.Now.ToString("dd-MM-yyyy HH:MM"));
                            dict.Add("Sender", reci.CustomerName);
                            dict.Add("DownloadLink", smsUrl);
                            var tempid = ConfigurationManager.AppSettings["SMSGiftAppdownloadlinkFlowId"];

                            Task.Run(async () => await Services.Msg91.Msg91Service.SendRecipientFlowSms(tempid, reci.ContactNo, dict));
                        }
                    }

                    WebEngageController webEngageController = new WebEngageController();
                    Task.Run(async () => await webEngageController.WebEngageStatusCall(order.Id, "Order Delivered"));


                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    int pageId = (int)PageNameEnum.MYWALLET;
                    string pageVersion = "1.6.2";
                    var cont = SZIoc.GetSerivce<IPageService>();
                    var content = cont.GetPageContent(pageId, pageVersion);
                    var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
                    var c = SZIoc.GetSerivce<ISZConfiguration>();
                    var numberOfOrder = c.GetConfigValue(ConfigEnums.NumberOfOrder.ToString());
                    var ordCount = db.Orders.Where(o => o.CustomerId == ord.CustomerId && o.OrderStatusId == 5).ToList();
                    bool isFirsrOrder = true;
                    if (ordCount.Count() > Convert.ToInt32(numberOfOrder))
                    {
                        isFirsrOrder = false;

                    }
                    var ser = SZIoc.GetSerivce<IPromoCodeService>();
                    if (isFirsrOrder)
                    {
                        var discount = ser.GetDiscountOnFirstOrder(cust.UserId);
                        if (discount.CreditAmount > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = discount.CreditAmount + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = discount.CreditAmount + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = discount.CustomerId;
                            walletNotificationRequest.UserID = discount.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }

                    var promocode = db.PromoCodes.Where(o => o.PromoId == ord.PromoId).FirstOrDefault();
                   
                    if (ord.PromoId.HasValue && ord.PromoId > 0)
                    {

                        var promoCashback = ser.PromoCodeCashBack(ord.Id);
                       
                        if (promoCashback > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = promoCashback + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = promoCashback + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = ord.CustomerId;
                            walletNotificationRequest.UserID = ord.Customer.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }
                    var cashBackApplicable = c.GetConfigValue(ConfigEnums.CashBackApplicable.ToString());

                    bool isCashBackApplicable = false;
                    if (cashBackApplicable == "1")
                    {
                        isCashBackApplicable = true;

                    }
                    if (isCashBackApplicable)
                    {
                        var Cashback = ser.CashBack(ord.Id);
                        if (Cashback.CashBackAmount > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = Cashback.CashBackAmount + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = Cashback.CashBackAmount + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = ord.CustomerId;
                            walletNotificationRequest.UserID = cust.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }

                    var dicti = new Dictionary<string, string>();
                    dicti.Add("ORDERID", order.Id.ToString());
                    dicti.Add("DATETIME", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));

                    var templeteid = ConfigurationManager.AppSettings["SMSDeliveredFlowId"];
                    
                    Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, ord.OrderTo, dicti));
                    //End Flow
                    Task.Run(async () => await fcmHelper.SendFirebaseNotification(ord.Id, FirebaseNotificationHelper.NotificationType.Order));
                }
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "CustomerName", order.CustomerId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "OrderStatusName", order.OrderStatusId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit1([Bind(Include = "Id,OrderDate,OrderPlacedBy,OrderTo,OrderAmount,CustomerId,OrderStatusId,ShopID")] Order order,int shopId = 0)
        {
            if (ModelState.IsValid)
            {
                var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                OrderDBO orderDBO = new OrderDBO();
                 int result = orderDBO.UpdatedShop(order.Id,order.ShopID.Value,u.Id, User.Identity.Name);
                if (result == 1)
                {
                    TempData["msg"] = "Updated Successfully";
                }
                else
                {
                    TempData["msg"] = "Please Select Different Shop It Is Same Shop";
                }

                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "CustomerName", order.CustomerId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "OrderStatusName", order.OrderStatusId);
            return View(order);
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AuthorizeSpirit(Roles = "Shopper, Packer")]
        public ActionResult IssueOrderList(int statusId = 0, string statusname = "", int soid = 0, int issuestatusId = 0, string issuestatusname = "")
        {
            string custId = User.Identity.GetUserId();
            var rUser = db.RUsers.Where(o => o.rUserId == custId)?.FirstOrDefault();
            List<OrderIssue> issueOrders = new List<OrderIssue>();
            issueOrders = db.OrderIssues.Include(o => o.Order).Where(o => o.Order.ShopID == (rUser.ShopId ?? 0))?.ToList();
            if (soid > 0) {
                issueOrders = db.OrderIssues.Include(o => o.Order).Where(o => o.Order.ShopID == (rUser.ShopId ?? 0) && o.OrderId == soid)?.ToList();
            }
            if (statusId > 0) {
                issueOrders = issueOrders.Where(o => o.Order.OrderStatusId == statusId)?.ToList();
            }
            if (statusId > 0)
            {
                issueOrders = issueOrders.Where(o => o.Order.OrderStatusId == statusId)?.ToList();
            }
            if (issuestatusId > 0)
            {
                issueOrders = issueOrders.Where(o => o.OrderIssueTypeId == issuestatusId)?.ToList();
            }
            ViewBag.StatusName = statusname;
            ViewBag.IssueStatusName = issuestatusname;
            ViewBag.soid = soid;

            var status = db.OrderStatus.ToList();
            var issueType = db.OrderIssueTypes.ToList();
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", statusId);
            ViewBag.IssueTypeId = new SelectList(issueType, "OrderIssueTypeId", "IssueTypeName", issuestatusId);
            return View(issueOrders);
        }
        [AuthorizeSpirit(Roles = "Shopper, Packer")]
        public ActionResult PackedList()
        {
            string custId = User.Identity.GetUserId();
            var rUser = db.RUsers.Where(o => o.rUserId == custId)?.FirstOrDefault();
            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            var route = routePlanDBO.PackedOrders(rUser.ShopId ?? 0);

            return View(route);
        }
        [AuthorizeSpirit(Roles = "Shopper, Packer")]
        public ActionResult PackList()
        {
            string custId = User.Identity.GetUserId();
            var rUser = db.RUsers.Where(o => o.rUserId == custId)?.FirstOrDefault();
            RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            var route = routePlanDBO.PackingOrders(rUser.ShopId ?? 0);
            var pCount = routePlanDBO.PackCount(rUser.ShopId ?? 0);

            ViewBag.PackCount = pCount;
            if (TempData["Msg"] != null)
            {
                ViewBag.Message = TempData["Msg"];
            }
            
            return View(route);
        }

        //[AuthorizeSpirit(Roles = "Shopper, Packer")]
        //public ActionResult PackStatus(string jobId)
        //{

        //    if (string.IsNullOrWhiteSpace(jobId))
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    string custId = User.Identity.GetUserId();
        //    var route = db.RoutePlans.Where(o => string.Compare(o.JobId, jobId, true) == 0)?.OrderBy(o=>o.AssignedDate).FirstOrDefault();
        //    return View(route);
        //}

        [AllowAnonymous]
        public ActionResult InvoiceToPdf(int? id)
        {
            var report = new Rotativa.ActionAsPdf("InvoiceMobile", new { id = id });
            return report;
        }
        [AllowAnonymous]
        public ActionResult InvoiceMobile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == 0)
            {
                ViewBag.NoData = "0";
                return View(new OrderDetailsView { Ord = new Order() });
            }
            Order order = db.Orders.Find(id);
            //var routePlan = db.RoutePlans.Where(o => o.OrderID == order.Id)?.ToList();
            //if (routePlan.Count <= 0) { order = null; }
            if (order == null)
            {
                return HttpNotFound();
            }

            var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.ProductDetail).Include(o => o.WineShop).Where(o => o.OrderId == id)?
               .OrderBy(o => o.Id)?
                .ToList();

            var mixDetails = db.MixerOrderItems.Include(o => o.Order).Include(o => o.MixerDetail).Include(o => o.MixerDetail.Mixer).Where(o => o.OrderId == id)?
               .OrderBy(o => o.MixerOrderItemId)?
                .ToList();


            Customer customer = null;
            if (order.CustomerId > 0)
            {
                customer = db.Customers.Find(order.CustomerId);
                if (customer.CustomerAddresses.Count() > 0)
                {
                    var address = customer.CustomerAddresses.ToList()[0];
                    customer.Address = address.Address;
                    customer.FormattedAddress = address.FormattedAddress;
                    customer.Flat = address.Flat;
                    customer.Landmark = address.Landmark;
                }
                else customer = null;
            }
            if (customer == null)
            {
                customer = order.Customer;
            }


            var shop = db.WineShops.Where(o => o.Id == order.ShopID).ToList();

            ViewBag.NoData = "1";
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy");
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName");
            ViewBag.ShopID = new SelectList(shop, "Id", "ShopName");
            return View(new OrderDetailsView { Ord = order, CurOrdDetail = orderDetails, Cust = customer, MixerItems = mixDetails });
        }

        [AuthorizeSpirit(Roles = "Shopper, Packer")]
        public ActionResult Pack(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == 0)
            {
                ViewBag.NoData = "0";
                return View(new OrderDetailsView { Ord = new Order() });
            }
           
            Order order = db.Orders.Find(id);
            //var routePlan = db.RoutePlans.Where(o => o.OrderID == order.Id)?.ToList();
            //if (routePlan.Count <= 0) { order = null; }
            if (order == null)
            {
                return HttpNotFound();
            }

            var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.ProductDetail).Include(o => o.WineShop).Where(o => o.OrderId == id)?
               .OrderBy(o=>o.Id)?
                .ToList();
            var mixerDetails = db.MixerOrderItems.Include(o => o.Order).Include(o => o.MixerDetail).Include(o => o.WineShop).Where(o => o.OrderId == id)?
               .OrderBy(o => o.OrderId)?
                .ToList();
            GiftBagDBO giftBagDBO = new GiftBagDBO();
            var giftDetails = giftBagDBO.GetGiftBagOrderItems(id.Value);
            OrderDBO orderDBO = new OrderDBO();
            var result = orderDBO.GetGoodiesOrderDetails(id.Value);
                foreach (var item in result)
                {
                    if (item.GName != null)
                    {
                        orderDetails.Where(x => x.Id == item.OrderDetailId).FirstOrDefault().GName = item.GName;
                    }                     
                }
            
           

            Customer customer = null;
            if (order.CustomerId > 0)
            {
                customer = db.Customers.Find(order.CustomerId);
                if (customer.CustomerAddresses.Count() > 0)
                {
                    var address = db.CustomerAddresses.Find(order.CustomerAddressId);
                    //var address = customer.CustomerAddresses.Where(o=>o.ShopId==order.ShopID)?.FirstOrDefault();
                    if (address != null)
                    {
                        customer.Address = address.Address;
                        customer.FormattedAddress = address.FormattedAddress;
                        customer.Flat = address.Flat;
                        customer.Landmark = address.Landmark;
                    }
                }
                else customer = null;
            }
            if (customer == null)
            {
                customer = order.Customer;
            }


            var shop = db.WineShops.Where(o => o.Id == order.ShopID).ToList();

            ViewBag.NoData = "1";
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "OrderPlacedBy");
            ViewBag.ProductID = new SelectList(db.ProductDetails, "ProductID", "ProductName");
            ViewBag.ShopID = new SelectList(shop, "Id", "ShopName");
            return View(new OrderDetailsView { Ord = order, CurOrdDetail = orderDetails, MixerItems= mixerDetails, Cust = customer ,GiftBagItems= giftDetails });
        }
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Pack")]
        [ValidateAntiForgeryToken]
        [AuthorizeSpirit(Roles = "Shopper, Packer")]
        public ActionResult PackConfrimed(int? id)
        {
            //int cancelOrder = (int)OrderStatusEnum.CancelByCustomer;
            int packed = (int)OrderStatusEnum.Packed;
            var ordStatuses = new[] 
            { 
                OrderStatusEnum.WalletRefundSuccessful,
                OrderStatusEnum.Delivered,
                OrderStatusEnum.WalletPaymentRefundSelected,
                OrderStatusEnum.IssueRefunded,
                OrderStatusEnum.OutForDelivery,
                OrderStatusEnum.Packed,
                OrderStatusEnum.IssueRefundFailed,
                OrderStatusEnum.BackToStore,
                OrderStatusEnum.DeliveryReached,
                OrderStatusEnum.PODPaymentSuccess,
                OrderStatusEnum.PODCashSelected,
                OrderStatusEnum.PODCashPaid,
                OrderStatusEnum.CustGeneratePin,
                OrderStatusEnum.DelPinVerified,
                OrderStatusEnum.PODOnlineSelected,
                OrderStatusEnum.OrderPickedUp,
                OrderStatusEnum.OCODCashPaid
            };
            var arryOrdStatuses = ordStatuses.Cast<int>().ToArray();

            Order order = db.Orders.Find(id);

            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            //order.OrderStatusId != cancelOrder
            if (!arryOrdStatuses.Contains(order.OrderStatusId) && ((order.OrderStatusId == 3 && order.PaymentTypeId == 1) || (order.OrderStatusId == 2 && (order.PaymentTypeId == 2 || order.PaymentTypeId == 5))))
            {
                order.OrderStatusId = packed;
                db.SaveChanges();

                OrderTrack orderTrack = new OrderTrack
                {
                    LogUserName = User.Identity.Name,
                    LogUserId = u.Id,
                    OrderId = order.Id,
                    StatusId = order.OrderStatusId,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();

            }
            else
            {
                var currentStatus = (OrderStatusEnum)order.OrderStatusId;
                TempData["Msg"] = $"The order cannot be packed if it has been packed already and is now in the {currentStatus} stage";
                return RedirectToAction("PackList");
            }

            //IList<Order> orders = new List<Order>();
            IList<RoutePlan> routePlans = new List<RoutePlan>();
            var route = db.DumpRoutePlans.Where(o => o.OrderID == order.Id)?.FirstOrDefault();
            string JobIdroute = default(string);
            if (route == null)
            {
                var route1 = db.RoutePlans.Where(o => o.OrderID == order.Id)?.FirstOrDefault();
                if (route1 != null)
                {
                    JobIdroute = route1.JobId;

                }
                
            }
            else
            {
                JobIdroute = route.JobId;
            }
            if (user.UserType1.UserTypeName.ToLower() == "packer")
            {
                RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                routePlans = routePlanDBO.PackingOrdersByJobId(JobIdroute);
                //orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Where(o => o.OrderStatusId == 3 && o.ShopID == user.ShopId).ToList();

            }
            CustomerApi2Controller.AddToFireStore(id.Value);

            WebEngageController webEngageController = new WebEngageController();
            Task.Run(async () => await webEngageController.WebEngageStatusCall(id.Value, "Order Packed"));
            FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
            Task.Run(async () => await fcmHelper.SendFirebaseNotification(id.Value, FirebaseNotificationHelper.NotificationType.Order));
            if (routePlans.Count > 0)
            {
                return RedirectToAction("Pack", new { id = routePlans[0].OrderID });
            }
            else if (JobIdroute == "" || JobIdroute == null)
            {
                return RedirectToAction("GetPackerAllScheduledDelivery", new { shopId = 0,  soid = 0,  shopname = "",  custno = "" });
            }
            else {
                //return RedirectToAction("Pack", new { id = 0 });
                var jobs = db.DeliveryJobs.Where(o => string.Compare(o.JobId, JobIdroute, true) == 0)?.FirstOrDefault();
                jobs.IsReady = true;
                jobs.ModifiedDate = DateTime.Now;
                db.SaveChanges();
               
                return RedirectToAction("PackList");

            }

            
        }

        [AuthorizeSpirit(Roles = "Shopper, Deliver, DeliveryManager")]
        public ActionResult DeliveryCompleted()
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            ViewBag.UserType = user.UserType1.UserTypeName;
            ViewBag.DeliveryUserId = user.DeliveryAgentId;
            var st = DateTime.Now;
            var cdate = DateTime.Now;

            var route = db.RoutePlans.Include(o => o.Customer).Include(o => o.Order).Include(o => o.DeliveryAgent).Where(o => o.DeliveryAgentId == user.DeliveryAgentId
            && o.Order.OrderStatusId == 5 && (!o.Order.TestOrder)).OrderByDescending(o => o.Order.DeliveryDate);
            //&& (o.SlotEnd ?? cadddate).Year == cdate.Year
            //           && (o.SlotEnd ?? cadddate).Month == cdate.Month
            //           && (o.SlotEnd ?? cadddate).Day == cdate.Day);
            //&& DateTime.Compare((o.SlotEnd ?? cadddate), DateTime.Now) <= 0);
            return View(route);
        }

        [AuthorizeSpirit(Roles = "Shopper, Deliver, DeliveryManager")]
        public ActionResult DeliveryConfrim(int? id)
        {
            if (id == null)
            {
                return Json(new { message = "Order not found." }, JsonRequestBehavior.AllowGet);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return Json(new { message = "Order not found." }, JsonRequestBehavior.AllowGet);
            }
            order.OrderStatusId = 5;
            order.DeliveryDate = DateTime.Now;
            db.SaveChanges();

            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = User.Identity.Name,
                LogUserId = u.Id,
                OrderId = order.Id,
                StatusId = order.OrderStatusId,
                TrackDate = DateTime.Now
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();

            UpdateDeliveryJob(order.Id);

            //WSendSMS wsms = new WSendSMS();
            //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSDelivered"], order.Id.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm tt") );
            //wsms.SendMessage(textmsg, order.Customer.ContactNo);

            //Flow SMS
            var dicti = new Dictionary<string, string>();
            dicti.Add("ORDERID", order.Id.ToString());
            dicti.Add("DATETIME", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));

            var templeteid = ConfigurationManager.AppSettings["SMSDeliveredFlowId"];

            Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.Customer.ContactNo, dicti));
            //End Flow

            return Json(new { message = "Order status updated." }, JsonRequestBehavior.AllowGet);
        }

        private void UpdateDeliveryJob(int Id)
        {
            var route = db.RoutePlans.Where(o => o.OrderID == Id).FirstOrDefault();
            var routePlan = db.RoutePlans.Where(o => o.JobId == route.JobId).ToList();

            bool IsOrderRemainToPacked = false;
            routePlan.ForEach((o) =>
            {
                var track = db.OrderTracks.Where(p => p.OrderId == o.Order.Id)?.OrderByDescending(a=>a.TrackDate).Take(1)?.FirstOrDefault();
                if (track.StatusId != 5) IsOrderRemainToPacked = true;
            });
            if (!IsOrderRemainToPacked)
            {
                var rPlan = routePlan.Take(1)?.FirstOrDefault();
                DeliveryJob deliveryJob = db.DeliveryJobs.Where(o => o.JobId == rPlan.JobId).FirstOrDefault();
                deliveryJob.CompletedFlag = true;
                db.SaveChanges();
            }
        }

        [AuthorizeSpirit(Roles = "Shopper, Deliver, DeliveryManager")]
        public ActionResult NoDelivery()
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            var st = DateTime.Now;
            var routep = db.RoutePlans.Include(o => o.Customer).Include(o => o.Order).Include(o => o.DeliveryAgent).Where(o => o.DeliveryAgentId == user.DeliveryAgentId && o.OrderStatusId == 6).ToList();
            var route = routep.Where(o => (!Convert.ToBoolean(o.Order.TestOrder)));
            //var route = db.RoutePlans.Include(o => o.Customer).Include(o => o.Order).Include(o => o.DeliveryAgent).Where(o => o.DeliveryAgentId == user.DeliveryAgentId && (st >= o.DeliveryStart && st <= o.DeliveryEnd)).ToList();

            foreach (var item in route)
            {
                if (item.Order.OrderStatusId == 6)
                {
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = User.Identity.Name,
                        LogUserId = u.Id,
                        OrderId = item.OrderID,
                        StatusId = 9,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    var routeplan = db.RoutePlans.Where(o => o.OrderID == item.OrderID)?.FirstOrDefault();
                    routeplan.isOutForDelivery = true;
                    db.SaveChanges();

                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSOutForDelivery"], item.Order.Id.ToString());
                    //wsms.SendMessage(textmsg, item.Order.Customer.ContactNo);

                    //Flow SMS
                    var dicti = new Dictionary<string, string>();
                    dicti.Add("ORDERID", item.Order.Id.ToString());

                    var templeteid = ConfigurationManager.AppSettings["SMSSubmittedFlowId"];

                    Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, item.Order.Customer.ContactNo, dicti));
                    //

                    var dagent = db.DeliveryAgents.Where(o => o.Id == user.DeliveryAgentId).FirstOrDefault();

                    if(dagent!=null)
                    {
                        dagent.IsAtShop = true;
                        db.SaveChanges();
                    }
                }
            }

            return Json(new { message = "Order status updated." }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeSpirit(Roles = "Shopper, Deliver, DeliveryManager")]
        public ActionResult UpdateDeliveryStatus(int userId)
        {
            var dagent = db.DeliveryAgents.Where(o => o.Id == userId).FirstOrDefault();

            if (dagent != null)
            {
                dagent.IsAtShop = false;
                db.SaveChanges();
            }

            return Json(new { message = "Status updated." }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet, ActionName("History")]
        public ActionResult OrderHistory(int? id)
        {
            var track = db.OrderTracks.Include(o=>o.OrderStatu).Where(o => o.OrderId == id).OrderBy(o=>o.OrderTrackId).ToList();
            return View("History",track);
        }

        [HttpPost]
        public ActionResult NoThere(int? orderId, string text)
        {
            if (orderId == null)
            {
                return Json(new { message = "Order id is null", status = "1" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return Json(new { message = "text is null", status = "2" }, JsonRequestBehavior.AllowGet);
            }
            if (orderId == 0)
            {
                ViewBag.NoData = "0";
                return Json(new { message = "Order id is 0.", status = "3" }, JsonRequestBehavior.AllowGet);
            }
            Order order = db.Orders.Find(orderId);
            Customer customer = db.Customers.Where(o =>string.Compare(o.ContactNo,order.OrderTo,true)==0)?.Take(1).FirstOrDefault();
            if (order == null || customer == null)
            {
                return Json(new { message = "Order or Customer is null.", status = "4" }, JsonRequestBehavior.AllowGet);
            }
            else {
                db.ProductNotTheres.Add(new ProductNotThere { CustomerId = customer.Id, OrderId = order.Id, ProductName = text, CreatedDate = DateTime.Now });
                db.SaveChanges();
            }
            return Json(new { message = "Status updated.", status="0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult RefundAmt(int? orderId)
        {
            if (orderId == null)
            {
                return RedirectToAction("Index", new { message = "Order id is null", status = "1" });
            }
            if (orderId == 0)
            {
                return RedirectToAction("Index", new { message = "Order id is 0.", status = "2" });
            }
            Order order = db.Orders.Find(orderId);
            var appPaytmhook = db.AppLogsPaytmHooks.Where(o => o.OrderId == order.Id && string.Compare(o.SendStatus, "Approved", true) == 0);
            if (appPaytmhook.Count() > 0)
            {
                Customer customer = db.Customers.Where(o => string.Compare(o.ContactNo, order.OrderTo, true) == 0)?.Take(1).FirstOrDefault();
                if (order.OrderStatusId == 3)
                {
                    PaytmPayment paytmPayment = new PaytmPayment();
                    int ret = paytmPayment.PaytmRefundApiCall(order.Id);
                    if (ret == 1)
                    {
                        order.OrderStatusId = 22;
                        db.SaveChanges();

                        var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = User.Identity.Name,
                            LogUserId = u.Id,
                            OrderId = order.Id,
                            StatusId = order.OrderStatusId,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                        db.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("Index", new { message = "Can not Refund.", status = "4" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", new { message = "Order status must be approved.", status = "3" });
                }
            }
            else
            {
                return RedirectToAction("Index", new { message = "Can not Refund.", status = "4" });
            }
            return RedirectToAction("Index", new { msg = "Refund Successfully." });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RefundStatusPay(int? orderId)
        {
            if (orderId == null)
            {
                return Json(new { message = "Order id is null", status = "1" }, JsonRequestBehavior.AllowGet);
            }
            if (orderId == 0)
            {
                return Json(new { message = "Order id is 0.", status = "2" }, JsonRequestBehavior.AllowGet);
            }
            Order order = db.Orders.Find(orderId);
            PaytmPayment paytmPayment = new PaytmPayment();
            int ret = paytmPayment.PaytmRefundStatusApiCall(order.Id);
            if (ret == 1)
            {
                order.OrderStatusId = 23;
                db.SaveChanges();

                var u = db.AspNetUsers.Where(o => o.Email == "subhamautomation@rainmail.com").FirstOrDefault();
                OrderTrack orderTrack = new OrderTrack
                {
                    LogUserName = u.Email,
                    LogUserId = u.Id,
                    OrderId = order.Id,
                    StatusId = order.OrderStatusId,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();
            }
            else
            {
                return Json(new { message = "Can not Refund.", status = "4" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Refund Successfully." }, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult PaidCancelOrders()
        //{
        //    var payHooks = db.Orders.Where(o => o.OrderStatusId == 16);
        //    db.AppLogsPaytmHooks.Include(o=>o.o).Where(o => string.Compare(o.SendStatus, "approved", true) == 0)?.FirstOrDefault();
        //    return View();
        //}
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpPost]
        public ActionResult PackIssue(int id, string[] odetailIds)
        {
            var uId = User.Identity.GetUserId();
            IssueDBO issueDBO = new IssueDBO();
            string oIds = string.Join(",",odetailIds);
            int issueId = issueDBO.DeliveryManagerTrackAgent(id, uId, oIds);
            if (issueId == 2)
            {
               
                return Json(new { msg = "Sorry this order is already packed/delivered. Cannot raise an issue/pack it again!", status = true }, JsonRequestBehavior.AllowGet);
            }
            if (issueId > 0)
            {
                SpiritUtility.GenerateZohoTikect(id, issueId);
                return Json(new { msg = "Issue created successfully.", status = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Unable to create Issue.", status = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateBackToStoreTo(int orderId)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            if (orderId<=0)
            {
                RedirectToAction("Index", new { msg = "Reassign fail as order not valid." });
            }
            var order = db.Orders.Find(orderId);
            if (order.PaymentTypeId==1)
            {
                OrderTrack orderTrack1 = new OrderTrack
                {
                    LogUserName = User.Identity.Name,
                    LogUserId = u.Id,
                    OrderId = order.Id,
                    StatusId = (int)OrderStatusEnum.Approved,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack1);
            }
            if (order.PaymentTypeId == 2 || order.PaymentTypeId == 5)
            {
                OrderTrack orderTrack1 = new OrderTrack
                {
                    LogUserName = User.Identity.Name,
                    LogUserId = u.Id,
                    OrderId = order.Id,
                    StatusId = (int)OrderStatusEnum.Submitted,
                    TrackDate = DateTime.Now
                };
                db.OrderTracks.Add(orderTrack1);
            }
            order.OrderStatusId = (int)OrderStatusEnum.Packed;
            db.SaveChanges();

            
            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = User.Identity.Name,
                LogUserId = u.Id,
                OrderId = order.Id,
                StatusId = order.OrderStatusId,
                TrackDate = DateTime.Now
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();

            return RedirectToAction("Index", new { msg = "Reassign Successfully." });
        }

        [HttpPost]
        public ActionResult ReSendSMSPaymentLink(ResendPaymentLink resendPaymentLink)
        {
            if (string.IsNullOrWhiteSpace(resendPaymentLink.ContactNo))
            {
                return Json(new { status = false, msg = "Contact number is not valid." }, JsonRequestBehavior.AllowGet);
            }
            if (resendPaymentLink.OrderId<=0)
            {
                return Json( new { status=false, msg = "Order Id is not valid." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                int maxSent = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSMSResent"]);
                var smsent = db.CustomerSMSSends.Where(o => o.OrderId == resendPaymentLink.OrderId && o.CustomerId == resendPaymentLink.CustomerId)?.FirstOrDefault();
                if (smsent == null || smsent.SMSSentCount <= maxSent)
                {
                    var cashPay = db.CashFreePays.Where(o => o.OrderId == resendPaymentLink.OrderId)?.OrderByDescending(o => o.CashFreePayId).FirstOrDefault();
                    var cashfreeLink = JsonConvert.DeserializeObject<CashFreePaymentOutput>(cashPay.VenderOutput);

                    var order = db.Orders.Find(resendPaymentLink.OrderId);

                    WSendSMS wsms = new WSendSMS();
                    string textmsg = string.Format(ConfigurationManager.AppSettings["CFResendLink"],
                        order.OrderAmount, order.Id.ToString(), cashfreeLink.paymentLink);
                    wsms.SendMessage(textmsg, resendPaymentLink.ContactNo);
                    if (smsent == null)
                    {
                        var sendSms = new CustomerSMSSend
                        {
                            OrderId = resendPaymentLink.OrderId,
                            CustomerId = resendPaymentLink.CustomerId,
                            ContactNo = resendPaymentLink.ContactNo,
                            SMSSentCount = 1
                        };
                        db.CustomerSMSSends.Add(sendSms);
                    }
                    else
                    {
                        smsent.SMSSentCount += 1;
                    }
                    db.SaveChanges();

                    var stResend = (int)OrderStatusEnum.CashFreePayLinkSmsResent;
                    var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = User.Identity.Name,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = stResend,
                        TrackDate = DateTime.Now,
                        Remark = "SMS Sent"
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();
                }
                else
                {
                    return Json(new { status = false, msg = "Max limit exists." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.Logging(ex.Message, ex.StackTrace);
                return Json(new { status = false, msg = "Error." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Contact number is not valid." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendEmailPaymentLink(ResendPaymentLink resendPaymentLink)
        {
            if (string.IsNullOrWhiteSpace(resendPaymentLink.Email))
            {
                return Json(new { status = false, msg = "Email is not valid." }, JsonRequestBehavior.AllowGet);
            }
            if (resendPaymentLink.OrderId <= 0)
            {
                return Json(new { status = false, msg = "Order Id is not valid." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                //int maxSent = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSMSResent"]);
                //var smsent = db.CustomerEmailSends.Where(o => o.OrderId == resendPaymentLink.OrderId && o.CustomerId == resendPaymentLink.CustomerId);

                var cashPay = db.CashFreePays.Where(o => o.OrderId == resendPaymentLink.OrderId)?.OrderByDescending(o => o.CashFreePayId).FirstOrDefault();
                var cashfreeLink = JsonConvert.DeserializeObject<CashFreePaymentOutput>(cashPay.VenderOutput);

                var order = db.Orders.Find(resendPaymentLink.OrderId);
                //SendEMail

                EmailSender _emailSender = new EmailSender();
                _emailSender.SendEmailAsync(new Dictionary<string, string> {
                      {"paylink", cashfreeLink.paymentLink},
                      {"orderid", resendPaymentLink.OrderId.ToString()}
                }, resendPaymentLink.Email, EmailSelectSubject.Sendpaymentlink, EmailSelectTemplate.SendPaymentlink);

                var sendEmail = new CustomerEmailSend
                {
                    OrderId = resendPaymentLink.OrderId,
                    EmailId = resendPaymentLink.Email,
                    CustomerId = resendPaymentLink.CustomerId
                };
                db.CustomerEmailSends.Add(sendEmail);

                db.SaveChanges();

                var stEmailsend = (int)OrderStatusEnum.CashFreePayLinkEmailResent;
                var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
                OrderTrack orderTrack = new OrderTrack
                {
                    LogUserName = User.Identity.Name,
                    LogUserId = u.Id,
                    OrderId = order.Id,
                    StatusId = stEmailsend,
                    TrackDate = DateTime.Now,
                    Remark = "Email Sent"
                };
                db.OrderTracks.Add(orderTrack);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                SpiritUtility.Logging(ex.Message, ex.StackTrace);
                return Json(new { status = false, msg = "Error." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "Emailed payment link to customer." }, JsonRequestBehavior.AllowGet);
        }


        [AuthorizeSpirit]
        public ActionResult MultipleOrder(int shopId = 0, string shopname = "", int statusId = 0, string statusname = "", int soid = 0, string custno = "")
        {

            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            IList<Order> orders = new List<Order>();
            bool isSearch = false;


            if (shopId > 0)
            {
                orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                    .Where(o => o.ShopID == shopId && (!o.TestOrder) && o.OrderGroupId != null)
                    .OrderByDescending(o => o.Id)
                        .Take(1000).ToList();
                isSearch = true;
            }
            if (statusId > 0)
            {
                if (orders.Count > 0) orders = orders.Where(o => o.OrderStatusId == statusId).ToList();
                else orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => o.OrderStatusId == statusId && (!o.TestOrder) && o.OrderGroupId != null)
                        .OrderByDescending(o => o.Id)
                        .Take(1000).ToList();
                isSearch = true;

            }
            if (soid > 0)
            {
                if (orders.Count > 0) orders = orders.Where(o => o.Id == soid).ToList();
                else orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => o.Id == soid && o.OrderGroupId != null).ToList();
                isSearch = true;
            }
            if (!string.IsNullOrWhiteSpace(custno))
            {
                if (orders.Count > 0) orders = orders.Where(o => string.Compare(o.OrderTo, custno, true) == 0).ToList();
                else orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => string.Compare(o.OrderTo, custno, true) == 0 && o.OrderGroupId != null).ToList();
                isSearch = true;
            }

            if (user.UserType1.UserTypeName.ToLower() == "agent")
            {


                //if (orders.Count > 0) orders = orders.Where(o => o.OrderStatusId == 1 || o.OrderStatusId == 2).OrderByDescending(o => o.Id).ToList();
                //else if(!isSearch) orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                //        .Where(o => o.OrderStatusId == 1 || o.OrderStatusId == 2 && (!o.TestOrder))
                //        .OrderByDescending(o => o.Id)
                //        .Take(1000).ToList();

            }
            else if (user.UserType1.UserTypeName.ToLower() == "packer")
            {
                var defaultStatuid = (statusId == 3 || statusId == 6) ? statusId : 3;

                if (orders.Count > 0)
                {
                    orders = orders.Where(o => o.OrderStatusId == defaultStatuid && o.ShopID == user.ShopId)?.OrderBy(o => o.Id).ToList();
                }
                else if (!isSearch)
                {
                    orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => o.OrderStatusId == defaultStatuid && o.ShopID == user.ShopId && (!o.TestOrder) && o.OrderGroupId != null)
                        .OrderBy(o => o.Id)
                        .Take(1000).ToList();
                }
            }
            else if (user.UserType1.UserTypeName.ToLower() == "deliver")
            {
                return RedirectToAction("Delivery"); ;
            }
            else
            {
                if (orders.Count > 0) orders = orders.ToList();
                else if (!isSearch) orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Include(o => o.WineShop)
                        .Where(o => (!o.TestOrder) && o.OrderGroupId != null)
                        .OrderByDescending(o => o.Id)
                        .Take(1000).ToList();
            }

            var shop = db.WineShops.ToList();
            var status = db.OrderStatus.ToList();
            ViewBag.ShopName = shopname;
            ViewBag.StatusName = statusname;
            ViewBag.soid = soid;
            ViewBag.CustNo = custno;
            ViewBag.ShopId = new SelectList(shop, "Id", "ShopName", shopId);
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", statusId);
            ViewBag.UserType = user.UserType1.UserTypeName;
            return View(orders);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper,Supplier")]
        public ActionResult SupplierOrder(int soid = 0)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            var supplier = db.Suppliers.Where(o => o.UserId == u.Id)?.FirstOrDefault();
            //var sup = db.Suppliers.Find(supplier.SupplierId);
            var mixers = db.MixerOrderItems.Include(o => o.Order).Include(o => o.Supplier)
                    .Where(o => o.SupplierId == supplier.SupplierId).Select(o => o.Order)
                    .GroupBy(o=>o.Id)
                    .Select(o=>o.FirstOrDefault()).ToList();

            ViewBag.Supplier = supplier;
            ViewBag.UserType = user.UserType1.UserTypeName;
            return View(mixers);
        }

        [AuthorizeSpirit(Roles = "Shopper,Supplier")]
        [Route("mulorder/{orderId}", Name = "GetMultipeOrderDetail")]
        public ActionResult MultipleOrderDetails(int orderId=0)
        {
            if (orderId == 0)
            {
                TempData["msg"] = "Order can not be empty.";
                return View();
            }
            var mixerOrder = db.MixerOrderItems.Include(o => o.Order).Where(o => o.OrderId == orderId).ToList();
            var statusID = new[] {OrderStatusEnum.Pending.ToString(),
                OrderStatusEnum.Submitted.ToString(),
                OrderStatusEnum.Approved.ToString(),
                OrderStatusEnum.Packed.ToString(),
                OrderStatusEnum.OutForDelivery.ToString(),
                OrderStatusEnum.Delivered.ToString(),
                OrderStatusEnum.Cancel.ToString(),
                OrderStatusEnum.CancelByCustomer.ToString()
            };
            var status = db.OrderStatus.Where(o => statusID.Contains(o.OrderStatusName)).ToList();
            int sId = (mixerOrder.Count() > 0) ? mixerOrder[0].Order.OrderStatusId : (int)OrderStatusEnum.Approved;

            var order = db.Orders.Find(orderId);
            var ordAddress = db.CustomerAddresses.Find(order.CustomerAddressId);


            var statusUpdate = new[] {
                OrderStatusEnum.Packed,
                OrderStatusEnum.OutForDelivery,
                OrderStatusEnum.Delivered,
            };
            var config = SZIoc.GetSerivce<ISZConfiguration>();
            string giftCharge = config.GetConfigValue(ConfigEnums.GiftCharge.ToString());

            var onlyStatus = statusUpdate.Cast<int>().ToArray();

            ViewBag.GiftCharge = giftCharge;
            ViewBag.UpdateAllowOnStatus = onlyStatus;
            ViewBag.StatusId = new SelectList(status, "Id", "OrderStatusName", sId);
            ViewBag.Order = order;
            ViewBag.OrderAddress = ordAddress;

            return View(mixerOrder);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "Shopper,Supplier")]
        [Route("mulorder-update", Name = "PostMultipleOrderUpdateStatus")]
        public ActionResult MultipleOrderUpdateStatus(int orderId, int dpStatusId = 0)
        {
            if (orderId == 0)
            {
                TempData["msg"] = "Order can not be empty.";
                return RedirectToRoute("GetMultipeOrderDetail", new { orderId = orderId });
            }
            if (dpStatusId == 0)
            {
                TempData["msg"] = "Status can not be empty.";
                return RedirectToRoute("GetMultipeOrderDetail", new { orderId = orderId });
            }

            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            var supplier = db.Suppliers.Where(o => o.UserId == u.Id)?.FirstOrDefault();

            var statusID = new[] {
                OrderStatusEnum.Packed,
                OrderStatusEnum.OutForDelivery,
                OrderStatusEnum.Delivered,
            };

            var onlyStatus = statusID.Cast<int>().ToArray();

            if (!onlyStatus.Contains(dpStatusId))
            {
                TempData["msg"] = "Selected Status can not be updated.";
                return RedirectToRoute("GetMultipeOrderDetail", new { orderId = orderId });
            }

            var ord = db.Orders.Find(orderId);
            ord.OrderStatusId = dpStatusId;
            db.SaveChanges();

            var ema = ConfigurationManager.AppSettings["TrackEmail"];
            //var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = u.Email,
                LogUserId = u.Id,
                OrderId = ord.Id,
                StatusId = ord.OrderStatusId,
                TrackDate = DateTime.Now,
                Remark="Updated by Supplier"
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();

            TempData["msg"] = "Status is updated successfully.";
            return RedirectToRoute("GetMultipeOrderDetail", new { orderId = orderId });
        }


        [AuthorizeSpirit(Roles = "Agent,SalesManager,DeliveryManager")]
        [StopAction]
        public ActionResult ScheduledDelivery(int shopId = 0,int soid=0, string shopname = "",string custno = "")
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();

            var schDel = scheduleDeliveryDBO.GetScheduledDeliveries(soid, shopname,custno);
            var shop = db.WineShops.ToList();
           
            ViewBag.ShopName = shopname;
            ViewBag.CustNo = custno;
            ViewBag.ShopId = new SelectList(shop, "Id", "ShopName", shopId);
            ViewBag.UserType = user.UserType1.UserTypeName;
            return View(schDel);
        }

        [AuthorizeSpirit(Roles = "Shopper,Packer")]
        [StopAction]
        public ActionResult GetAllScheduledDelivery(int shopId = 0, int soid = 0, string shopname = "", string custno = "")
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();

            var schDel = scheduleDeliveryDBO.GetAllScheduledDeliveries(soid, shopname, custno);
            var shop = db.WineShops.ToList();

            ViewBag.ShopName = shopname;
            ViewBag.CustNo = custno;
            ViewBag.ShopId = new SelectList(shop, "Id", "ShopName", shopId);
            ViewBag.UserType = user.UserType1.UserTypeName;

            return View("ScheduledDelivery",schDel);
        }
        
        [AuthorizeSpirit(Roles = "Shopper,Packer")]
        [StopAction]
        public ActionResult GetPackerAllScheduledDelivery(int shopId = 0, int soid = 0, string shopname = "", string custno = "")
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();

            var wineShop = db.WineShops.Where(a => a.Id == user.ShopId).FirstOrDefault();
            shopname = wineShop.ShopName;
            var schDel = scheduleDeliveryDBO.GetPackerAllScheduledDeliveries(soid, shopname, custno);
            var shop = db.WineShops.ToList();

            ViewBag.ShopName = shopname;
            ViewBag.CustNo = custno;
            ViewBag.ShopId = new SelectList(shop, "Id", "ShopName", shopId);
            ViewBag.UserType = user.UserType1.UserTypeName;

            return View("ScheduledDelivery", schDel);
        }

        [AuthorizeSpirit(Roles = "Shopper,Packer")]
        [StopAction]
        public ActionResult Release(int id)
        {
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();
            var release = scheduleDeliveryDBO.UpdateScheduleDeliveryRelease(id);
            if (release == 1)
            {
                return RedirectToAction("GetAllScheduledDelivery");
            }
            else
            {
                return RedirectToAction("GetAllScheduledDelivery");
            }

        }
        [AuthorizeSpirit(Roles = "Shopper,Packer")]
        public ActionResult FileDetails(FileUpload model)
        {
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();
            List<FileUpload> list = new List<FileUpload>();
            DataTable dtFiles = scheduleDeliveryDBO.GetFileDetails();
            foreach (DataRow dr in dtFiles.Rows)
            {
                list.Add(new FileUpload
                {
                    FileId = @dr["Id"].ToString(),
                    FileName = @dr["FILENAME"].ToString(),
                    FileUrl = @dr["FILEURL"].ToString(),
                    CreatedDate =Convert.ToDateTime(@dr["CreatedDate"])
                });
            }
            model.FileList = list;
            return View(model);
        }

        [AuthorizeSpirit(Roles = "Shopper,Packer")]
        [HttpPost]
        public ActionResult FileDetails(HttpPostedFileBase files)
        {
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();
            FileUpload model = new FileUpload();
            List<FileUpload> list = new List<FileUpload>();
            DataTable dtFiles = scheduleDeliveryDBO.GetFileDetails();
            foreach (DataRow dr in dtFiles.Rows)
            {
                list.Add(new FileUpload
                {
                    FileId = @dr["Id"].ToString(),
                    FileName = @dr["FILENAME"].ToString(),
                    FileUrl = @dr["FILEURL"].ToString(),
                    CreatedDate = Convert.ToDateTime(@dr["CreatedDate"])
                });
            }
            model.FileList = list;

            if (files != null)
            {
                var Extension = Path.GetExtension(files.FileName);
                var fileName = "my-file-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                string path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                model.FileUrl = Url.Content(Path.Combine("~/UploadedFiles/", fileName));
                model.FileName = fileName;

                if (scheduleDeliveryDBO.SaveFile(model))
                {
                    files.SaveAs(path);
                    TempData["AlertMessage"] = "Uploaded Successfully !!";
                    return RedirectToAction("FileDetails", "Orders");
                }
                else
                {
                    ModelState.AddModelError("", "Error In Add File. Please Try Again !!!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please Choose Correct File Type !!");
                return View(model);
            }
            return RedirectToAction("FileDetails", "Orders");
        }


        [AuthorizeSpirit(Roles = "Shopper,Packer")]
        public ActionResult GetFiles(FileUpload model)
        {
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();
            List<FileUpload> list = new List<FileUpload>();
            DataTable dtFiles = scheduleDeliveryDBO.GetFileDetails();
            foreach (DataRow dr in dtFiles.Rows)
            {
                list.Add(new FileUpload
                {
                    FileId = @dr["Id"].ToString(),
                    FileName = @dr["FILENAME"].ToString(),
                    FileUrl = @dr["FILEURL"].ToString(),
                    CreatedDate = Convert.ToDateTime(@dr["CreatedDate"])
                });
            }
            model.FileList = list;
            return View(model);
        }

        public FileResult Download(string FileID)
        {
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();
            int CurrentFileID = Convert.ToInt32(FileID);
            DataTable dtFiles = scheduleDeliveryDBO.GetFiles(CurrentFileID);
            string CurrentFileName="";
            foreach (DataRow dr in dtFiles.Rows)
            {
                CurrentFileName= dr["FileName"].ToString();
            };

                string contentType = string.Empty;

            if (CurrentFileName.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }

            else if (CurrentFileName.Contains(".docx"))
            {
                contentType = "application/docx";
            }
            else if (CurrentFileName.Contains(".xlxs"))
            {
                contentType = "application/xlxs";
            }
            else if (CurrentFileName.Contains(".exc"))
            {
                contentType = "application/exc";
            }

           var fullpath = Server.MapPath("~/UploadedFiles/") + CurrentFileName;
            return File(fullpath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(fullpath));
            //return File(CurrentFileName, contentType, CurrentFileName);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper,OrderFullFillment")]
        public ActionResult MrpIssue(int orderId = 0)
        {

            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            IList<OrderDetailMrpIssueDO> orders = new List<OrderDetailMrpIssueDO>();
           
            if (orderId > 0)
            {
                OrderDBO orderDBO = new OrderDBO();
                orders=orderDBO.GetMrpIssue(orderId);


            }
           
            ViewBag.UserType = user.UserType1.UserTypeName;
            return View(orders);
        }

        [HttpPost]
        [Route("~/mrpissue-save", Name = "MrpIssueSave")]
        [AuthorizeSpirit(Roles = "Shopper,OrderFullFillment")]
        public ActionResult MrpIssueSave(List<OrderDetailMrpIssueDO> orderDetail,string remarks, int totalAmount,int totalNewAmount,int diffAmount)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            if (orderDetail.FirstOrDefault().OrderId < 1)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                int statusId = 0, MrpIssueId = 0, orderId = 0;
                OrderDBO orderDBO = new OrderDBO();
                orderId = orderDetail.FirstOrDefault().OrderId;
                foreach (var item in orderDetail)
                {

                    
                    if (diffAmount > 0)
                    {
                        statusId = 70;
                    }
                    else if(diffAmount <0)
                    {
                        statusId = 69;
                    }
                    if (item.IsMrpIssue)
                    {
                        MrpIssueId = orderDBO.UpdateOrderDetailMrpIssue(item.OrderId, item.ProductID, item.ItemQty, item.IsMrpIssue, item.NewMrp, item.NewAmount, u.Id, u.Email, remarks, statusId);
                    }
                    

                }
                if (diffAmount > 0 && MrpIssueId > 0)
                {
                    if (PaymentLinkForExistOrder(Convert.ToString(orderId)))
                    {
                        SendPaymentLink(orderId);
                        return Json(new { status = true, msg = "Resent payment link to customer." }, JsonRequestBehavior.AllowGet);
                    }

                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var payCash = paymentLinkLogsService.CashFreePaymentForMrpIssue(orderId, diffAmount, MrpIssueId);

                    return Json(new { status = payCash.Status, msg = payCash.Message }, JsonRequestBehavior.AllowGet);
                }
                if (diffAmount < 0 && MrpIssueId > 0)
                {
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var refund = paymentLinkLogsService.CashFreeReFundForMrpIssue(orderId, diffAmount, MrpIssueId);
                    return Json(new { status = refund.Status, msg = refund.Message }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Unable to SAVE the MRPIssue." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, msg = "MRPIssue saved succeefully." }, JsonRequestBehavior.AllowGet);
        }
        private bool PaymentLinkForExistOrder(string orderId)
        {
            //string checkPartialPay = $"{orderId}_PartialPay";
            int ordId = Convert.ToInt32(orderId);
            var cashPay = db.CashFreePays.Where(o => o.OrderId == ordId);
            if (cashPay.Count() <= 0)
            {
                return false;
            }
            var paylink = cashPay.OrderByDescending(o => o.CashFreePayId).FirstOrDefault();
            var cashfreeLink = JsonConvert.DeserializeObject<CashFreeOrderCreate>(paylink.InputValue);

            if (!cashfreeLink.orderId.Contains("PartialPay")) return false;

            var apiPayCount = db.AppLogsCashFreeHooks.Where(o => o.OrderId == orderId && o.OrderIdPartial.Contains(cashfreeLink.orderId) && o.OrderIdPartial.Contains("PartialPay")).Count();

            if (apiPayCount > 0) return false;

            return true;
        }
        private void SendPaymentLink(int orderId)
        {
            var cashPay = db.CashFreePays.Where(o => o.OrderId == orderId)?.OrderByDescending(o => o.CashFreePayId).FirstOrDefault();
            var cashfreeLink = JsonConvert.DeserializeObject<CashFreePaymentOutput>(cashPay.VenderOutput);

            var order = db.Orders.Find(orderId);

            WSendSMS wsms = new WSendSMS();
            string textmsg = string.Format(ConfigurationManager.AppSettings["CFResendLink"],
                cashPay.Amt, order.Id.ToString(), cashfreeLink.paymentLink);
            wsms.SendMessage(textmsg, order.Customer.ContactNo);

            var stResend = (int)OrderStatusEnum.CashFreePayLinkSmsResent;
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            OrderTrack orderTrack = new OrderTrack
            {
                LogUserName = User.Identity.Name,
                LogUserId = u.Id,
                OrderId = order.Id,
                StatusId = stResend,
                TrackDate = DateTime.Now,
                Remark = "SMS Sent"
            };
            db.OrderTracks.Add(orderTrack);
            db.SaveChanges();
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult ConfigMaster()
        {
            ConfigMasterDBO configMasterDBO = new ConfigMasterDBO();
            var configdtl = configMasterDBO.GetConfigMasterDetails(0);
            if (configdtl == null)
            {
                return HttpNotFound();
            }
            
            return View(configdtl);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "Shopper")]
        public JsonResult SaveConfigMaster(int configMasterId,bool valueText,string description)
        {
            string uId = User.Identity.GetUserId();
            var aspUSer = db.AspNetUsers.Find(uId);
            ConfigMasterDBO configMasterDBO = new ConfigMasterDBO();
            var res = configMasterDBO.UpdateConfigMasterDetails(configMasterId,valueText,description,aspUSer.Email);
            var configdtl = configMasterDBO.GetConfigMasterDetail(configMasterId);
            return Json(new { ConfigMasterId = configdtl.ConfigMasterId, KeyText = configdtl.KeyText, ValueText = configdtl.ValueText , Description =configdtl.Description ,Message="Record Updated Successfully", status = true });
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult ShopIndex()
        {
            OrderDBO orderDBO = new OrderDBO();
            var shopList = orderDBO.GetShopList();
            return View(shopList);
        }
        [HttpPost]
        [AllowAnonymous]
        [AuthorizeSpirit(Roles = "Shopper")]
        public JsonResult UpdateShopDetail(int shopId, string contactNo,bool flag)
        {
            OrderDBO orderDBO = new OrderDBO();
            var res = orderDBO.UpdateShop(shopId,contactNo,flag);
            var shop = orderDBO.GetShop(shopId);
           
            return Json(new { ShopId = shop.ShopId, ShopName = shop.ShopName, Address = shop.Address ,ShopPhoneNo =shop.ShopPhoneNo, OperationFlag= shop.OperationFlag,Vat =shop.Vat,status =true,Message ="Record Updated" });
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult ZoneIndex()
        {
            OrderDBO orderDBO = new OrderDBO();
            var zoneList = orderDBO.GetZoneList();
            var shop = db.WineShops.Where(x =>x.OperationFlag == true).ToList();
            ViewBag.Shops = new SelectList(shop, "Id", "ShopName");
            return View(zoneList);
        }
        [HttpPost]
        [AllowAnonymous]
        [AuthorizeSpirit(Roles = "Shopper")]
        public JsonResult UpdateZoneDetail(int zoneId, int shopId, bool flag)
        {
            OrderDBO orderDBO = new OrderDBO();
            var res = orderDBO.UpdateZone(zoneId, shopId, flag);
            var zone = orderDBO.GetZone(zoneId);

            return Json(new { ZoneID = zone.ZoneID, ZoneName = zone.ZoneName, ShopName = zone.ShopID, ShopID = zone.ShopID, OperationFlag = zone.OperationalFlag,status = true, Message = "Record Updated" });
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult ConfigurableETA()
        {
            ConfigurableETADBO configurableETADBO = new ConfigurableETADBO();
            var configurableETAList = configurableETADBO.GetConfigurableETAList();
            var shop = db.WineShops.Where(x =>x.OperationFlag == true).ToList();
            ViewBag.Shops = new SelectList(shop, "Id", "ShopName");
            return View(configurableETAList);
        }
        [HttpPost]
        [AllowAnonymous]
        [AuthorizeSpirit(Roles = "Shopper")]
        public JsonResult UpdateConfigurableETADetail(int configurableETAId, string deliveryStartHours,string deliveryEndHours, int shopId, DateTime dryDay,string firstDeliverySTInMin,string firstDeliveryETInMin,int delDeadLineStart,int delDeadLineEnd ,string remarks)
        {
            ConfigurableETADBO configurableETADBO = new ConfigurableETADBO();
            string uId = User.Identity.GetUserId();
            var aspUSer = db.AspNetUsers.Find(uId);
            var configurableETADO = new ConfigurableETADO
            {   
                ConfigurableETAId =configurableETAId,
                DeliveryStartHours = deliveryStartHours,
                DeliveryEndHours = deliveryEndHours,
                ShopId = shopId,
                DryDay = dryDay,
                FirstDeliverySTInMin =firstDeliverySTInMin,
                FirstDeliveryETInMin = firstDeliveryETInMin,
                DelDeadLineStart = delDeadLineStart,
                DelDeadLineEnd = delDeadLineEnd,
                Remarks = remarks,
                ModifiedBy = aspUSer.Email

            };
            var res = configurableETADBO.UpdateConfigurableETA(configurableETADO);
            var configurableETA = configurableETADBO.GetConfigurableETA(configurableETAId);

            return Json(new { 
                
                ConfigurableETAId = configurableETA.ConfigurableETAId,
                DeliveryStartHours = configurableETA.DeliveryStartHours,
                DeliveryEndHours = configurableETA.DeliveryEndHours,
                ShopId = configurableETA.ShopId,
                DryDay = configurableETA.DryDay,
                FirstDeliverySTInMin = configurableETA.FirstDeliverySTInMin,
                FirstDeliveryETInMin = configurableETA.FirstDeliveryETInMin,
                DelDeadLineStart = configurableETA.DelDeadLineStart,
                DelDeadLineEnd = configurableETA.DelDeadLineEnd,
                Remarks = configurableETA.Remarks,
                status = true, 
                Message = "Record Updated" 
            });
        }

        [AllowAnonymous]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult UpdateDryDay(DateTime dryDay)
        {
            ConfigurableETADBO configurableETADBO = new ConfigurableETADBO();
            string uId = User.Identity.GetUserId();
            var aspUSer = db.AspNetUsers.Find(uId);
            DateTime dateTime =Convert.ToDateTime(dryDay);
            var res = configurableETADBO.UpdateDryDayForAllShops(dateTime, aspUSer.Email);
            if (res ==1)
            {
                ViewBag.Msg = "Dry Day Updated For All Shops Successfully";
                var configurableETAList = configurableETADBO.GetConfigurableETAList();
                var shop1 = db.WineShops.Where(x => x.OperationFlag == true).ToList();
                ViewBag.Shops = new SelectList(shop1, "Id", "ShopName");
                return View("ConfigurableETA",configurableETAList);
            }
            ViewBag.Msg = "Dry Day Updated For All Shops Successfully";
            var configurableETAList1 = configurableETADBO.GetConfigurableETAList();
            var shop = db.WineShops.Where(x => x.OperationFlag == true).ToList();
            ViewBag.Shops = new SelectList(shop, "Id", "ShopName");
            return View("ConfigurableETA",configurableETAList1);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult ProductCompetitorLink(int id)
        {
            IList<ProductCompetitorLinkDO> productCompetitorLinkDO =null;
            IList<ProductCompetitorLinkDO> singleList = null;
            ProductDBO productDBO = new ProductDBO();
            //productCompetitorLinkDO = productDBO.GetCompetitorProductDetail(id);
            productCompetitorLinkDO = productDBO.GetCompetitorProductDetailByProductName(0);
            var isSelected = true;
            singleList = productDBO.GetCompetitorProductDetailByProductName(id);
            ViewBag.ProductName = singleList.FirstOrDefault().ProductName;
            ViewBag.Id = id;
            ViewBag.AllProductList = new SelectList(singleList, "CompetitorProductRefIDs", "CopProductName", isSelected);
            ViewBag.SingleProduct = new SelectList(singleList, "CompetitorProductRefIDs", "CopProductName");
            return View(productCompetitorLinkDO);
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult ProductCompetitorLinkList()
        {
            ProductDBO productDBO = new ProductDBO();
            IList<ProductCompetitorLinkDO> productCompetitorLinkDO = null;
            productCompetitorLinkDO = productDBO.GetCompetitorProductDetail(0);
            IEnumerable<IGrouping<string, ProductCompetitorLinkDO>> groups = productCompetitorLinkDO.GroupBy(x => x.ProductName);
            var res = groups;
            ViewBag.ProductList = new SelectList(res, "CompetitorProductRefIDs", "CopProductName");
            return View(groups);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "Shopper")]
        public JsonResult AddAndUpdateCompetitorProduct(int id, string[] competPrudts,string action)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            ProductDBO productDBO = new ProductDBO();
            string products = String.Join(",", competPrudts);
            var prods = productDBO.AddAndUpdateCompetitorProduct(id, products,u.Email,action);

            return Json(new { Competitors = prods });
        }

        [HttpGet]
        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult AddProductCompetitorLink()
        {
            IList<ProductCompetitorLinkDO> productCompetitorLinkDO = null;
            IList<ProductCompetitorLinkDO> notLinkProductList = null;
            ProductDBO productDBO = new ProductDBO();
            productCompetitorLinkDO = productDBO.GetCompetitorProductDetail(1);

            notLinkProductList = productDBO.GetCompetitorProductDetailByProductName(0);
            ViewBag.NotLinkProductList = new SelectList(notLinkProductList, "CompetitorProductRefIDs", "CopProductName");
            ViewBag.AllProductList = new SelectList(productCompetitorLinkDO, "CompetitorProductRefIDs", "CopProductName");
            return View(productCompetitorLinkDO);
        }

        [HttpPost]
        [AuthorizeSpirit(Roles = "Shopper")]
        public JsonResult AddCompetitorProduct(string[] produtId, string[] competPrudts, string action)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            ProductDBO productDBO = new ProductDBO();
            string products = String.Join(",", competPrudts);
            string id = String.Join(",", produtId);
            var prods = productDBO.AddAndUpdateCompetitorProduct(Convert.ToInt32(id), products, u.Email, action);

            return Json(new { Competitors = prods });
        }

        [AuthorizeSpirit(Roles = "ShopAdmin")]
        [StopAction]
        public ActionResult OrderWiseBreakDown(DateTime? date, int? page, int? month , int? year, string[] shopids)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            OrderDBO orderDBO = new OrderDBO();
            IList<OrderWiseBreakDownDO> orderWiseBreakDownDO = null;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var shop = from del in db.DelManageShops
                       join sh in db.WineShops on del.ShopID equals sh.Id
                       where del.UserType == 15 && del.rUserId == user.rUserId
                       select sh;
            ViewBag.Shops = new SelectList(shop, "Id", "ShopName");
            string ids=null;
            if (shopids !=null)
            {
                ids = String.Join(",", shopids);
            }
            if (pageNumber > 1)
            {
                ids = TempData.Peek("Ids").ToString();
                month = Convert.ToInt32(TempData.Peek("Month"));
                year = Convert.ToInt32(TempData.Peek("Year"));
                date = Convert.ToDateTime(TempData.Peek("Date"));
                orderWiseBreakDownDO = orderDBO.GetOrderWiseBreakDown(pageNumber, pageSize, date, month, year, ids, user.rUserId);
                var sumOfTotal = orderWiseBreakDownDO.Sum(a => a.Total);
                ViewBag.SumOfTotal = sumOfTotal;
            }
            else if (pageNumber == 1 && (TempData["Date"] != null && Convert.ToInt32(TempData.Peek("Month")) == 0 && Convert.ToInt32(TempData.Peek("Year"))==0) && (TempData["Ids"] != null && TempData["Ids"].ToString() != ""))
            {
                ids = TempData.Peek("Ids").ToString();
                date = Convert.ToDateTime(TempData.Peek("Year"));
                orderWiseBreakDownDO = orderDBO.GetOrderWiseBreakDown(pageNumber, pageSize, date, month, year, ids, user.rUserId);
                var sumOfTotal = orderWiseBreakDownDO.Sum(a => a.Total);
                ViewBag.SumOfTotal = sumOfTotal;
            }
            else if (pageNumber == 1 && (TempData["Date"] == null && Convert.ToInt32(TempData.Peek("Month")) > 0 && Convert.ToInt32(TempData.Peek("Year")) > 0) && (TempData["Ids"] != null && TempData["Ids"].ToString() != ""))
            {
                ids = TempData.Peek("Ids").ToString();
                year = Convert.ToInt32(TempData.Peek("Year"));
                date = Convert.ToDateTime(TempData.Peek("Date"));
                orderWiseBreakDownDO = orderDBO.GetOrderWiseBreakDown(pageNumber, pageSize, date, month, year, ids, user.rUserId);
                var sumOfTotal = orderWiseBreakDownDO.Sum(a => a.Total);
                ViewBag.SumOfTotal = sumOfTotal;
            }
            else
            {
                TempData["Ids"] = "";
                TempData["Date"] = "";
                TempData["Month"] = "";
                TempData["Year"] = "";
                if (shopids != null)
                {
                    ids = String.Join(",", shopids);
                    TempData["Ids"] = ids;
                }
                if (date == null)
                {
                    date = DateTime.Now;
                }
                TempData["Date"] = date;
                TempData["Month"] = month;
                TempData["Year"] = year;
                orderWiseBreakDownDO = orderDBO.GetOrderWiseBreakDown(pageNumber, pageSize, date, month, year, ids, user.rUserId);
                var sumOfTotal = orderWiseBreakDownDO.Sum(a => a.Total);
                ViewBag.SumOfTotal = sumOfTotal;
            }
            return View(orderWiseBreakDownDO.ToPagedList(pageNumber, pageSize));
        }

        [AuthorizeSpirit(Roles = "DeliverySupervisor,DeliveryManager")]
        [StopAction]
        public ActionResult DailyPODCashCollection(DateTime? date ,int? page,string[] shopids)
        {
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
            OrderDBO orderDBO = new OrderDBO();
            IList<DailyPODCashCollectionDO> dailyPODCashCollectionDOs = null;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            string ids = null;
            if (user.UserType ==7)
            {
                var shop = db.WineShops.Where(x => x.OperationFlag == true).ToList();
                ViewBag.Shops = new SelectList(shop, "Id", "ShopName");
            }
            else
            {
                var shop = from del in db.DelManageShops
                           join sh in db.WineShops on del.ShopID equals sh.Id
                           where del.UserType == 10 && del.rUserId == user.rUserId
                           select sh;
                ViewBag.Shops = new SelectList(shop, "Id", "ShopName");
            }
            if (pageNumber > 1)
            {
                ids = TempData.Peek("Ids").ToString();
                date = Convert.ToDateTime(TempData.Peek("Date"));
                dailyPODCashCollectionDOs = orderDBO.GetDailyPODCashColletion(date, ids, user.rUserId);
                var sumOfTotal = dailyPODCashCollectionDOs.Sum(a => a.Total);
                ViewBag.SumOfTotal = sumOfTotal;
            }
            else if (pageNumber == 1 && TempData["Date"] !=null &&  (TempData["Ids"] != null && TempData["Ids"].ToString() != ""))
            {
                ids = TempData.Peek("Ids").ToString();
                date = Convert.ToDateTime(TempData.Peek("Date"));
                dailyPODCashCollectionDOs = orderDBO.GetDailyPODCashColletion(date, ids, user.rUserId);
                var sumOfTotal = dailyPODCashCollectionDOs.Sum(a => a.Total);
                ViewBag.SumOfTotal = sumOfTotal;
            }
            else
            {
                TempData["Ids"] = "";
                TempData["Date"] = "";
                if (shopids != null)
                {
                    ids = String.Join(",", shopids);
                    TempData["Ids"] = ids;
                }
                if (date == null)
                {
                    date = DateTime.Now;
                }
                TempData["Date"] = date;
                dailyPODCashCollectionDOs = orderDBO.GetDailyPODCashColletion(date, ids, user.rUserId);
                var sumOfTotal = dailyPODCashCollectionDOs.Sum(a => a.Total);
                ViewBag.SumOfTotal = sumOfTotal;
            }
            return View(dailyPODCashCollectionDOs.ToPagedList(pageNumber, pageSize));
            
        }

        public ActionResult LiveTracking(int id)
        {
            OrderDBO orderDBO = new OrderDBO();
            string sharedUrl = orderDBO.GetHyperTrackSharedUrl(id);
            if (sharedUrl ==null || sharedUrl =="")
            {
                ViewBag.Message = $"Tracking is not available for Order Id : {id}";

            }
            ViewBag.LiveTracking = sharedUrl;
            return View();
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult CashCollectionBackToStore()
        {
            OrderDBO orderDBO = new OrderDBO();
            var result = orderDBO.GetDeliveryPaymentCashCollection();
            if (TempData["Message"] !=null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(result);
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult UpdateCashCollectionBackToStore(int orderId,int delAgentId)
        {
            OrderDBO orderDBO = new OrderDBO();
            var u = db.AspNetUsers.Where(o => o.Email == User.Identity.Name).FirstOrDefault();
            var result = orderDBO.UpdateCashCollectionBackToStore(orderId,delAgentId);
            if (result == 1)
            {
                
                TempData["Message"] = "Cash Collection Updated Successfully";
                db.OrderTracks.Add(new OrderTrack { OrderId = orderId, TrackDate = DateTime.Now, StatusId = 46, LogUserId = u.Id, LogUserName = u.Email, Remark="Hand over clear by cashier" });
                db.SaveChanges();
            }
            else if(result == 2)
            {
                TempData["Message"] = "BackToStore Updated Successfully";
                db.OrderTracks.Add(new OrderTrack { OrderId = orderId, TrackDate = DateTime.Now, StatusId = 46, LogUserId = u.Id, LogUserName = u.Email, Remark = "Hand over clear by cashier" });
                db.SaveChanges();
            }
            else
            {
                TempData["Message"] = "No Record Updated";
            }
            return RedirectToAction("CashCollectionBackToStore");
        }

        [AuthorizeSpirit(Roles = "Shopper")]
        public ActionResult FullRefund(int id)
        {
            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
            var refund = paymentLinkLogsService.CashFreeReFundForBackToStore(id);
            TempData["RefundSMS"] = refund.Message;
            return RedirectToAction("Details", "Orders", new { id = id });
        }



    }
}
