using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services;
using RainbowWine.Services.DBO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Linq;
using System.IO;
using System.Text;
using RestClient.Net;
using RainbowWine.Providers;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using SZInfrastructure;
using SZData.Interfaces;
using static RainbowWine.Services.FireStoreService;
using Google.Cloud.Firestore;
using RainbowWine.Services.DO;

namespace RainbowWine.Controllers
{
    [RoutePrefix("api/v3/UserManage")]
    [EnableCors("*", "*", "*")]
    public class UserManageController : ApiController
    {

        rainbowwineEntities db = new rainbowwineEntities();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserManageController()
        {
        }

        public UserManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [HttpPost]
        [Route("GetUserByPhone")]
        [AllowAnonymous]
        public IHttpActionResult GetUserByPhone(UserManageGetUserByPhone_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var data = db.Database.SqlQuery<UserManageGetUserByPhone_Resp>("USP_Get_LoginDetails @PhoneNo",
                    new SqlParameter("PhoneNo", req.PhoneNo)).ToList();

                    if (data != null && data.Count > 0)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = data,
                            Status = true,
                            Message = "Success"

                        };
                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("generateotp")]
        [AllowAnonymous]
        public IHttpActionResult UserGenerateToken(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var OTPno = SpiritUtility.GenerateToken();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    //var rcust = db.Customers.Where(o => string.Compare(o.ContactNo, req.PhoneNo, true) == 0 && o.RegisterSource == "m")?.FirstOrDefault();
                    //var otp = db.CustomerOTPVerifies.Where(o => o.CustomerId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    var otp = db.CustomerOTPVerifies.Where(o => o.ContactNo == req.PhoneNo && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        CustomerOTPVerify customerOTPVerify = new CustomerOTPVerify
                        {
                            ContactNo = req.PhoneNo,
                            CreatedDate = DateTime.Now,
                            //CustomerId = rcust.Id,
                            OTP = OTPno,
                            IsDeleted = false
                        };
                        db.CustomerOTPVerifies.Add(customerOTPVerify);
                        db.SaveChanges();
                    }
                    else
                    {
                        var _otpno = new SqlParameter { ParameterName = "_otpno", Value = OTPno };
                        var _phoneNo = new SqlParameter { ParameterName = "_phoneNo", Value = req.PhoneNo };
                        //var _CustomerId = new SqlParameter { ParameterName = "_CustomerId", Value = rcust.Id };


                        SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.VarChar, 200);
                        returnCode.Direction = ParameterDirection.Output;

                        //db.Database.ExecuteSqlCommand("USP_ModifyOTP @_otpno,@_phoneNo,@_CustomerId,@Result out", _otpno, _phoneNo, _CustomerId, returnCode);
                        db.Database.ExecuteSqlCommand("USP_ModifyOTP @_otpno,@_phoneNo,@Result out", _otpno, _phoneNo, returnCode);
                        string Status = Convert.ToString(returnCode.Value);
                        if (Status != null)
                        {
                            OTPno = Status;
                        }
                    }

                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSGenToken"], OTPno);
                    //wsms.SendMessage(textmsg, req.PhoneNo);


                    //Flow SMS
                    var dicti = new Dictionary<string, string>();
                    dicti.Add("OTP", OTPno);

                    var templeteid = ConfigurationManager.AppSettings["SMSGenTokenFlowId"];

                    Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, req.PhoneNo, dicti));
                    //
                    responseStatus.Message = $"A verification code has been sent to {req.PhoneNo}. Standard rates apply.";

                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("verifyotp")]
        [AllowAnonymous]
        public IHttpActionResult UserGenerateTokenVerify(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    //var rcust = db.Customers.Where(o => string.Compare(o.ContactNo, req.PhoneNo, true) == 0 && o.RegisterSource == "m")?.FirstOrDefault();
                    //var otp = db.CustomerOTPVerifies.Where(o => o.CustomerId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();

                    var otp = db.CustomerOTPVerifies.Where(o => o.ContactNo == req.PhoneNo && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "There is no verification otp.";
                    }
                    else
                    {
                        if (string.Compare(req.OTP, otp.OTP) == 0)
                        {
                            otp.VerifiedDate = DateTime.Now;
                            otp.IsDeleted = true;
                            db.SaveChanges();
                            responseStatus.Status = true;
                            responseStatus.Message = "Verification Successful.";

                        }
                        else
                        {
                            responseStatus.Status = false;
                            responseStatus.Message = "Verification code does not match.";
                        }
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }

        }

        [HttpPost]
        [Route("ChangePassword")]
        [AllowAnonymous]
        public IHttpActionResult ChangePasswordByPhoneNo(ChangePasswordByPhoneNo_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var u = db.AspNetUsers.Where(o => o.Email == req.Email)?.FirstOrDefault();
                    string custId = u.Id;

                    var rcust = db.Customers.Where(o => string.Compare(o.UserId, custId, true) == 0 && o.RegisterSource == "m")?.FirstOrDefault();
                    if (rcust == null)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Customer does not exists."
                        };
                        return Content(HttpStatusCode.BadRequest, responseStatus);
                    }
                    else if (string.IsNullOrWhiteSpace(rcust.UserId))
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Customer does not exists."
                        };
                        return Content(HttpStatusCode.BadRequest, responseStatus);
                    }
                    string code = UserManager.GeneratePasswordResetTokenAsync(custId).Result;
                    var result = UserManager.ResetPasswordAsync(custId, code, req.Password).Result;

                    // var email = db.AspNetUsers.Where(e => e.Email == req.Email).SingleOrDefault();
                    if (result.Succeeded)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Record saved successfully"
                        };
                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }

                    return Ok(responseStatus);

                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }

        }


        [HttpPost]
        [Route("ResetPassword")]
        public IHttpActionResult ResetPassword(ResetPasswordByEmail_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var u = db.AspNetUsers.Where(o => o.Email == req.Email)?.FirstOrDefault();
                    string custId = u.Id;

                    var rcust = db.Customers.Where(o => string.Compare(o.UserId, custId, true) == 0 && o.RegisterSource == "m")?.FirstOrDefault();
                    if (rcust == null)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Customer does not exists."
                        };
                        return Content(HttpStatusCode.BadRequest, responseStatus);
                    }
                    else if (string.IsNullOrWhiteSpace(rcust.UserId))
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Customer does not exists."
                        };
                        return Content(HttpStatusCode.BadRequest, responseStatus);
                    }
                    var user = UserManager.FindById(rcust.UserId);
                    var passresult = UserManager.CheckPassword(user, req.OldPassword);
                    if (!passresult)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Incorrect old password."
                        };
                        return Content(HttpStatusCode.BadRequest, responseStatus);
                    }

                    string code = UserManager.GeneratePasswordResetTokenAsync(rcust.UserId).Result;
                    var result = UserManager.ResetPasswordAsync(rcust.UserId, code, req.NewPassword).Result;

                    if (result.Succeeded)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = true,
                            Message = "Password Changed successfully"
                        };
                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }

                    return Ok(responseStatus);

                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }

        }

        [HttpPost]
        [Route("UserRatings")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult Insert_UserRatings(Insert_UserRatings_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            OrderDBO orderDBO = new OrderDBO();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    using (var db = new rainbowwineEntities())
                    {
                        string Id = User.Identity.GetUserId();
                        var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                        var Rating = new SqlParameter { ParameterName = "Rating", Value = req.Rating };
                        var Title = new SqlParameter { ParameterName = "Title", Value = req.Title };
                        var Review = new SqlParameter { ParameterName = "Review", Value = req.Review };
                        var Comment = new SqlParameter { ParameterName = "Comment", Value = req.Comment };
                        var CustomerId = new SqlParameter { ParameterName = "CustomerId ", Value = custId.Id };
                        var OrderId = new SqlParameter { ParameterName = "OrderId", Value = req.OrderId };


                        SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.Int, 200);
                        returnCode.Direction = ParameterDirection.Output;

                        db.Database.ExecuteSqlCommand("USP_INSERT_RatingDetails @Rating,@Title,@Review,@Comment,@CustomerId,@OrderId ,@Result out", Rating, Title, Review, Comment, CustomerId, OrderId, returnCode);
                        var Result = (int)returnCode.Value;

                        if (Result != 0)
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = true,
                                Message = "Record saved successfully"
                            };
                            //The code below adds an incentive or penalty based on user ratings, for the delivery boys
                           int res= orderDBO.OrderRatingIncentive(req.Rating.Value,req.OrderId.Value);
                            //var res = db.Database.ExecuteSqlCommand("OrderRating_Incentive_Ins @Rating, @OrderId", Rating, OrderId);
                            if (res == 1)
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    string fcm_title = default(string);
                                    string fcm_msg = default(string);
                                    if (req.Rating == 5)
                                    {
                                        fcm_title = "WOW! A 5 STAR RATING for your Order Delivery!";
                                        fcm_msg = "You just earned Rs. 5 because a customer gave you a 5 star rating!!";
                                    }
                                    else if (req.Rating == 1 || req.Rating == 2)
                                    {
                                        fcm_title = "OH NO! A LOW RATING!";
                                        fcm_msg = "You just lost Rs. 5 because a customer gave you a 1 or 2 star rating!!";
                                    }
                                    var jsonValue = new
                                    {
                                        OrderId = req.OrderId,
                                        Title = fcm_title,
                                        Message = fcm_msg
                                    };
                                    var serializeJson = JsonConvert.SerializeObject(jsonValue);
                                    var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
                                    var resp = client.PostAsync(ConfigurationManager.AppSettings["DeliveryAppGeneralNotifURL"], content).Result;
                                    var ret = resp.Content.ReadAsStringAsync().Result;
                                }
                            }
                            //End of code for delivery boy incentive/penalty
                        }
                        else
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = false,
                                Message = "Failed"
                            };
                        }
                        return Ok(responseStatus);
                    }

                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }
        }

        [HttpPost]
        [Route("InsertNotifyOrderDelivery")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult InsertNotifyOrderDelivery(NotifyOrderDelivery_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    using (var db = new rainbowwineEntities())
                    {
                        string Id = User.Identity.GetUserId();
                        var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                        var OrderId = new SqlParameter { ParameterName = "OrderId", Value = req.OrderId };
                        var CustomerId = new SqlParameter { ParameterName = "CustomerId ", Value = custId.Id };

                        SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.Int, 200);
                        returnCode.Direction = ParameterDirection.Output;

                        db.Database.ExecuteSqlCommand("Notify_OrderDelivery_Ins @OrderId,@CustomerId,@Result out", OrderId, CustomerId, returnCode);
                        var Result = (int)returnCode.Value;

                        if (Result != 0)
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = Result,
                                Status = true,
                                Message = "Record saved successfully"
                            };
                        }
                        else
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = false,
                                Message = "Failed"
                            };
                        }
                        return Ok(responseStatus);
                    }

                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }
        }

        [HttpPost]
        [Route("GetNotifyOrderDelivery")]
        [AllowAnonymous]
        public IHttpActionResult GetNotifyOrderDelivery()
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var data = db.Database.SqlQuery<GetNotifyOrderDelivery_Resp>("Notify_OrderDelivery_Sel").ToList();

                    if (data != null && data.Count > 0)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = data,
                            Status = true,
                            Message = "Success"

                        };
                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("UpdateNotifyOrderDelivery")]
        [AllowAnonymous]
        public IHttpActionResult UpdateNotifyOrderDelivery(UpdateNotifyOrderDelivery_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    using (var db = new rainbowwineEntities())
                    {
                        var NotifyId = new SqlParameter { ParameterName = "NotifyId", Value = req.NotifyId };


                        var Result = db.Database.ExecuteSqlCommand("Notify_OrderDelivery_Update @NotifyId", NotifyId);

                        if (Result != 0)
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = Result,
                                Status = true,
                                Message = "Record Updated successfully"
                            };
                        }
                        else
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = false,
                                Message = "Failed"
                            };
                        }
                        return Ok(responseStatus);
                    }

                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }
        }

        [HttpPost]
        [Route("OrderDetails")]
        [AllowAnonymous]
        public IHttpActionResult OrderDetails(OrderDetails_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    using (var db = new rainbowwineEntities())
                    {
                        OrderDBO objOrderDBO = new OrderDBO();
                        var data1 = objOrderDBO.GetOrderDetails(req.OrderId);
                        if (data1 != null && data1.Count > 0)
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = data1,
                                Status = true,
                                Message = "Success"

                            };
                        }
                        else
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = false,
                                Message = "Failed"
                            };
                        }
                        return Ok(responseStatus);
                    }
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpGet]
        [Route("GetRatingStarOption")]
        public HttpResponseMessage GetRatingStarOption()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //db.Configuration.ProxyCreationEnabled = false;
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var RatingStarData = db.RatingStars.Include(r => r.RatingOptions).ToList();
                var oSerialize = JsonConvert.SerializeObject(RatingStarData, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                var deSerialize = JsonConvert.DeserializeObject<List<RatingStar>>(oSerialize);
                responseStatus.Data = deSerialize;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("InsertNotifyProductAval")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult InsertNotifyProductAval(InsertNotifyProductAval_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    using (var db = new rainbowwineEntities())
                    {
                        string Id = User.Identity.GetUserId();
                        var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                        var ProuctID = new SqlParameter { ParameterName = "ProuctID", Value = req.ProuctID };
                        var ShopId = new SqlParameter { ParameterName = "ShopId", Value = req.ShopId };
                        var CustomerId = new SqlParameter { ParameterName = "CustomerId ", Value = custId.Id };

                        SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.Int, 200);
                        returnCode.Direction = ParameterDirection.Output;

                        db.Database.ExecuteSqlCommand("Notify_ProductAval_Ins @ProuctID,@ShopId,@CustomerId,@Result out", ProuctID, ShopId, CustomerId, returnCode);
                        var Result = (int)returnCode.Value;

                        if (Result != 0)
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = true,
                                Message = "Record saved successfully"
                            };
                        }
                        else
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = false,
                                Message = "Failed"
                            };
                        }
                        return Ok(responseStatus);
                    }

                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }
        }

        [HttpGet]
        [Route("GetLastOrderDetails")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetLastOrderDetails()
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    GetLastOrderDetails_Resp obj = new GetLastOrderDetails_Resp();
                    string Id = User.Identity.GetUserId();
                    var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                    SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.Int, 200);
                    returnCode.Direction = ParameterDirection.Output;

                    var result = db.Database.SqlQuery<GetLastOrderDetails_Resp>("LastOrderDetails_Sel @CustomerId",
                    new SqlParameter("CustomerId", custId.Id)).ToList();


                    foreach (var e in result)
                    {
                        var data = db.Database.SqlQuery<Insert_UserRatings_Req>("GetReviewDetails_Sel @CustomerId,@OrderID",
                                     new SqlParameter("CustomerId", custId.Id),
                                       new SqlParameter("OrderID", e.OrderID)
                                        ).SingleOrDefault();

                        if (data != null)
                        {
                            e.Review = data;
                        }
                        else
                        {
                            e.Review = null;
                        }
                    }

                    if (result != null)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = result,
                            Status = true,
                            Message = "Success"

                        };
                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("ProductNotificationDetails")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetProductNotificationDetails(GetProductNotificationDetails_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {

                    using (var db = new rainbowwineEntities())
                    {
                        OrderDBO objOrderDBO = new OrderDBO();
                        string Id = User.Identity.GetUserId();
                        var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                        var data1 = objOrderDBO.ProductNotificationDetails(req.ShopId, req.ProuctId, custId.Id);
                        if (data1 != null && data1.Count > 0)
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = data1,
                                Status = true,
                                Message = "Success"

                            };
                        }
                        else
                        {
                            responseStatus = new ResponseStatus()
                            {
                                Data = null,
                                Status = false,
                                Message = "Failed"
                            };
                        }
                        return Ok(responseStatus);
                    }
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }
        }

        [HttpPost]
        [Route("AllPremiumProductDetails")]
        //[Authorize(Roles = "Customer")]
        public HttpResponseMessage GetAllPremiumProductDetails(GetAllPremiumProductDetails_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                //var prod = db.Inventories.Where(o => o.ShopID == shopId).Select(o => o.ProductDetail).ToList();
                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetAllPremiumProductDetails(req.shopId);
                responseStatus.Data = prod;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpGet]
        [Route("GetRecommPremiumProductDetails")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetRecommPremiumProductDetails(int shopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                ProductDBO productDBO = new ProductDBO();
                var prod = productDBO.GetRecomPremiumProductDetails(shopId, custId.Id);
                responseStatus.Data = prod;
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }


        [HttpPost]
        [Route("premium-product-search-volume/shop/{shopId}/product/{productRefId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetProductSearch(int shopId, int productRefId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            ProductDBO productDBO = new ProductDBO();
            var prod = productDBO.GetProductPremiumVolById(shopId, productRefId);

            responseStatus.Data = prod.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

        [HttpPost]
        [Route("GetRecentOrderDetails")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetRecentOrderDetails(RecentOrderDetails_req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    RecentOrderDetails obj = new RecentOrderDetails();
                    string Id = User.Identity.GetUserId();
                    var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                    var result = Enumerable.Repeat(new RecentOrderDetails(), 1).ToList();


                    foreach (var e in result)
                    {
                        var data = db.Database.SqlQuery<RecentOrderDetails_Resp>("RecentOrderDetails_Sel @CustomerId",
                                   new SqlParameter("CustomerId", custId.Id)).ToList();

                        if (data != null && data.Count > 0)
                        {
                            var b = db.Database.SqlQuery<RecentOrderDetails>("Questionsdetails_Sel @_group,@CustomerID",
                             new SqlParameter("_group", req.GroupID),
                             new SqlParameter("CustomerID", custId.Id)).ToList();
                            result[0].Text = b[0].Text;
                            result[0].GroupOne = b[0].GroupOne;
                            result[0].GroupTwo = b[0].GroupTwo;
                            result[0].DateText = b[0].DateText;
                            result[0].TimeText = b[0].TimeText;
                            e.QuestionsResp = data;
                        }
                        else
                        {
                            result[0].Text = ConfigurationManager.AppSettings["NewUserSupportQues"].ToString();
                            result[0].GroupOne = req.GroupID;
                            result[0].GroupTwo = null;
                            result[0].DateText = DateTime.Now.ToString("dd-MM-yyyy");
                            result[0].TimeText = DateTime.Now.ToString("hh:mm tt");
                            e.QuestionsResp = null;
                        }
                    }

                    if (result != null)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = result,
                            Status = true,
                            Message = "Success"

                        };
                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("GetTicketQuestionDetails")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetTicketQuestionDetails(RecentOrderDetails_req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    string Id = User.Identity.GetUserId();
                    var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                    //  var result = db.Database.SqlQuery<QuestionTitle>("Title_Sel @Orderid",
                    //new SqlParameter("Orderid", req.OrderId)).ToList();
                    List<QuestionTitle> List = new List<QuestionTitle>();
                    var result = Enumerable.Repeat(new QuestionTitle(), 1).ToList();

                    foreach (var e in result)
                    {
                        var data = db.Database.SqlQuery<TicketQuestionDetailsResp>("TicketQuestionDetails_Sel @Orderid,@GroupID,@Request,@QuestionID,@Question,@CustomerId",
                   new SqlParameter("Orderid", req.OrderId != null ? req.OrderId : (object)DBNull.Value),
                      new SqlParameter("GroupID", req.GroupID != null ? req.GroupID : (object)DBNull.Value),
                         new SqlParameter("Request", req.Request != null ? req.Request : (object)DBNull.Value),
                            new SqlParameter("QuestionID", req.QuestionID != null ? req.QuestionID : (object)DBNull.Value),
                               new SqlParameter("Question", req.Question != null ? req.Question : (object)DBNull.Value),
                                 new SqlParameter("CustomerId", custId.Id)).ToList();

                        //var a = (dynamic)null;
                        var a = db.Database.SqlQuery<QuestionTitle>("Title_Sel @Orderid",
                                    new SqlParameter("Orderid", req.OrderId)).ToList();
                        if (data != null && data.Count > 1)
                        {
                            result[0].DateText = a[0].DateText;
                            result[0].TimeText = a[0].TimeText;
                            result[0].Title = a[0].Title;
                            result[0].OrderID = a[0].OrderID;
                            e.QuestionsResp = data;
                        }
                        else
                        {
                            e.QuestionsResp = null;
                            result[0].Title = "Please elaborate your issue in details";
                            result[0].DateText = a[0].DateText;
                            result[0].TimeText = a[0].TimeText;
                            result[0].OrderID = a[0].OrderID;

                        }

                    }

                    string Msg = string.Empty;
                    if (result[0].QuestionsResp == null)
                    {
                        Msg = "Final";
                    }
                    else
                    {
                        Msg = "Success";
                    }

                    if (result != null)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = result,
                            Status = true,
                            Message = Msg

                        };

                        if (req.QuestionID != null)
                        {
                            //  var data = JsonConvert.DeserializeObject<Dictionary<string,string>>(responseStatus);
                            var CustomerId = new SqlParameter { ParameterName = "CustomerId", Value = custId.Id };
                            var Response = new SqlParameter { ParameterName = "Response", Value = responseStatus.ToString() };

                            SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.VarChar, 200);
                            returnCode.Direction = ParameterDirection.Output;

                            db.Database.ExecuteSqlCommand("InsertRespchat_Ins @CustomerId,@Response", CustomerId, Response);
                        }

                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpGet]
        [Route("GetTicketCommunicationHistory")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetTicketCommunicationHistory()
        {
            try
            {
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.Select(x => x.Id).FirstOrDefault();

                var response = db.Database.SqlQuery<TicketCommunicationHistory>("Get_Ticket_CommunicationHistory @CustomerId",
               new SqlParameter("CustomerId", custId.Value.ToString())).ToList();

                var data = new
                {
                    Data = response,
                    Status = true,
                    Message = "Success"

                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                var responseStatus = new ResponseStatus()
                {
                    Data = null,
                    Status = true,
                    Message = "Some error occurred while processign the request" + " " + ex.Message
                };
                SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                db.Dispose();
                return Content(HttpStatusCode.InternalServerError, responseStatus);
            }

        }

        [HttpGet]
        [Route("GetTicketCommunicationHistory/{index}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetTicketCommunicationHistoryPagging(int index=1)
        {
            try
            {
                string Id = User.Identity.GetUserId();
                var custId = db.Customers.Where(o => o.UserId == Id)?.Select(x => x.Id).FirstOrDefault();

                var response = db.Database.SqlQuery<TicketCommunicationHistory>("Get_Ticket_CommunicationHistory_Paging @index, @size, @CustomerId",
               new SqlParameter("index", index.ToString()),
               new SqlParameter("size", "10"),
               new SqlParameter("CustomerId", custId.Value.ToString())).ToList();

                var data = new
                {
                    Data = response,
                    Status = true,
                    Message = "Success"

                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                var responseStatus = new ResponseStatus()
                {
                    Data = null,
                    Status = true,
                    Message = "Some error occurred while processign the request" + " " + ex.Message
                };
                SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                db.Dispose();
                return Content(HttpStatusCode.InternalServerError, responseStatus);
            }

        }

        [HttpPost]
        [Route("InsertOrderIssue")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult InsertOrderIssue()
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    string Id = User.Identity.GetUserId();
                    var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();

                    var json = HttpContext.Current.Request.Params["reqjson"];

                    InsertOrderIssue_Req req = JsonConvert.DeserializeObject<InsertOrderIssue_Req>(json);


                    var OrderId = new SqlParameter { ParameterName = "OrderId", Value = req.OrderId != null ? req.OrderId : (object)DBNull.Value };
                    var Issue = new SqlParameter { ParameterName = "Issue", Value = req.Issue };
                    var CustomerId = new SqlParameter { ParameterName = "CustomerId", Value = custId.Id };
                    SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.Int, 200);
                    returnCode.Direction = ParameterDirection.Output;

                    db.Database.ExecuteSqlCommand("InsertOrderIssueDetails @CustomerId,@OrderId,@Issue,@Result out", CustomerId, OrderId, Issue, returnCode);
                    var Result = returnCode.Value;

                    string fnm = string.Empty;
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            HttpPostedFile f = HttpContext.Current.Request.Files[i];
                            string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                            var FileExtension = Path.GetExtension(f.FileName).Substring(1);
                            var Fname = FileName + "." + FileExtension;
                            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/TicketDocuments/");
                            fnm = filePath + Fname;
                            f.SaveAs(fnm);
                            var DocPath = new SqlParameter { ParameterName = "DocPath", Value = Fname };
                            var TicketID = new SqlParameter { ParameterName = "TicketID", Value = Result };

                            db.Database.ExecuteSqlCommand("InsertOrderIssueDocuments @DocPath,@TicketID", DocPath, TicketID);

                        }

                    }

                    var data = db.Database.SqlQuery<ChatApiRespDetails>("ChatDetailsforzoho_sel @CustomerId,@OrderId,@TicketID",
                   new SqlParameter("CustomerId", custId.Id),
                    new SqlParameter("OrderId", req.OrderId != null ? req.OrderId : (object)DBNull.Value),
                     new SqlParameter("TicketID", Result)

                   ).ToList();

                    ChatAPI objChatAPI = new ChatAPI();
                    string subject = data[0].Subject!=null? data[0].Subject:"Query for new customer.";
                    string description = data[0].Description != null ? data[0].Description : req.Issue;
                    string ph = data[0].PhoneNo;
                    string email = data[0].Email;

                    Models.Cf cf = new Models.Cf();
                    cf.cf_permanentaddress = null;
                    cf.cf_dateofpurchase = null;
                    cf.cf_phone = null;
                    cf.cf_numberofitems = null;
                    cf.cf_url = null;
                    cf.cf_secondaryemail = null;
                    cf.cf_severitypercentage = "0.0";
                    cf.cf_modelname = "F3 2017";
                    objChatAPI.subCategory = "Sub General";
                    objChatAPI.cf = cf;

                    objChatAPI.productId = "";
                    objChatAPI.contactId = "26909000000149179";
                    objChatAPI.subject = subject;
                    objChatAPI.dueDate = "";
                    objChatAPI.departmentId = "26909000000010772";
                    objChatAPI.channel = "chat";
                    objChatAPI.description = description;
                    objChatAPI.priority = "High";
                    objChatAPI.classification = "";
                    objChatAPI.assigneeId = "26909000000064001";
                    objChatAPI.phone = ph;
                    objChatAPI.category = "general";
                    objChatAPI.email = email;
                    objChatAPI.status = "Open";
                    //
                    ChatHelper objChatHelper = new ChatHelper();
                    string TicID = objChatHelper.chatapi(objChatAPI);

                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        if (TicID != null && TicID != "")
                        {
                            objChatHelper.chatapiAttchmentPath(fnm, TicID);
                        }
                    }

                    if (!string.IsNullOrEmpty(TicID))
                    {
                        //
                        var respo = db.Database.ExecuteSqlCommand("InsertTicketCommunicationHistory @Orderid,@GroupID,@Request,@QuestionID,@Question,@CustomerId",
                new SqlParameter("Orderid", req.OrderId != null ? req.OrderId : (object)DBNull.Value),
                   new SqlParameter("GroupID", (object)DBNull.Value),
                      new SqlParameter("Request", "Please elaborate your issue in details."),
                         new SqlParameter("QuestionID", (object)DBNull.Value),
                            new SqlParameter("Question", req.Issue),
                              new SqlParameter("CustomerId", custId.Id));

                        var respo1 = db.Database.ExecuteSqlCommand("InsertTicketCommunicationHistory @Orderid,@GroupID,@Request,@QuestionID,@Question,@CustomerId",
            new SqlParameter("Orderid", req.OrderId != null ? req.OrderId : (object)DBNull.Value),
               new SqlParameter("GroupID", (object)DBNull.Value),
                  new SqlParameter("Request", "Thank you for raising the ticket. Your ticket number is- " + TicID),
                     new SqlParameter("QuestionID", (object)DBNull.Value),
                        new SqlParameter("Question", (object)DBNull.Value),
                          new SqlParameter("CustomerId", custId.Id));
                    }

                    if (Result != null)
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = "Thank you for raising the ticket. Your ticket number is- " + TicID,
                            Status = true,
                            Message = "Success"

                        };
                    }
                    else
                    {
                        responseStatus = new ResponseStatus()
                        {
                            Data = null,
                            Status = false,
                            Message = "Failed"
                        };
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }


        [HttpGet]
        [Route("order-payment-type")]
        public IHttpActionResult GetPaymentType()
        {
            ResponseStatus responseStatus = new ResponseStatus();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    //string Id = User.Identity.GetUserId();
                    //var custId = db.Customers.Where(o => o.UserId == Id)?.FirstOrDefault();
                    OrderDBO orderDBO = new OrderDBO();
                    var payType = orderDBO.GetOrderPaymentType();
                    if (payType == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "No Payment Type found.";
                        return Content(HttpStatusCode.NotFound, responseStatus);
                    }
                    responseStatus.Status = true;
                    responseStatus.Data = payType;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request" + " " + ex.Message
                    };
                    SpiritUtility.AppLogging($"ApiError_GenerateToken: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("delivery/order-reached")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult DeliveryOrderReached(int orderId)
        {
            if (orderId == 0)
            {
                return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string uId = User.Identity.GetUserId();

                    var statusCancel = (int)OrderStatusEnum.CancelByCustomer;
                    var order = db.Orders.Find(orderId);
                    if (order.OrderStatusId == statusCancel)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "The customer has cancelled this order. Please proceed with the next order.";
                        
                    }
                    else
                    {
                        OrderDBO orderDBO = new OrderDBO();
                        orderDBO.UpdatedOrderStatus(orderId, uId, OrderStatusEnum.DeliveryReached.ToString());

                        responseStatus.Status = true;
                        responseStatus.Message = "Update order status as REACHED.";
                    }
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_DeliveryOrderReached: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

        }

        [HttpGet]
        [Route("order")]
        public IHttpActionResult GetOrder(int orderId)
        {
            if (orderId == 0)
            {
                return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string uId = User.Identity.GetUserId();
                    //var aspUser = db.AspNetUsers.Find(uId);
                    OrderDBO orderDBO = new OrderDBO();
                    var ord = orderDBO.GetOrder(orderId);
                    if (ord == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "Order not found.";

                        return Content(HttpStatusCode.NotFound, responseStatus);
                    }
                    responseStatus.Data = ord;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    SpiritUtility.AppLogging($"Api_DeliveryOrderReached: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

        }

        [HttpPost]
        [Route("generatepin")]
        [AllowAnonymous]
        public IHttpActionResult UserGeneratePin(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var pinNo = SpiritUtility.GenerateToken4D();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var rcust = db.AspNetUsers.Where(o => string.Compare(o.Email, req.Email, true) == 0)?.FirstOrDefault();
                    var otp = db.CustomerPayPins.Where(o => o.UserId == rcust.Id && o.OrderId==req.OrderId && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        CustomerPayPin customerPayPin = new CustomerPayPin
                        {
                            ContactNo = rcust.PhoneNumber,
                            CreatedDate = DateTime.Now,
                            UserId = rcust.Id,
                            Email = req.Email,
                            Pin = pinNo,
                            OrderId=req.OrderId,
                            IsDeleted = false
                        };
                        db.CustomerPayPins.Add(customerPayPin);
                        db.SaveChanges();
                    }
                    else
                    {
                        var _pinno = new SqlParameter { ParameterName = "_pinno", Value = pinNo };
                        var _phoneNo = new SqlParameter { ParameterName = "_phoneNo", Value = rcust.PhoneNumber };
                        var _userId = new SqlParameter { ParameterName = "_UserId", Value = rcust.Id };
                        var _email = new SqlParameter { ParameterName = "_Email", Value = req.Email };
                        var _orderId = new SqlParameter { ParameterName = "_OrderId", Value = req.OrderId };


                        SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.VarChar, 200);
                        returnCode.Direction = ParameterDirection.Output;

                        db.Database.ExecuteSqlCommand("CustomerPayPin_ByOrder_InsUpd @_pinno,@_phoneNo,@_UserId,@_Email,@_OrderId,@Result out", _pinno, _phoneNo, _userId, _email, _orderId, returnCode);
                        string Status = Convert.ToString(returnCode.Value);
                        if (Status != null)
                        {
                            pinNo = Status;
                        }
                    }

                    int statusCustGeneratePin = (int)OrderStatusEnum.CustGeneratePin;
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = req.OrderId,
                        StatusId = statusCustGeneratePin,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    responseStatus.Data = new { pin = pinNo, contactNo = req.PhoneNo };
                    responseStatus.Message = $"A verification code has been sent to {req.PhoneNo}. Standard rates apply";

                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processing the request"
                    };
                    SpiritUtility.AppLogging($"Api_UserGeneratePin: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("generatepinbydelivery")]
        [AllowAnonymous]
        public IHttpActionResult UserGeneratePinByDelivery(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var pinNo = SpiritUtility.GenerateToken4D();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var rcust = db.AspNetUsers.Where(o => string.Compare(o.Email, req.Email, true) == 0)?.FirstOrDefault();
                    var otp = db.CustomerPayPins.Where(o => o.UserId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        CustomerPayPin customerPayPin = new CustomerPayPin
                        {
                            ContactNo = rcust.PhoneNumber,
                            CreatedDate = DateTime.Now,
                            UserId = rcust.Id,
                            Email = req.Email,
                            Pin = pinNo,
                            IsDeleted = false
                        };
                        db.CustomerPayPins.Add(customerPayPin);
                        db.SaveChanges();
                    }
                    else
                    {
                        var pno = string.IsNullOrWhiteSpace(rcust.PhoneNumber) ? "" : rcust.PhoneNumber;
                        var _pinno = new SqlParameter { ParameterName = "_pinno", Value = pinNo };
                        var _phoneNo = new SqlParameter { ParameterName = "_phoneNo", Value = pno };
                        var _userId = new SqlParameter { ParameterName = "_UserId", Value = rcust.Id };
                        var _email = new SqlParameter { ParameterName = "_Email", Value = req.Email };


                        SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.VarChar, 200);
                        returnCode.Direction = ParameterDirection.Output;

                        db.Database.ExecuteSqlCommand("CustomerPayPin_InsUpd @_pinno,@_phoneNo,@_UserId,@_Email,@Result out", _pinno, _phoneNo, _userId, _email, returnCode);
                        string Status = Convert.ToString(returnCode.Value);
                        if (Status != null)
                        {
                            pinNo = Status;
                        }
                    }

                    int statusCustGeneratePin = (int)OrderStatusEnum.DelPinGenerated;
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    foreach (var item in req.OrderIds)
                    {
                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = item,
                            StatusId = statusCustGeneratePin,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                    }
                    db.SaveChanges();

                    responseStatus.Data = new { pin = pinNo, contactNo = req.PhoneNo };
                    responseStatus.Message = $"A verification code has been sent to {req.PhoneNo}. Standard rates apply";

                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processing the request"
                    };
                    SpiritUtility.AppLogging($"Api_UserGeneratePinPackerSubmit: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }

        }

        [HttpPost]
        [Route("verifypin")]
        [AllowAnonymous]
        public IHttpActionResult UserVerifyPin(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    string uId = User.Identity.GetUserId();
                    var delEamil = db.AspNetUsers.Where(x => x.Id == uId).FirstOrDefault().Email;
                    db.Configuration.ProxyCreationEnabled = false;
                    var otp = db.CustomerPayPins.Where(o => string.Compare(o.Email, delEamil, true)==0 && o.OrderId==req.OrderId && o.IsDeleted==false)?.FirstOrDefault();
                    if (otp == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "There is no verification pin.";
                    }
                    else
                    {
                        if (string.Compare(req.Pin, otp.Pin) == 0)
                        {
                            otp.VerifiedDate = DateTime.Now;
                            otp.IsDeleted = true;
                            db.SaveChanges();
                            responseStatus.Status = true;
                            responseStatus.Message = "Verification Successful.";

                        }
                        else
                        {
                            responseStatus.Status = false;
                            responseStatus.Message = "Verification pin does not match.";
                        }
                    }

                    int statusDelPinVerified = (int)OrderStatusEnum.DelPinVerified;
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = req.OrderId,
                        StatusId = statusDelPinVerified,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_UserVerifyPin: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }

        }

        [HttpPost]
        [Route("verifypinbyshop")]
        [AllowAnonymous]
        public IHttpActionResult UserVerifyPinByShop(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var pinNo = SpiritUtility.GenerateToken4D();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var rcust = db.AspNetUsers.Where(o => string.Compare(o.Email, req.Email, true) == 0)?.FirstOrDefault();
                    var otp = db.CustomerPayPins.Where(o => o.UserId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "There is no verification pin.";
                    }
                    else
                    {
                        if (string.Compare(req.Pin, otp.Pin) == 0)
                        {
                            otp.VerifiedDate = DateTime.Now;
                            otp.IsDeleted = true;
                            db.SaveChanges();
                            responseStatus.Status = true;
                            responseStatus.Message = "Verification Successful.";

                        }
                        else
                        {
                            responseStatus.Status = false;
                            responseStatus.Message = "Verification pin does not match.";
                        }
                    }
                    if (responseStatus.Status == true)
                    {
                        int statusCustGeneratePin = (int)OrderStatusEnum.ShopPinVerified;
                        var ema = ConfigurationManager.AppSettings["TrackEmail"];
                        var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                        foreach (var item in req.OrderIds.Distinct())
                        {
                            OrderTrack orderTrack = new OrderTrack
                            {
                                LogUserName = u.Email,
                                LogUserId = u.Id,
                                OrderId = item,
                                StatusId = statusCustGeneratePin,
                                TrackDate = DateTime.Now
                            };
                            db.OrderTracks.Add(orderTrack);
                            var ord = db.Orders.Where(x => x.Id == item).FirstOrDefault();
                            if (ord.OrderStatusId == 27)
                            {
                                WebEngageController webEngageController = new WebEngageController();
                                Task.Run(async () => await webEngageController.WebEngageStatusCall(item, "Order Return Completed"));

                            }
                        }
                        db.SaveChanges();

                    }

                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processing the request"
                    };
                    SpiritUtility.AppLogging($"Api_UserGeneratePinPackerSubmit: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }

        }


        [HttpPost]
        [Route("generatepin-packer-submit")]
        [AllowAnonymous]
        public IHttpActionResult UserGeneratePinPackerSubmit(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            var pinNo = SpiritUtility.GenerateToken4D();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var rcust = db.AspNetUsers.Where(o => string.Compare(o.Email, req.Email, true) == 0)?.FirstOrDefault();
                    var otp = db.CustomerPayPins.Where(o => o.UserId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        CustomerPayPin customerPayPin = new CustomerPayPin
                        {
                            ContactNo = rcust.PhoneNumber,
                            CreatedDate = DateTime.Now,
                            UserId = rcust.Id,
                            Email = req.Email,
                            Pin = pinNo,
                            IsDeleted = false
                        };
                        db.CustomerPayPins.Add(customerPayPin);
                        db.SaveChanges();
                    }
                    else
                    {
                        var pno = string.IsNullOrWhiteSpace(rcust.PhoneNumber) ? "" : rcust.PhoneNumber;
                        var _pinno = new SqlParameter { ParameterName = "_pinno", Value = pinNo };
                        var _phoneNo = new SqlParameter { ParameterName = "_phoneNo", Value = pno };
                        var _userId = new SqlParameter { ParameterName = "_UserId", Value = rcust.Id };
                        var _email = new SqlParameter { ParameterName = "_Email", Value = req.Email };


                        SqlParameter returnCode = new SqlParameter("@Result", SqlDbType.VarChar, 200);
                        returnCode.Direction = ParameterDirection.Output;

                        db.Database.ExecuteSqlCommand("CustomerPayPin_InsUpd @_pinno,@_phoneNo,@_UserId,@_Email,@Result out", _pinno, _phoneNo, _userId, _email, returnCode);
                        string Status = Convert.ToString(returnCode.Value);
                        if (Status != null)
                        {
                            pinNo = Status;
                        }
                    }

                    int statusCustGeneratePin = (int)OrderStatusEnum.CustGeneratePin;
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    foreach (var item in req.OrderIds)
                    {
                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = item,
                            StatusId = statusCustGeneratePin,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);
                    }
                    db.SaveChanges();

                    responseStatus.Data = new { pin = pinNo, contactNo = req.PhoneNo };
                    responseStatus.Message = $"A verification code has been sent to {req.PhoneNo}. Standard rates apply";

                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {
                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processing the request"
                    };
                    SpiritUtility.AppLogging($"Api_UserGeneratePinPackerSubmit: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }

            }
        }

        [HttpPost]
        [Route("verifypin-packer-submit")]
        [AllowAnonymous]
        public IHttpActionResult UserVerifyPinpackersubmit(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    //var u = db.AspNetUsers.Where(o => o.Email == req.Email)?.FirstOrDefault();
                    //string custId = u.Id;
                    db.Configuration.ProxyCreationEnabled = false;
                    var rcust = db.AspNetUsers.Where(o => string.Compare(o.Email, req.Email, true) == 0)?.FirstOrDefault();
                    // var rcust = db.Customers.Where(o => string.Compare(o.UserId, custId, true) == 0)?.FirstOrDefault();
                    var otp = db.CustomerPayPins.Where(o => o.UserId == rcust.Id && (!o.IsDeleted ?? false))?.FirstOrDefault();
                    if (otp == null)
                    {
                        responseStatus.Status = false;
                        responseStatus.Message = "There is no verification pin.";
                    }
                    else
                    {
                        if (string.Compare(req.Pin, otp.Pin) == 0)
                        {
                            otp.VerifiedDate = DateTime.Now;
                            otp.IsDeleted = true;
                            db.SaveChanges();
                            responseStatus.Status = true;
                            responseStatus.Message = "Verification Successful.";

                        }
                        else
                        {
                            responseStatus.Status = false;
                            responseStatus.Message = "Verification pin does not match.";
                        }
                    }

                    int statusDelPinVerified = (int)OrderStatusEnum.DelPinVerified;
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    foreach (var item in req.OrderIds)
                    {
                        OrderTrack orderTrack = new OrderTrack
                        {
                            LogUserName = u.Email,
                            LogUserId = u.Id,
                            OrderId = item,
                            StatusId = statusDelPinVerified,
                            TrackDate = DateTime.Now
                        };
                        db.OrderTracks.Add(orderTrack);

                    }
                    db.SaveChanges();

                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_UserVerifyPinpackersubmit: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
            }

        }

        [HttpPost]
        [Route("online-on-delivery")]
        public IHttpActionResult OnlineOnDelivery(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    int stCod = (int)OrderPaymentType.OOD;

                    db.Configuration.ProxyCreationEnabled = false;

                    var routeOrder = db.RoutePlans.Include(o => o.Order).Where(o => o.OrderID == req.OrderId)?.FirstOrDefault();
                    var order = db.Orders.Find(req.OrderId);

                    var deliveryPayment = db.DeliveryPayments.Where(o => o.OrderId == req.OrderId)?.FirstOrDefault();

                    if (deliveryPayment != null) {
                        deliveryPayment.PaymentTypeId = stCod;
                        deliveryPayment.CreatedDate = DateTime.Now;
                        deliveryPayment.AmountPaid = Convert.ToDouble(order.OrderAmount);
                    }
                    else
                    {
                        deliveryPayment = new DeliveryPayment
                        {
                            AmountPaid = Convert.ToDouble(order.OrderAmount),
                            CreatedDate = DateTime.Now,
                            DeliveryAgentId = routeOrder.DeliveryAgentId,
                            DelPaymentConfirm = true,
                            JobId = routeOrder.JobId,
                            OrderId = req.OrderId,
                            ShopId = order.ShopID,
                            PaymentTypeId = stCod,
                            ShopAcknowledgement = false
                        };
                        db.DeliveryPayments.Add(deliveryPayment);
                    }
                    db.SaveChanges();

                    int stPODOnlineSelected = (int)OrderStatusEnum.PODOnlineSelected;

                    order.OrderStatusId = stPODOnlineSelected;
                    db.SaveChanges();

                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = stPODOnlineSelected,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    var oSerializeAgent = JsonConvert.SerializeObject(deliveryPayment, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryPayment>(oSerializeAgent);

                    responseStatus.Data = deSerializeAgent;
                    responseStatus.Status = true;
                    responseStatus.Message = "Order marked as OOD.";
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_OnlineOnDelivery: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        [HttpPost]
        [Route("cash-on-delivery")]
        public IHttpActionResult CashOnDelivery(UserGenerateToken_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    int stCod = (int)OrderPaymentType.COD;

                    db.Configuration.ProxyCreationEnabled = false;

                    var routeOrder = db.RoutePlans.Include(o => o.Order).Where(o => o.OrderID == req.OrderId)?.FirstOrDefault();
                    var order = db.Orders.Find(req.OrderId);


                    var deliveryPayment = db.DeliveryPayments.Where(o => o.OrderId == req.OrderId)?.FirstOrDefault();

                    if (deliveryPayment != null)
                    {
                        deliveryPayment.PaymentTypeId = stCod;
                        deliveryPayment.CreatedDate = DateTime.Now;
                        deliveryPayment.AmountPaid = Convert.ToDouble(order.OrderAmount);
                    }
                    else
                    {
                        deliveryPayment = new DeliveryPayment
                        {
                            AmountPaid = Convert.ToDouble(order.OrderAmount),
                            CreatedDate = DateTime.Now,
                            DeliveryAgentId = routeOrder.DeliveryAgentId,
                            DelPaymentConfirm = false,
                            JobId = routeOrder.JobId,
                            OrderId = req.OrderId,
                            ShopId = order.ShopID,
                            PaymentTypeId = stCod,
                            ShopAcknowledgement = false
                        };
                        db.DeliveryPayments.Add(deliveryPayment);
                    }
                    db.SaveChanges();

                    int stPODCashSelected = (int)OrderStatusEnum.PODCashSelected;

                    order.OrderStatusId = stPODCashSelected;
                    db.SaveChanges();

                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = stPODCashSelected,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();


                    var oSerializeAgent = JsonConvert.SerializeObject(deliveryPayment, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryPayment>(oSerializeAgent);

                    responseStatus.Data = deSerializeAgent;
                    responseStatus.Status = true;
                    responseStatus.Message = "Order marked as COD.";
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_CashOnDelivery: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        [HttpPost]
        [Route("cash-on-delivery-confirm")]
        public IHttpActionResult CashOnDeliveryCofirm(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;


                    int payType = (int)OrderPaymentType.OCOD;
                    int stOCODCashPaid = (int)OrderStatusEnum.OCODCashPaid;
                    int stPODCashPaid = (int)OrderStatusEnum.PODCashPaid;
                    int stApproved= (int)OrderStatusEnum.Approved;
                    var order = db.Orders.Find(orderId);
                    int trackStatus;
                    DeliveryPayment delPayment = null;
                    if (order.PaymentTypeId == payType)
                    {
                        trackStatus = stOCODCashPaid;
                        delPayment = AgentCashOrder(orderId);
                    }
                    else
                    {
                        trackStatus = stPODCashPaid;
                        delPayment = db.DeliveryPayments.Where(o => o.OrderId == orderId)?.OrderByDescending(o => o.DeliveryPaymentId).FirstOrDefault(); // db.DeliveryPayments.Find(deliverypaymentid);
                        delPayment.DelPaymentConfirm = true;
                        db.SaveChanges();

                        //var order = db.Orders.Find(delPayment.OrderId);
                        order.OrderStatusId = stPODCashPaid;
                        db.SaveChanges();
                    }
                    var ema = ConfigurationManager.AppSettings["TrackEmail"];
                    var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                    
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = stApproved,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges(); 

                    orderTrack = new OrderTrack
                    {
                        LogUserName = u.Email,
                        LogUserId = u.Id,
                        OrderId = order.Id,
                        StatusId = trackStatus,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();
                    //Create new api to update inventory
                    //PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    //paymentLinkLogsService.InventoryUpdate(order.Id);

                    var oSerializeAgent = JsonConvert.SerializeObject(delPayment, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryPayment>(oSerializeAgent);
                    responseStatus.Data = deSerializeAgent;
                    responseStatus.Status = true;
                    responseStatus.Message = "Cash collected.";
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processing the request"
                    };
                    SpiritUtility.AppLogging($"Api_CashOnDeliveryCofirm: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        private DeliveryPayment AgentCashOrder(int orderId)
        {
            int stCOODCashPaid = (int)OrderStatusEnum.OCODCashPaid;
            int stOcod = (int)OrderPaymentType.OCOD;

            var order = db.Orders.Find(orderId);
            var routeOrder = db.RoutePlans.Include(o => o.Order).Where(o => o.OrderID == orderId)?.FirstOrDefault();

            DeliveryPayment deliveryPayment = new DeliveryPayment
            {
                AmountPaid = Convert.ToDouble(order.OrderAmount),
                CreatedDate = DateTime.Now,
                DeliveryAgentId = routeOrder.DeliveryAgentId,
                DelPaymentConfirm = true,
                JobId = routeOrder.JobId,
                OrderId = order.Id,
                ShopId = order.ShopID,
                PaymentTypeId = stOcod,
                ShopAcknowledgement = false
            };
            db.DeliveryPayments.Add(deliveryPayment);
            db.SaveChanges();

            order.OrderStatusId = stCOODCashPaid;
            db.SaveChanges();

            return deliveryPayment;
        }

        [HttpPost]
        [Route("add-order")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult AddOrderByLoginUser(APIOrder model)
        {
            if (model == null)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Failed to add order. Please try again.", Data = model });
            else if (model.AddressId <= 0)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Address field cannot be blank.", Data = model });
            else if (model.OrderItems == null || model.OrderItems.Count <= 0)
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Your cart is empty", Data = model });

            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            string uId = User.Identity.GetUserId();
            decimal intconfigAmtTotal = Convert.ToDecimal(ConfigurationManager.AppSettings["MinTotalAmtOrd"]);

            var inventItem = UserCustomerV2Controller.InventoryCheck(model.OrderItems);
            if (inventItem.TotalAmount < intconfigAmtTotal)
            {
                return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = $"Minimum order amount is {intconfigAmtTotal} rs." });
            }
            if (inventItem.OrderItems.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, new ResponseStatus { Status = false, Message = "Inventory issue.", ErrorType = "inventory", Data = new { orderId = 0, rejectedItems = inventItem } });
            }

            using (var db = new rainbowwineEntities())
            {
                int custAdress = model.AddressId;
                var aspUser = db.AspNetUsers.Where(o => o.Id == uId)?.FirstOrDefault();
                Customer customer = db.Customers.Where(o => o.UserId == uId)?.FirstOrDefault();
                string userName = aspUser.Email;
                CustomerAddress customerAddress = db.CustomerAddresses.Where(o => o.CustomerId == customer.Id && o.CustomerAddressId == custAdress)?.FirstOrDefault();

                bool operationFlag = db.WineShops.Find(customerAddress.ShopId)?.OperationFlag ?? false;

                if (!operationFlag)
                    return Content(HttpStatusCode.InternalServerError, new ResponseStatus { Status = false, Message = "Unfortunately, the selected shop is not operating at the moment. Please choose another shop.", Data = model });

                bool testorder = (ConfigurationManager.AppSettings["TestOrder"] == "1") ? true : false;

                db.Configuration.ProxyCreationEnabled = false;
                Order order = null;
                OrderTrack orderTrack = null;
                //using (var transaction = db.Database.BeginTransaction())
                //{
                try
                {
                    order = new Order
                    {
                        OrderDate = DateTime.Now,
                        CustomerId = customer.Id,
                        OrderAmount = 0,
                        OrderPlacedBy = userName,
                        OrderTo = customer.ContactNo,
                        OrderStatusId = 1,
                        ShopID = customerAddress.ShopId,
                        DeliveryPickup = "Delivery",
                        PaymentDevice = "Android",
                        TestOrder = testorder,
                        ZoneId = 0,
                        LicPermitNo = model.PremitNo,
                        CustomerAddressId = model.AddressId,
                        OrderType = "m",
                        PaymentTypeId = model.PaymentTypeId
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();
                    //OrderTrackingLog(order.Id, uId, User.Identity.Name, 1);
                    orderTrack = new OrderTrack
                    {
                        LogUserName = userName,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = 1,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    OrderDetail orderDetail = null;
                    decimal totalAmt = 0;
                    foreach (var item in model.OrderItems)
                    {
                        orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ItemQty = item.Qty,
                            ProductID = item.ProductId,
                            Price = item.Price,
                            ShopID = item.ShopID
                        };
                        db.OrderDetails.Add(orderDetail);
                        int q = item.Qty;
                        decimal p = item.Price;
                        decimal t = q * p;
                        totalAmt += t;
                    }
                    db.SaveChanges();
                    //OrderTrackingLog(order.Id, uId, User.Identity.Name, 2);
                    int configPremitValue = Convert.ToInt32(ConfigurationManager.AppSettings["PremitValue"]);
                    int premitVlaue = (string.IsNullOrWhiteSpace(model.PremitNo)) ?
                        configPremitValue : 0;

                    order.OrderAmount = totalAmt + premitVlaue;
                    order.OrderStatusId = 2;
                    orderTrack = new OrderTrack
                    {
                        LogUserName = userName,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = 2,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    //transaction.Commit();
                }
                catch (Exception ex)
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Your order hasn’t been placed yet. Please try again";

                    db.Dispose();
                    SpiritUtility.AppLogging($"Api_add-order_Transaction: {ex.Message}", ex.StackTrace);
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
                if (order != null && order.Id > 0)
                {
                    try
                    {
                        responseStatus.Data = new { orderId = order.Id, rejectedItems = new List<APIOrderDetails>() };

                        //Update orderid to Customer ETA 
                        var orderETAobject = db.CustomerEtas.Where(i => i.CustomerId == customer.Id);
                        if (orderETAobject != null && orderETAobject.Count() > 0)
                        {
                            var orderETA = orderETAobject.OrderByDescending(j => j.Id).Take(1).FirstOrDefault();
                            orderETA.OrderId = order.Id;
                            db.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {
                        SpiritUtility.AppLogging($"Api_add-order_SMS-Payment: {ex.Message}", ex.StackTrace);

                        //responseStatus.Status = false;
                        //responseStatus.Message = $"{ex.Message}";
                    }
                    //OrderTrackingLog(order.Id, uId, User.Identity.Name, 17);
                    orderTrack = new OrderTrack
                    {
                        LogUserName = userName,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = 35,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    responseStatus.Status = true;
                    responseStatus.Message = "Your order has been placed successfully.";

                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    Task.Run(async()=>await fcmHelper.SendFirebaseNotification(order.Id, FirebaseNotificationHelper.NotificationType.Order));
                }
                else
                {
                    responseStatus.Status = false;
                    responseStatus.Message = "Your order hasn’t been placed yet. Please try again.";
                }
            }

            //}
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("delivery-ordertotal")]
        public IHttpActionResult DeliveryCount()
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {

                    string uId = User.Identity.GetUserId();
                    var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();

                    RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                    var d = routePlanDBO.RoutePlansDeliveryCount(ruser.DeliveryAgentId ?? 0);

                    responseStatus.Data = d;
                    responseStatus.Status = true;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_DeliveryCount: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        [HttpPost]
        [Route("delivery-ordercount")]
        public IHttpActionResult DeliveryCount2()
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {

                    string uId = User.Identity.GetUserId();
                    var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();

                    RoutePlanDBO routePlanDBO = new RoutePlanDBO();
                    var d = routePlanDBO.RoutePlansDeliveryCount(ruser.DeliveryAgentId ?? 0, uId);

                    responseStatus.Data = d;
                    responseStatus.Status = true;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_DeliveryCount: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        [HttpPost]
        [Route("delivery-allbacktostore")]
        public IHttpActionResult DeliveryBacktoStore(UserManageFliter_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {

                    string uId = User.Identity.GetUserId();
                    db.Configuration.ProxyCreationEnabled = false;
                    var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();

                    var d = db.DeliveryBackToStores.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId);

                    var yr = req.CreatedDate.Year;
                    if (yr > 1999)
                    {
                        var mth = req.CreatedDate.Month;
                        var day = req.CreatedDate.Day;
                        var d2 = d.Where(o => o.CreatedDate.Value.Year == yr && o.CreatedDate.Value.Month == mth && o.CreatedDate.Value.Day == day);
                        d = d2;
                    }
                    var oSerialize = JsonConvert.SerializeObject(d, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerialize = JsonConvert.DeserializeObject<List<DeliveryBackToStore>>(oSerialize);
                    responseStatus.Data = deSerialize;
                    responseStatus.Status = true;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        [HttpPost]
        [Route("delivery-allcash")]
        [AllowAnonymous]
        public IHttpActionResult DeliveryCash(UserManageFliter_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {

                    string uId = User.Identity.GetUserId();
                    db.Configuration.ProxyCreationEnabled = false;
                    var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();

                    var d = db.DeliveryPayments.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId);

                    var yr = req.CreatedDate.Year;
                    if (yr > 1999)
                    {
                        var mth = req.CreatedDate.Month;
                        var day = req.CreatedDate.Day;
                        var d2 = d.Where(o => o.CreatedDate.Value.Year == yr && o.CreatedDate.Value.Month == mth && o.CreatedDate.Value.Day == day);
                        d = d2;
                    }

                    var oSerialize = JsonConvert.SerializeObject(d, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerialize = JsonConvert.DeserializeObject<List<DeliveryPayment>>(oSerialize);
                    responseStatus.Data = deSerialize;
                    responseStatus.Status = true;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        [HttpPost]
        [Route("temp/delivery-handover")]
        public IHttpActionResult DeliveryHandover()
        {
            ResponseStatus responseStatus = new ResponseStatus();



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    string uId = User.Identity.GetUserId();
                    db.Configuration.ProxyCreationEnabled = false;
                    var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();
                    var delagent = db.DeliveryAgents.Include(o => o.WineShop).Where(o => o.Id == ruser.DeliveryAgentId).FirstOrDefault();
                    var aspUSer = db.AspNetUsers.Find(uId);



                    OrderDBO orderDBO = new OrderDBO();
                    var delpay = orderDBO.GetDeliveryPaymentDetails(ruser.DeliveryAgentId ?? 0);
                    var delback = orderDBO.GetDeliveryBacktoStoreDetails(ruser.DeliveryAgentId ?? 0);
                    //var delpay = db.DeliveryPayments.Include(o => o.Order).Include(o => o.Order.OrderDetails).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.ShopAcknowledgement!=true);
                    //var delback = db.DeliveryBackToStores.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.ShopAcknowledgement != true);



                    var oSerializeAgent = JsonConvert.SerializeObject(delagent, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryAgent>(oSerializeAgent);



                    //var oSerialize = JsonConvert.SerializeObject(delpay, Formatting.None,
                    //       new JsonSerializerSettings()
                    //       {
                    //           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //       });
                    //var deSerialize = JsonConvert.DeserializeObject<List<DeliveryPayment>>(oSerialize);



                    //var oSerializeBack = JsonConvert.SerializeObject(delback, Formatting.None,
                    //       new JsonSerializerSettings()
                    //       {
                    //           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //       });
                    //var deSerializeBack = JsonConvert.DeserializeObject<List<DeliveryBackToStore>>(oSerializeBack);



                    responseStatus.Data = new { cash = delpay, backtostore = delback, user = new { email = aspUSer.Email, agent = deSerializeAgent } };
                    responseStatus.Status = true;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {



                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);



                }
                finally
                {
                    db.Dispose();
                }
            }



        }

        [HttpPost]
        [Route("delivery-handover")]
        public IHttpActionResult DeliveryHandoverNew()
        {
            ResponseStatus responseStatus = new ResponseStatus();



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    string uId = User.Identity.GetUserId();
                    db.Configuration.ProxyCreationEnabled = false;
                    var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();
                    var delagent = db.DeliveryAgents.Include(o => o.WineShop).Where(o => o.Id == ruser.DeliveryAgentId).FirstOrDefault();
                    var aspUSer = db.AspNetUsers.Find(uId);



                    OrderDBO orderDBO = new OrderDBO();
                    var delpay = orderDBO.GetDeliveryPaymentDetails(ruser.DeliveryAgentId ?? 0);
                    var delback = orderDBO.GetDeliveryBacktoStoreDetailsNew(ruser.DeliveryAgentId ?? 0);
                    //var delpay = db.DeliveryPayments.Include(o => o.Order).Include(o => o.Order.OrderDetails).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.ShopAcknowledgement!=true);
                    //var delback = db.DeliveryBackToStores.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.ShopAcknowledgement != true);



                    var oSerializeAgent = JsonConvert.SerializeObject(delagent, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                    var deSerializeAgent = JsonConvert.DeserializeObject<DeliveryAgent>(oSerializeAgent);



                    //var oSerialize = JsonConvert.SerializeObject(delpay, Formatting.None,
                    //       new JsonSerializerSettings()
                    //       {
                    //           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //       });
                    //var deSerialize = JsonConvert.DeserializeObject<List<DeliveryPayment>>(oSerialize);



                    //var oSerializeBack = JsonConvert.SerializeObject(delback, Formatting.None,
                    //       new JsonSerializerSettings()
                    //       {
                    //           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //       });
                    //var deSerializeBack = JsonConvert.DeserializeObject<List<DeliveryBackToStore>>(oSerializeBack);







                    responseStatus.Data = new { cash = delpay, backtostore = delback, user = new { email = aspUSer.Email, agent = deSerializeAgent } };
                    responseStatus.Status = true;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {



                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);



                }
                finally
                {
                    db.Dispose();
                }
            }



        }

        [HttpPost]
        [Route("delivery-handover-comfirm/{jobId}")]
        public IHttpActionResult DeliveryHandoverConfirm(string jobId)
        {
            ResponseStatus responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {

                    string uId = User.Identity.GetUserId();
                    db.Configuration.ProxyCreationEnabled = false;
                    var ruser = db.RUsers.Where(o => o.rUserId == uId).FirstOrDefault();
                   
                    var delpay = db.DeliveryPayments.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.JobId== jobId && o.ShopAcknowledgement != true);
                    var delback = db.DeliveryBackToStores.Include(o => o.Order).Include(o => o.Order.RoutePlans).Where(o => o.DeliveryAgentId == ruser.DeliveryAgentId && o.JobId == jobId && o.ShopAcknowledgement != true);
                    bool packerConfirm = (delpay.Count() == 0 && delback.Count() == 0);

                    responseStatus.Data = new { packerConfirm = packerConfirm };
                    responseStatus.Status = true;
                    return Ok(responseStatus);
                }
                catch (Exception ex)
                {

                    responseStatus = new ResponseStatus()
                    {
                        Data = null,
                        Status = true,
                        Message = "Some error occurred while processign the request"
                    };
                    SpiritUtility.AppLogging($"Api_DeliveryBacktoStore: {ex.Message}", ex.StackTrace);
                    db.Dispose();
                    return Content(HttpStatusCode.InternalServerError, responseStatus);

                }
                finally
                {
                    db.Dispose();
                }
            }

        }

        [HttpPost]
        [Route("order-status/{orderId}")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult GetOrderStatus(int orderId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            //db.Configuration.ProxyCreationEnabled = false;
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var track = db.OrderTracks.Include(o => o.OrderStatu).Where(o => o.OrderId == orderId).ToList();
                var order = db.Orders.Include(o => o.OrderStatu).Where(o => o.Id == orderId).FirstOrDefault();
                var oSerialize = JsonConvert.SerializeObject(track, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                var deSerialize = JsonConvert.DeserializeObject<List<OrderTrack>>(oSerialize);
                var oSerializeorder = JsonConvert.SerializeObject(order, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                var deSerializeorder = JsonConvert.DeserializeObject<Order>(oSerializeorder);
                responseStatus.Data = new { oTrack = deSerialize, order = deSerializeorder };
            }
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("get-order-cancel-reasons")]
        public IHttpActionResult CancelResonList()
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    var cancelOption = db.CancelOptions.ToList();

                    responseStatus.Status = true;
                    responseStatus.Data = cancelOption;
                    responseStatus.Message = "Cancel Reasons.";
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
                finally
                {
                    db.Dispose();
                }
            }
            return Ok(responseStatus);
        }
        
        [HttpPost]
        [Route("order-cancel")]
        public IHttpActionResult OrderCancelByCustomer(BackToStoreStatus backToStoreStatus)
        {
            if (backToStoreStatus.OrderId == 0)
            {
                return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    int orderId = backToStoreStatus.OrderId;
                    string remark = backToStoreStatus.Remark;
                    string uId = User.Identity.GetUserId();
                    var aspUser = db.AspNetUsers.Find(uId);
                    Order order = db.Orders.Find(orderId);

                    int orderstatus = (int)OrderStatusEnum.OutForDelivery;
                    int orstReached = (int)OrderStatusEnum.DeliveryReached;
                    bool isOutForDelivery = (order.OrderStatusId == orderstatus || order.OrderStatusId== orstReached) ? true : false;
                    if (order == null)
                    {
                        db.Dispose();
                        return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = $"There is no such OrderID {orderId}." });
                    }

                    db.Configuration.ProxyCreationEnabled = false;

                    int custCancel = (int)OrderStatusEnum.CancelByCustomer;
                    order.OrderStatusId = custCancel;
                    order.DeliveryDate = DateTime.Now;
                    db.SaveChanges();
                    OrderDBO orderDBO = new OrderDBO();
                   
                    if (order.OrderStatusId == 5)
                    {

                        //HyperTracking Complted
                        HyperTracking hyperTracking = new HyperTracking();
                        Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(order.Id));

                        //Live Tracking FireStore
                        CustomerApi2Controller.DeleteToFireStore(order.Id);

                        orderDBO.UpdatedScheduledOrder(order.Id);

                       
                    }

                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = aspUser.Email,//User.Identity.Name,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = order.OrderStatusId,
                        TrackDate = DateTime.Now,
                        Remark = remark
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    //inventory revert
                    //PaymentLinkLogsService serv = new PaymentLinkLogsService();
                    //serv.RevertInventory(order.Id);

                    //Add order into back to store if Delivery boy out for delivery
                    if(isOutForDelivery)
                    {
                        //inventory revert
                        PaymentLinkLogsService serv = new PaymentLinkLogsService();
                        serv.RevertInventory(order.Id);
                        serv.RevertMixerInventory(order.Id);

                        //Added for Back to store for packer
                        var routeOrder = db.RoutePlans.Include(o => o.Order).Where(o => o.OrderID == orderId)?.FirstOrDefault();
                        DeliveryBackToStore delBackToStore = new DeliveryBackToStore
                        {
                            OrderAmount = Convert.ToDouble(order.OrderAmount),
                            CreatedDate = DateTime.Now,
                            DeliveryAgentId = routeOrder.DeliveryAgentId,
                            Reason = $"(ByCustomer) - {remark}",
                            JobId = routeOrder.JobId,
                            OrderId = order.Id,
                            ShopId = order.ShopID,
                            ShopAcknowledgement = false
                        };
                        db.DeliveryBackToStores.Add(delBackToStore);
                        db.SaveChanges();

                    }
                    else
                    {
                        PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                        paymentLinkLogsService.RevertInventory(order.Id);
                        paymentLinkLogsService.RevertMixerInventory(order.Id);
                    }

                    responseStatus.Status = true;
                    responseStatus.Message = $"Order cancel successfully.";
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

            return Ok(responseStatus);
        }


        [HttpPost]
        [Route("delivery/order-delivered")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult DeliveryOrderPlaced(int orderId)
        {
            if (orderId == 0)
            {
                return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = "OrderId is null;" });
            }
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                try
                {
                    string uId = User.Identity.GetUserId();
                    var aspUser = db.AspNetUsers.Find(uId);
                    Order order = db.Orders.Find(orderId);
                    if (order == null)
                    {
                        db.Dispose();
                        return Content(HttpStatusCode.NotFound, new ResponseStatus { Status = false, Message = $"This is no such OrderID {orderId}." });
                    }

                    db.Configuration.ProxyCreationEnabled = false;

                    var statusCancel = (int)OrderStatusEnum.CancelByCustomer;
                    if (order.OrderStatusId == statusCancel)
                    {
                        responseStatus.Status = true;
                        responseStatus.Message = "Order cancel by customer.";
                        return Ok(responseStatus);
                    }
                    
                    order.OrderStatusId = 5;
                    order.DeliveryDate = DateTime.Now;
                    db.SaveChanges();

                    OrderDBO orderDBO = new OrderDBO();
                    
                    if (order.OrderStatusId == 5)
                    {
                        //HyperTracking Complted
                        HyperTracking hyperTracking = new HyperTracking();
                        Task.Run(async () => await hyperTracking.HyperTrackTripCompleted(order.Id));


                        //Live Tracking FireStore
                        CustomerApi2Controller.DeleteToFireStore(order.Id);
                        orderDBO.UpdatedScheduledOrder(order.Id);

                    }
                    OrderTrack orderTrack = new OrderTrack
                    {
                        LogUserName = aspUser.Email,//User.Identity.Name,
                        LogUserId = uId,
                        OrderId = order.Id,
                        StatusId = order.OrderStatusId,
                        TrackDate = DateTime.Now
                    };
                    db.OrderTracks.Add(orderTrack);
                    db.SaveChanges();

                    FirebaseNotificationHelper fcmHelper = new FirebaseNotificationHelper();
                    int pageId = (int)PageNameEnum.MYWALLET;
                    string pageVersion = "1.6.2";
                    var cont = SZIoc.GetSerivce<IPageService>();
                    var content = cont.GetPageContent(pageId, pageVersion);
                    var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
                    var c = SZIoc.GetSerivce<ISZConfiguration>();
                    var numberOfOrder = c.GetConfigValue(ConfigEnums.NumberOfOrder.ToString());
                    var ordCount = db.Orders.Where(o => o.CustomerId == order.CustomerId && o.OrderStatusId == 5);
                    bool isFirsrOrder = true;
                    if (ordCount.Count() > Convert.ToInt32(numberOfOrder))
                    {
                        isFirsrOrder = false;

                    }
                    var ser = SZIoc.GetSerivce<IPromoCodeService>();
                    if (isFirsrOrder)
                    {
                        var discount = ser.GetDiscountOnFirstOrder(order.Customer.UserId);
                        if (discount.CreditAmount > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = discount.CreditAmount + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = discount.CreditAmount + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = discount.CustomerId;
                            walletNotificationRequest.UserID = discount.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }

                    var promocode = db.PromoCodes.Where(o => o.PromoId == order.PromoId).FirstOrDefault();

                    if (order.PromoId.HasValue && order.PromoId > 0)
                    {
                        var promoCashback = ser.PromoCodeCashBack(order.Id);
                        if (promoCashback > 0)
                        {
                            WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                            walletNotificationRequest.Title = promoCashback + " " + conCart[PageContentEnum.Text28.ToString()];
                            walletNotificationRequest.Message = promoCashback + " " + conCart[PageContentEnum.Text29.ToString()];
                            walletNotificationRequest.CustomerID = order.CustomerId;
                            walletNotificationRequest.UserID = order.Customer.UserId;
                            Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                        }
                    }
                    var Cashback = ser.CashBack(order.Id);
                    if (Cashback.CashBackAmount > 0)
                    {
                        WalletNotificationRequest walletNotificationRequest = new WalletNotificationRequest();
                        walletNotificationRequest.Title = Cashback.CashBackAmount + " " + conCart[PageContentEnum.Text28.ToString()];
                        walletNotificationRequest.Message = Cashback.CashBackAmount + " " + conCart[PageContentEnum.Text29.ToString()];
                        walletNotificationRequest.CustomerID = order.CustomerId;
                        walletNotificationRequest.UserID = order.Customer.UserId;
                        Task.Run(async () => await fcmHelper.SendFirebaseNotificationForWallet(walletNotificationRequest, FirebaseNotificationHelper.NotificationType.Wallet));
                    }

                    //WSendSMS wsms = new WSendSMS();
                    //string textmsg = string.Format(ConfigurationManager.AppSettings["SMSDelivered"], order.Id.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                    //wsms.SendMessage(textmsg, order.Customer.ContactNo);

                    //Flow SMS
                    var dicti = new Dictionary<string, string>();
                    dicti.Add("ORDERID", order.Id.ToString());
                    dicti.Add("DATETIME", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));

                    var templeteid = ConfigurationManager.AppSettings["SMSDeliveredFlowId"];

                    Task.Run(async () => await Services.Msg91.Msg91Service.SendFlowSms(templeteid, order.Customer.ContactNo, dicti));
                    //End Flow

                    Task.Run(async () => await fcmHelper.SendFirebaseNotification(orderId, FirebaseNotificationHelper.NotificationType.Order));
                }
                catch (Exception ex)
                {
                    db.Dispose();

                    SpiritUtility.AppLogging($"Api_DeliveryOrderPlaced: {ex.Message}", ex.StackTrace);
                    responseStatus.Status = false;
                    responseStatus.Message = $"{ex.Message}";
                    return Content(HttpStatusCode.InternalServerError, responseStatus);
                }
            }

            return Ok(responseStatus);
        }


        [HttpPost]
        [Route("delivery/order-search")]
        [Authorize(Roles = "Deliver")]
        public IHttpActionResult SearchOrder(NotifyOrderDelivery_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            string uId = User.Identity.GetUserId();
            var ruser = db.RUsers.Where(o => o.rUserId == uId)?.FirstOrDefault();
            OrderDBO orderDBO = new OrderDBO();
            var ord = orderDBO.GetDeliveryAgentOrder(req.OrderId ?? 0, ruser.DeliveryAgentId ?? 0);
            if (ord == null)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order not found.";

                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            responseStatus.Data = ord;
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("inventory-update")]
        [Authorize(Roles = "Customer")]
        public IHttpActionResult ImventoryUpdated(NotifyOrderDelivery_Req req)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            var ord = db.Orders.Find(req.OrderId);
            if (ord == null)
            {
                responseStatus.Status = false;
                responseStatus.Message = "Order not found.";

                return Content(HttpStatusCode.NotFound, responseStatus);
            }
            PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
            paymentLinkLogsService.InventoryUpdate(ord.Id);

            //responseStatus.Data = ord;
            responseStatus.Status = true;
            responseStatus.Message = "Inventory updated successfully.";
            return Ok(responseStatus);
        }

        [HttpPost]
        [Route("checkmobileregister/{phoneNo}")]
        public IHttpActionResult CheckMobileREgister(string phoneNo)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };

            if (string.IsNullOrWhiteSpace(phoneNo))
            {
                responseStatus.Status = false;
                responseStatus.Message = "Invalid mobile number.";

                return Content(HttpStatusCode.BadRequest, responseStatus);
            }

            var cust = db.Customers.Where(o => string.Compare(o.ContactNo, phoneNo, true) == 0 && string.Compare(o.RegisterSource, "m", true) == 0)?.Select(o => new { Id = o.Id, CustomerName=o.CustomerName, ContactNo=o.ContactNo }).FirstOrDefault();
            Customer deSerialize = null;
            if (cust != null)
            {
                var oSerialize = JsonConvert.SerializeObject(cust, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                 deSerialize = JsonConvert.DeserializeObject<Customer>(oSerialize);
            }
            responseStatus.Data = deSerialize;
            responseStatus.Status = true;
            responseStatus.Message = (cust == null) ?"Mobile number does not exists.": "Mobile number already exists";
            return Ok(responseStatus);
        }

        [HttpGet]
        [Route("product/recommended/{shopId}")]
        [Authorize(Roles = "Customer")]
        public HttpResponseMessage GetRecommendedProduct(int shopId)
        {
            ResponseStatus responseStatus = new ResponseStatus { Status = true };
            using (var db = new rainbowwineEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var uID = User.Identity.GetUserId();
                Customer customer = db.Customers.Where(o => o.UserId == uID)?.FirstOrDefault();

                ProductDBO productDBO = new ProductDBO();
                var recom = productDBO.GetRecomProductDetailsByCust(shopId, customer.Id);
                responseStatus.Data = recom.ToList();
            }
            return Request.CreateResponse(HttpStatusCode.OK, responseStatus);
        }

    }
}