using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using Dapper;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using RainbowWine.Controllers;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Models.Packers;
using RainbowWine.Services.DO;
using static RainbowWine.Services.FireStoreService;

namespace RainbowWine.Services.DBO
{
    
    public class PackersDBO
    {
        public List<VW_FilterBarcode> CheckBarcodeExist(string barcodeid, int shopid)
        {
            List<VW_FilterBarcode> lst = new List<VW_FilterBarcode>();
            using (var context = new rainbowwineEntities())
            {
                var lstfilter = context.Database.SqlQuery<VW_FilterBarcode>(
                "exec SP_BarcodeFilter @BarcodeID,@ShopId",
                 new SqlParameter("BarcodeID", barcodeid),
                  new SqlParameter("ShopId", shopid)).ToList().FirstOrDefault();
                lst.Add(lstfilter);
            }
            return lst;

        }

        public List<VW_FilterBarcode> GetProductByPRoductName(string productname, int shopid)
        {
            List<VW_FilterBarcode> lst = new List<VW_FilterBarcode>();
            using (var context = new rainbowwineEntities())
            {
                var lstfilter = context.Database.SqlQuery<VW_FilterBarcode>(
                "exec SP_ProductNameFilter @ProductName,@ShopId",
                 new SqlParameter("ProductName", productname),
                  new SqlParameter("ShopId", shopid)).ToList();
                lst = lstfilter;
            }
            return lst;

        }

        public List<VW_FilterBarcode> GetAllProductsByShop(int ShopId)
        {
            List<VW_FilterBarcode> lst = new List<VW_FilterBarcode>();
            using (var context = new rainbowwineEntities())
            {
                lst = context.Database.SqlQuery<VW_FilterBarcode>(
                "exec [ProductInventory_ByShop_Sel] @ShopId",
                 new SqlParameter("ShopId", ShopId)).ToList();
            }
            return lst;
        }

        public List<VW_FilterBarcode> GetRecentModifiedProducts(int ShopID, string ModifiedDate)
        {
            
            List<VW_FilterBarcode> lst = new List<VW_FilterBarcode>();
            if (ModifiedDate == null)
            {
                return lst;
            }
            using (var context = new rainbowwineEntities())
            {
                lst = context.Database.SqlQuery<VW_FilterBarcode>(
                "exec [ProductInventoryRecent_ByShop_Sel] @ShopId, @LastModified",
                 new SqlParameter("ShopId", ShopID), new SqlParameter("LastModified", ModifiedDate)).ToList();
                //lst.Add(lstfilter);
            }
            return lst;
        }
        public string _SPDeductProductfromSALESInventory(PackersInput packersInput)
        {
            string result = "";
            using (var context = new rainbowwineEntities())
            {
                var inventory = context.Inventories.Where(p => p.ProductID == packersInput.ID && p.ShopID == packersInput.ShopID).SingleOrDefault();
                int prev = inventory.QtyAvailable;
                if (prev >= packersInput.Quantity && prev != 0)
                {

                    inventory.LastModified = DateTime.Now;
                    inventory.LastModifiedBy = packersInput.User;
                    inventory.QtyAvailable = Convert.ToInt32(prev) - packersInput.Quantity;
                    context.SaveChanges();
                    InventoryTrack inventrack = new InventoryTrack() { ProductID = packersInput.ID, ShopID = packersInput.ShopID, QtyAvailBefore = prev, QtyAvailAfter = inventory.QtyAvailable, ModifiedDate = DateTime.Now, CreatedDate = DateTime.Now, ChangeSource = packersInput.ChangeSource };
                    context.InventoryTracks.Add(inventrack);
                    context.SaveChanges();
                    result = inventory.QtyAvailable.ToString();
                }
               else if (prev == 0 && (packersInput.ChangeSource == 1 || packersInput.ChangeSource == 3 || packersInput.ChangeSource == 8))
                {
                    //inventory.LastModified = DateTime.Now;
                    //inventory.QtyAvailable = packersInput.Quantity;
                    //context.SaveChanges();
                    InventoryTrack inventrack = new InventoryTrack() { ProductID = packersInput.ID, ShopID = packersInput.ShopID, QtyAvailBefore = packersInput.Quantity, QtyAvailAfter = prev, ModifiedDate = DateTime.Now, CreatedDate = DateTime.Now, ChangeSource = 13 };
                    context.InventoryTracks.Add(inventrack);
                    context.SaveChanges();
                    result = inventory.QtyAvailable.ToString();
                }
                else
                {
                    result = "_" + inventory.QtyAvailable.ToString();
                }
                
                // result = "_"+inventory.QtyAvailable.ToString();
            }

            return result;

        }

        public List<spInventoryTrack_Sel_Grouped> InventoryTrack_Sel_Grouped(int shopid, string date, int changesource)
        {
            List<spInventoryTrack_Sel_Grouped> glst = new List<spInventoryTrack_Sel_Grouped>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spInventoryTrack_Sel_Grouped>(
                 "exec InventoryTrack_Sel_Grouped @ShopId,@ChangeSource,@CreatedDate",

                  new SqlParameter("ShopId", shopid),
                  new SqlParameter("ChangeSource", changesource),
                  new SqlParameter("CreatedDate", Convert.ToDateTime(date).ToShortDateString())).ToList();
                glst = lst;
            }
            return glst;

        }

        public List<spInventoryDetailsGrouped_Sel> GetStockMovement(int shopid, string date)
        {
            List<spInventoryDetailsGrouped_Sel> glst = new List<spInventoryDetailsGrouped_Sel>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spInventoryDetailsGrouped_Sel>(
                 "exec InventoryDetailsGrouped_Sel @ShopId,@CreatedDate",
                  new SqlParameter("ShopId", shopid),
                  new SqlParameter("CreatedDate", Convert.ToDateTime(date))).ToList<spInventoryDetailsGrouped_Sel>();

                glst = lst;
            }
            return glst;

        }

        public List<spRoutePlan_DeliveryShop_Sel> GetDeliveredDetails(int ShopID)
        {
            List<spRoutePlan_DeliveryShop_Sel> glst = new List<spRoutePlan_DeliveryShop_Sel>();
            using (var context = new rainbowwineEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "ShopId",
                    Value = ShopID
                };
                var lst = context.Database.SqlQuery<spRoutePlan_DeliveryShop_Sel>(
                 "exec RoutePlan_DeliveryShop_Sel @ShopId ", idParam).ToList<spRoutePlan_DeliveryShop_Sel>();
                glst = lst;
            }
            return glst;

        }

        public spOrders_OrderDetails_Sel GetOrderDetails(int OrderId)
        {
            spOrders_OrderDetails_Sel lstorder = new spOrders_OrderDetails_Sel();
            using (var context = new rainbowwineEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "OrderId",
                    Value = OrderId
                };


                /*   var result = context.Database
                 .SqlQuery<spOrders_OrderDetails_Sel>("Orders_OrderDetails_Sel @OrderId", idParam)
                 .ToList().FirstOrDefault();*/
                using (var multiResultSet = context.MultiResultSetSqlQuery("exec Orders_OrderDetails_Sel @OrderId", idParam))
                {
                    var SP_Order = multiResultSet.ResultSetFor<spOrders_OrderDetails_Sel>().ToDictionary(x => x.Id);
                    var Orders = multiResultSet.ResultSetFor<OrderDetailSel>().ToArray();
                    // var getOrder = multiResultSet.ResultSetFor<OrderDetail>().GetNextResult<ProductDetail>();
                    // var getOrder1 = multiResultSet.ResultSetFor<OrderDetail>().GetNextResult<ProductSize>();
                    foreach (var Order in Orders)
                    {

                        SP_Order[Order.OrderId].OrderDetails.Add(Order);
                    }

                    lstorder = SP_Order.Values.SingleOrDefault();

                }

                //lstorder = result;
            }
            return lstorder;

        }

        public int FunOrderConfirmed(int? id, string AspNetUser)
        {
            int result = 0;
            using (var db = new rainbowwineEntities())
            {
                int cancelOrder = (int)OrderStatusEnum.CancelByCustomer;
                int packed = (int)OrderStatusEnum.Packed;

                Order order = db.Orders.Find(id);

                var u = db.AspNetUsers.Where(o => o.Email == AspNetUser).FirstOrDefault();
                var user = db.RUsers.Include("UserType1").Where(o => o.rUserId == u.Id)?.FirstOrDefault();

                if (order.OrderStatusId != cancelOrder)
                {
                    order.OrderStatusId = packed;
                    db.SaveChanges();

                   
                    //Live Tracking FireStore
                    CustomerApi2Controller.AddToFireStore(order.Id);
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = AspNetUser,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = order.OrderStatusId,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();



                    //IList<Order> orders = new List<Order>();
                    IList<RoutePlan> routePlans = new List<RoutePlan>();
                    var route = db.DumpRoutePlans.Where(o => o.OrderID == order.Id)?.FirstOrDefault();
                    string JobIdroute = default(string);
                    if (route == null)
                    {
                        var route1 = db.RoutePlans.Where(o => o.OrderID == order.Id)?.FirstOrDefault();
                        JobIdroute = route1.JobId;
                    }
                    else
                    {
                        JobIdroute = route.JobId;
                    }
                    if (user.UserType1.UserTypeName.ToLower() == "packer")
                    {

                        routePlans = PackingOrdersByJobId(JobIdroute);
                        //orders = db.Orders.Include(o => o.Customer).Include(o => o.OrderStatu).Where(o => o.OrderStatusId == 3 && o.ShopID == user.ShopId).ToList();

                    }
                    if (routePlans.Count > 0)
                    {
                        //return RedirectToAction("Pack", new { id = routePlans[0].OrderID });
                    }
                    else
                    {
                        //return RedirectToAction("Pack", new { id = 0 });
                        var jobs = db.DeliveryJobs.Where(o => string.Compare(o.JobId, JobIdroute, true) == 0)?.FirstOrDefault();
                        jobs.IsReady = true;
                        jobs.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        //return RedirectToAction("PackList");
                    }
                    result = 1;
                }
            }

            return result;

        }

        public List<RoutePlan> PackingOrdersByJobId(string jobId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    routePlans = dbQuery.Query<RoutePlan>($"Select * from RoutePlan r inner join orders o on o.Id=r.OrderId where jobid='{jobId}' and o.orderstatusid=3").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public int SPAddProducttoInventory(PackersInput packersInput)
        {
            int result = 0;
            using (var context = new rainbowwineEntities())
            {
                var inventory = context.Inventories.Where(p => p.ProductID == packersInput.ID && p.ShopID == packersInput.ShopID).SingleOrDefault();
                int prev = inventory.QtyAvailable;
                inventory.QtyAvailable = Convert.ToInt32(prev) + packersInput.Quantity;
                inventory.LastModified = DateTime.Now;
                inventory.LastModifiedBy = packersInput.User;
                context.SaveChanges();
                InventoryTrack inventrack = new InventoryTrack() { ProductID = packersInput.ID, ShopID = packersInput.ShopID, QtyAvailBefore = prev, QtyAvailAfter = inventory.QtyAvailable, ModifiedDate = DateTime.Now, CreatedDate = DateTime.Now, ChangeSource = packersInput.ChangeSource };
                context.InventoryTracks.Add(inventrack);
                context.SaveChanges();
                result = inventory.QtyAvailable;
                /*var Existbarcode = context.ProductBarcodeLinks.Where(p => p.BarcodeID == BarcodeID).SingleOrDefault();
                if (Existbarcode == null)
                {
                    var barcodeproductlink = new ProductBarcodeLinks()
                    {
                        BarcodeID = BarcodeID,
                        ProductID = ID,
                        ShopID = ShopID

                    };
                    context.ProductBarcodeLinks.Add(barcodeproductlink);

                }*/
                // context.SaveChanges();
                // result = 1;
            }
            return result;

        }

        public List<MixerBarcodeLink> GetMixerProductBarcode(int productid, int shopid)
        {
            using (var context = new rainbowwineEntities())
            {
                return context.MixerBarcodeLinks.Where(x => x.MixerDetailId == productid).ToList();
            }
        }

        public List<ProductBarcodeLink> GetProductBarcode(int productid, int shopid)
        {
            using (var context = new rainbowwineEntities())
            {
                return context.ProductBarcodeLinks.Where(x => x.ProductID == productid).ToList();
            }
        }

        /**************************************************Dashboard *****************************************************/
        public spPacker_Dashboard_Sel GetDashboardKPIS(int ShopID)
        {
            spPacker_Dashboard_Sel dashboard = new spPacker_Dashboard_Sel();
            using (var context = new rainbowwineEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "ShopId",
                    Value = ShopID
                };
                var lst = context.Database.SqlQuery<spPacker_Dashboard_Sel>(
                 "exec Packer_Dashboard_Sel @ShopId ", idParam).FirstOrDefault();
                dashboard = lst;
            }
            return dashboard;

        }

        /**************************************************Dashboard *****************************************************/

        public int DeliveryManagerTrackAgent(PackIssueInput packIssueInput, string odetailIds)
        {
            int ret = 1;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", packIssueInput.id);
                    para.Add("@UserId", packIssueInput.UserId);
                    para.Add("@odetailIds", odetailIds);
                    ret = dbQuery.Query<int>("Issue_ByPacker_Ins",
                        param: para, commandType: CommandType.StoredProcedure).Single();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return ret;
        }

        public WineShop GetShopDetails(int shopid)
        {
            using (var context = new rainbowwineEntities())
            {
                return context.WineShops.Where(x => x.Id == shopid).SingleOrDefault();
            }
        }

        public List<DDProductList> GetProductListString()
        {
            List<DDProductList> lst = new List<DDProductList>();
            using (var context = new rainbowwineEntities())
            {
                var list1 = (from p in context.ProductDetails
                             join b in context.ProductBarcodeLinks
                             on p.ProductID equals b.ProductID into pb
                             from pr in pb.DefaultIfEmpty()
                             select new DDProductList
                             {
                                 ProductID = p.ProductID,
                                 ProductName = p.ProductName,
                                 Price = p.Price,
                                 BarcodeId = pr.BarcodeID


                             }).ToList();
                lst = list1;
            }
            return lst;
        }

        public int SPDeductProductfromInventory(PackersInput packersInput)
        {
            int result = 0;
            using (var context = new rainbowwineEntities())
            {
                var inventory = context.Inventories.Where(p => p.ProductID == packersInput.ID && p.ShopID == packersInput.ShopID).SingleOrDefault();
                int prev = inventory.QtyAvailable;


                inventory.LastModified = DateTime.Now;
                inventory.LastModifiedBy = packersInput.User;
                inventory.QtyAvailable = Convert.ToInt32(prev) - packersInput.Quantity;
                context.SaveChanges();
                InventoryTrack inventrack = new InventoryTrack() { ProductID = packersInput.ID, ShopID = packersInput.ShopID, QtyAvailBefore = prev, QtyAvailAfter = inventory.QtyAvailable, ModifiedDate = DateTime.Now, CreatedDate = DateTime.Now, ChangeSource = packersInput.ChangeSource };
                context.InventoryTracks.Add(inventrack);
                context.SaveChanges();
                result = inventory.QtyAvailable;
            }

            return result;

        }

        public List<spRoutePlan_Unpacked_Sel> GetUnpackedDetails(int ShopID)
        {
            List<spRoutePlan_Unpacked_Sel> lstunpacked = new List<spRoutePlan_Unpacked_Sel>();
            using (var context = new rainbowwineEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "ShopId",
                    Value = ShopID
                };
                var lst = context.Database.SqlQuery<spRoutePlan_Unpacked_Sel>(
                 "exec RoutePlan_Unpacked_Sel @ShopId ", idParam).ToList<spRoutePlan_Unpacked_Sel>();
                lstunpacked = lst;
            }
            return lstunpacked;

        }

        public List<spRoutePlan_OutForDelivery_Sel> GetOutForDeliveryDetails(int ShopID)
        {
            List<spRoutePlan_OutForDelivery_Sel> lstunpacked = new List<spRoutePlan_OutForDelivery_Sel>();
            using (var context = new rainbowwineEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "ShopId",
                    Value = ShopID
                };
                var lst = context.Database.SqlQuery<spRoutePlan_OutForDelivery_Sel>(
                 "exec RoutePlan_OutForDelivery_Sel @ShopId ", idParam).ToList<spRoutePlan_OutForDelivery_Sel>();
                lstunpacked = lst;
            }
            return lstunpacked;

        }

        public List<spOrderIssue_Sel> GetIssuedDetails(int ShopID)
        {
            List<spOrderIssue_Sel> lstunpacked = new List<spOrderIssue_Sel>();
            using (var context = new rainbowwineEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "ShopId",
                    Value = ShopID
                };
                var lst = context.Database.SqlQuery<spOrderIssue_Sel>(
                 "exec OrderIssue_Sel @ShopId ", idParam).ToList<spOrderIssue_Sel>();
                lstunpacked = lst;
            }
            return lstunpacked;

        }

        public List<spRoutePlan_BackToStore_Archive_Sel> GetBackToStoreDetailsArchive(int ShopID, string date)
        {
            List<spRoutePlan_BackToStore_Archive_Sel> glist = new List<spRoutePlan_BackToStore_Archive_Sel>();
            using (var context = new rainbowwineEntities())
            {
                var lst = context.Database.SqlQuery<spRoutePlan_BackToStore_Archive_Sel>(
                 "exec RoutePlan_BackToStore_Archive_Sel @ShopId,@CreatedDate",
                  new SqlParameter("ShopId", ShopID),
                  new SqlParameter("CreatedDate", (date == "" ? null : Convert.ToDateTime(date).ToShortDateString()))).ToList<spRoutePlan_BackToStore_Archive_Sel>();
                glist = lst;
            }
            return glist;
        }

        public List<spRoutePlan_BackToStore_Sel> GetBackToStoreDetails(int ShopID)
        {
            List<spRoutePlan_BackToStore_Sel> glist = new List<spRoutePlan_BackToStore_Sel>();
            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spRoutePlan_BackToStore_Sel>(
                 "exec RoutePlan_BackToStore_Sel @ShopId",
                  new SqlParameter("ShopId", ShopID)).ToList<spRoutePlan_BackToStore_Sel>();
                glist = lst;
            }
            return glist;

        }

        public List<spRoutePlan_CancelledOrder_Sel> GetCancelledOrder(int ShopID)
        {
            List<spRoutePlan_CancelledOrder_Sel> glst = new List<spRoutePlan_CancelledOrder_Sel>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spRoutePlan_CancelledOrder_Sel>(
                 "exec RoutePlan_CancelledOrder_Sel @ShopId",
                  new SqlParameter("ShopId", ShopID)).ToList<spRoutePlan_CancelledOrder_Sel>();
                glst = lst;
            }
            return glst;

        }

        public int AddNewBarcode(PackersInput packersInput)
        {
            int result = 0;
            using (var context = new rainbowwineEntities())
            {
                var inventory = context.Inventories.Where(p => p.ProductID == packersInput.ID && p.ShopID == packersInput.ShopID).SingleOrDefault();
                int prev = inventory.QtyAvailable;
                inventory.QtyAvailable = Convert.ToInt32(prev) + packersInput.Quantity;
                context.SaveChanges();
                InventoryTrack inventrack = new InventoryTrack() { ProductID = packersInput.ID, ShopID = packersInput.ShopID, QtyAvailBefore = prev, QtyAvailAfter = inventory.QtyAvailable, ModifiedDate = DateTime.Now, CreatedDate = DateTime.Now, ChangeSource = packersInput.ChangeSource };
                context.InventoryTracks.Add(inventrack);
                context.SaveChanges();
                var barcodeproductlink = new ProductBarcodeLink()
                {
                    BarcodeID = packersInput.BarcodeID,
                    ProductID = packersInput.ID,
                    ShopID = packersInput.ShopID

                };
                context.ProductBarcodeLinks.Add(barcodeproductlink);
                context.SaveChanges();
                result = 1;
            }

            return result;
        }

        public List<spDeliveryAgents_ByShopID_Sel_Result> GetAllAgent(int shopid)
        {
            List<spDeliveryAgents_ByShopID_Sel_Result> glst = new List<spDeliveryAgents_ByShopID_Sel_Result>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spDeliveryAgents_ByShopID_Sel_Result>(
                 "exec DeliveryAgents_ByShopID_Sel @ShopId",
                  new SqlParameter("ShopId", shopid)).ToList<spDeliveryAgents_ByShopID_Sel_Result>();
                glst = lst;
            }
            return glst;

        }

        public List<spDeliveryPayment_ByAgentID_Sel> Getcashcollection(int shopid, int agentid)
        {
            List<spDeliveryPayment_ByAgentID_Sel> glst = new List<spDeliveryPayment_ByAgentID_Sel>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spDeliveryPayment_ByAgentID_Sel>(
                 "exec DeliveryPayment_ByAgentID_Sel @DeliveryAgentId,@ShopId",
                  new SqlParameter("ShopId", shopid),
                  new SqlParameter("DeliveryAgentId", agentid)).ToList<spDeliveryPayment_ByAgentID_Sel>();
                glst = lst;
            }
            return glst;

        }

        public List<spDeliveryBackToStore_ByAgentID_Sel> GetOrderCollection(int shopid, int agentid)
        {
            List<spDeliveryBackToStore_ByAgentID_Sel> glst = new List<spDeliveryBackToStore_ByAgentID_Sel>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spDeliveryBackToStore_ByAgentID_Sel>(
                 "exec DeliveryBackToStore_ByAgentID_Sel @DeliveryAgentId,@ShopId",
                  new SqlParameter("ShopId", shopid),
                  new SqlParameter("DeliveryAgentId", agentid)).ToList<spDeliveryBackToStore_ByAgentID_Sel>();
                glst = lst;
            }
            return glst;

        }

        public RUser GetRUser(string ruserid)
        {
            using (var context = new rainbowwineEntities())
            {
                return context.RUsers.Where(x => x.rUserId == ruserid).SingleOrDefault();
            }
        }

        public List<RoutePlan> PackingOrders(int shopId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlanDump_Packing_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DeliveryId").ToList();
                    List<RoutePlan> routePlans2 = new List<RoutePlan>();
                    //if (routePlans.Count <= 0)
                    //{
                    //    para.Add("@ShopId", shopId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlan_Packing_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DeliveryId").ToList();
                    //}

                    if (routePlans == null) routePlans = new List<RoutePlan>();

                    if (routePlans2.Count > 0)
                    {
                        routePlans = routePlans.Concat(routePlans2).ToList();
                    }
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public RainbowWine.Models.Packers.PackerCount PackCount(int shopId)
        {
            RainbowWine.Models.Packers.PackerCount packerCount = new RainbowWine.Models.Packers.PackerCount();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    packerCount = dbQuery.Query<RainbowWine.Models.Packers.PackerCount>("Packer_Count_Sel",
                        param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return packerCount;
        }

        /****************************Acknowledge Order and cash collection***********************/
        public int AckCashCollection(bool flag, string paymentid)
        {
            int result;

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.ExecuteSqlCommand(
                 "exec DeliveryPayment_Acknowledge_Upd @ShopAcknowledgement,@DeliveryPaymentId",
                  new SqlParameter("ShopAcknowledgement", (flag == true ? 1 : 0)),
                  new SqlParameter("DeliveryPaymentId", paymentid));
                result = Convert.ToInt32(lst.ToString());
            }
            return result;

        }

        public int AckOrderCollection(bool flag, string storeid)
        {
            int result;

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.ExecuteSqlCommand(
                 "exec DeliveryBackToStore_Acknowledge_Upd @ShopAcknowledgement,@DeliveryBackToStoreId",
                  new SqlParameter("ShopAcknowledgement", (flag == true ? 1 : 0)),
                  new SqlParameter("DeliveryBackToStoreId", storeid));
                result = Convert.ToInt32(lst.ToString());
            }
            return result;

        }

        /****************************Acknowledge Order and cash collection***********************/

        public List<spInventoryTrack_Sel> InventoryTrackSel(int shopid, string date, int changesource)
        {
            List<spInventoryTrack_Sel> glst = new List<spInventoryTrack_Sel>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spInventoryTrack_Sel>(
                 "exec InventoryTrack_Sel @ShopId,@ChangeSource,@CreatedDate",

                  new SqlParameter("ShopId", shopid),
                  new SqlParameter("ChangeSource", changesource),
                  new SqlParameter("CreatedDate", Convert.ToDateTime(date).ToShortDateString())).ToList();
                glst = lst;
            }
            return glst;

        }

        public List<OrderTrack_Sel> OrderTrackSel(int shopid, string date, string OrderStatusId)
        {
            List<OrderTrack_Sel> glst = new List<OrderTrack_Sel>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<OrderTrack_Sel>(
                 "exec OrderTrack_Sel @ShopId,@OrderStatusId,@CreatedDate",

                  new SqlParameter("ShopId", shopid),
                  new SqlParameter("OrderStatusId", Convert.ToInt32(OrderStatusId)),
                   new SqlParameter("CreatedDate", Convert.ToDateTime(date).ToShortDateString())).ToList();
                glst = lst;
            }
            return glst;

        }

        public List<DeliveryPayment> GetCashCollectionArchive(int ShopID, string date)
        {
            List<DeliveryPayment> glist = new List<DeliveryPayment>();
            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<DeliveryPayment>(
                 "exec DeliveryPayment_Archive_Sel @ShopId,@CreatedDate",
                  new SqlParameter("ShopId", ShopID),
                  new SqlParameter("CreatedDate", (date != "" ? Convert.ToDateTime(date).ToShortDateString() : null))).ToList<DeliveryPayment>();
                glist = lst;
            }
            return glist;

        }

        public int SPUpdateProduct(PackersInput packersInput)
        {
            int result = 0;
            using (var context = new rainbowwineEntities())
            {
                var inventory = context.Inventories.Where(p => p.ProductID == packersInput.ID && p.ShopID == packersInput.ShopID).SingleOrDefault();

                int prev = inventory.QtyAvailable;
                inventory.QtyAvailable = Convert.ToInt32(packersInput.Quantity);
                inventory.LastModified = DateTime.Now;
                inventory.LastModifiedBy = packersInput.User;
                context.SaveChanges();
                InventoryTrack inventrack = new InventoryTrack() { ProductID = packersInput.ID, ShopID = packersInput.ShopID, QtyAvailBefore = prev, QtyAvailAfter = packersInput.Quantity, ModifiedDate = DateTime.Now, CreatedDate = DateTime.Now, ChangeSource = packersInput.ChangeSource };
                context.InventoryTracks.Add(inventrack);
                context.SaveChanges();
                result = inventory.QtyAvailable;
            }

            return result;

        }

        public spRoutePlan_DelCount_Packer_Sel GetBackTosStoreNotify(int ShopID)
        {
            spRoutePlan_DelCount_Packer_Sel glist = new spRoutePlan_DelCount_Packer_Sel();
            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spRoutePlan_DelCount_Packer_Sel>(
                 "exec RoutePlan_DelCount_Packer_Sel @ShopId",
                  new SqlParameter("ShopId", ShopID)).SingleOrDefault();
                glist = lst;
            }
            return glist;

        }

        public List<spInventory_Shop_Sel> CurrentstockByshopID(int shopid)
        {
            List<spInventory_Shop_Sel> glst = new List<spInventory_Shop_Sel>();

            using (var context = new rainbowwineEntities())
            {

                var lst = context.Database.SqlQuery<spInventory_Shop_Sel>(
                 "exec Inventory_Shop_Sel @ShopID",
                  new SqlParameter("ShopID", shopid)).ToList<spInventory_Shop_Sel>();
                glst = lst;
            }
            return glst;

        }

        public WineShopData ShopIdandShopName(int shopId)
        {
            WineShopData wineShop = new WineShopData();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    wineShop = dbQuery.Query<WineShopData>("ShopIdName_Sel",
                        param: para, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return wineShop;
        }

        public List<spRoutePlan_Packed_Sel> GetPackedDetails(int ShopID)
        {
            List<spRoutePlan_Packed_Sel> lstunpacked = new List<spRoutePlan_Packed_Sel>();
            using (var context = new rainbowwineEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "ShopId",
                    Value = ShopID
                };
                var lst = context.Database.SqlQuery<spRoutePlan_Packed_Sel>(
                 "exec RoutePlan_Packed_Sel @ShopId ", idParam).ToList<spRoutePlan_Packed_Sel>();
                lstunpacked = lst;
            }
            return lstunpacked;

        }

        public IList<CollectionDetails> GetShopCollectionDetails(int shopID,DateTime date)
        {
            IList<CollectionDetails> collectionDetails = null;
                using (var context = new rainbowwineEntities())
                {

                var data = context.Database.SqlQuery<CollectionDetails>(
                      "exec CashShopCollection_Sel @ShopId,@Date",
                      new SqlParameter("ShopId", shopID),
                      new SqlParameter("Date", Convert.ToDateTime(date).ToShortDateString())).ToList();
                collectionDetails = data;
                }
            
            return collectionDetails;

        }

        public List<VW_FilterBarcode> GetProductByPRoductNameNew(string productname, int shopid , int isFilter)
        {
            List<VW_FilterBarcode> lst = new List<VW_FilterBarcode>();
            using (var context = new rainbowwineEntities())
            {
                var lstfilter = context.Database.SqlQuery<VW_FilterBarcode>(
                "exec SP_ProductNameFilter1 @ProductName,@ShopId,@IsFilter",
                 new SqlParameter("ProductName", productname),
                  new SqlParameter("ShopId", shopid),
                  new SqlParameter("IsFilter", isFilter)).ToList();
                lst = lstfilter;
            }
            return lst;

        }

        public int InventoryAdditionUpdate(InventoryDO inventoryDO)
        {
            int result =0;

            using (var context = new rainbowwineEntities())
            {

                var res = context.Database.ExecuteSqlCommand(
                 "exec InventoryAddition_Upd  @ID,@Price,@Quantity,@BarcodeID,@ShopID,@ChangeSource,@UserId,@Type",
                  new SqlParameter("ID",inventoryDO.ProductId),
                  new SqlParameter("Price", inventoryDO.Price),
                  new SqlParameter("Quantity", inventoryDO.Quantity),
                  new SqlParameter("BarcodeID", inventoryDO.BarcodeID),
                  new SqlParameter("ShopID", inventoryDO.ShopID),
                  new SqlParameter("ChangeSource", inventoryDO.ChangeSource),
                  new SqlParameter("UserId", inventoryDO.UserId),
                  new SqlParameter("Type", inventoryDO.Type)
                  );
                result = Convert.ToInt32(res.ToString());
            }
            return result;

        }

        public int InventoryDeductionUpdate(InventoryDO inventoryDO)
        {
            int result =0;

            using (var context = new rainbowwineEntities())
            {

                var res = context.Database.ExecuteSqlCommand(
                 "exec InventoryDeduction_Upd  @ID,@Price,@Quantity,@BarcodeID,@ShopID,@ChangeSource,@UserId,@Type",
                  new SqlParameter("ID", inventoryDO.ProductId),
                  new SqlParameter("Price", inventoryDO.Price),
                  new SqlParameter("Quantity", inventoryDO.Quantity),
                  new SqlParameter("BarcodeID", inventoryDO.BarcodeID),
                  new SqlParameter("ShopID", inventoryDO.ShopID),
                  new SqlParameter("ChangeSource", inventoryDO.ChangeSource),
                  new SqlParameter("UserId", inventoryDO.UserId),
                  new SqlParameter("Type", inventoryDO.Type)
                  );
                result = Convert.ToInt32(res.ToString());
            }
            return result;

        }

        public int InventoryOverrideUpdate(InventoryDO inventoryDO)
        {
            int result =0;

            using (var context = new rainbowwineEntities())
            {

                var res = context.Database.ExecuteSqlCommand(
                 "exec InventoryOverride_Upd  @ID,@Price,@Quantity,@BarcodeID,@ShopID,@ChangeSource,@UserId,@Type",
                  new SqlParameter("ID", inventoryDO.ProductId),
                  new SqlParameter("Price", inventoryDO.Price),
                  new SqlParameter("Quantity", inventoryDO.Quantity),
                  new SqlParameter("BarcodeID", inventoryDO.BarcodeID),
                  new SqlParameter("ShopID", inventoryDO.ShopID),
                  new SqlParameter("ChangeSource", inventoryDO.ChangeSource),
                  new SqlParameter("UserId", inventoryDO.UserId),
                  new SqlParameter("Type", inventoryDO.Type)
                  );
                result = Convert.ToInt32(res.ToString());
            }
            return result;

        }

    }
}