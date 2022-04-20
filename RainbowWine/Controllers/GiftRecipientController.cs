using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Providers;
using RainbowWine.Services.DBO;
using RainbowWine.Services.DO;
using RainbowWine.Services.Msg91;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SZData.Interfaces;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace RainbowWine.Controllers
{
    public class GiftRecipientController : Controller
    {
        private rainbowwineEntities db = new rainbowwineEntities();
        ResponseStatus responseStatus = new ResponseStatus { Status = true };
        OTPService oTPService = new OTPService();
        GiftBagDBO giftBagDBO = new GiftBagDBO();
        private string _message;
        private object _data;
        private int id;
        // GET: GiftRecipient
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //[Route("GiftRecipient/OTPSend")]
        public ActionResult OTPSend(string token)
        {
            if (token ==null)
            {
                return View("OTPSendQR");
            }
           
                ViewBag.Token = token;
                return View();
            
      
        }
       
        public ActionResult OTPVerification(string mobile ,int oId =0,string token="")
        {
            ViewBag.MobileNo = mobile;
            ViewBag.Token = Encrypt(mobile);
            ViewBag.OrderId = oId;
            WebEngageController webEngageController = new WebEngageController();
            Task.Run(async () => await webEngageController.WebEngageStatusCall(oId, "Gift Contact Entered"));
            ViewBag.Recipient = giftBagDBO.GetRecipient(oId, mobile);
            var result = giftBagDBO.GetOtpCount(mobile);
            if (oId != Convert.ToInt32(Decrypt(token)))
            {
                ViewBag.Message = "Authentication Failed";
            }
            else if (result.cnt >= 3)
            {
                ViewBag.Message = "You cannot generate any more OTPs. Try again later.";
            }
            else
            {
                
                Task.Run(async () => await webEngageController.WebEngageStatusCall(oId, "Gift OTP Verified"));
            }
            return View();
        }

        public ActionResult RecipientDetail(string mobile, string token, string orderId)
        {
            int pageId = (int)PageNameEnum.GIFT;
            string pageVersion = "1.0";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var pageImages = content.PageImages.Select(o => new { Key = o.ImageKey, Value = o.ImageUrl }).ToDictionary(o => o.Key, o => o.Value);
            var birthdayImg = pageImages[PageImageEnum.BirthdayImg.ToString()];
            var anniversaryImg = pageImages[PageImageEnum.AnniversaryImg.ToString()];
            var weddingImg = pageImages[PageImageEnum.WeddingImg.ToString()];
            var defaultImg = pageImages[PageImageEnum.DefaultImg.ToString()];
            var recipient = giftBagDBO.GetUserRecipientOrderDetails(Convert.ToInt32(orderId));
            if (token == null)
            {
                token = "1234";
            }
            var decVal = Decrypt(token);
            if (decVal == mobile.Trim())
            {

                if (recipient != null)
                {
                    if (recipient.Occasion == "Anniversary")
                    {
                        recipient.GiftUrl = anniversaryImg;

                    }
                    else if (recipient.Occasion == "Birthday")
                    {
                        recipient.GiftUrl = birthdayImg;
                    }
                    else if (recipient.Occasion == "Wedding")
                    {
                        recipient.GiftUrl = weddingImg;
                    }
                    else
                    {
                        recipient.GiftUrl = defaultImg;
                    }
                    var recUpd = giftBagDBO.UpdateUserRecipientOrder(recipient.OrderId,"");
                    WebEngageController webEngageController = new WebEngageController();
                    Task.Run(async () => await webEngageController.WebEngageStatusCall(recipient.OrderId, "Gift Msg Opened"));
                }
            }
            else
            {
                recipient = null;
            }

            return View("RecipientDetails", recipient);
        }

        public ActionResult RecipientDetails(string mobile)
        {
            int pageId = (int)PageNameEnum.GIFT;
            string pageVersion = "1.0";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var pageImages = content.PageImages.Select(o => new { Key = o.ImageKey, Value = o.ImageUrl }).ToDictionary(o => o.Key, o => o.Value);
            var birthdayImg = pageImages[PageImageEnum.BirthdayImg.ToString()];
            var anniversaryImg = pageImages[PageImageEnum.AnniversaryImg.ToString()];
            var weddingImg = pageImages[PageImageEnum.WeddingImg.ToString()];
            var defaultImg = pageImages[PageImageEnum.DefaultImg.ToString()];
            var recipient = giftBagDBO.GetUserRecipientOrder(mobile);
            if (recipient != null)
            {
                if (recipient.Occasion == "Anniversary")
                {
                    recipient.GiftUrl = anniversaryImg;

                }
                else if (recipient.Occasion == "Birthday")
                {
                    recipient.GiftUrl = birthdayImg;
                }
                else if (recipient.Occasion == "Wedding")
                {
                    recipient.GiftUrl = weddingImg;
                }
                else
                {
                    recipient.GiftUrl = defaultImg;
                }
                var recUpd = giftBagDBO.UpdateUserRecipientOrder(recipient.OrderId,"");
                WebEngageController webEngageController = new WebEngageController();
                Task.Run(async () => await webEngageController.WebEngageStatusCall(recipient.OrderId, "Gift Msg Opened"));
            }

            return View(recipient);
        }

        [HttpGet]
        [Route("GiftRecipient/Gift")]
        public ActionResult Gift(int id)
        {

            int pageId = (int)PageNameEnum.GIFT;
            string pageVersion = "1.0";
            var c = SZIoc.GetSerivce<IPageService>();
            var content = c.GetPageContent(pageId, pageVersion);
            var conCart = content.PageContents.Select(o => new { Key = o.ContentKey, Value = o.ContentValue }).ToDictionary(o => o.Key, o => o.Value);
            var pageImages = content.PageImages.Select(o => new { Key = o.ImageKey, Value = o.ImageUrl }).ToDictionary(o => o.Key, o => o.Value);
            var birthdayImg = pageImages[PageImageEnum.BirthdayImg.ToString()];
            var anniversaryImg = pageImages[PageImageEnum.AnniversaryImg.ToString()];
            var weddingImg = pageImages[PageImageEnum.WeddingImg.ToString()];
            var defaultImg = pageImages[PageImageEnum.DefaultImg.ToString()];
            var recipient = giftBagDBO.GetUserRecipientOrderDetails(id);
            if (recipient != null)
            {
                if (recipient.Occasion == "Anniversary")
                {
                    recipient.GiftUrl = anniversaryImg;

                }
                else if (recipient.Occasion == "Birthday")
                {
                    recipient.GiftUrl = birthdayImg;
                }
                else if (recipient.Occasion == "Wedding")
                {
                    recipient.GiftUrl = weddingImg;
                }
                else
                {
                    recipient.GiftUrl = defaultImg;
                }
                var recUpd = giftBagDBO.UpdateUserRecipientOrder(recipient.OrderId,"");
            }


            return View("RecipientDetails", recipient);
        }
        [HttpPost]
        public JsonResult RequestForOTP(string mobile, string token)
        {
            var ordId = Decrypt(token.Trim());
            var recipient = giftBagDBO.GetUserRecipientOrderDetails(Convert.ToInt32(ordId));
            var IsRecipientExist = giftBagDBO.GetRecipient(Convert.ToInt32(ordId),mobile);
            if (IsRecipientExist == 0)
            {
                _data = false;
                _message = "We couldn't find an order against this contact number.";
            }
           else if (recipient != null && (recipient.OrderId == Convert.ToInt32(ordId)))
            {
                if (recipient != null)
                {
                    var OTP = GenerateOTP();


                    var oldOTPEntry = db.CustomerOTPVerifies.Where(x => x.ContactNo == mobile && x.IsDeleted == false).FirstOrDefault();

                    if (oldOTPEntry != null && oldOTPEntry.CreatedDate.Value.AddMinutes(4) < DateTime.Now)
                    {
                        oldOTPEntry.IsDeleted = true;
                        db.SaveChanges();
                        oldOTPEntry = null;
                    }

                    if ((oldOTPEntry != null) && (oldOTPEntry.IsDeleted == false))
                    {
                        OTP = oldOTPEntry.OTP;
                        Task.Run(async () => await oTPService.ResendOTP(mobile));
                    }
                    else
                    {
                        var newOTPEntry = new CustomerOTPVerify()
                        {
                            ContactNo = mobile,
                            CustomerId = recipient.GiftRecipientId,
                            IsDeleted = false,
                            OTP = OTP,
                            CreatedDate = DateTime.Now,
                        };

                        db.CustomerOTPVerifies.Add(newOTPEntry);
                        db.SaveChanges();
                        Task.Run(async () => await oTPService.SendOTP(mobile, OTP));
                        //await _messageService.SendOTP(mobile, $"{OTP}");

                    }

                    _message = "OTP has been sent";
                    _data = true;
                    id =Convert.ToInt32(ordId);
                    WebEngageController webEngageController = new WebEngageController();
                    Task.Run(async () => await webEngageController.WebEngageStatusCall(id, "Gift Contact Opened"));

                }
                else
                {
                    _data = false;
                    _message = "Mobile number does not exists";
                }

            }
            else
            {
                _data = false;
                _message = "Authorization Failed";
            }
            return Json(new { Data = _data, message = _message,oId=id }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult VerifyOTP(OTPVerifyRequestDO request)
        {
            var customerOTP = db.CustomerOTPVerifies.Where(x => x.ContactNo == request.Mobile && x.OTP == request.OTP && x.IsDeleted == false).FirstOrDefault();
           
            if (customerOTP != null)
            {
                customerOTP.VerifiedDate = DateTime.Now;
                customerOTP.HashCode = GenerateHashCode();
                db.SaveChanges();

                _message = "OTP is verified";
                _data = new { IsVerified = true, HashCode = customerOTP.HashCode };
                var recipient = giftBagDBO.GetUserRecipientOrder(request.Mobile);
                if (recipient != null)
                {
                    WebEngageController webEngageController = new WebEngageController();
                    Task.Run(async () => await webEngageController.WebEngageStatusCall(recipient.OrderId, "Gift OTP Verified"));
                }
            }
            else
            {
                _data = new { IsVerified = false, HashCode = string.Empty }; ;
                _message = "You have entered wrong OTP";
            }
            responseStatus.Message = _message;
            responseStatus.Data = _data;
            return Json(new { Data = _data, message = _message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ValidateRecipient(string mobile)
        {
            string token = string.Empty;
            var recipient = giftBagDBO.GetUserRecipientOrder(mobile);
            if (recipient != null )
            {
                 token = Encrypt(recipient.OrderId.ToString());
                _data = true;
                WebEngageController webEngageController = new WebEngageController();
                Task.Run(async () => await webEngageController.WebEngageStatusCall(recipient.OrderId, "Gift Contact Opened"));
            }
            else if (recipient==null)
            {
                _message = "Sorry! This number is not a registered gift recipient";
                _data = false;
            }

            return Json(new { Data = _data, message = _message, token= token }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cocktail()
        {
            return View();
        }

        #region Non Action
        public static string GenerateHashCode()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public static string GenerateOTP()
        {
            Random rnd = new Random();
            int number = rnd.Next(0, 1000000);

            return number.ToString("D6");
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                cipherText = cipherText.Replace(" ", "+");
                //cipherText = "HYtMQvIjw4/tqzqwJN1HQ==";
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] cipherBytes = Convert.FromBase64String(cipherText.Trim());
                using (Aes encryptor = Aes.Create())
                {
                    
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {

                            
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {

                return "123";
            }
        }

        #endregion
    }
}