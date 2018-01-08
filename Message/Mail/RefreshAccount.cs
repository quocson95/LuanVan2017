using System;
using Newtonsoft.Json;

namespace FreeHand.Message.Mail
{
    [JsonObject]
    public class RefreshAccount
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }               
    }
}
