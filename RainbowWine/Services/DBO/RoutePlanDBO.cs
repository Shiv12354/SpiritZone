using Dapper;
using RainbowWine.Data;
using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DBO
{
    public class RoutePlanDBO
    {
        public List<RoutePlan> GetDeliveryOrders(int agentId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    routePlans = dbQuery.Query<RoutePlan, Order, RoutePlan>("RoutePlan_ByAgent_Sel",
                        (r, o) => { r.Order = o; return r; },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "Id").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public List<RoutePlan> DeliveryStart(string jobId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@JobId", jobId);
                    routePlans = dbQuery.Query<RoutePlan>("RoutePlan_DelStartOrder_Sel",
                        param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }
        public List<RoutePlan> DeliveryStart(string jobId, int agentId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@JobId", jobId);
                    para.Add("@JobId", agentId);
                    routePlans = dbQuery.Query<RoutePlan>("RoutePlan_DelStartOrder_Sel1",
                        param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
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

        public List<RoutePlan> PackedOrders(int shopId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlan_Packed_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DeliveryId").ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }
        public PackerCount PackCount(int shopId)
        {
            PackerCount packerCount = new PackerCount();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ShopId", shopId);
                    packerCount = dbQuery.Query<PackerCount>("Packer_Count_Sel",
                        param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return packerCount;
        }
        public List<RoutePlan> PackingOrdersByJobId(string jobId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@JobId", jobId);
                    routePlans = dbQuery.Query<RoutePlan>("RoutePlan_PackingOrder_Sel",
                        param: para, commandType: CommandType.StoredProcedure).ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }
        public List<RoutePlan> DevlieryOrders(int agentId, int statusId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    para.Add("@StatusId", statusId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlan_Delivery_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DelAgentId").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public List<RoutePlan> DevlieredOrders(int agentId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlan_AgentDelivered_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DelAgentId").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }
        public List<RoutePlan> DevlieryOrdersTrack(int agentId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlan_DelTracking_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DelAgentId").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public List<RoutePlan> DevlieryOrders(int agentId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlan_DeliveryBoy_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DelAgentId").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public List<RoutePlan> NewDevlieryOrders(int agentId,int index)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    para.Add("@index", index);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("New_RoutePlan_DeliveryBoy_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DelAgentId").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }
        public List<RoutePlan> DevlieryOrders(int agentId, int statusId, string jobId)
        {
            List<RoutePlan> routePlans = new List<RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    para.Add("@StatusId", statusId);
                    para.Add("@JobId", jobId);
                    routePlans = dbQuery.Query<RoutePlan, Order, OrderStatu, DeliveryAgent, RoutePlan>("RoutePlan_DeliveryTrack_Sel",
                        (r, o, os, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "OrdId,StatusId,DelAgentId").ToList();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }
        public List<RoutePlanDO> DeliveryManagerTrackAgent(string userId, int shopId, int orderId, int issueId)
        {
            List<RoutePlanDO> routePlans = new List<RoutePlanDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    para.Add("@ShopId", shopId);
                    para.Add("@OrderId", orderId);
                    para.Add("@OrderIssueId", issueId);
                    routePlans = dbQuery.Query<RoutePlanDO, Order, Customer, OrderStatu, CustomerAddress, DeliveryAgent, RoutePlanDO>("Routeplan_DelManager_sel",
                        (r, o, c, os, cd, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.OrderAddress = cd;
                            r.Customer = c;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "DelOrderId,CustId,DelStatusId,DelAddId,DelID").ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public List<RoutePlanDO> DeliveryManagerTrackAgentDelivered(string userId, int shopId, int orderId, int issueId)
        {
            List<RoutePlanDO> routePlans = new List<RoutePlanDO>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@UserId", userId);
                    para.Add("@ShopId", shopId);
                    para.Add("@OrderId", orderId);
                    para.Add("@OrderIssueId", issueId);
                    routePlans = dbQuery.Query<RoutePlanDO, Order, Customer, OrderStatu, CustomerAddress, DeliveryAgent, RoutePlanDO>("Routeplan_DelManager_Delivered_Sel",
                        (r, o, c, os, cd, da) =>
                        {
                            r.Order = o;
                            r.Order.OrderStatu = os;
                            r.OrderAddress = cd;
                            r.Customer = c;
                            r.DeliveryAgent = da;
                            return r;
                        },
                        param: para, commandType: CommandType.StoredProcedure,
                        splitOn: "DelOrderId,CustId,DelStatusId,DelAddId,DelID").ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public RoutePlanDeliveryCountDO RoutePlansDeliveryCount(int agentId)
        {
            RoutePlanDeliveryCountDO routePlans = new RoutePlanDeliveryCountDO();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    //para.Add("@JobId", jobId);
                    routePlans = dbQuery.Query<RoutePlanDeliveryCountDO>("RoutePlan_DelCount_Sel",
                        param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public RoutePlanDeliveryCountDO RoutePlansDeliveryCount(int agentId, string userId)
        {
            RoutePlanDeliveryCountDO routePlans = new RoutePlanDeliveryCountDO();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@DeliveryAgentId", agentId);
                    para.Add("@UserId", userId);
                    routePlans = dbQuery.Query<RoutePlanDeliveryCountDO>("RoutePlan_DelCount_Sel2",
                        param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return routePlans;
        }

        public RoutePlanDO Ordertracking(int orderId)
        {
            var cDictionary = new Dictionary<int, RoutePlanDO>();
            List<RoutePlanDO> routePlans = new List<RoutePlanDO>();
            var quDictionary = new Dictionary<int, OrderTrack>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    var result = dbQuery.Query<RoutePlanDO, Order, OrderDetail, OrderTrack, DeliveryAgent, WineShop, OrderStatu, RoutePlanDO>("OrderProcessTracking_SEL",
                        (RP, O, OD, OT, DA, WS, OS) =>
                        {
                            if (!cDictionary.TryGetValue(RP.id, out var delpay))
                            {
                                delpay = RP;
                                delpay.Order = O;
                                delpay.Order.OrderDetails.Add(OD);
                                var trackStatus = OS;
                                OT.OrderStatu = trackStatus;
                                delpay.OrderTrack.Add(OT);
                                delpay.DeliveryAgent = DA;
                                delpay.WineShop = WS;
                                cDictionary.Add(RP.id, delpay);
                            }
                            else
                            {
                                delpay = cDictionary[RP.id];
                                var trackStatus = OS;
                                OT.OrderStatu = trackStatus;
                                delpay.OrderTrack.Add(OT);
                            }


                            return RP;
                        },
                         param: para,
                         splitOn: "OId,OrderDetailId,OrderTrackId,DeliveryAgentId,WineShopId,OrderStatusId",
                         commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }


            }

            var objCust = cDictionary?.Values;
            return objCust.FirstOrDefault();
        }

        public CustomerAddressDO GetCustomerAddress(int orderId)
        {
            CustomerAddressDO prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    prodDetail = dbQuery.Query<CustomerAddressDO>("CustomerAddress_Sel",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }

        public CustomerAddressDO CheckOutForDelivery(int orderId)
        {
            CustomerAddressDO prodDetail = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@OrderId", orderId);
                    prodDetail = dbQuery.Query<CustomerAddressDO>("CheckOutForDeliveryStatu_sel",
                    param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return prodDetail;
        }
    }
}