using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RainbowWine.Data;
using RainbowWine.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RainbowWine.Services;
using System.Configuration;

namespace RainbowWine.Services.Filters
{
    public class SpiritActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                rainbowwineEntities db = new rainbowwineEntities();
                var u = db.AspNetUsers.Where(o => o.Email == filterContext.HttpContext.User.Identity.Name).FirstOrDefault();
                if (u != null)
                {
                    var user = db.RUsers.Include(o => o.UserType1).Where(o => o.rUserId == u.Id)?.FirstOrDefault();
                    if (user != null)
                    {
                        var deliveryUser = db.DeliveryAgentLogins.Where(o => o.DeliveryAgentId == user.DeliveryAgentId)?.OrderByDescending(o=>o.Id).FirstOrDefault();

                        int dlogin = 0;
                        if (deliveryUser != null)
                        {
                            var ddate = deliveryUser.OnDuty.ToString("yyyy-MM-dd");
                            var cdate = DateTime.Now.ToString("yyyy-MM-dd");
                            if(string.Compare(ddate,cdate,true)==0)
                            {
                                dlogin = 1;
                            }
                        }
                        filterContext.Controller.ViewBag.UserType = user.UserType1.UserTypeName;
                        filterContext.Controller.ViewBag.DeliveryLogin = dlogin;
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}