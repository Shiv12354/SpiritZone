using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class UserSpecificOfferDO
    {
        public int SpecificOfferMapId { get; set; }
        public int CustomerID { get; set; }
        public int OfferId { get; set; }
        public string OfferType { get; set; }
        public int NumberOfUse { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}