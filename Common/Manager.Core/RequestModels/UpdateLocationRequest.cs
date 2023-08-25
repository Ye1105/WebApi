using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Manager.Core.RequestModels
{
    public class UpdateProvinceCityRequest
    {
        /// <summary>
        /// 省
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [JsonProperty("city")]
        public string? City { get; set; }
    }
}
