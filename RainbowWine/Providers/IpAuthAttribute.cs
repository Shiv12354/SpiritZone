using RainbowWine.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SZInfrastructure;

namespace RainbowWine.Providers
{
    public class IpAuthAttribute : ActionFilterAttribute
    {
        public bool checkConfigValue()
        {
            var c = SZIoc.GetSerivce<ISZConfiguration>();
            var ipRestriction = c.GetConfigValue(ConfigEnums.IpRestriction.ToString());
            if (ipRestriction == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (checkConfigValue())
            {
                base.OnActionExecuting(filterContext);
                string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                string ips = ConfigurationManager.AppSettings["Ips"].ToString();// "182.72.70.178,0.0.0.0, 127.0.0.1,35.154.231.25,52.183.165.53";

                if (!ips.Contains(ip))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "SpiritLoginRegister"
                    }));
                }
            }
        }
    }
}