using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class MixerOrderDetailsDO
    {

        public ICollection<OrdersExtDO> Order { get; set; }
        public ICollection<OrderDetailExtDO> OrderDetails { get; set; }
        public ICollection<MixerDetailExtDO> MixerDetails { get; set; }
    }

    public class OrdersExtDO
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string LicPermitNo { get; set; }
        public long? ShopID { get; set; }
        public long? ZoneId { get; set; }
        public string MyProperty { get; set; }
        public string OrderAmount { get; set; }
        public DateTime DeliveryDate { get; set; }

        public int PaymentTypeId { get; set; }

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
        public bool IsDeleted { get; set; }

        public int Rating { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
    public class OrderDetailExtDO
    {
        public int Id { get; set; }
        public int ItemQty { get; set; }
        public int Price { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Capacity { get; set; }
        public int Current_Price { get; set; }
        public int AvailableQty { get; set; }
       
    }
    public class MixerDetailExtDO
    {
        public int MixerId { get; set; }
        public string MixerName { get; set; }
        public int MixerDetailId { get; set; }
        public int ItemQty { get; set; }
        public int Price { get; set; }
        public string MixerImage { get; set; }
        public string Capacity { get; set; }
        public int Current_Price { get; set; }
        public int AvailableQty { get; set; }
        public int SupplierId { get; set; }

    }
}