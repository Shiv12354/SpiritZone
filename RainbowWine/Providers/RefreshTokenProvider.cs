using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using RainbowWine.Data;
using RainbowWine.Models;

namespace RainbowWine.Providers
{
    public class RefreshTokenProvider: IAuthenticationTokenProvider
    {
        //public void Create(AuthenticationTokenCreateContext context)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task CreateAsync(AuthenticationTokenCreateContext context)
        //{
        //    string guid = Guid.NewGuid().ToString();

        //    AuthenticationProperties refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
        //    {
        //        IssuedUtc = context.Ticket.Properties.IssuedUtc,
        //        ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
        //    };
        //    AuthenticationTicket refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

        //    refreshTokens.TryAdd(guid, refreshTokenTicket);

        //    context.SetToken(guid);
        //}

        //public void Receive(AuthenticationTokenReceiveContext context)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        //{
        //    string header = context.OwinContext.Request.Headers["Authorization"];

        //    if (refreshTokens.TryRemove(context.Token, out AuthenticationTicket ticket))
        //    {
        //        context.SetTicket(ticket);
        //    }
        //}

        //private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString(); //GetRefreshToken(context, HttpContext.Current.Request.Form["username"]);
            // maybe only create a handle the first time, then re-use for same client
            // copy properties and set the desired lifetime of refresh token
            //var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            //{
            //    IssuedUtc = context.Ticket.Properties.IssuedUtc,
            //    ExpiresUtc = DateTime.Now.AddYears(1)
            //};
            //var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);
            
            using (var db = new rainbowwineEntities())
            {
                var username = HttpContext.Current.Request.Form["username"];
                var user = db.AspNetUsers.Where(o => o.Email == username)?.FirstOrDefault();
                var urList = db.UserRefreshTokens.Where(o => o.UserId == user.Id);
                if (urList.Count() > 0)
                {
                    db.UserRefreshTokens.RemoveRange(urList);
                    db.SaveChanges();
                }
                var ur = new UserRefreshToken
                {
                    UserId = user.Id,
                    Token = guid,
                    IssuedDate = DateTime.UtcNow,
                    ExpiresDate = DateTime.UtcNow.AddYears(1)
                };
                context.Ticket.Properties.IssuedUtc = ur.IssuedDate;
                context.Ticket.Properties.ExpiresUtc = ur.ExpiresDate;
                ur.ProtectedTicket = context.SerializeTicket();
                db.UserRefreshTokens.Add(ur);
                db.SaveChanges();
            }
            //_refreshTokens.TryAdd(guid, context.Ticket);
            //_refreshTokens.TryAdd(newRefreshToken, refreshTokenTicket);

            // consider storing only the hash of the handle
            context.SetToken(guid);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            //AuthenticationTicket ticket;
            var clientid = HttpContext.Current.Request.Form["username"];
            using (var db = new rainbowwineEntities())
            {
                var logUser = db.AspNetUsers.Where(o => o.Email == clientid)?.FirstOrDefault();
                var refreshToken = db.UserRefreshTokens.Where(o => o.UserId == logUser.Id && o.Token==context.Token)?.FirstOrDefault();
                if (refreshToken != null && refreshToken.ExpiresDate > DateTime.Now) 
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
            }
            //if (_refreshTokens.TryRemove(context.Token, out ticket))
            //{
            //    context.SetTicket(ticket);
            //}
        }
        public static string GetRefreshToken(AuthenticationTokenCreateContext context, string username)
        {
            var newRefreshToken = Guid.NewGuid().ToString();
            using (var db = new rainbowwineEntities())
            {
                //var username = HttpContext.Current.Request.Form["username"];
                var user = db.AspNetUsers.Where(o => o.Email == username)?.FirstOrDefault();
                var urList = db.UserRefreshTokens.Where(o => o.UserId == user.Id);
                if (urList.Count() > 0)
                {
                    db.UserRefreshTokens.RemoveRange(urList);
                    db.SaveChanges();
                }
                var ur = new UserRefreshToken
                {
                    UserId = user.Id,
                    Token = newRefreshToken,
                    IssuedDate = DateTime.Now,
                    ExpiresDate = DateTime.Now.AddYears(1),
                    ProtectedTicket = context.SerializeTicket()
                };
                db.UserRefreshTokens.Add(ur);
                db.SaveChanges();
            }
           
            return newRefreshToken;
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}