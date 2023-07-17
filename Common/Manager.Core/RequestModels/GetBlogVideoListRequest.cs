using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetBlogVideoListRequest : QueryParameters
    {
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        [JsonProperty("bId")]
        public Guid? BId { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("channel")]
        public string? Channel { get; set; }

        [JsonProperty("collection")]
        public string? Collection { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("startTime")]
        public DateTime? StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }
    }
}