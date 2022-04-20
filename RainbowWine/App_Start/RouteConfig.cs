using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RainbowWine
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
             name: "GiftRecipientDetails",
             url: "RecipientDetails",
             defaults: new { controller = "GiftRecipient", action = "RecipientDetails", id = UrlParameter.Optional }
         );

            routes.MapRoute(
             name: "Cocktails",
             url: "Cocktail",
             defaults: new { controller = "GiftRecipient", action = "Cocktail", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "GiftRecipientDetail",
             url: "RecipientDetail",
             defaults: new { controller = "GiftRecipient", action = "RecipientDetail", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "GiftRequestForOTP",
             url: "RequestForOTP",
             defaults: new { controller = "GiftRecipient", action = "RequestForOTP", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "GiftValidateRecipient",
             url: "ValidateRecipient",
             defaults: new { controller = "GiftRecipient", action = "ValidateRecipient", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "GiftVerification",
             url: "OTPVerification",
             defaults: new { controller = "GiftRecipient", action = "OTPVerification", id = UrlParameter.Optional }
         );
            
            routes.MapRoute(
              name: "GiftUrlOTPSend",
              url: "OTPSend",
              defaults: new { controller = "GiftRecipient", action = "OTPSend", id = UrlParameter.Optional }
          );
            routes.MapRoute(
               name: "GiftUrl",
               url: "gift/{id}",
               defaults: new { controller = "GiftRecipient", action = "Gift", id = UrlParameter.Optional }
           );
           // routes.MapRoute(
           //    name: "WebEngageUrl",
           //    url: "WebEngageCall/{orderid}/{status}",
           //    defaults: new { controller = "WebEngage", action = "WebEngageCall", orderid = UrlParameter.Optional ,status=UrlParameter.Optional }
           //);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
