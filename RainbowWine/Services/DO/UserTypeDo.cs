using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class UserTypeDO
    {
        [Required]
        public int UserTypeId { get; set; }
        [Required]
        public string UserTypeName { get; set; }
    }
}