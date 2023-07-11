using Manager.Core.Models.Accounts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Blogs
{
    public class BlogForward
    {
        [Key]
        /// <summary>
        /// 转发的同时会产生一条新的blog记录，Id表示这条blog的Id
        /// </summary>
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// 转发的同时会产生一条新的blog记录，BId表示这条blog的uId
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 被转发的用户id
        /// </summary>
        [JsonProperty("buId")]
        public Guid? BuId { get; set; }

        /// <summary>
        /// 转发信息
        /// </summary>
        [JsonProperty("message")]
        public string? Message { get; set; }

        /// <summary>
        /// 被转发的最原始blog id
        /// </summary>
        [JsonProperty("baseBId")]
        public Guid? BaseBId { get; set; }

        /// <summary>
        /// 上一个被转发的blog id
        /// </summary>
        [JsonProperty("prevBId")]
        public Guid? PrevBId { get; set; }

        /// <summary>
        /// 如果是转发评论，则上一个转转发的评论的id
        /// </summary>
        [JsonProperty("prevCId")]
        public Guid? PrevCId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// 0 启用  1 禁用 2 审核中 3 审核失败
        /// </summary>
        [JsonProperty("status")]
        public sbyte? Status { get; set; } = (sbyte)Enums.Status.ENABLE;

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
        /// 关联的blog
        /// </summary>
        [NotMapped]
        [JsonProperty("blog")]
        public Blog? Blog { get; set; }

        /// <summary>
        /// 当前网站登录用户wId是否点赞
        /// </summary>
        [NotMapped]
        [JsonProperty("isLike")]
        public bool? IsLike { get; set; } = false;

        /// <summary>
        /// 点赞数
        /// </summary>
        [NotMapped]
        [JsonProperty("like")]
        public int? Like { get; set; }
    }
}