using RainbowWine.Data;
using RainbowWine.Models;
using RainbowWine.Services;
using RainbowWine.Services.Gateway;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace RainbowWine.Controllers
{
    [RoutePrefix("payment")]
    [EnableCors("*", "*", "*")]
    public class PaymentController : Controller
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("cashfree/return")]
        [Route("cashfree/notify")]
        public ActionResult CashFreeReturn()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string pecodeValue = new StreamReader(req).ReadToEnd();
            //pecodeValue = "orderId=26322&orderAmount=1000.00&referenceId=536877&txStatus=SUCCESS&paymentMode=CREDIT_CARD&txMsg=Transaction Successful&txTime=2020-09-13 10:06:16&signature=lQyvA7iB8p7ERl4hCpkp1Le27bpkrZU65zOJ1eargb4=";

            string pdecode = Server.UrlDecode(pecodeValue);
            string pdecodevalue = Server.UrlDecode(pdecode);
            //Write to file
            //SpiritUtility.Logging("Payment inputs", pecodeValue);

            CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
            CashFreePaymentNotify payNodtify = new CashFreePaymentNotify();
            var ret = payNodtify.ProcessPayment(decodevlaue, pdecode);

            string msg = "";
            if (ret.Status)
            {
                if (string.Compare(ret.Message, "Approved", true) == 0)
                    msg = "Thank you for payment.";
                else if (string.Compare(ret.Message, "AlreadyApproved", true) == 0)
                    msg = "Thank you for payment.";
                else if (string.Compare(ret.Message, "AmountNotMatch", true) == 0)
                    msg = "Thank you, payment made is miss match.";
                else msg = "Please contact administrator.";
            }
            ViewBag.msg = msg;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("cashfree/modify/return")]
        [Route("cashfree/modify/notify")]
        public ActionResult CashFreeModify()
        {
            string msg = "";
            try
            {
                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                try
                {
                    string pecodeValue = new StreamReader(req).ReadToEnd();
                    //pecodeValue = "orderId%3D151%26orderAmount%3D1260.00%26referenceId%3D365109%26txStatus%3DSUCCESS%26paymentMode%3DCREDIT_CARD%26txMsg%3DTransaction%20Successful%26txTime%3D2020-06-17%2017%3A24%3A30%26signature%3Dj3454cDTeuOoEu8Zsq6aa%20SY%201NVXQpcpW%2FBpp27%2FFg%3D%26";
                    string pdecode = Server.UrlDecode(pecodeValue);
                    string pdecodevalue = Server.UrlDecode(pdecode);
                    //Write to file

                    CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = paymentLinkLogsService.UpdateOrderModifyToApprove(decodevlaue, pdecode);
                    msg = "Thank you for payment.";
                    //Colsed the issue
                    var ord2 = decodevlaue.OrderId2.Split('_');
                    if (ord2.Length == 4)
                    {
                        var modifyId = Convert.ToInt32(ord2[3]);
                        using (var db = new rainbowwineEntities())
                        {

                            var modify = db.OrderModifies.Find(modifyId);
                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            string uId = u.Id;

                            int issueClosed = (int)IssueType.Closed;
                            modify.StatusId = issueClosed;
                            db.SaveChanges();

                            var modifytrack = new OrderModifyTrack
                            {
                                OrderId = modify.OrderId,
                                OrderModifyId = modify.Id,
                                Remark = "Payment Succesfull",
                                OrderStatusId = issueClosed,
                                TrackDate = DateTime.Now,
                                UserId = uId,
                            };
                            db.OrderModifyTracks.Add(modifytrack);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = "Please contact administrator.";
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CashFreeReturn: {ex.Message}", ex.StackTrace);
                msg = "Please contact administrator.";
            }
            ViewBag.msg = msg;
            return View("CashFreeReturn");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("cashfree/issue/return")]
        [Route("cashfree/issue/notify")]
        public ActionResult CashFreeIssue()
        {
            string msg = "";
            try
            {
                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                try
                {
                    string pecodeValue = new StreamReader(req).ReadToEnd();
                    //pecodeValue = "orderId=55723_PartialPay_1_156&orderAmount=370.00&referenceId=227590574&txStatus=SUCCESS&paymentMode=DEBIT_CARD&txMsg=Transaction Success&txTime=2020-10-02 19:09:36&signature=QYwFu7iPeI1wbmlxjxtMbOt6Jm1GyN7dRbxekPp746A=&";
                    string pdecode = Server.UrlDecode(pecodeValue);
                    string pdecodevalue = Server.UrlDecode(pdecode);
                    //Write to file

                    CashFreeSetApproveResponse decodevlaue = SpiritUtility.GetCashFreeHooksDeserialize(pdecodevalue);
                    PaymentLinkLogsService paymentLinkLogsService = new PaymentLinkLogsService();
                    var ret = paymentLinkLogsService.UpdateOrderIssueToApprove(decodevlaue, pdecode);
                    msg = "Thank you for payment.";
                    //Colsed the issue
                    var ord2 = decodevlaue.OrderId2.Split('_');
                    if (ord2.Length == 4)
                    {
                        var issueId = Convert.ToInt32(ord2[3]);
                        using (var db = new rainbowwineEntities())
                        {

                            var issue = db.OrderIssues.Find(issueId);
                            var ema = ConfigurationManager.AppSettings["TrackEmail"];
                            var u = db.AspNetUsers.Where(o => o.Email == ema).FirstOrDefault();
                            string uId = u.Id;

                            int issueClosed = (int)IssueType.Closed;
                            issue.OrderStatusId = issueClosed;
                            db.SaveChanges();

                            var issetrack = new OrderIssueTrack
                            {
                                OrderId = issue.OrderId,
                                OrderIssueId = issue.OrderIssueId,
                                Remark = "Payment Succesfull",
                                OrderStatusId = 6,
                                TrackDate = DateTime.Now,
                                UserId = uId,
                            };
                            db.OrderIssueTracks.Add(issetrack);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = "Please contact administrator.";
                }
            }
            catch (Exception ex)
            {
                SpiritUtility.AppLogging($"Api_CashFreeReturn: {ex.Message}", ex.StackTrace);
                msg = "Please contact administrator.";
            }
            ViewBag.msg = msg;
            return View("CashFreeReturn");
        }

    }
}