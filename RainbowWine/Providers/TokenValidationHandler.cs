using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RainbowWine.Models;

namespace RainbowWine.Providers
{
    public class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            StringContent stringContent = new StringContent("");
            string token;

            //chek if a token exists in the request header
            if (!TryRetrieveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                //allow requests with no token - whether a action method needs an authentication can be set with the claimsauthorization attribute
                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(ConfigurationManager.AppSettings["Key"]));


                var handler = new JwtSecurityTokenHandler();

                //Replace the issuer and audience with your URL (ex. http:localhost:12345)
                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = ConfigurationManager.AppSettings["Audience"],
                    ValidIssuer = ConfigurationManager.AppSettings["Issuer"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = LifetimeValidator,
                    IssuerSigningKey = securityKey
                };

                //extract and assign the user of the jwt
                Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out _);
                HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out _);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                var responseStatus = new ResponseStatus { Status = false, Message = "Authorization has been denied for this request." };
                stringContent = new StringContent(JsonConvert.SerializeObject(responseStatus), Encoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {
                statusCode = HttpStatusCode.InternalServerError;
                var responseStatus = new ResponseStatus { Status = false, Message = "Authorization has been denied for this request." };
                stringContent = new StringContent(JsonConvert.SerializeObject(responseStatus), Encoding.UTF8, "application/json");
            }
            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { Content= stringContent }, cancellationToken);
        }

        public bool LifetimeValidator(DateTime? notBefore,
            DateTime? expires,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.Now < expires) return true;
            }
            return false;
        }

    }
}