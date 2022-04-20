using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using System.ComponentModel;
using RainbowWine.Services;
using System.Configuration;
using RainbowWine.Services.DBO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/v2/user")]
    [DisplayName("Manage")]
    [Authorize]
    public class UsersV2Controller : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UsersV2Controller()
        {
        }

        public UsersV2Controller(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IEnumerable<Claim> GetTokenClaims(ApplicationUser user, IList<string> roles)
        {
            string r = string.Join(",", roles);

            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Email),// Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Role, r)
            };
        }

        private UserViewModel AuthenticateAsync(LoginViewModel login)
        {

            UserViewModel userModel = null;
            ApplicationUser user = null;
            try
            {
                user = UserManager.FindByEmail(login.Email);

            }
            catch (Exception ex)
            {
            }
            if (user != null)
            {
                PasswordVerificationResult passResult = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, login.Password);
                if (passResult == PasswordVerificationResult.Success)
                {
                    userModel = new UserViewModel { name = user.UserName, email = user.Email, Id = user.Id };
                    return userModel;
                }
                if (passResult == PasswordVerificationResult.Failed)
                {

                }
            }
            return userModel;

        }
        private string BuildToken(UserViewModel userViewModel)
        {
            var user = UserManager.FindByEmail(userViewModel.name);
            var userClaims = UserManager.GetClaims(user.Id);
            var roles = UserManager.GetRoles(user.Id);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(ConfigurationManager.AppSettings["Issuer"],
              ConfigurationManager.AppSettings["Audience"],
              GetTokenClaims(user, roles),
              expires: DateTime.Now.AddYears(1),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GetAuthorizationToken()
        {
            var headerAuth = HttpContext.Current.Request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(headerAuth))
            {
                var hAuth = headerAuth.ToString().Split(' ');
                if (hAuth.Length == 2 && !string.IsNullOrWhiteSpace(hAuth[1]))
                {
                    return hAuth[1];
                }
            }
            return string.Empty;
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        [HttpPost]
        [Route("phone/{ContactNo}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetCustomer(string ContactNo)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            if (string.IsNullOrWhiteSpace(ContactNo))
            {
                responseStatus.Status = false;
                responseStatus.Message = "Phone number cannot be blank.";
                return Request.CreateResponse(HttpStatusCode.NotFound, responseStatus);
            }
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var rcust = db.Customers.Include(o => o.CustomerAddresses).Where(o => string.Compare(o.ContactNo, ContactNo, true) == 0)?.FirstOrDefault();
                var oSerialize = JsonConvert.SerializeObject(rcust, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                var deSerialize = JsonConvert.DeserializeObject<Customer>(oSerialize);
                responseStatus.Data = deSerialize;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("get")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetCustomer()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string custId = User.Identity.GetUserId();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var rcust = db.Customers.Include(o => o.CustomerAddresses).Where(o => o.UserId == custId)?.FirstOrDefault();
                var oSerialize = JsonConvert.SerializeObject(rcust, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                var deSerialize = JsonConvert.DeserializeObject<Customer>(oSerialize);
                responseStatus.Data = deSerialize;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public HttpResponseMessage Login(LoginViewModel model)
        {
            //throw new Exception("Applciation error");
            HttpResponseMessage response = null;
            var user = AuthenticateAsync(model);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                var newRefreshToken = Guid.NewGuid().ToString();
                //refresh token
                using (var db = new rainbowwineEntities())
                {
                    var u = db.AspNetUsers.Where(o => o.Email == model.Email)?.FirstOrDefault();
                    string custId = u.Id;
                    var rcust = db.Customers.Where(o => string.Compare(o.UserId, custId, true) == 0)?.FirstOrDefault();
                    int uId = rcust.Id;// Convert.ToInt32(user.Id);
                    bool isVerifiedOtp = true;
                    var otp = db.CustomerOTPVerifies.Where(o => o.ContactNo == rcust.ContactNo);
                    if (otp.Count() > 0)
                    {
                        var fotp = otp.OrderByDescending(o => o.CustomerOTPVerifyId).Where(o => (o.IsDeleted ?? false)==true)?.FirstOrDefault();
                        if (fotp == null)
                        { isVerifiedOtp = false; }
                    }
                    else { isVerifiedOtp = false; }

                    if (!isVerifiedOtp)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new ResponseStatus
                        {
                            Status = true,
                            Data = new LoginToken
                            {
                                UserName = model.Email,
                                IsOTPVerified = false
                            }
                        });
                    }
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
                        IssuedDate = DateTime.UtcNow,
                        ExpiresDate = DateTime.UtcNow.AddYears(1)
                    };
                    db.UserRefreshTokens.Add(ur);
                    db.SaveChanges();
                }

                LoginToken loginToken = new LoginToken
                {
                    AccessToken = tokenString,
                    Expires = DateTime.Now.AddYears(1).ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT",
                    TokenType = "bearer",
                    Status = "200",
                    UserName = model.Email,
                    RefreshToken = newRefreshToken,
                    IsOTPVerified = true
                };
                response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus
                {
                    Status = true,
                    Data = loginToken
                });
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus
                {
                    Status = false,
                    Message = "Invalid username and password."
                });
            }
            //var hostName = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            //using (HttpClient client = new HttpClient())
            //{
            //    Uri url = new Uri($"{hostName}/token");
            //    var body = new Dictionary<string, string>();
            //    body.Add("grant_type", "password");
            //    body.Add("username", model.Email);
            //    body.Add("password", model.Password);


            //    var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
            //    response = client.SendAsync(req).Result;
            //    var ret = JsonConvert.DeserializeObject<object>(response.Content.ReadAsStringAsync().Result);
            //    response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = true, Data = ret });

            //}
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("relogin")]
        public HttpResponseMessage RefreshGenerator(RefreshTokenViewModel refreshModel)
        {
            HttpResponseMessage response = null;
            var token = refreshModel.Token;// GetAuthorizationToken();
            if (string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(refreshModel.RefreshToken))
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Input parameters is not valid. Please contact adminstrator." });

            string userId = default(string);
            string userEmail = default(string);
            var principal = GetPrincipalFromExpiredToken(token);
            var userIdClaim = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userJti = principal.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

            if (userIdClaim == null || userJti == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Invalid Input. Please contact administrator." });

            userId = userIdClaim.Value;
            userEmail = userJti.Value;

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userEmail))
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Invalid Input. Please contact administrator." });

            if (string.Compare(refreshModel.UserName, userEmail, true) != 0)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Invalid token. Please try again." });

            var newRefreshToken = Guid.NewGuid().ToString();
            bool validRTokenDB = false;
            string newJwtToken = default(string);
            using (var db = new rainbowwineEntities())
            {
                var refreshToken = db.UserRefreshTokens.Where(o => o.UserId == userId && o.Token == refreshModel.RefreshToken)?.FirstOrDefault();
                if (refreshToken != null && refreshToken.ExpiresDate > DateTime.Now)
                {
                    validRTokenDB = true;
                    newJwtToken = BuildToken(new UserViewModel { name = userEmail, Id = userId });

                    refreshToken.ExpiresDate = DateTime.Now.AddYears(1);
                    refreshToken.Token = newRefreshToken;
                    db.SaveChanges();

                }
            }

            if (!validRTokenDB)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Invalid refresh token. Please try again" });


            return Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus
            {
                Status = true,
                Data = new
                {
                    access_token = newJwtToken,
                    token_type = "bearer",
                    username = userEmail,
                    status = 200,
                    refresh_token = newRefreshToken
                }
            });

            //var hostName = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            //using (HttpClient client = new HttpClient())
            //{
            //    Uri url = new Uri($"{hostName}/token");
            //    var body = new Dictionary<string, string>();
            //    body.Add("grant_type", "refresh_token");
            //    body.Add("username", token.UserName);
            //    body.Add("refresh_token", token.RefreshToken);

            //    var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
            //    response = client.SendAsync(req).Result;
            //    var ret = JsonConvert.DeserializeObject<object>(response.Content.ReadAsStringAsync().Result);
            //    response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = true, Data = ret });

            //}
            //return response;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        //[Authorize(Roles = "Customer")]
        public HttpResponseMessage APIUserRegister(CustomerRegisterModel model)
        {
            HttpResponseMessage response = null;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new rainbowwineEntities())
                    {
                            var custExists = db.Customers.Where(o => o.ContactNo == model.ContactNo
                        && string.Compare(o.RegisterSource, "m", true) == 0)?.FirstOrDefault();

                        if (custExists != null)
                        {
                            bool isVerifiedOtp = true;
                            var otp = db.CustomerOTPVerifies.Where(o => o.CustomerId == custExists.Id);
                            if (otp.Count() > 0)
                            {
                                var fotp = otp.OrderByDescending(o => o.CustomerOTPVerifyId).Where(o => !o.IsDeleted ?? false)?.FirstOrDefault();
                                if (fotp != null)
                                { isVerifiedOtp = false; }
                            }
                            return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                new ResponseStatus
                                {
                                    Status = false,
                                    Message = "This phone number already exists. Please try logging in with another phone number. ",
                                    Data = new { IsOTPVerified = isVerifiedOtp }
                                });
                        }
                        db.Configuration.ProxyCreationEnabled = false;
                        var usertype = db.UserTypes.Where(o => string.Compare(o.UserTypeName, "customer", true) == 0)?.FirstOrDefault();
                        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.ContactNo };
                        //var custExists = db.Customers.Where(o => o.ContactNo == model.ContactNo)?.FirstOrDefault();
                        //if (custExists == null)
                        //{
                        var result = UserManager.CreateAsync(user, model.Password).Result;
                        if (result.Succeeded)
                        {
                            var userUpdate = db.AspNetUsers.Find(user.Id);
                            userUpdate.CreatedDate = DateTime.Now;
                            userUpdate.ModifiedDate = DateTime.Now;
                            db.SaveChanges();

                            AccountController accountController = new AccountController(UserManager, SignInManager);
                            accountController.AddRoleToUser(user.Id, usertype.UserTypeId);
                            Customer customer = new Customer
                            {
                                CustomerName = model.CustomerName,
                                ContactNo = model.ContactNo,
                                CreatedDate = DateTime.Now,
                                DOB = model.DOB,
                                UserId = user.Id,
                                RegisterSource = "m"
                            };
                            db.Customers.Add(customer);
                            db.SaveChanges();
                            //db.CustomerAddresses.Add(new CustomerAddress
                            //{
                            //    CustomerId = customer.Id,
                            //    Address = model.Address,
                            //    Flat = model.Flat,
                            //    Landmark = model.Landmark,
                            //    PlaceId = model.PlaceId,
                            //    FormattedAddress = model.FormattedAddress,
                            //    ShopId = model.ShopId ?? 0
                            //});
                            //db.SaveChanges();
                            //response = Login(new LoginViewModel { Email = model.Email, Password = model.Password, RememberMe = false });
                            response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = true, Message = "Account created successfully." });
                        }
                        else
                        {
                            string message = default(string);
                            foreach (var error in result.Errors)
                            {
                                message += error + ", ";
                            }
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = message });
                        }
                        //}
                        //else
                        //{
                        //    response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Customer contact number already exists." });
                        //}
                    }
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                         .SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage));
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = messages });
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + " 'InnerException':" + ex.InnerException.Message;
                SpiritUtility.AppLogging($"Api_AllUserRegister: {ex.Message}", ex.StackTrace);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = message });
            }

            return response;
        }

        [HttpPost]
        [Route("delete-address/{Id}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage APIUserGetAddress(int Id)
        {
            HttpResponseMessage response = null;
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string custId = User.Identity.GetUserId();
                    var custExists = db.Customers.Where(o => o.UserId == custId)?.FirstOrDefault();
                    var custAdressExists = db.CustomerAddresses.Where(o => o.CustomerAddressId == Id && o.CustomerId == custExists.Id)?.FirstOrDefault();
                    db.CustomerAddresses.Remove(custAdressExists);
                    db.SaveChanges();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = false, Message = "Address deleted successfully." });
                }
                catch (Exception ex)
                {
                    db.Dispose();
                    string message = ex.Message + " 'InnerException':" + ex.InnerException.Message;
                    SpiritUtility.AppLogging($"Api_APIUserAddAddress: {ex.Message}", ex.StackTrace);
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus
                    {
                        Status = false,
                        Message = message
                    });
                }
            }

            return response;
        }

        [HttpPost]
        [Route("add-address")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage APIUserAddAddress(UserCustomerAddress model)
        {
            HttpResponseMessage response = null;
            if (ModelState.IsValid)
            {
                using (var db = new rainbowwineEntities())
                {
                    try
                    {
                        string custId = User.Identity.GetUserId();
                        var custExists = db.Customers.Where(o => o.UserId == custId)?.FirstOrDefault();
                        db.CustomerAddresses.Add(new CustomerAddress
                        {
                            CustomerId = custExists.Id,
                            Address = model.Address,
                            Flat = model.Flat,
                            Landmark = model.Landmark,
                            PlaceId = model.PlaceId,
                            FormattedAddress = model.FormattedAddress,
                            ShopId = model.ShopId ?? 0,
                            Latitude = model.Latitude,
                            Longitude = model.Longitude,
                            CreatedDate = DateTime.Now,
                            AddressType = model.AddressType,
                            ZoneId = model.ZoneId
                        });
                        db.SaveChanges();
                        response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = false, Message = "Address updated successfully.", Data = model });
                    }
                    catch (Exception ex)
                    {
                        db.Dispose();
                        string message = ex.Message + " 'InnerException':" + ex.InnerException.Message;
                        SpiritUtility.AppLogging($"Api_APIUserAddAddress: {ex.Message}", ex.StackTrace);
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus
                        {
                            Status = false,
                            Message = message,
                            Data = model
                        });
                    }
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                     .SelectMany(x => x.Errors)
                                     .Select(x => x.ErrorMessage));
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = messages, Data = model });
            }


            return response;
        }

        [HttpPost]
        [Route("update-address")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage APIUserUpdateAddress(UserCustomerAddress model)
        {
            HttpResponseMessage response = null;
            if (model.CustomerAddressId <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Address Id is null.", Data = model });
            }
            if (ModelState.IsValid)
            {
                using (var db = new rainbowwineEntities())
                {
                    try
                    {
                        var custAdressExists = db.CustomerAddresses.Where(o => o.CustomerAddressId == model.CustomerAddressId)?.FirstOrDefault();

                        custAdressExists.Address = model.Address;
                        custAdressExists.Flat = model.Flat;
                        custAdressExists.Landmark = model.Landmark;
                        custAdressExists.PlaceId = model.PlaceId;
                        custAdressExists.FormattedAddress = model.FormattedAddress;
                        custAdressExists.ShopId = model.ShopId ?? 0;
                        custAdressExists.Latitude = model.Latitude;
                        custAdressExists.Longitude = model.Longitude;
                        custAdressExists.AddressType = model.AddressType;
                        custAdressExists.ZoneId = model.ZoneId;

                        db.SaveChanges();

                        response = Request.CreateResponse(HttpStatusCode.OK, new ResponseStatus { Status = false, Message = "Address updated successfully.", Data = model });
                    }
                    catch (Exception ex)
                    {
                        db.Dispose();
                        string message = ex.Message + " 'InnerException':" + ex.InnerException.Message;
                        SpiritUtility.AppLogging($"Api_APIUserAddAddress: {ex.Message}", ex.StackTrace);
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus
                        {
                            Status = false,
                            Message = message,
                            Data = model
                        });
                    }
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                     .SelectMany(x => x.Errors)
                                     .Select(x => x.ErrorMessage));
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = messages, Data = model });
            }


            return response;
        }

        [HttpPost]
        [Route("generateotp")]
        [AllowAnonymous]
        public HttpResponseMessage UserGenerateToken(LoginViewModel model)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            //string custId = User.Identity.GetUserId();
            var token = SpiritUtility.GenerateToken();
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var u = db.AspNetUsers.Where(o => o.Email == model.Email)?.FirstOrDefault();
                    string custId = u.Id;
                    var rcust = db.Customers.Where(o => string.Compare(o.UserId, custId, true) == 0)?.FirstOrDefault();
                    var otp = db.CustomerOTPVerifies.Where(o => o.CustomerId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        CustomerOTPVerify customerOTPVerify = new CustomerOTPVerify
                        {
                            ContactNo = rcust.ContactNo,
                            CreatedDate = DateTime.Now,
                            CustomerId = rcust.Id,
                            OTP = token,
                            IsDeleted = false
                        };
                        db.CustomerOTPVerifies.Add(customerOTPVerify);
                        db.SaveChanges();
                    }
                    else
                    {
                        token = otp.OTP;
                    }
                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSGenToken"], token);
                    //wsms.SendMessage(textmsg, rcust.ContactNo);

                    //Flow SMS
                    var dicti = new Dictionary<string, string>();
                    dicti.Add("OTP", token);

                    var templeteid = ConfigurationManager.AppSettings["SMSGenTokenFlowId"];

                    Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, rcust.ContactNo, dicti));
                    //

                    responseStatus.Message = $"A verification code has been sent to {rcust.ContactNo}. Standard rates apply.";

                }
                catch (Exception ex)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = ex.Message;

                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"ApiError_GenerateToken:{ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);

                    db.Dispose();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("verifyotp")]
        [AllowAnonymous]
        //[Authorize(Roles = "Customer")]
        public HttpResponseMessage UserGenerateTokenVerify(LoginViewModel model)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var u = db.AspNetUsers.Where(o => o.Email == model.Email)?.FirstOrDefault();
                    string custId = u.Id;
                    db.Configuration.ProxyCreationEnabled = false;
                    var rcust = db.Customers.Where(o => string.Compare(o.UserId, custId, true) == 0)?.FirstOrDefault();
                    var otp = db.CustomerOTPVerifies.Where(o => o.CustomerId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "There is no verification otp.";
                    }
                    else
                    {
                        if (string.Compare(model.Code, otp.OTP) == 0)
                        {
                            otp.VerifiedDate = DateTime.Now;
                            otp.IsDeleted = true;
                            db.SaveChanges();

                            responseStatus.Status = true;
                            responseStatus.Message = "Your number is verified.";
                            return Login(model);
                        }
                        else
                        {
                            responseStatus.Status = false;
                            responseStatus.Message = "Verification failed. Please try again.";
                        }
                    }

                }
                catch (Exception ex)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = ex.Message;

                    //db.AppLogs.Add(new AppLog
                    //{
                    //    CreateDatetime = DateTime.Now,
                    //    Error = $"ApiError_GenerateToken:{ex.Message}",
                    //    Message = ex.StackTrace,
                    //    MachineName = System.Environment.MachineName
                    //});
                    //db.SaveChanges();

                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseStatus);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("address")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetCustomerAddress()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var address = db.CustomerAddresses.Where(o => o.Customer.UserId == uId).ToList();
                responseStatus.Data = address;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("agent")]
        [Authorize(Roles = "Deliver")]
        public HttpResponseMessage GetAgent()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string uId = User.Identity.GetUserId();
                DeliveryAgentDBO agentDBO = new DeliveryAgentDBO();
                var rcust = agentDBO.GetAgent(uId);
                var cdate = DateTime.Now;

                var loginAgent = db.DeliveryAgentLogins.Where(o => o.DeliveryAgentId == rcust.Id
                && o.OnDuty.Year == cdate.Year && o.OnDuty.Month == cdate.Month && o.OnDuty.Day == cdate.Day)?.FirstOrDefault();

                responseStatus.Data = new
                {
                    Id = rcust.Id,
                    DeliveryExecName = rcust.DeliveryExecName,
                    Contact = rcust.Contact,
                    OnDuty = loginAgent?.OnDuty,
                    OffDuty = loginAgent?.OffDuty,
                    IsOnOff = loginAgent?.IsOnOff
                };
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
