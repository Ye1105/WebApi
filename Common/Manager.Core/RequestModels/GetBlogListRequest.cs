using Manager.Core.Enums;
using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetBlogListRequest : QueryParameters
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// 发表博客的用户
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 种类： 0.公开  1.仅自己可见  2.好友圈  3.粉丝  4.热推  5.广告
        /// </summary>
        [JsonProperty("sort")]
        public BlogSort? Sort { get; set; }

        /// <summary>
        /// 类型    0.全部  1.文本   2.头条文章  3.图片   4.音乐   4.视频
        /// </summary>
        [JsonProperty("type")]
        public BlogType? Type { get; set; }

        /// <summary>
        /// 0 默认不处理  1 原创  2 转发 
        /// </summary>
        [JsonProperty("fId")]
        public BlogForwardType? FId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonProperty("startTime")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 1.[主页的博客]=>我自己+朋友圈+我是粉丝的博客
        /// 2.[朋友圈的博客]=>只查询朋友圈的博客
        /// 3.[特别关注的博客]=>只查询特别关注的博客
        /// 4.[自定义分组的博客]=>只查询自定义分组的博客
        /// </summary>
        [JsonProperty("scope")]
        public BlogScope? Scope { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("grp")]
        public string? Grp { get; set; }
    }
}