using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{

    public class PostdatedOfferDO
    {
        public int PostdatedOfferId { get; set; }
        public int PromoId { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public DateTime OfferStartDate { get; set; }
        public DateTime OfferEndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int AvailOrderId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }
    public class PreBookingPromoOfferProductLink
    {
        public int PreBookingPromoOfferProductLinkId { get; set; }
        public int ProductRefId { get; set; }
        public int PromoId { get; set; }
        public int ExpireInDays { get; set; }
        public DateTime Createddate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int DaysFromCurrent { get; set; }
        public int DaysFromStart { get; set; }

    }

    public class Order_OrderDetailsDO
    {
        public int ProductID { get; set; }
        public bool PreBook { get; set; }
        public int PromoId { get; set; }


    }
}
