using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace RainbowWine.Models.Zoho
{ 
    public partial class APIOutput
    {
        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ticketNumber")]
        public long TicketNumber { get; set; }

        [JsonProperty("layoutId")]
        public string LayoutId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusType")]
        public string StatusType { get; set; }

        [JsonProperty("createdTime")]
        public DateTimeOffset CreatedTime { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subCategory")]
        public string SubCategory { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("dueDate")]
        public DateTimeOffset? DueDate { get; set; }

        [JsonProperty("responseDueDate")]
        public object ResponseDueDate { get; set; }

        [JsonProperty("commentCount")]
        public long CommentCount { get; set; }

        [JsonProperty("sentiment")]
        public object Sentiment { get; set; }

        [JsonProperty("threadCount")]
        public long ThreadCount { get; set; }

        [JsonProperty("closedTime")]
        public DateTimeOffset? ClosedTime { get; set; }

        [JsonProperty("onholdTime")]
        public object OnholdTime { get; set; }

        [JsonProperty("departmentId")]
        public string DepartmentId { get; set; }

        [JsonProperty("contactId")]
        public string ContactId { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("assigneeId")]
        public string AssigneeId { get; set; }

        [JsonProperty("teamId")]
        public object TeamId { get; set; }

        [JsonProperty("channelCode")]
        public object ChannelCode { get; set; }

        [JsonProperty("webUrl")]
        public Uri WebUrl { get; set; }

        [JsonProperty("lastThread")]
        public LastThread LastThread { get; set; }

        [JsonProperty("customerResponseTime")]
        public DateTimeOffset CustomerResponseTime { get; set; }

        [JsonProperty("isArchived")]
        public bool IsArchived { get; set; }

        [JsonProperty("isSpam")]
        public bool IsSpam { get; set; }

        [JsonProperty("source")]
        public Source Source { get; set; }
    }

    public partial class LastThread
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("isDraft")]
        public bool IsDraft { get; set; }

        [JsonProperty("isForward")]
        public bool IsForward { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }
    }

    public partial class Source
    {
        [JsonProperty("appName")]
        public object AppName { get; set; }

        [JsonProperty("appPhotoURL")]
        public object AppPhotoUrl { get; set; }

        [JsonProperty("permalink")]
        public object Permalink { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("extId")]
        public object ExtId { get; set; }
    }

}