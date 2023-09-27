using Manager.Core.Enums;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class AddBlogCommentAndForwardRequest
    {
        [JsonProperty("blog")]
        public BlogRequest Blog { get; set; }

        [JsonProperty("blogComment")]
        public BlogCommentRequest BlogComment { get; set; }

        [JsonProperty("blogForward")]
        public BlogForwardRequest BlogForward { get; set; }
    }

    #region BlogRequest 参数

    public class BlogRequest
    {
        /// <summary>
        /// 种类： 0.公开  1.仅自己可见  2.好友圈  3.粉丝  4.热推  5.广告
        /// </summary>
        [JsonProperty("sort")]
        public BlogSort? Sort { get; set; }

        /// <summary>
        /// 类型  -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)
        /// </summary>
        [JsonProperty("type")]
        public BlogType? Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// 转发Id
        /// </summary>
        [JsonProperty("fId")]
        public Guid FId { get; set; }
    }

    #endregion BlogRequest 参数

    #region BlogCommentRequest 参数

    public class BlogCommentRequest
    {

        /// <summary>
        /// blog表Id
        /// </summary>
        [JsonProperty("bId")]
        public Guid BId { get; set; }

        /// <summary>
        /// 被评论的用户Id => BeCommentUserId
        /// </summary>
        [JsonProperty("buId")]
        public Guid BuId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 0.评论   1.【回复】来评论【评论】  2.【回复】来评论【回复】
        /// </summary>
        [JsonProperty("type")]
        public CommentType Type { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [JsonProperty("pId")]
        public Guid PId { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("grp")]
        public Guid Grp { get; set; }
    }

    #endregion BlogCommentRequest 参数

    #region BlogForwardRequest 参数

    public class BlogForwardRequest
    {
        /// <summary>
        /// 转发信息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 被转发的最原始blog id
        /// </summary>
        [JsonProperty("baseBId")]
        public Guid BaseBId { get; set; }

        /// <summary>
        /// 上一个被转发的blog id
        /// </summary>
        [JsonProperty("prevBId")]
        public Guid PrevBId { get; set; }

        /// <summary>
        /// 上一个被转发的blog uId
        /// </summary>
        [JsonProperty("buId")]
        public Guid BuId { get; set; }

        /// <summary>
        /// 如果是转发评论，则上一个转转发的评论的id
        /// </summary>
        [JsonProperty("prevCId")]
        public Guid PrevCId { get; set; }
    }

    #endregion BlogForwardRequest 参数
}