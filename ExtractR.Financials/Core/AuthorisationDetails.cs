using Newtonsoft.Json;

namespace ExtractR.Financials.Core
{
    public abstract class AuthorisationDetails
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }
        [JsonProperty("reference")]
        public string Reference { get; set; }
    }
}
