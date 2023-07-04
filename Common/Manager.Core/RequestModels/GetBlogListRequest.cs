﻿using Manager.Core.Enums;
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
        /// 当前登录网站的用户
        /// </summary>
        [JsonProperty("wId")]
        public Guid? WId { get; set; }

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
        /// 是否原创
        /// </summary>
        [JsonProperty("isFId")]
        public bool? IsFId { get; set; }

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

        [JsonProperty("status")]
        public Status? Status { get; set; }
    }
}