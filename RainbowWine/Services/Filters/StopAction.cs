
using System.Configuration;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace RainbowWine.Services.Filters
{
    public class StopAction: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller)filterContext.Controller;
            var stopAccess = ConfigurationManager.AppSettings["ExpSessionMin"];
            if (!string.IsNullOrWhiteSpace(stopAccess))
            {
                if (string.Compare(stopAccess,"noset",true)==0 && string.Compare(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "home", true) != 0)
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
        }
    }
}