using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class UpdateGroupRequest
    {
        [JsonProperty("grps")]
        public string[] Grps { get; set; }
    }
}