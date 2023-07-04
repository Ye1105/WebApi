using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Blogs
{
    public class BlogVideoLike
    {
        [Key]
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("vId")]
        public Guid VId { get; set; }

        [JsonProperty("uId")]
        public Guid UId { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}