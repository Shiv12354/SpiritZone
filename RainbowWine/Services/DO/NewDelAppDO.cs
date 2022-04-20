using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class EarningAndAttendance
    {
        public int Id { get; set; }
        public int DeliveryAgentId { get; set; }       
        public string DelAgentName { get; set; }
        public string DelContactNo { get; set; }
        public DateTime OnDuty { get; set; }
        public DateTime OffDuty { get; set; }
        public bool IsOnOff { get; set; }
        public string BreaksInMinutes { get; set; }
        public int TotalEarning { get; set; }
        public int PotentialEarning { get; set; }
        public int ShopId { get; set; }
        public float ShopLatitude { get; set; }
        public float ShopLongitude { get; set; }
       
    }

    public class NewDelOrderDetails
    {
        public int OrderId { get; set; }
        public int OrderAmount { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
        public string PaymentType { get; set; }
        public string LicPermitNo { get; set; }
        public DateTime CreationDate { get; set; }
        public int ShopID { get; set; }
        public string CustContactNo { get; set; }
        public string CustName { get; set; }
        public float CustLatitude { get; set; }
        public float CustLongitude { get; set; }
        public string Address { get; set; }
        public string Flat { get; set; }
        public string Landmark { get; set; }
        public float ShopLatitude { get; set; }
        public float ShopLongitude { get; set; }
        public DateTime Deliveries_60 { get; set; }
        public DateTime Deliveries_90 { get; set; }
        public double EarningAmount60 { get; set; }
        public double EarningAmount90 { get; set; }
        public bool IsPickedUp { get; set; }
        public DateTime PickedUpDate { get; set; }
        public DateTime AssignedDate { get; set; }
        public string JobId { get; set; }
        public int PaymentTypeId { get; set; }
        public string RecipientContactNo { get; set; }
        public string GiftIcon { get; set; }
        public string GiftMsg { get; set; }
        public string DisplayMsgRecipient { get; set; }
        public string DisplayMsgCustomer { get; set; }
        public bool IsGift { get; set; }

    }
    
    public class HandOver
    {
        public int DeliveryAgentId { get; set; }
        public int TotalAmount { get; set; }
        public int ItemQty { get; set; }
    }

    public class Earning
    {
        public int DeliveryAgentId { get; set; }
        public int TotalEarning { get; set; }
        public int OneDayEarning { get; set; }

    }

    public class LastSevendaysEarning
    {
        public int DeliveryAgentId { get; set; }
        public int TotalEarning { get; set; }
        public DateTime EarningDate { get; set; }

    }

    public class OrdersList
    {
        public int AllOrders { get; set; }
        public int CashOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int BackToStoreOrders { get; set; }
        public int PendingOrders { get; set; }
    }

    public class DetailsView
    {
        public int OrderId { get; set; }
        public string OrderStatusName { get; set; }
        public int OrderStatusId { get; set; }
        public int OrderAmount { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string DiffInMinutes { get; set; }
        public string Remarks { get; set; }
        public string OrderType { get; set; }
    }

    public class DeliveryEarningWithPenalty
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DeliveryAgentId { get; set; }
        public int OrderId { get; set; }
        public System.DateTime DeliveredDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Earning { get; set; }
        public string Descriptions { get; set; }
        public bool IsPenalty { get; set; }
        public DateTime OrderDate { get; set; }
        public string DiffInMinutes { get; set; }
    }

    public class OrderItem
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentTypeId { get; set; }
        public string OrderStatusName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int OrderAmount { get; set; }
        public int ShopID { get; set; }
        public string OrderDetails { get; set; }
        public string MixerDetails { get; set; }
        public string DeliveryExecName { get; set; }
        public string LicPermitNo { get; set; }
        public string PermitCost { get; set; }
        public DateTime PackedDate { get; set; }
        public string AgentContact { get; set; }
        public string GiftDetails { get; set; }

        public List<OrderDetailItems> LineItems
        {
            get
            {
                var items = new List<OrderDetailItems>();
                try
                {
                    var str1 = string.IsNullOrEmpty(OrderDetails) ? null : OrderDetails.Split('|');

                    if (str1 !=null)
                    {
                        foreach (var item in str1)
                        {
                            var str2 = item.Split(',');
                            var newItem = new OrderDetailItems();
                            newItem.ProductId = Convert.ToInt32(str2[0]);
                            newItem.Capacity = str2[1];
                            newItem.Qty = Convert.ToInt32(str2[2]);
                            newItem.Price = Convert.ToInt32(str2[3]);
                            newItem.ProductImage = str2[4];
                            newItem.ProductName = str2[5];
                            newItem.IsReserve = str2[6] == "0" ? false : true;
                            items.Add(newItem);
                        }
                    }

                    var mixerarr = string.IsNullOrEmpty(MixerDetails) ? null : MixerDetails.Split('|');

                    if (mixerarr !=null)
                    {
                        foreach (var item in mixerarr)
                        {
                            var str2 = item.Split(',');
                            var newItem = new OrderDetailItems();
                            newItem.IsMixer = true;
                            newItem.ProductId = Convert.ToInt32(str2[0]);
                            newItem.Capacity = str2[1];
                            newItem.Qty = Convert.ToInt32(str2[2]);
                            newItem.Price = Convert.ToInt32(str2[3]);
                            newItem.ProductImage = str2[4];
                            newItem.ProductName = str2[5];
                            items.Add(newItem);
                        }
                    }

                    var gifterarr = string.IsNullOrEmpty(GiftDetails) ? null : GiftDetails.Split('|');

                    if (gifterarr !=null)
                    {
                        foreach (var item in gifterarr)
                        {
                            var str2 = item.Split(',');
                            var newItem = new OrderDetailItems();
                            newItem.IsGift = true;
                            newItem.ProductId = Convert.ToInt32(str2[0]);
                            newItem.Capacity = str2[1];
                            newItem.Qty = Convert.ToInt32(str2[2]);
                            newItem.Price = Convert.ToInt32(str2[3]);
                            newItem.ProductImage = str2[4];
                            newItem.ProductName = str2[5];
                            items.Add(newItem);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                return items;
            }
        }

    }

    public class OrderDetailItems
    {
        public int ProductId { get; set; }
        public string Capacity { get; set; }
        public int Price { get; set; }
        public int Qty { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public int Rating { get; set; }
        public bool IsGift { get; set; }
        public int SubTotal
        {
            get
            {
                return Price * Qty;
            }
        }
        public bool IsMixer { get; set; } = false;
        public bool IsReserve { get; set; } = false;
    }

    public class DeliveryAgentDetail
    {
        public int DeliveryAgentId { get; set; }
        public string DelAgentName { get; set; }
        public string DelContactNo { get; set; }
        public DateTime OnDuty { get; set; }
        public DateTime OffDuty { get; set; }
        public bool IsOnOff { get; set; }
        public int ShopId { get; set; }
        public float ShopLatitude { get; set; }
        public float ShopLongitude { get; set; }
        public int TotalDeliveredOrders { get; set; }

    }

    public class HypertrackDeliveryDevices
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string HyperTrackDeviceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool isMetaSet { get; set; }
    }

    public class Dates
    {
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }

    }

    public class HypertrackTripDetail
    {
        public string OrderId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Address { get; set; }
        public DateTime CommittedTime { get; set; }
        public DateTime Scheduled_At { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Views
    {
        public string embed_url { get; set; }
        public string share_url { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Destination
    {
        public string address { get; set; }
        public Geometry geometry { get; set; }
        public int radius { get; set; }
        public string destination_id { get; set; }
        public string trip_id { get; set; }
        public string geofence_id { get; set; }
        public bool delayed { get; set; }
        public bool arrived { get; set; }
        public object arrived_at { get; set; }
        public object exited_at { get; set; }
        public DateTime scheduled_at { get; set; }
        public string share_url { get; set; }
    }

    public class TripOrder
    {
        public string account_id { get; set; }
        public string order_id { get; set; }
        public object metadata { get; set; }
        public DateTime scheduled_at { get; set; }
        public string device_id { get; set; }
        public Destination destination { get; set; }
        public string share_url { get; set; }
        public string status { get; set; }
        public DateTime started_at { get; set; }
        public int service_time { get; set; }
        public bool delayed { get; set; }
        public object estimate { get; set; }
    }

    public class Root
    {
        public string trip_id { get; set; }
        public DateTime started_at { get; set; }
        public object completed_at { get; set; }
        public string status { get; set; }
        public string device_id { get; set; }
        public Views views { get; set; }
        public object metadata { get; set; }
        public List<TripOrder> orders { get; set; }
        public Destination destination { get; set; }
        public object summary { get; set; }
        public bool optimise_route { get; set; }
        public object estimate { get; set; }
    }

    public class HyperTrackTripResponse
    {
        public string TripId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public string SharedUrl { get; set; }
        public string DeviceId { get; set; }
        public int DeliveryAgentId { get; set; }
        public int OrderId { get; set; }

    }

}