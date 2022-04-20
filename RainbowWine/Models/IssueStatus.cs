using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class IssueStatus
    {
        [Required]
        public int OrderIssueId { get; set; }
        [Required]
        public string Remark { get; set; }
        [Required]
        public float? UpdateAmt { get; set; }
        [Required]
        public float? DiffAmt { get; set; }
        [Required]
        public int? OrderIssueTypeId { get; set; }

        public Nullable<bool> IsPartialRefund { get; set; } = false;
    }
    public class OrderModifyStatus
    {
        [Required]
        public int OrderModifyId { get; set; }
        [Required]
        public string Remark { get; set; }
        [Required]
        public float? UpdateAmt { get; set; }
        [Required]
        public float? DiffAmt { get; set; }
        [Required]
        public int? OrderTypeId { get; set; }
        public int StatusId { get; set; }
        public int OrderId { get; set; }
    }
}