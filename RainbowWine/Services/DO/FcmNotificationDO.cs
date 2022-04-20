using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class FcmNotificationDO
    {
        public int FcmNotificationId { get; set; }
        public int CustomerId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Id { get; set; }
        public string NotificationType { get; set; }
        public string NavigationPage { get; set; }
        public string IconUrl { get; set; }
        public string BannerUrl { get; set; }
        public bool IsRead { get; set; }
        public string OtherParameters { get; set; }
    }
}