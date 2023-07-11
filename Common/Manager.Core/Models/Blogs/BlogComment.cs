using Manager.Core.Enums;
using Manager.Core.Models.Accounts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Blogs
{
    public class BlogComment
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

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
        /// 0.评论   1.【回复】来评论【评论】  2.【回复】来评论【回复】
        /// </summary>
        [JsonProperty("type")]
        public sbyte Type { get; set; }

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

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// 0 非置顶  1 置顶
        /// </summary>
        [JsonProperty("top")]
        public sbyte? Top { get; set; } = (sbyte)BoolType.NO;

        /// <summary>
        /// 0 启用  1 禁用 2 审核中 3 审核失败
        /// </summary>
        [JsonProperty("status")]
        public sbyte? Status { get; set; } = (sbyte)Enums.Status.ENABLE;

        /// <summary>
        /// 点赞数
        /// </summary>
        [NotMapped]
        [JsonProperty("like")]
        public int? Like { get; set; }

        /// <summary>
        /// 评论用户信息
        /// </summary>
        [NotMapped]
        [JsonProperty("uInfo")]
        public AccountInfo? UInfo { get; set; }

        /// <summary>
        /// 被评论用户信息
        /// </summary>
        [NotMapped]
        [JsonProperty("buInfo")]
        public AccountInfo? BuInfo { get; set; }

        /// <summary>
        /// 当前网站登录用户wId是否点赞
        /// </summary>
        [NotMapped]
        [JsonProperty("isLike")]
        public bool? IsLike { get; set; } = false;

        /// <summary>
        /// 回复总数
        /// </summary>
        [NotMapped]
        [JsonProperty("replyCount")]
        public int? ReplyCount { get; set; } = 0;

        /// <summary>
        /// 当前评论的回复数据
        /// </summary>
        [NotMapped]
        [JsonProperty("replyList")]
        public IList<BlogComment>? ReplyList { get; set; } = new List<BlogComment>();
    }
}