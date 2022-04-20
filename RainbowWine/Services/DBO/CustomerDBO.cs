using Dapper;
using RainbowWine.Data;
using RainbowWine.Models;
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
    public class CustomerDBO
    {
        public List<CustomerDO> SearchPhone(string text)
        {
            List<CustomerDO> cust = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    string sel = $"select * from Customer c inner join CustomerAddress cad on cad.CustomerId = c.Id inner join WineShop s on s.id=cad.ShopId where c.RegisterSource='w' and c.ContactNo like '%{text}%'";
                    cust = dbQuery.Query<CustomerDO, CustomerAddress, WineShop, CustomerDO>(sel,
                        (c, cd, s) =>
                        {
                            if (cd != null)
                            {
                                cd.WineShop = s;
                                c.CustomerAddresses.Add(cd);
                            }
                            return c;
                        },
                        splitOn: "CustomerAddressId, Id").ToList();

                    if (cust.Count() == 0)
                    {
                        sel = $"select top 1 * from Customer c left outer join WineShop s on s.id=c.ShopId where c.RegisterSource='w' and c.ContactNo like '%{text}%'";
                        cust = dbQuery.Query<CustomerDO, WineShop, CustomerDO>(sel,
                            (c, s) =>
                        {
                            c.Shop = s;
                            return c;
                        },
                        splitOn: "CustomerAddressId, Id").ToList();
                    }
                }
                finally
                {
                    dbQuery.Close();
                }
            }
            return cust;
        }

        public List<Order> GetOrders(int custId)
        {
            List<Order> ord = null;
            var cDictionary = new Dictionary<int, Order>();
            var quDictionary = new Dictionary<int, OrderDetail>();
            var qeDictionary = new Dictionary<int, RoutePlan>();
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    //string sel = $"select o.*,od.Id as 'OdetailId',od.*, os.Id as 'OStatusId', os.*, r.* from Orders o "
                    //    + $" inner join OrderDetail od on od.OrderId = o.Id"
                    //    + $" inner join OrderStatus os on os.Id = o.OrderStatusId"
                    //    + $" left outer join RoutePlan r on r.OrderID = o.Id"
                    //    + $" where o.CustomerId ={custId}";

                    var para = new DynamicParameters();
                    para.Add("@CustomerId", custId);
                    
                    ord = dbQuery.Query<Order, OrderDetail, OrderStatu, RoutePlan, Order>("Orders_ByCustomer_Sel",
                        (o, od, s, r) =>
                        {
                            if (!cDictionary.TryGetValue(o.Id, out var cust))
                            {
                                cust = o;
                                cust.OrderStatu = s;
                                cDictionary.Add(o.Id, cust);
                            }
                            else
                            {
                                cust = cDictionary[o.Id];
                            }
                            if (od != null)
                            {
                                if (!quDictionary.TryGetValue(od.Id, out var qUser))
                                {
                                    //qUser = od;
                                    ////cust.QSenseCustomers = new List<QSenseCustomer>();
                                    //quDictionary.Add(od.Id, qUser);
                                    cust.OrderDetails.Add(od);
                                }
                            }
                            if (r != null)
                            {
                                if (!qeDictionary.TryGetValue(r.id, out var qUqser))
                                {
                                    //qUser = od;
                                    ////cust.QSenseCustomers = new List<QSenseCustomer>();
                                    //quDictionary.Add(od.Id, qUser);
                                    cust.RoutePlans.Add(r);
                                }
                            }
                            //o.OrderStatu = s;
                            //if (od != null)
                            //{
                            //   o.OrderDetails.Add(od);
                            //}
                            return o;
                        },
                        param: para,
                        commandType:CommandType.StoredProcedure,
                        splitOn: "OdetailId, OStatusId, id").ToList();

                }
                finally
                {
                    dbQuery.Close();
                }
                
            }
            var objCust = cDictionary?.Values;
            return objCust.ToList();
        }
        public List<CustomerContact> GetCustomerContact()
        {
            List<CustomerContact> cust = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    cust = dbQuery.Query<CustomerContact>("CustomerContact_Sel",
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return cust;
        }

        public List<CustomerSignup> SignUpComstomer()
        {
            List<CustomerSignup> cust = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    cust = dbQuery.Query<CustomerSignup>("Customer_Signup",
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return cust;
        }

        public CheckCredibleUser CkeckCredibleCustomer(string contactNo)
        {
            CheckCredibleUser checkCredibleUser = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ContactNo", contactNo);
                    checkCredibleUser = dbQuery.Query<CheckCredibleUser>("CheckCredibleUser", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return checkCredibleUser;
        }

        public int AddSZCreditToCustomer(int amount, string contactNo)
        {
            int result = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@Amount", amount);
                    para.Add("@ContactNo", contactNo);
                    result = dbQuery.Query<int>("ExistingUser_Sel", param: para,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return result;
        }

        public int AddCustomerContactDetails(CustomerContactDO customerContactDO)
        {
            int result = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@CustomerName", customerContactDO.CustomerName);
                    para.Add("@CustomerContact", customerContactDO.ContactNo);
                    para.Add("@CustomerAddress", customerContactDO.Address);
                    para.Add("@CustomerPlaceId", customerContactDO.Dest_Place_Id);
                    para.Add("@latitute", customerContactDO.Latitude);
                    para.Add("@Longitute", customerContactDO.Longitude);
                    result = dbQuery.Query<int>("CustomerContact_Ins", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {

                }
            }
            return result;
        }

        public List<ServicableShopAndZoneDO> SevicableShopAndZone(string servicableType)
        {
            List<ServicableShopAndZoneDO> service = null;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@ServicableType", servicableType);
                    service = dbQuery.Query<ServicableShopAndZoneDO>("ServiceableShopAndZone_Sel",param:para,
                        commandType: CommandType.StoredProcedure).ToList();

                }
                finally
                {
                    dbQuery.Close();
                }

            }
            return service;
        }

        public int UpdateConfigMstFlagShopAndZone(string requestKey)
        {
            int result = 0;
            using (IDbConnection dbQuery = new SqlConnection(ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString))
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@RequestKey", requestKey);
                    result = dbQuery.Query<int>("ConfigMasterShopZoneFlag_Upd", param: para, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {

                }
            }
            return result;
        }

        
    }
}