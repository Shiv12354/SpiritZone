using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models.Packers
{
    public class spOrders_OrderDetails_Sel
    {
        public int Id { get; set; }
        public System.DateTime OrderDate { get; set; }
        public decimal OrderAmount { get; set; }
        public int CustomerId { get; set; }
        public Nullable<int> CustomerAddressId { get; set; }
        public int OrderStatusId { get; set; }
        public Nullable<int> ShopID { get; set; }
        public string LicPermitNo { get; set; }
        public string OrderType { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string PhoneNo { get; set; }
        public string ShopPhoneNo { get; set; }
        public string LicNo { get; set; }
        public string CLNo { get; set; }
        public string GST { get; set; }
        public string VAT { get; set; }
        public string CustAddress { get; set; }
        public string CustomerUserId { get; set; }
        public string ContactNo { get; set; }
        public string CustomerName { get; set; }
        public string FormattedAddress { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public IList<OrderDetailSel> OrderDetails { get; set; }

        public spOrders_OrderDetails_Sel()
        {
            OrderDetails = new List<OrderDetailSel>();
        }
    }
}