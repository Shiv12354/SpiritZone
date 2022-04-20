using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class WebEngageDO
    {
            public int OrderId { get; set; }
            public int OrderStatusId { get; set; }
            public int PaymentTypeId { get; set; }
            public string OrderStatusName { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public int OrderAmount { get; set; }
            public int ShopID { get; set; }
            public int CustomerAddressId { get; set; }
            public string Address { get; set; }
            public double? WalletAmountUsed { get; set; } = 0;
            public double? PromoDiscountAmount { get; set; } = 0;
            public int CustomerId { get; set; }
            public string Comment { get; set; }
            public int? Rating { get; set; }
            public string OrderDetails { get; set; }
            public string MixerDetails { get; set; }
            public string GoodyDetails { get; set; }
            public string DeliveryExecName { get; set; }
            public string AgentContact { get; set; }
            public List<OrderDetailItem> LineItems
            {
                get
                {
                    var items = new List<OrderDetailItem>();
                    try
                    {
                        var str1 = string.IsNullOrEmpty(OrderDetails) ? "".Split('|') : OrderDetails.Split('|');

                        if (str1.Length > 0)
                        {
                            foreach (var item in str1)
                            {
                                var str2 = item.Split(',');
                                var newItem = new OrderDetailItem();
                                newItem.ProductId = Convert.ToInt32(str2[0]);
                                newItem.ProductID = Convert.ToInt32(str2[0]);
                                newItem.Capacity = str2[1];
                                newItem.Qty = Convert.ToInt32(str2[2]);
                                newItem.Price = Convert.ToInt32(str2[3]);
                                newItem.ProductImage = str2[4];
                                newItem.Rating = Convert.ToInt32(str2[5]);
                                newItem.ProductName = str2[6];
                                newItem.IsReserve = str2[7] == "0" ? false : true;
                                newItem.Category = str2[9];
                                 newItem.SubCategory = str2[10];

                            if (str2.Length >= 9)
                                    newItem.OrderDetailId = string.IsNullOrEmpty(str2[8]) ? 0 : Convert.ToInt32(str2[8]);

                                if (str2.Length >= 10)
                                    newItem.LaunchFlag = string.IsNullOrEmpty(str2[9]) || str2[9] == "0" ? true : false;

                                items.Add(newItem);
                            }
                        }

                        var mixerarr = string.IsNullOrEmpty(MixerDetails) ? "".Split('|') : MixerDetails.Split('|');

                        if (mixerarr.Length > 0 && !string.IsNullOrEmpty(MixerDetails))
                        {
                            foreach (var item in mixerarr)
                            {
                                var str2 = item.Split(',');
                                var newItem = new OrderDetailItem();
                                newItem.IsMixer = true;
                                newItem.ProductId = Convert.ToInt32(str2[0]);
                                newItem.ProductID = Convert.ToInt32(str2[0]);
                                newItem.Capacity = str2[1];
                                newItem.Qty = Convert.ToInt32(str2[2]);
                                newItem.Price = Convert.ToInt32(str2[3]);
                                newItem.ProductImage = str2[4];
                                newItem.Rating = Convert.ToInt32(str2[5]);
                                newItem.ProductName = str2[6];

                                if (str2.Length >= 8)
                                    newItem.OrderDetailId = string.IsNullOrEmpty(str2[7]) ? 0 : Convert.ToInt32(str2[7]);

                                items.Add(newItem);
                            }
                        }

                        var goodyarr = string.IsNullOrEmpty(GoodyDetails) ? "".Split('|') : GoodyDetails.Split('|');

                        if (goodyarr.Length > 0 && !string.IsNullOrEmpty(GoodyDetails))
                        {
                            foreach (var item in goodyarr)
                            {
                                var str2 = item.Split(',');
                                var newItem = new OrderDetailItem();
                                newItem.IsGoody = true;
                                newItem.ProductId = Convert.ToInt32(str2[0]);
                                newItem.Capacity = str2[1];
                                newItem.Qty = Convert.ToInt32(str2[2]);
                                newItem.Price = 0;
                                newItem.ProductImage = str2[4];
                                newItem.Rating = 0;
                                newItem.ProductName = str2[6];

                                if (str2.Length >= 8)
                                    newItem.OrderDetailId = string.IsNullOrEmpty(str2[7]) ? 0 : Convert.ToInt32(str2[7]);

                                items.Add(newItem);
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    var sorted = (from c in items
                                  orderby c.OrderDetailId ascending, c.Price descending
                                  select c).ToList();

                    return sorted;
                }
            }
            public string LicPermitNo { get; set; }
            public string Code { get; set; }
            public string BagCost { get; set; }
            public DateTime? DeliveryScheduledDate { get; set; }
            public bool? IsScheduledOrder { get; set; }
            public bool? IsGift { get; set; }
            public string RecipientName { get; set; }

        }
    public class OrderDetailItem
    {
        public int ProductId { get; set; }
        public int ProductID { get; set; }
        public string Capacity { get; set; }
        public int Price { get; set; }
        public int Qty { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public int Rating { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int SubTotal
        {
            get
            {
                return Price * Qty;
            }
        }
        public bool IsMixer { get; set; } = false;
        public bool IsReserve { get; set; } = false;
        public bool IsGoody { get; set; } = false;
        public bool? LaunchFlag { get; set; } = false;
        public int OrderDetailId { get; set; } = 0;

    }
}