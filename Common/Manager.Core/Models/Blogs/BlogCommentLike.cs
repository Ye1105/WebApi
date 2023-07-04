using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Blogs
{
    public class BlogCommentLike
    {
        [Key]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 评论id
        /// </summary>
        [JsonProperty("cId")]
        public Guid? CId { get; set; }

        /// <summary>
        /// 点赞的用户
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }
    }
}