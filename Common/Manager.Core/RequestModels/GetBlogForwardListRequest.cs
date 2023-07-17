using Manager.Core.Enums;
using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetBlogForwardListRequest : QueryParameters
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 被转发的用户id
        /// </summary>
        [JsonProperty("buId")]
        public Guid? BuId { get; set; }

        /// <summary>
        /// 被转发的最原始blog id
        /// </summary>
        [JsonProperty("baseBId")]
        public Guid? BaseBId { get; set; }

        /// <summary>
        ///  上一个被转发的blog id
        /// </summary>
        [JsonProperty("prevBId")]
        public Guid? PrevBId { get; set; }

        /// <summary>
        /// 被转发的评论id
        /// </summary>
        [JsonProperty("prevCId")]
        public Guid? PrevCId { get; set; }

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
        /// 范围
        /// </summary>
        [JsonProperty("scope")]
        public ForwardScope? Scope { get; set; }
    }
}