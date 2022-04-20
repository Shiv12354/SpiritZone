using Microsoft.AspNet.Identity;
using RainbowWine.Data;
using RainbowWine.Models.Packers;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/1.1")]
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "Packer")]
    public class PackersController : ApiController
    {
        rainbowwineEntities db = new rainbowwineEntities();
        PackersDBO posbll = new PackersDBO();

        [HttpPost]
        [Route("CheckBarcodeExist/{barcodeid}/{shopId}")]
        public IHttpActionResult CheckBarcodeExist(string barcodeid, int shopId)
        {
            return Ok(posbll.CheckBarcodeExist(barcodeid, shopId));
        }

        [HttpPost]
        [Route("GetProductByPRoductName/{productname}/{shopId}")]
        public IHttpActionResult GetProductByPRoductName(string productname,int shopId)
        {
            return Ok(posbll.GetProductByPRoductName(productname, shopId));
        }
        [HttpPost]
        [Route("DeductdProductFromSaleInventory")]
        public IHttpActionResult DeductdProductFromSaleInventory(PackersInput packersInput)
        {
            var user = User.Identity.GetUserId();
            packersInput.User = user;
            return Ok(posbll._SPDeductProductfromSALESInventory(packersInput));
        }

        [HttpPost]
        [Route("GetInventoryInTrack_Sel_Grouped/{shopid}/{date}/{changesource}")]
        public IHttpActionResult GetInventoryInTrack_Sel_Grouped(int shopId,string date, int changesource)
        {
            return Ok(posbll.InventoryTrack_Sel_Grouped(Convert.ToInt32(shopId), date, changesource));
        }
        [HttpPost]
        [Route("GetStockMovement_Sel_Grouped/{shopid}/{date}")]
        public IHttpActionResult GetStockMovement_Sel_Grouped(int shopId,string date)
        {
            return Ok(posbll.GetStockMovement(Convert.ToInt32(shopId), date));
        }

        [HttpPost]
        [Route("GetAllProductsByShop/{ShopId}")]
        public IHttpActionResult AllProductsByShop(int shopId)
        {
            return Ok(posbll.GetAllProductsByShop(shopId));
        }

        [HttpPost]
        [Route("GetRecentModifiedProducts")]
        public IHttpActionResult RecentModifiedProducts(RecentModifiedProductModel recModModel)
        {
            if (String.IsNullOrWhiteSpace(recModModel.LastModified))
            {
                var resp = new Dictionary<string, string>();
                resp.Add("MissingParameter", "Please send a valid last modified date in string format." );
                return Ok(resp);
            }    
            return Ok(posbll.GetRecentModifiedProducts(recModModel.ShopId, recModModel.LastModified));
        }

        [HttpPost]
        [Route("GetOrderDetails/{orderid}")]
        public IHttpActionResult GetOrderDetails(int OrderId)
        {
            return Ok(posbll.GetOrderDetails(OrderId));
        }
        
        [HttpPost]
        [Route("Pack/{id:int?}")]
        public IHttpActionResult PackConfrimed(int? id=0)
        {
            var userId = User.Identity.GetUserId();
            var email= db.AspNetUsers.Where(o => o.Id == userId).FirstOrDefault();
            var status = posbll.FunOrderConfirmed(id, email.Email) == 1 ? true : false;
            CustomerApi2Controller.AddToFireStore(id.Value);
            return Ok(status);
        }

        [HttpPost]
        [Route("AddProducttoInventory")]
        public IHttpActionResult AddProducttoInventory(PackersInput packersInput)
        {
            var user = User.Identity.GetUserId();
            packersInput.User = user;
            return Ok(posbll.SPAddProducttoInventory(packersInput));
        }

        [HttpPost]
        [Route("GetBarcodeByProductId/{productid}/{shopid}/{ismixer}")]
        public IHttpActionResult GetBarcodeByProductId(int productid, int shopId, bool isMixer)
        {

            if (isMixer)
            {
                var result1 = posbll.GetMixerProductBarcode(productid, Convert.ToInt32(shopId));
                return Ok(result1);

            }

            var result = posbll.GetProductBarcode(productid, Convert.ToInt32(shopId));

            return Ok(result);
        }

        [HttpPost]
        [Route("GetDashboarKPIs/{shopId}")]
        public IHttpActionResult GetDashboarKPIs(int shopId)
        {
            return Ok(posbll.GetDashboardKPIS(shopId));
        }

        [HttpPost]
        [Route("PackIssue")]
        public IHttpActionResult PackIssue(PackIssueInput packIssueInput)
        {
            var uId = User.Identity.GetUserId();
            string oIds = string.Join(",", packIssueInput.odetailIds);
            packIssueInput.UserId = uId;
            int issueId = posbll.DeliveryManagerTrackAgent(packIssueInput, oIds);
            var shop = posbll.GetShopDetails(packIssueInput.shopId);
            //if (issueId > 0)
            //{
            //    posbll.GenerateZohoTikect(id, issueId, shopnumber, shop.ShopName);
            //    return Ok(new { msg = "Issue created successfully.", status = true });
            //}
            return Ok(new { msg = "Unable to create Issue.", status = false });
        }

        [HttpPost]
        [Route("GetProductDD")]
        public IHttpActionResult GetProductDD()
        {

            return Ok(posbll.GetProductListString());
        }

        [HttpPost]
        [Route("DeductdProductFromInventory")]
        public IHttpActionResult DeductdProductFromInventory(PackersInput packersInput)
        {
            var user = User.Identity.GetUserId();
            packersInput.User = user;
            return Ok(posbll.SPDeductProductfromInventory(packersInput));
        }

        [HttpPost]
        [Route("GetUnpackedDetails/{shopId}")]
        public IHttpActionResult GetUnpackedDetails(int shopId)
        {
            return Ok(posbll.GetUnpackedDetails(shopId));
        }

        [HttpPost]
        [Route("GetOutForDeliveryDetails/{shopId}")]
        public IHttpActionResult GetOutForDeliveryDetails(int shopId)
        {
            return Ok(posbll.GetOutForDeliveryDetails(shopId));
        }

        [HttpPost]
        [Route("GetIssuedDetails/{shopId}")]
        public IHttpActionResult GetIssuedDetails(int shopId)
        {
            return Ok(posbll.GetIssuedDetails(shopId));
        }

        [HttpPost]
        [Route("GetDeliveredDetails/{shopId}")]
        public IHttpActionResult GetDeliveredDetails(int shopId)
        {
            return Ok(posbll.GetDeliveredDetails(shopId));
        }

        [HttpPost]
        [Route("GetBackToStoreDetailsArchive/{shopId}/{date}")]
        public IHttpActionResult GetBackToStoreDetailsArchive(int shopId, string date)
        {
            return Ok(posbll.GetBackToStoreDetailsArchive(shopId, date));
        }

        [HttpPost]
        [Route("GetBackToStoreDetails/{shopId}")]
        public IHttpActionResult GetBackToStoreDetails(int shopId)
        {
            return Ok(posbll.GetBackToStoreDetails(shopId));
        }

        [HttpPost]
        [Route("GetCancelled/{shopId}")]
        public IHttpActionResult GetCancelled(int shopId)
        {
            return Ok(posbll.GetCancelledOrder(shopId));
        }
        [HttpPost]
        [Route("AddNewBarcode")]
        public IHttpActionResult AddNewBarcode(PackersInput packersInput)
        {
            var user = User.Identity.GetUserId();
            packersInput.User = user;
            return Ok(posbll.AddNewBarcode(packersInput));
        }
        [HttpPost]
        [Route("getAllagents/{shopId}")]
        public IHttpActionResult getAllagents(int shopId)
        {
            return Ok(posbll.GetAllAgent(shopId));
        }

        [HttpPost]
        [Route("getCashCollection/{shopId}/{agentid}")]
        public IHttpActionResult getCashCollection(int shopId, int agentid)
        {
            return Ok(posbll.Getcashcollection(shopId, agentid));
        }

        [HttpPost]
        [Route("getOrderCollection/{shopId}/{agentid}")]
        public IHttpActionResult getOrderCollection(int shopId, int agentid)
        {
            return Ok(posbll.GetOrderCollection(shopId, agentid));
        }

        [HttpPost]
        [Route("PackList")]
        public IHttpActionResult PackList()
        {
            string custId = User.Identity.GetUserId();
            var rUser = posbll.GetRUser(custId);
            // RoutePlanDBO routePlanDBO = new RoutePlanDBO();
            var route = posbll.PackingOrders(rUser.ShopId ?? 0);
            var pCount = posbll.PackCount(rUser.ShopId ?? 0);
            //ViewBag.PackCount = pCount;
            return Ok(new { msg = pCount, status = false });
        }

        [HttpPost]
        [Route("AckCashCollection/{flag}/{ids}")]
        public IHttpActionResult AckCashCollection(bool flag, string ids)
        {
            return Ok(posbll.AckCashCollection(flag, ids));
        }

        [HttpPost]
        [Route("AckOrderCollection/{flag}/{ids}")]
        public IHttpActionResult AckOrderCollection(bool flag, string ids)
        {
            return Ok(posbll.AckOrderCollection(flag, ids));
        }

        [HttpPost]
        [Route("GetInventoryTrackSel/{shopid}/{date}/{changesource}")]
        public IHttpActionResult GetInventoryTrackSel(int shopId, string date, int changesource)
        {

            return Ok(posbll.InventoryTrackSel(Convert.ToInt32(shopId), date, changesource));
        }

        [HttpPost]
        [Route("GetOrderTrackSel/{shopid}/{date}/{OrderStatusId}")]
        public IHttpActionResult GetOrderTrackSel(int shopId, string date, string OrderStatusId)
        {
            return Ok(posbll.OrderTrackSel(Convert.ToInt32(shopId), date, OrderStatusId));
        }

        [HttpPost]
        [Route("GetCashArchive/{shopid}/{date}")]
        public IHttpActionResult GetCashArchive(int shopId, string date)
        {
            return Ok(posbll.GetCashCollectionArchive(Convert.ToInt32(shopId), date));
        }

        [HttpPost]
        [Route("UpdateProduct")]
        public IHttpActionResult UpdateProduct(PackersInput packersInput)
        {
            var user = User.Identity.GetUserId();
            packersInput.User = user;
            return Ok(posbll.SPUpdateProduct(packersInput));
        }

        [HttpPost]
        [Route("NotifyBackToStore/{shopid}")]
        public IHttpActionResult NotifyBackToStore(int shopId)
        {
            return Ok(posbll.GetBackTosStoreNotify(Convert.ToInt32(shopId)));
        }

        [HttpPost]
        [Route("GetCurrentStockByShop/{shopid}")]
        public IHttpActionResult GetCurrentStockByShop(int shopId)
        {
            return Ok(posbll.CurrentstockByshopID(shopId));
        }

        [HttpPost]
        [Route("getshopidname")]
        public IHttpActionResult GetShopIdAndName()
        {
            var uId = User.Identity.GetUserId();
            var ruser = posbll.GetRUser(uId);
            var shopId = ruser.ShopId ?? 0;
            var shop = posbll.ShopIdandShopName(Convert.ToInt32(ruser.ShopId));
            
            return Ok(shop);
        }

        [HttpPost]
        [Route("GetPackedDetails/{shopid}")]
        public IHttpActionResult GetPackedDetails(int shopId)
        {
            return Ok(posbll.GetPackedDetails(Convert.ToInt32(shopId)));
        }

        [HttpPost]
        [Route("GetShopCollectionDetails/{shopid}/{date}")]
        public IHttpActionResult GetShopCollectionDetails(int shopId,DateTime date)
        {
            return Ok(posbll.GetShopCollectionDetails(shopId,date));
        }

        [HttpPost]
        [Route("GetScheduledOrdersByShop/{shopid}")]
        public IHttpActionResult GetScheduledOrdersByShop(int shopId)
        {
            ScheduleDeliveryDBO scheduleDeliveryDBO = new ScheduleDeliveryDBO();
            var data=scheduleDeliveryDBO.GetScheduledOrderListForShop(shopId);
            var result = data.Select(x => new
            {
                OrderId =x.OrderId,
                ScheduleStartDate = x.ScheduledStart,
                ScheduleEndDate = x.ScheduledEnd,
                OrderStatus =x.OrderStatusName
            });
            return Ok(result);
        }

        [HttpPost]
        [Route("GetProductByPRoductNameNew/{productname}/{shopId}/{isfilter}")]
        public IHttpActionResult GetProductByPRoductNameNew(string productname, int shopId,int isFilter)
        {
            return Ok(posbll.GetProductByPRoductNameNew(productname, shopId,isFilter));
        }

        [HttpPost]
        [Route("InventoryAdditionUpdate")]
        public IHttpActionResult InventoryAdditionUpdate(InventoryDO inventotyDO)
        {
            var res =posbll.InventoryAdditionUpdate(inventotyDO);

            if (res > 0)
            {
                var result = new
                {
                    Message = "Inventory Added successfully",
                    Data = res

                };
                return Ok(result);
            }
            else
            {
                return Ok("Inventory Addition failed");
            }
        }

        [HttpPost]
        [Route("InventoryDeductionUpdate")]
        public IHttpActionResult InventoryDeductionUpdate(InventoryDO inventotyDO)
        {
            var res =posbll.InventoryDeductionUpdate(inventotyDO);
            if (res > 0)
            {
                var result = new
                {
                    Message = "Inventory Deducted successfully",
                    Data = res

                };
                return Ok(result);
            }
            else
            {
                return Ok("Inventory Deduction failed");
            }
        }
        [HttpPost]
        [Route("InventoryOverrideUpdate")]
        public IHttpActionResult InventoryOverrideUpdate(InventoryDO inventotyDO)
        {
            var res = posbll.InventoryOverrideUpdate(inventotyDO);
            if (res > 0)
            {
                var result = new
                {
                    Message = "Inventory overriden successfully",
                    Data = res

                };
                return Ok(result);
            }
            else
            {
                return Ok("Inventory Override failed");
            }
        }
    }
}
