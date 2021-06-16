using Newtonsoft.Json;

namespace EVERFI.Foundry.Classes
{
    internal class AccessToken
    {
        [JsonProperty("access_token")]
        internal string access_token { get; set; }
        [JsonProperty("token_type")]
        internal string token_type { get; set; }
        [JsonProperty("expires_in")]
        int expires_in { get; set; }
        [JsonProperty("created_at")]
        int created_at { get; set; }
    }
}