using Manager.Core.Enums;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class AddBlogCommentRequest
    {
        /// <summary>
        /// blog表Id
        /// </summary>
        [JsonProperty("bId")]
        public Guid? BId { get; set; }

        /// <summary>
        /// 评论用户Id  => CommentUserId
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 被评论的用户Id => BeCommentUserId
        /// </summary>
        [JsonProperty("buId")]
        public Guid? BuId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [JsonProperty("message")]
        public string? Message { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        [JsonProperty("like")]
        public int? Like { get; set; }

        /// <summary>
        ///类型 : 0.评论 1.【回复】来评论【评论】  2.【回复】来评论【回复】
        /// </summary>
        [JsonProperty("type")]
        public CommentType Type { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [JsonProperty("pId")]
        public Guid? PId { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("grp")]
        public Guid? Grp { get; set; }
    }
}