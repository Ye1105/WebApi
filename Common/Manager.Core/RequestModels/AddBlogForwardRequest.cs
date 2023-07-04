using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class AddBlogForwardRequest
    {
        [JsonProperty("blog")]
        public BlogRequest Blog { get; set; }

        [JsonProperty("blogForward")]
        public BlogForwardRequest BlogForward { get; set; }
    }
}