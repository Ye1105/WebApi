using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Blogs
{
    public class BlogImageLike
    {
        [Key]
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("iId")]
        public Guid IId { get; set; }

        [JsonProperty("uId")]
        public Guid UId { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}