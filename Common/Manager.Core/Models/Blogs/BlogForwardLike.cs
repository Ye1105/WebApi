using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Blogs
{
    public class BlogForwardLike
    {
        [Key]
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("fId")]
        public Guid FId { get; set; }

        [JsonProperty("uId")]
        public Guid UId { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}