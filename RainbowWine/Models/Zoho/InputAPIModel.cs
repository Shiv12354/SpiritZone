using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace RainbowWine.Models.Zoho
{
    public partial class InputAPIModel
    {
        [JsonProperty("subCategory")]
        public string SubCategory { get; set; }

        [JsonProperty("cf")]
        public Cf Cf { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("contactId")]
        public string ContactId { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("dueDate")]
        public string DueDate { get; set; }

        [JsonProperty("departmentId")]
        public string DepartmentId { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("classification")]
        public string Classification { get; set; }

        [JsonProperty("assigneeId")]
        public string AssigneeId { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class Cf
    {
        [JsonProperty("cf_permanentaddress")]
        public object CfPermanentaddress { get; set; }

        [JsonProperty("cf_dateofpurchase")]
        public object CfDateofpurchase { get; set; }

        [JsonProperty("cf_phone")]
        public object CfPhone { get; set; }

        [JsonProperty("cf_numberofitems")]
        public object CfNumberofitems { get; set; }

        [JsonProperty("cf_url")]
        public object CfUrl { get; set; }

        [JsonProperty("cf_secondaryemail")]
        public object CfSecondaryemail { get; set; }

        [JsonProperty("cf_severitypercentage")]
        public string CfSeveritypercentage { get; set; }

        [JsonProperty("cf_modelname")]
        public string CfModelname { get; set; }
    }


    public class OutputAPIModel
    {
        public DateTime modifiedTime { get; set; }
        public string subCategory { get; set; }
        public string statusType { get; set; }
        public string subject { get; set; }
        public DateTime dueDate { get; set; }
        public string departmentId { get; set; }
        public string channel { get; set; }
        public object onholdTime { get; set; }
        public Source source { get; set; }
        public object resolution { get; set; }
        public List<object> sharedDepartments { get; set; }
        public object closedTime { get; set; }
        public string approvalCount { get; set; }
        public bool isTrashed { get; set; }
        public DateTime createdTime { get; set; }
        public string id { get; set; }
        public bool isResponseOverdue { get; set; }
        public DateTime customerResponseTime { get; set; }
        public object productId { get; set; }
        public string contactId { get; set; }
        public string threadCount { get; set; }
        public List<object> secondaryContacts { get; set; }
        public string priority { get; set; }
        public object classification { get; set; }
        public string commentCount { get; set; }
        public string taskCount { get; set; }
        public object accountId { get; set; }
        public string phone { get; set; }
        public string webUrl { get; set; }
        public bool isSpam { get; set; }
        public string status { get; set; }
        public string ticketNumber { get; set; }
        public object customFields { get; set; }
        public bool isArchived { get; set; }
        public string description { get; set; }
        public string timeEntryCount { get; set; }
        public object channelRelatedInfo { get; set; }
        public object responseDueDate { get; set; }
        public bool isDeleted { get; set; }
        public string modifiedBy { get; set; }
        public object email { get; set; }
        public object layoutDetails { get; set; }
        public object channelCode { get; set; }
        public Cf cf { get; set; }
        public string layoutId { get; set; }
        public object assigneeId { get; set; }
        public object teamId { get; set; }
        public string attachmentCount { get; set; }
        public string category { get; set; }

    }

}