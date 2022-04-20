using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services.DO;

namespace RainbowWine.Services.DBO
{
    public class OrderDBO
    {
        public List<OrderDO> GetCustomerContact()
        {
            List<OrderDO> ord = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    ord = dbQuery.Query<OrderDO, CustomerAddress, OrderDO>("Order_Zone_Sel",
                        (o, c) =>
                        {
                            o.CustAddress = c;
                            return o;
                        },
                        splitOn: "CustomerAddressId",
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ord;
        }

        public List<OrderDO> GetOrderDetails(int? OrderId)
        {
            List<OrderDO> order = new List<OrderDO>();
            var cDictionary = new Dictionary<int, OrderDO>();
            var quDictionary = new Dictionary<int, OrderDetail>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", OrderId);
                    order = dbQuery.Query<OrderDO, OrderStatu, WineShop, OrderDetail, CustomerAddress, ProductDetailsExtDO, ProductSize, OrderDO>("OrderDetails_sel",
                        (o, os, ws, od, ca, pd, ps) =>
                        {
                            //o.OrderDetails = od;
                            //return o;
                            if (!cDictionary.TryGetValue(o.Id, out var ord))
                            {
                                ord = o;
                                ord.OrderStatu = os;
                                ord.WineShop = ws;
                                o.CustAddress = ca;
                                cDictionary.Add(o.Id, ord);
                            }
                            else
                            {
                                ord = cDictionary[o.Id];
                            }
                            if (od != null)
                            {
                                if (!quDictionary.TryGetValue(od.Id, out var ordetail))
                                {
                                    ordetail = od;
                                    ordetail.ProductDetail = pd;
                                    pd.prdSize = ps;
                                    quDictionary.Add(od.Id, ordetail);
                                    ord.OrderDetails.Add(ordetail);
                                }
                            }
                            return o;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OStatisId,Wid,ODetailID,caddid,prodid,prdsizeID").ToList();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            var objCust = cDictionary?.Values;
            return objCust.ToList();
        }

        public List<DeliveryPayment> GetDeliveryPaymentDetails(int agentId)
        {
            var cDictionary = new Dictionary<int, DeliveryPayment>();
            var quDictionary = new Dictionary<int, OrderDetail>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    var order = dbQuery.Query<DeliveryPayment, Order, OrderDetail, ProductDetail, ProductSize, DeliveryPayment>("OrderDetail_Handover_Sel",
                        (dp, o, od, pd, pz) =>
                        {
                            //o.OrderDetails = od;
                            //return o;
                            if (!cDictionary.TryGetValue(dp.DeliveryPaymentId, out var delpay))
                            {
                                delpay = dp;
                                delpay.Order = o;
                                var detail = od;
                                detail.ProductDetail = pd;
                                detail.ProductDetail.Category = pz.Capacity;
                                delpay.Order.OrderDetails.Add(detail);
                                cDictionary.Add(dp.DeliveryPaymentId, delpay);
                            }
                            else
                            {
                                delpay = cDictionary[dp.DeliveryPaymentId];
                            }
                            if (od != null)
                            {
                                if (!quDictionary.TryGetValue(od.Id, out var ordetail))
                                {
                                    ordetail = od;
                                    ordetail.ProductDetail = pd;
                                    ordetail.ProductDetail.Category = pz.Capacity;
                                    delpay.Order.OrderDetails.Add(ordetail);
                                }
                            }
                            return dp;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "Id,ODetail,pid,ProductSizeID").ToList();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            var objCust = cDictionary?.Values;
            return objCust.ToList();
        }

        public List<DeliveryBackToStore> GetDeliveryBacktoStoreDetails(int agentId)
        {
            var cDictionary = new Dictionary<int, DeliveryBackToStore>();
            var quDictionary = new Dictionary<int, OrderDetail>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    var order = dbQuery.Query<DeliveryBackToStore, Order, OrderDetail, ProductDetail, ProductSize, DeliveryBackToStore>("OrderDetail_Handover_BackToStore_Sel",
                        (dp, o, od, pd, pz) =>
                        {
                            //o.OrderDetails = od;
                            //return o;
                            if (!cDictionary.TryGetValue(dp.DeliveryBackToStoreId, out var delpay))
                            {
                                delpay = dp;
                                delpay.Order = o;
                                var detail = od;
                                detail.ProductDetail = pd;
                                detail.ProductDetail.Category = pz.Capacity;
                                delpay.Order.OrderDetails.Add(detail);
                                cDictionary.Add(dp.DeliveryBackToStoreId, delpay);
                            }
                            else
                            {
                                delpay = cDictionary[dp.DeliveryBackToStoreId];
                            }
                            if (od != null)
                            {
                                if (!quDictionary.TryGetValue(od.Id, out var ordetail))
                                {
                                    ordetail = od;
                                    ordetail.ProductDetail = pd;
                                    ordetail.ProductDetail.Category = pz.Capacity;
                                    delpay.Order.OrderDetails.Add(ordetail);
                                }
                            }
                            return dp;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "Id,ODetail,pid,ProductSizeID").ToList();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            var objCust = cDictionary?.Values;
            return objCust.ToList();
        }

        public List<DeliveryBackToStore> GetDeliveryBacktoStoreDetailsNew(int agentId)
        {
            var cDictionary = new Dictionary<int, DeliveryBackToStore>();
            var quDictionary = new Dictionary<int, OrderDetail>();
            var quMixerDictionary = new Dictionary<int, MixerOrderItem>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    var order = dbQuery.Query<DeliveryBackToStore, Order, OrderDetail, ProductDetail, ProductSize, MixerDetailItem, DeliveryBackToStore>("OrderDetail_Handover_BackToStore_Sel_New",
                        (dp, o, od, pd, pz, mixeritems) =>
                        {
                            //o.OrderDetails = od;
                            //return o;
                            if (!cDictionary.TryGetValue(dp.DeliveryBackToStoreId, out var delpay))
                            {
                                delpay = dp;
                                delpay.Order = o;
                                var detail = od;
                                detail.ProductDetail = pd;
                                detail.ProductDetail.Category = pz.Capacity;
                                delpay.Order.OrderDetails.Add(detail);
                                cDictionary.Add(dp.DeliveryBackToStoreId, delpay);
                            }
                            else
                            {
                                delpay = cDictionary[dp.DeliveryBackToStoreId];
                            }
                            if (od != null)
                            {
                                if (!quDictionary.TryGetValue(od.Id, out var ordetail))
                                {
                                    ordetail = od;
                                    ordetail.ProductDetail = pd;
                                    ordetail.ProductDetail.Category = pz.Capacity;
                                    if (!delpay.Order.OrderDetails.Any(x => x.Id == ordetail.Id))
                                        delpay.Order.OrderDetails.Add(ordetail);
                                }
                            }
                            if (mixeritems != null)
                            {
                                if (!quMixerDictionary.TryGetValue(od.Id, out var mixerDetail))
                                {
                                    mixerDetail = new MixerOrderItem() { MixerOrderItemId = mixeritems.MixerOrderItemId, MixerDetailId = mixeritems.MixerDetailId, ItemQty = mixeritems.MixerItemQty, Price = mixeritems.MixerPrice };
                                    mixerDetail.MixerDetail = new MixerDetail() { MixerDetailId = mixeritems.MixerDetailId, MixerDetailName = mixeritems.MixerName, Price = mixeritems.MixerPrice };

                                    if (!delpay.Order.MixerOrderItems.Any(x => x.MixerOrderItemId == mixerDetail.MixerOrderItemId))
                                    {
                                        delpay.Order.MixerOrderItems.Add(mixerDetail);
                                        delpay.Order.OrderDetails.Add(new OrderDetail()
                                        {
                                            ProductDetail = new ProductDetail()
                                            {
                                                ProductName = mixerDetail.MixerDetail.MixerDetailName,
                                                Category = mixeritems.MixerCapacity
                                            },
                                            ItemQty = mixerDetail.ItemQty.Value,
                                            Price = mixeritems.MixerPrice,
                                        });
                                    }
                                }
                            }
                            return dp;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "Id,ODetail,pid,ProductSizeID,MixerOrderItemId").ToList();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            var objCust = cDictionary?.Values;
            return objCust.ToList();
        }

        public List<DeliveryBackToStore> GetDelV3BacktoStoreDetailsNew(int agentId)
        {
            var cDictionary = new Dictionary<int, DeliveryBackToStore>();
            var quDictionary = new Dictionary<int, OrderDetail>();
            var quMixerDictionary = new Dictionary<int, MixerOrderItem>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    var order = dbQuery.Query<DeliveryBackToStore, Order, OrderDetail, ProductDetail, ProductSize, MixerDetailItem, DeliveryBackToStore>("DelV3_OrderDetail_Handover_BackToStore_Sel_New",
                        (dp, o, od, pd, pz, mixeritems) =>
                        {
                            //o.OrderDetails = od;
                            //return o;
                            if (!cDictionary.TryGetValue(dp.DeliveryBackToStoreId, out var delpay))
                            {
                                delpay = dp;
                                delpay.Order = o;
                                var detail = od;
                                detail.ProductDetail = pd;
                                detail.ProductDetail.Category = pz.Capacity;
                                delpay.Order.OrderDetails.Add(detail);
                                cDictionary.Add(dp.DeliveryBackToStoreId, delpay);
                            }
                            else
                            {
                                delpay = cDictionary[dp.DeliveryBackToStoreId];
                            }
                            if (od != null)
                            {
                                if (!quDictionary.TryGetValue(od.Id, out var ordetail))
                                {
                                    ordetail = od;
                                    ordetail.ProductDetail = pd;
                                    ordetail.ProductDetail.Category = pz.Capacity;
                                    if (!delpay.Order.OrderDetails.Any(x => x.Id == ordetail.Id))
                                        delpay.Order.OrderDetails.Add(ordetail);
                                }
                            }
                            if (mixeritems != null)
                            {
                                if (!quMixerDictionary.TryGetValue(od.Id, out var mixerDetail))
                                {
                                    mixerDetail = new MixerOrderItem() { MixerOrderItemId = mixeritems.MixerOrderItemId, MixerDetailId = mixeritems.MixerDetailId, ItemQty = mixeritems.MixerItemQty, Price = mixeritems.MixerPrice };
                                    mixerDetail.MixerDetail = new MixerDetail() { MixerDetailId = mixeritems.MixerDetailId, MixerDetailName = mixeritems.MixerName, Price = mixeritems.MixerPrice };

                                    if (!delpay.Order.MixerOrderItems.Any(x => x.MixerOrderItemId == mixerDetail.MixerOrderItemId))
                                    {
                                        delpay.Order.MixerOrderItems.Add(mixerDetail);
                                        delpay.Order.OrderDetails.Add(new OrderDetail()
                                        {
                                            ProductDetail = new ProductDetail()
                                            {
                                                ProductName = mixerDetail.MixerDetail.MixerDetailName,
                                                Category = mixeritems.MixerCapacity
                                            },
                                            ItemQty = mixerDetail.ItemQty.Value,
                                            Price = mixeritems.MixerPrice,
                                        });
                                    }
                                }
                            }
                            return dp;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "Id,ODetail,pid,ProductSizeID,MixerOrderItemId").ToList();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            var objCust = cDictionary?.Values;
            return objCust.ToList();
        }

        public int UpdateIssueOrder(int issueId, int orderId)
        {
            int ret = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderIssueId", issueId);
                    para.Add("@OrderId", orderId);
                    ret = dbQuery.Execute("OrderDetail_IssueDetail_Upd", param: para,
                        commandType: CommandType.StoredProcedure);
                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ret;
        }

        public int UpdateOrderModify(int oModifyId, int orderId)
        {
            int ret = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderModifyId", oModifyId);
                    para.Add("@OrderId", orderId);
                    ret = dbQuery.Execute("OrderDetailModify_Detail_Upd", param: para,
                        commandType: CommandType.StoredProcedure);
                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ret;
        }

        public List<NotifyOrder> ProductNotificationDetails(int? shopId, int? ProuctId, int? CustomerId)
        {
            List<NotifyOrder> notifyorder = new List<NotifyOrder>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();

                    para.Add("@ShopId", shopId);
                    para.Add("@ProuctId", ProuctId);
                    para.Add("@CustomerId", CustomerId);

                    notifyorder = dbQuery.Query<NotifyOrder, Notify, NotifyOrder>("NotificationDetails_Shop_ProductWise",
                        (n, noo) =>
                        {
                            n.Notify = noo;
                            return n;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "NOrderid").ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return notifyorder;
        }


        public List<PaymentType> GetOrderPaymentType()
        {
            List<PaymentType> ord = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    ord = dbQuery.Query<PaymentType>("PaymentType_Sel",
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ord;
        }

        public void UpdatedOrderStatus(int orderId, string userId, string orderStatus)
        {
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    para.Add("@OrderId", orderId);
                    para.Add("@OrderStatus", orderStatus);
                    int ret = dbQuery.Execute("Orders_Status_Ins", param: para,
                        commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }

            }
        }

        public Order GetOrder(int orderId)
        {
            Order ord = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    ord = dbQuery.Query<Order, OrderStatu, OrderDetail, Order>("Orders_Sel",
                        (o, os, od) =>
                        {
                            o.OrderStatu = os;
                            o.OrderDetails.Add(od);
                            return o;
                        },
                        param: para,
                        splitOn: "StatusId,oDetail",
                        commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ord;
        }

        public List<Order> GetDeliveryAgentOrder(int orderId, int agentId)
        {
            List<Order> ord = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId.ToString());
                    para.Add("@DeliveryAgentId", agentId);
                    ord = dbQuery.Query<Order, OrderStatu, OrderDetail, Order>("Orders_DeliverySearch_Sel",
                        (o, os, od) =>
                        {
                            o.OrderStatu = os;
                            o.OrderDetails.Add(od);
                            return o;
                        },
                        param: para,
                        splitOn: "StatusId,oDetail",
                        commandType: CommandType.StoredProcedure)?.ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ord;
        }

        public int UpdateOrderDetailModify(int orderId, string agentId)
        {
            int ret = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@UserId", agentId);
                    ret = dbQuery.Execute("OrderDetailModify_Ins", param: para,
                        commandType: CommandType.StoredProcedure);
                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ret;
        }

        public List<OrderDO> GetAssignmentOrder(int shopId)
        {
            List<OrderDO> ord = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopID", shopId);
                    ord = dbQuery.Query<OrderDO, CustomerAddress, CustomerEta, OrderDO>("Order_Assignment_Sel",
                        (o, cd, eta) =>
                        {
                            if (cd != null) o.CustAddress = cd;
                            if (eta != null) o.OrderEta = eta;
                            return o;
                        },
                        param: para,
                        splitOn: "CustomerAddressId,EtaId",
                        commandType: CommandType.StoredProcedure)?.ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ord;
        }

        public int UpateAssignmentOrder(int shopId, string jobId, string orderIds, int agentId, string userId)
        {
            int ret;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@DeliveryAgentId", agentId);
                    para.Add("@OrderIDds", orderIds);
                    para.Add("@JobId", jobId);
                    para.Add("@UserID", userId);
                    ret = dbQuery.Execute("Routeplan_Assignment_Ins",
                        param: para,
                        commandType: CommandType.StoredProcedure);

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ret;
        }

        public MixerOrderDetailsDO GetOrderMixerDetails(int OrderId)
        {
            MixerOrderDetailsDO mixerOrderDetailsDO = new MixerOrderDetailsDO();
            var cDictionary = new Dictionary<int, ProductCategoryExtDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", OrderId);
                    var results = dbQuery.QueryMultiple("Order_MixerOrderDetails_Sel",
                    param: para,
                    commandType: CommandType.StoredProcedure);
                    var order = results.Read<OrdersExtDO>().ToList();
                    var orderDet = results.Read<OrderDetailExtDO>().ToList();
                    var mixerDet = results.Read<MixerDetailExtDO>().ToList();

                    mixerOrderDetailsDO.Order = order;
                    mixerOrderDetailsDO.OrderDetails = orderDet;
                    mixerOrderDetailsDO.MixerDetails = mixerDet;

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return mixerOrderDetailsDO;
            //OrderExtDO ord = new OrderExtDO(); ;
            //var cDictionary = new Dictionary<int, OrderExtDO>();

            //using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        var para = new DynamicParameters();
            //        para.Add("@OrderId", OrderId);
            //        ord = dbQuery.Query<OrderExtDO, OrderDetail, MixerOrderItem,MixerDetail, OrderExtDO>("Order_MixerOrderDetails_Sel",
            //            (O, OD, MO,MD) =>
            //            {

            //                if (!cDictionary.TryGetValue(O.Id, out var orderd))
            //                {
            //                    orderd = O;
            //                    orderd.OrderDetail.Add(OD);
            //                    orderd.MixerOrderItem.Add(MO);
            //                    orderd.MixerDetails.Add(MD);
            //                    cDictionary.Add(O.Id, orderd);
            //                }
            //                else
            //                {
            //                    orderd = cDictionary[O.Id];
            //                }


            //                return O;
            //            },
            //            param: para,
            //            splitOn: "ProductRefId,MixerOrderItemId,MixerDetailId",
            //            commandType: CommandType.StoredProcedure)?.FirstOrDefault();

            //    }
            //    finally
            //    {
            //        dbQuery.Close();
            //    }

            //}
            //return ord;
        }

        public List<RatingStarttExtDO> OrderAllRating(int CustomerId)
        {

            List<RatingStarttExtDO> prodDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerId", CustomerId);
                    prodDetail = dbQuery.Query<RatingStarttExtDO>("OrderAllRating_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<OrderDetailsExtDO> OrderDetailsRating(int index, int size, int CustomerId, int rating)
        {

            List<OrderDetailsExtDO> prodDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@index", index);
                    para.Add("@size", size);
                    para.Add("@CustomerId", CustomerId);
                    para.Add("@Rating", rating);
                    prodDetail = dbQuery.Query<OrderDetailsExtDO>("OrderDetailsRating_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<CurrentOrderDO> BottomOrder(int customerId)
        {
            List<CurrentOrderDO> currentOrderDO = new List<CurrentOrderDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@customerId", customerId);
                    var results = dbQuery.QueryMultiple("Order_Bottom_Sel",
                      param: para, commandType: CommandType.StoredProcedure);

                    var curOrd = results.Read<CurrentOrderDO>().ToList();
                    //var delOrd = results.Read<DeliverOrderDO>().ToList();
                    currentOrderDO = curOrd;
                    //bottomOrderDO.CurrentOrderDO.AddRange(delOrd) ;

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return currentOrderDO;
        }

        public List<CommittedDateDO> GetCommittedDate(DateTime currentDate, int shopId)
        {

            List<CommittedDateDO> prodDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@currentDate", currentDate);
                    para.Add("@shopId", shopId);
                    prodDetail = dbQuery.Query<CommittedDateDO>("GetCommittedDateForProduct_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public List<CommittedDateDO> GetMixerCommittedDate(DateTime currentDate, int mixerDetailId, int shopId)
        {

            List<CommittedDateDO> prodDetail = null;

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@currentDate", currentDate);
                    para.Add("@MixerDetailId", mixerDetailId);
                    para.Add("@shopId", shopId);
                    prodDetail = dbQuery.Query<CommittedDateDO>("CommittedDateForMixer_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public OrderIdsDO GetOrders(string orderId)
        {
            OrderIdsDO orderIdsDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
               
                    try
                    {
                        var para = new DynamicParameters();
                        para.Add("@OrderId", orderId);
                        orderIdsDO = dbQuery.Query<OrderIdsDO>("OrdersGroup_Sel",param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        

                    } 
                    finally
                    {
                        dbQuery.Close();
                    }
            }
            return orderIdsDO;




        }

        public int OrderRatingIncentive(int Rating,int OrderId)
        {
            
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Rating", Rating);
                    para.Add("@OrderId", OrderId);
                   int res = dbQuery.Query<int>("OrderRating_Incentive_Ins", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return res;

                }
                finally
                {
                    dbQuery.Close();
                }
            }
           




        }

        public LiveOrderTracking GetOrderDetailWithLocation(int orderId)
        {
            LiveOrderTracking liveOrderTracking = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    liveOrderTracking = dbQuery.Query<LiveOrderTracking>("GetOrderDetailsWithLocation", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return liveOrderTracking;




        }
        public List<StatusChange> GetManipulatedTracks(List<OrderTrack> tracks, int orderStatusId)
        {
            var data = new List<StatusChange>() {
                    new StatusChange()
                    {
                        DisplayStatus="Order Recieved",
                        Icon =tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Approved || x.StatusId==(int)OrderStatusEnum.Submitted)
                        ?"/content/images/icon/Order_Received_1.png": "/content/images/icon/Order_Received_0.png",
                        Time =tracks.LastOrDefault(x=>x.StatusId==(int)OrderStatusEnum.Approved || x.StatusId==(int)OrderStatusEnum.Submitted ).TrackDate.ToString("hh:mm tt"),
                        IsCompleted = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Approved || x.StatusId==(int)OrderStatusEnum.Submitted),
                        IsActive = false,
                    },
                    new StatusChange()
                    {
                        DisplayStatus =tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Packed)?"Order Packed" :"Packing your order",
                        Icon =  tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Packed)?
                        "/content/images/icon/Packing_your order_1.png":"/content/images/icon/Packing_your order_0.png",
                        Time = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Packed)?$"{tracks.FirstOrDefault(x=>x.StatusId==(int)OrderStatusEnum.Packed).TrackDate.ToString("hh:mm tt")}":string.Empty,
                        IsCompleted = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Packed),
                        IsActive =orderStatusId==(int)OrderStatusEnum.Approved || orderStatusId==(int)OrderStatusEnum.Submitted
                    },
                     new StatusChange()
                     {
                        DisplayStatus =tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.OutForDelivery)?"Out for delivery"
                        :"Waiting for your order to be picked",
                       Icon = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.OutForDelivery)?
                         "/content/images/icon/Order_Packed_1.png":"/content/images/icon/Order_Packed_0.png",
                        Time = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.OutForDelivery)?$"{tracks.FirstOrDefault(x=>x.StatusId==(int)OrderStatusEnum.OutForDelivery).TrackDate.ToString("hh:mm tt")}":string.Empty,
                        IsCompleted = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.OutForDelivery),
                        IsActive =orderStatusId==(int)OrderStatusEnum.Packed
                     },
                      new StatusChange()
                     {
                        DisplayStatus =tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.DeliveryReached)?"Delivery agent reached"
                        :"Delivery agent hasn't reached yet",
                        Icon =tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.DeliveryReached || x.StatusId==(int)OrderStatusEnum.PODCashSelected
                        || x.StatusId==(int)OrderStatusEnum.PODOnlineSelected || x.StatusId==(int)OrderStatusEnum.PODPaymentSuccess
                        || x.StatusId==(int)OrderStatusEnum.PODCashPaid)?
                        "/content/images/icon/Delivery_started_1.png":"/content/images/icon/Delivery_started_0.png",
                        Time = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.DeliveryReached || x.StatusId==(int)OrderStatusEnum.PODCashSelected
                        || x.StatusId==(int)OrderStatusEnum.PODOnlineSelected || x.StatusId==(int)OrderStatusEnum.PODPaymentSuccess
                        || x.StatusId==(int)OrderStatusEnum.PODCashPaid)
                        ?$"{tracks.FirstOrDefault(x=>x.StatusId==(int)OrderStatusEnum.DeliveryReached).TrackDate.ToString("hh:mm tt")}":string.Empty,
                        IsCompleted = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.DeliveryReached || x.StatusId==(int)OrderStatusEnum.PODCashSelected
                        || x.StatusId==(int)OrderStatusEnum.PODOnlineSelected || x.StatusId==(int)OrderStatusEnum.PODPaymentSuccess
                        || x.StatusId==(int)OrderStatusEnum.PODCashPaid),
                        IsActive = orderStatusId==(int)OrderStatusEnum.OutForDelivery
                     },
                      new StatusChange()
                     {
                        DisplayStatus =tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Delivered)?"Delivered"
                        :"Not delivered",
                        Icon = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Delivered)?
                        "/content/images/icon/Order_Accepted_1.png": "/content/images/icon/Order_Accepted_0.png",
                        Time = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Delivered)?$"{tracks.FirstOrDefault(x=>x.StatusId==(int)OrderStatusEnum.Delivered).TrackDate.ToString("hh:mm tt")}":string.Empty,
                        IsCompleted = tracks.Any(x=>x.StatusId==(int)OrderStatusEnum.Delivered),
                        IsActive = orderStatusId==(int)OrderStatusEnum.DeliveryReached || orderStatusId==(int)OrderStatusEnum.PODCashSelected
                        || orderStatusId==(int)OrderStatusEnum.PODOnlineSelected ||orderStatusId==(int)OrderStatusEnum.PODPaymentSuccess
                        || orderStatusId==(int)OrderStatusEnum.PODCashPaid
                     }
            };

            return data;
        }

        public int UpdatedScheduledOrder(int OrderId)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", OrderId);
                    int res = dbQuery.Query<int>("UpdateScheduledDeliveryOrder", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return res;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public List<OrderDetailMrpIssueDO> GetMrpIssue(int orderId)
        {
            List<OrderDetailMrpIssueDO> liveOrderTracking = new List<OrderDetailMrpIssueDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    liveOrderTracking = dbQuery.Query<OrderDetailMrpIssueDO>("MprIssue_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return liveOrderTracking;




        }

        public int UpdateOrderDetailMrpIssue(int orderId,int productId,int qty,bool isMrpIssue,int newMrp,int newAmount,string userId,string email,string remarks,int statusId)
        {
            int  mrpIssueId=0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@ProductId", productId);
                    para.Add("@Qty", qty);
                    para.Add("@IsMrpIssue", isMrpIssue);
                    para.Add("@NewMrp", newMrp);
                    para.Add("@NewAmount", newAmount);
                    para.Add("@UserId", userId);
                    para.Add("@Email", email);
                    para.Add("@Remarks", remarks);
                    para.Add("@StatusId", statusId);
                    mrpIssueId= dbQuery.Query<int>("OrderDetails_MrpIssue_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return mrpIssueId;




        }

        public void UpdateOrderDetailMrpIssueAfterPaymentSuccess(int orderId)
        {
            
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                   
                    dbQuery.Query<int>("MrpIssuePaymentandRefund", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
           




        }

        public int UpdatedShop(int orderId,int shopId,string userId,string email)
        {

            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@ShopId", shopId);
                    para.Add("@UserId", userId);
                    para.Add("@Email", email);
                    int res = dbQuery.Query<int>("UpdateShopExistingOrder", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    return res;

                }
                finally
                {
                    dbQuery.Close();
                }
            }





        }

        public List<GoodiesOrderDetailsDO> GetGoodiesOrderDetails(int orderId)
        {
            List<GoodiesOrderDetailsDO> goodiesOrderDetailsDOs = new List<GoodiesOrderDetailsDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    goodiesOrderDetailsDOs = dbQuery.Query<GoodiesOrderDetailsDO>("GoodiesOrderDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return goodiesOrderDetailsDOs;




        }

        public int GoodiesInventoryQtyAdded(int orderId)
        {
            int result = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                     result = dbQuery.Query<int>("GoodiesInventry_Addition", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;




        }
        public int GoodiesInventoryQtyDeduction(int orderId)
        {
            int result = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    result = dbQuery.Query<int>("GoodiesInventry_Deduction", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return result;




        }

        public AppVersionDO GetAppVersion(int orderId)
        {
            AppVersionDO appVersionDO = new AppVersionDO();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    appVersionDO = dbQuery.Query<AppVersionDO>("AppVersion_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return appVersionDO;




        }

        public OrderEditDetails GetOrderEditDetails(int orderId)
        {
            OrderEditDetails orderEditDetails = new OrderEditDetails();
            using (IDbConnection dbQuery= new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId",orderId);
                    orderEditDetails = dbQuery.Query<OrderEditDetails>("OrderEditDetails_Sel",param :para,commandType:CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return orderEditDetails;
        }

        public ContactDeatailDO GetContactDetails(int orderId, string userId)
        {
            ContactDeatailDO contactDeatailDO = new ContactDeatailDO();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@UserId", userId);
                    contactDeatailDO = dbQuery.Query<ContactDeatailDO>("GetContactDetail_Call", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return contactDeatailDO;
        }

        public List<WineShopDO> GetShopList()
        {
            List<WineShopDO> wineShopDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", 0);
                    para.Add("@ContactNo", null);
                    para.Add("@Flag", 0);
                    wineShopDO = dbQuery.Query<WineShopDO>("Wineshop_Sel", param: para,
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return wineShopDO;
        }

        public WineShopDO GetShop(int shopId)
        {
            WineShopDO wineShopDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ContactNo", null);
                    para.Add("@Flag", 0);
                    wineShopDO = dbQuery.Query<WineShopDO>("Wineshop_Sel", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return wineShopDO;
        }

        public int UpdateShop(int shopId,string contactNo,bool flag)
        {
            int wineShopDO = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    para.Add("@ContactNo", contactNo);
                    para.Add("@Flag", flag);
                    wineShopDO = dbQuery.Query<int>("Wineshop_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return wineShopDO;
        }

        public List<ZoneDO> GetZoneList()
        {
            List<ZoneDO> zoneDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ZoneId", 0);
                    para.Add("@ShopId", 0);
                    para.Add("@Flag", 0);
                    zoneDO = dbQuery.Query<ZoneDO>("ZoneUpd_Sel", param: para,
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return zoneDO;
        }

        public ZoneDO GetZone(int zoneId)
        {
            ZoneDO zoneDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ZoneId", zoneId);
                    para.Add("@ShopId", 0);
                    para.Add("@Flag", 0);
                    zoneDO = dbQuery.Query<ZoneDO>("ZoneUpd_Sel", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return zoneDO;
        }

        public int UpdateZone(int zoneId,int shopId,bool flag)
        {
            int ZoneDO = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ZoneId", zoneId);
                    para.Add("@ShopId", shopId);
                    para.Add("@Flag", flag);
                    ZoneDO = dbQuery.Query<int>("ZoneUpd_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return ZoneDO;
        }

        public List<OrderWiseBreakDownDO> GetOrderWiseBreakDown(int index,int size, DateTime? date,int? month,int? year,string shopids,string userId)
        {
            List<OrderWiseBreakDownDO> orderWiseBreakDownDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Index", index);
                    para.Add("@Size", size);
                    para.Add("@Date", date);
                    para.Add("@Month", month);
                    para.Add("@Year", year);
                    para.Add("@ShopIds", shopids);
                    para.Add("@UserId", userId);
                    orderWiseBreakDownDO = dbQuery.Query<OrderWiseBreakDownDO>("OrderWiseBreakDown_Sel", param: para,
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return orderWiseBreakDownDO;
        }

        public List<DailyPODCashCollectionDO> GetDailyPODCashColletion(DateTime? date ,string shopids, string userId)
        {
            List<DailyPODCashCollectionDO> dailyPODCashCollectionDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Date", date);
                    para.Add("@ShopIds", shopids);
                    para.Add("@UserId", userId);
                    dailyPODCashCollectionDO = dbQuery.Query<DailyPODCashCollectionDO>("DailyPODCashCollection_Sel", param: para,
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return dailyPODCashCollectionDO;
        }

        public int VerifyAppVersion(string appVersion , string appPlatForm)
        {
            int res=0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@AppVersion", appVersion);
                    para.Add("@AppPlatForm", appPlatForm);
                    res = dbQuery.Query<int>("AppVersionExist", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;




        }

        public string GetInterUserContactNo(string userId)
        {
            string res = string.Empty;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    res = dbQuery.Query<string>("GetInterUserContactNo_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;




        }

        public CashFreeRefundDO GetRefundDetails(int orderId)
        {
            CashFreeRefundDO cashFreeRefund = new CashFreeRefundDO();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    cashFreeRefund = dbQuery.Query<CashFreeRefundDO>("CashFreeRefund_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return cashFreeRefund;




        }

        public double GetRefundInitiatedAmount(int orderId)
        {
            double res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    res = dbQuery.Query<double>("CashFreeRefundInitiated_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;




        }

        public int UpdateRefundInitiatedFailed(int orderId ,int issueId)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@IssueId", issueId);
                    res = dbQuery.Query<int>("CashFreeRefundInitiatedFailed_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;




        }

        public int CheckRefundInitiated(int orderId, int issueId)
        {
            int res = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    para.Add("@IssueId", issueId);
                    res = dbQuery.Query<int>("Check_CashFreeRefundInitiated_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;




        }

        public List<AllTypeRefundDetailsDO> GetAllTypeRefundDetails(int orderId)
        {
            List<AllTypeRefundDetailsDO> allTypeRefundDetailsDO = null; 
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    allTypeRefundDetailsDO = dbQuery.Query<AllTypeRefundDetailsDO>("Get_AllTypeRefundDetails_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return allTypeRefundDetailsDO;




        }

        public string GetHyperTrackSharedUrl(int orderId)
        {
            string res=string.Empty;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    res = dbQuery.Query<string>("HyperTrackTripResponseOrderwise_Sel", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;




        }

        public List<DeliveryBacktoStoreCashColletionDO> GetDeliveryPaymentCashCollection()
        {
            List<DeliveryBacktoStoreCashColletionDO> deliveryBacktoStoreCashColletionDO = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();

                    deliveryBacktoStoreCashColletionDO = dbQuery.Query<DeliveryBacktoStoreCashColletionDO>("DeliveryPayment_CashCollection_Sel", param: para, commandType: CommandType.StoredProcedure).ToList();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return deliveryBacktoStoreCashColletionDO;




        }

        public int UpdateCashCollectionBackToStore(int orderId,int delAgentId)
        {
            int res=0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {

                try
                {
                    var para = new DynamicParameters();
                    para.Add("OrderId",orderId);
                    para.Add("DelAgentId",delAgentId);
                    res = dbQuery.Query<int>("DelBackToStoreCashCollection_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();


                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return res;

        }

    }
}