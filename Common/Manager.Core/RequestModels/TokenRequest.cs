using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class TokenRequest
    {
        //[JsonProperty("accessToken")]
        //public string AccessToken { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }
}