using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class CustomerViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid contact number")]
        public string ContactNo { get; set; }
        [Required]
        public string Address { get; set; }
        public string FormattedAddress { get; set; }
        public string PlaceId { get; set; }
        [Required]
        public string Flat { get; set; }
        public string Landmark { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Latitude { get; set; }
    }
}