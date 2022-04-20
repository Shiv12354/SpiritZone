using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class UserIdentity
    {
        //public RUser rUser()
        //{
        //    RUser rUser = null;
        //    using (rainbowwineEntities db = new rainbowwineEntities())
        //    {
        //        db.Configuration.ProxyCreationEnabled = false;
        //        var u = db.AspNetUsers.Where(o => o.Email == HttpContext.Current.User.Identity.Name).FirstOrDefault();
        //        var user = db.RUsers.Include("UserType1").Where(o => o.rUserId == u.Id)?.FirstOrDefault();
        //        if (string.Compare(user.UserType1.UserTypeName, "cusomter", true) != 0)
        //        {
        //            throw new Exception("Loggedin user must be cusomter type.");
        //        }
        //    }
        //}
    }
}