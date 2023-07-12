using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Blogs
{
    public class BlogTopic
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 0.新  1.推荐  2.热搜
        /// </summary>
        [JsonProperty("type")]
        public sbyte Type { get; set; }

        /// <summary>
        /// 阅读数量
        /// </summary>
        [JsonProperty("readCount")]
        public int ReadCount { get; set; }

        /// <summary>
        /// 讨论数量
        /// </summary>
        [JsonProperty("discussCount")]
        public int DiscussCount { get; set; }

        /// <summary>
        /// 搜索数量
        /// </summary>
        [JsonProperty("searchCount")]
        public int SearchCount { get; set; }

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
    }
}