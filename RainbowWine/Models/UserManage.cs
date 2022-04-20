using RainbowWine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public class OrderInputParam
    {
        public int OrderId { get; set; }
    }
    public class InputDateParam
    {
        public DateTime? Date { get; set; }
    }
    public class UserManageGetUserByPhone_Req
    {
        public string PhoneNo { get; set; }
    }

    public class UserManageFliter_Req
    {
        public DateTime CreatedDate { get; set; }
    }

    public class UserManageGetUserByPhone_Resp
    {
        public string PhoneNo { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string RegisterSource { get; set; }
    }

    public class UserGenerateToken_Req
    {
        public string PhoneNo { get; set; }
        public int? CustomerId { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
        public string Pin { get; set; }
        public int OrderId { get; set; }
        public int[] OrderIds { get; set; }

    }
    public class ChangePasswordByPhoneNo_Req
    {
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
    public class ResetPasswordByEmail_Req
    {
        public string OldPassword { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }

    }

    public class Insert_UserRatings_Req
    {
        public int? Rating { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public string Comment { get; set; }
        public int? OrderId { get; set; }
    }
    public class NotifyOrderDelivery_Req
    {
        public int? OrderId { get; set; }
    }
    public class OrderDelivery_Req
    {
        public string OrderId { get; set; }
    }

    public class GetNotifyOrderDelivery_Resp
    {
        public int? NotifyId { get; set; }
        public int? CustomerId { get; set; }
        public int? NotificationTypeId { get; set; }
        public string NotifyName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsNotified { get; set; }
        public int? OrderId { get; set; }

    }
    public class UpdateNotifyOrderDelivery_Req
    {
        public int? NotifyId { get; set; }
    }
    public class OrderDetails_Req
    {
        public int? OrderId { get; set; }
    }
    public class OrderDetails_Resp
    {
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? ShopID { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderPlacedBy { get; set; }
        public decimal? OrderAmount { get; set; }
        public int? OrderStatusId { get; set; }
        public string OrderTo { get; set; }
        public string CustAddress { get; set; }
        public string FormattedAddress { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string Region { get; set; }
        public string ProductDescription { get; set; }
        public int? ProductRefID { get; set; }
        public string ProductType { get; set; }
        public string Category { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopPhoneNo { get; set; }
        public string LicNo { get; set; }
        public string CLNo { get; set; }
        public string GST { get; set; }
        public string VAT { get; set; }
    }

    public class GetRatingStarOptionResp
    {
        public List<RatingStar> lstRatingStar { get; set; }
        public List<RatingOption> lstRatingOption { get; set; }
         
    }

    public class InsertNotifyProductAval_Req
    {
        public int? ProuctID { get; set; }
        public int? ShopId { get; set; }
    }
     
    public class GetLastOrderDetails_Resp
    {
        public int? OrderID { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? DeliveryDateTime { get; set; }
        public int? CustomerId { get; set; }
        public Insert_UserRatings_Req Review { get; set; }

        //public List<Insert_UserRatings_Req> Review { get; set; }
    }

    public class GetProductNotificationDetails_Req
    {
        public int? ProuctId { get; set; }
        public int? ShopId { get; set; }
    }
    public class GetAllPremiumProductDetails_Req
    {
        public int? shopId { get; set; }
    }
    public class RecentOrderDetails_req
    {
        public string GroupID { get; set; }
        public int? OrderId { get; set; }
        public string Request { get; set; }
        public string QuestionID { get; set; }
        public string Question { get; set; }
    }

    public class RecentOrderDetails
    {
        public string Text { get; set; }
        public string GroupOne { get; set; }
        public string GroupTwo { get; set; }
        public string DateText { get; set; }
        public string TimeText { get; set; }
        public List<RecentOrderDetails_Resp> QuestionsResp { get; set; }
    }

    public class RecentOrderDetails_Resp
    {
        public int? OrderID { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? DeliveryDateTime { get; set; }
        public int? CustomerId { get; set; }
        public string Options { get; set; }
    }
    public class QuestionTitle
    {
        public string Title { get; set; }
        public int? OrderID { get; set; }
        public string DateText { get; set; }
        public string TimeText { get; set; }
        public List<TicketQuestionDetailsResp> QuestionsResp { get; set; }
    }
    public class TicketQuestionDetailsResp
    {
        public int? QuestionID { get; set; }
        public string Question { get; set; }
        public string GroupOne { get; set; }
        public string GroupTwo { get; set; }
        public int? OrderID { get; set; }

    }

    public class InsertOrderIssue_Req
    {
        public int? OrderId { get; set; }
        public string Issue { get; set; }
        
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Cf
    {
        public object cf_permanentaddress { get; set; }
        public object cf_dateofpurchase { get; set; }
        public object cf_phone { get; set; }
        public object cf_numberofitems { get; set; }
        public object cf_url { get; set; }
        public object cf_secondaryemail { get; set; }
        public string cf_severitypercentage { get; set; }
        public string cf_modelname { get; set; }

    }

    public class ChatAPI
    {
        public string subCategory { get; set; }
        public Cf cf { get; set; }
        public string productId { get; set; }
        public string contactId { get; set; }
        public string subject { get; set; }
        public string dueDate { get; set; }
        public string departmentId { get; set; }
        public string channel { get; set; }
        public string description { get; set; }
        public string priority { get; set; }
        public string classification { get; set; }
        public string assigneeId { get; set; }
        public string phone { get; set; }
        public string category { get; set; }
        public string email { get; set; }
        public string status { get; set; }

    }

    public class ChatApiRespDetails
    {
       
        public string Subject { get; set; }
        public string Description { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }


    }




}