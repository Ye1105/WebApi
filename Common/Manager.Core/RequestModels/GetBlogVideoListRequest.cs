using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetBlogVideoListRequest : QueryParameters
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 博客id
        /// </summary>
        [JsonProperty("bId")]
        public Guid? BId { get; set; }

        /// <summary>
        /// 当前网站登录的用户
        /// </summary>
        [JsonProperty("wId")]
        public Guid? WId { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("channel")]
        public string? Channel { get; set; }

        [JsonProperty("collection")]
        public string? Collection { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

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
        /// 0 启用  1 禁用  2 审核中  3 审核失败
        /// </summary>
        [JsonProperty("status")]
        public sbyte? Status { get; set; } = (sbyte)Enums.Status.ENABLE;
    }
}