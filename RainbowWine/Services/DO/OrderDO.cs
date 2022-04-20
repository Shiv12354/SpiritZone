using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RainbowWine.Data;

namespace RainbowWine.Services.DO
{
    public class OrderDO:Order
    {
        public CustomerAddress CustAddress { get; set; }
        public CustomerEta OrderEta { get; set; }
        
    }

    public class OrderExtDO:Order
    {
        public OrderExtDO()
        {
            //this.Order = new HashSet<Order>();
            this.OrderDetail = new HashSet<OrderDetail>();
            this.MixerOrderItem = new HashSet<MixerOrderItem>();
            this.MixerDetails = new HashSet<MixerDetail>();
        }
        //public ICollection<Order> Order { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }
        public ICollection<MixerOrderItem> MixerOrderItem { get; set; }
        public ICollection<MixerDetail> MixerDetails { get; set; }
    }

    public class OrderExttDO
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string LicPermitNo { get; set; }
        public long? ShopID { get; set; }
        public long? ZoneId { get; set; }
        public string MyProperty { get; set; }
        public string OrderAmount { get; set; }

        public long? OStatisId { get; set; }
        public string OrderStatusName { get; set; }

        public long CustomerAddressId { get; set; }
        public long CustomerId { get; set; }
        public string Address { get; set; }
        public string FormattedAddress { get; set; }
        public string PlaceId { get; set; }
        public string Flat { get; set; }
        public string Landmark { get; set; }
        public long ShopId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string AddressType { get; set; }

        public string FcmRegToken { get; set; }

        public int Rating { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        public int OrderDetailId { get; set; }
        public int ItemQTY { get; set; }
        public int Price { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Capacity { get; set; }
        public int currentPrice { get; set; }
        public int AvailableQty { get; set; }

        public int MixerId { get; set; }
        public int MixerDetailId { get; set; }
        public int MDItemQty { get; set; }
        public int MDPrice { get; set; }
        public string MixerImage { get; set; }
        public string MDCapacity { get; set; }
        public int MDCurrent_Price { get; set; }
        public int MDQtyAvailable { get; set; }
    }

     public class OrderIdsDO
    {
        public string OrderIds { get; set; }
    }

    public class OrderEditDetails
    {
        public int Id { get; set; }
        public string OrderPlacedBy { get; set; }
        public string OrderTo { get; set; }
        public string OrderAmount { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public int ShopID { get; set; }

    }

    public class ContactDeatailDO
    {
        public string CustContactNo { get; set; }
        public string InterUserContactNo { get; set; }
    }

}