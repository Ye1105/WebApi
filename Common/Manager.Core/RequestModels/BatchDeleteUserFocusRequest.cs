using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class BatchDeleteUserFocusRequest
    {
        [JsonProperty("uIds")]
        public List<Guid> UIds { get; set; }
    }
}