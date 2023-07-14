using Manager.Core.Enums;
using Manager.Core.Models.Accounts;
using Manager.Core.Models.Users;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Blogs
{
    public class Blog
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 种类： 0.公开  1.仅自己可见  2.好友圈  3.粉丝  4.热推  5.广告
        /// </summary>
        [JsonProperty("sort")]
        public sbyte Sort { get; set; }

        /// <summary>
        /// 类型  -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)
        /// </summary>
        [JsonProperty("type")]
        public sbyte? Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty("body")]
        public string? Body { get; set; }

        /// <summary>
        /// 转发Id
        /// </summary>
        [JsonProperty("fId")]
        public Guid? FId { get; set; }

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
        /// 转发blog
        /// </summary>
        [NotMapped]
        [JsonProperty("fBlog")]
        public Blog? FBlog { get; set; }

        /// <summary>
        /// 图片集合
        /// </summary>
        [NotMapped]
        [JsonProperty("images")]
        public IList<BlogImage>? Images { get; set; } = new List<BlogImage>();

        /// <summary>
        /// 视频
        /// </summary>
        [NotMapped]
        [JsonProperty("video")]
        public BlogVideo? Video { get; set; }

        /// <summary>
        /// 用户信息表
        /// </summary>
        [NotMapped]
        [JsonProperty("accountInfo")]
        public AccountInfo? AccountInfo { get; set; }

        /// <summary>
        /// 关系表
        /// </summary>
        [NotMapped]
        [JsonProperty("userFocus")]
        public UserFocus? UserFocus { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        [NotMapped]
        [JsonProperty("comment")]
        public int? Comment { get; set; }

        /// <summary>
        /// 转发数
        /// </summary>
        [NotMapped]
        [JsonProperty("forward")]
        public int? Forward { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        [NotMapped]
        [JsonProperty("like")]
        public Int64? Like { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        [NotMapped]
        [JsonProperty("favorite")]
        public Int64? Favorite { get; set; }

        /// <summary>
        /// 当前网站的登录用户是否点赞
        /// </summary>
        [NotMapped]
        [JsonProperty("isLike")]
        public bool? IsLike { get; set; } = false;

        /// <summary>
        /// 是否收藏
        /// </summary>
        [NotMapped]
        [JsonProperty("isFavorite")]
        public bool? IsFavorite { get; set; } = false;
    }
}