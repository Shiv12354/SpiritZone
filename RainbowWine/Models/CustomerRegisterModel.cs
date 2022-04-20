using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class CustomerRegisterModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "ContactNo")]
        public string ContactNo { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        public string Code { get; set; }
    }
    public class UserCustomerAddress
    {
        [Key]
        public int CustomerAddressId { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Formatted Address")]
        public string FormattedAddress { get; set; }
        [Required]
        [Display(Name = "Place Id")]
        public string PlaceId { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Latitude { get; set; }
        public string Flat { get; set; }
        public string Landmark { get; set; }
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Shop Id")]
        public int? ShopId { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Address Type")]
        public string AddressType { get; set; }
        [Display(Name = "ZoneId")]
        public int ZoneId { get; set; }
    }
}