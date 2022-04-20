using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RainbowWine
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    var referrer = Request.UrlReferrer;

        //    if (referrer != null)
        //    {
        //        string[] allowedOrigin = new string[] { referrer.Scheme + "://" + referrer.Authority };
        //        var origin = HttpContext.Current.Request.Headers["Origin"];
        //        if (origin != null && allowedOrigin.Contains(origin))
        //        {
        //            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", origin);
        //            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT");
        //        }
        //    }
        //}

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
            RouteData rd = RouteTable.Routes.GetRouteData(context);

            if (rd != null)
            {
                string controllerName = rd.GetRequiredString("controller");
                string actionName = rd.GetRequiredString("action");
                //Response.Write("c:" + controllerName + " a:" + actionName);
                //Response.End();
            }
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                rainbowwineEntities db = new rainbowwineEntities();
                db.AppLogs.Add(new AppLog {
                    CreateDatetime=DateTime.Now,
                    Error=exception.Message,
                    Message=exception.StackTrace,
                    MachineName= System.Environment.MachineName
                });
                db.SaveChanges();
            }
        }
    }
}
