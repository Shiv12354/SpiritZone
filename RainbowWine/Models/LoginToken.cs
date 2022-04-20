using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace RainbowWine.Models
{
    public class LoginToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("isOTPVerified")]
        public bool IsOTPVerified { get; set; }
    }
}