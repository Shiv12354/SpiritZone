using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainbowWine.Models
{
    public class ProductAdmin
    {
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Product Name is Required")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Package Size is Required")]
        [Range(1,100)]
        public int PackingSize { get; set; }
        [Required(ErrorMessage = "Price is Required")]
        [Range(1, 100000)]
        public float Price { get; set; }
        [Required(ErrorMessage = "Category is Required")]
        public string Category { get; set; }
        public int ProductSizeId { get; set; }
        [Required]
        public int ProductSize { get; set; }
        public bool IsDelete { get; set; }
        public bool IsFeature { get; set; }
        public bool IsShowcaseView { get; set; }
        public bool IsShowcasePremiumView { get; set; }
        public bool IsPremium { get; set; }
        public string Barcode { get; set; }
        public string ProductImage { get; set; }
        public string ProductThumbImage { get; set; }
        public string UserName { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public string MasterProductName { get; set; }
        public string Capacity { get; set; }
    }
}