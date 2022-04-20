using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.Email
{
    public class MailChartShareModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "The recipient's e-mail address is required!")]
        public string Recipient { get; set; }

        [EmailAddress]
        public string Cc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public List<IFormFile> Attachments { get; set; }

    }
}