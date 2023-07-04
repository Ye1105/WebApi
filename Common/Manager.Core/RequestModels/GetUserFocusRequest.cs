using Manager.Core.Enums;
using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetUserFocusRequest : QueryParameters
    {
        /// <summary>
        /// 关注人id
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 被关注的用户 => BefocusUserId
        /// </summary>
        [JsonProperty("buId")]
        public Guid? BuId { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("grp")]
        public string? Grp { get; set; }

        /// <summary>
        /// 0：关注 1：特别关注
        /// </summary>
        [JsonProperty("relation")]
        public RelationEnum? Relation { get; set; }

        /// <summary>
        /// 关注的渠道 0 Mobile 1 HTML5
        /// </summary>
        [JsonProperty("channel")]
        public sbyte? Channel { get; set; }

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
        /// 当前网站的登录用户id
        /// </summary>
        [JsonProperty("wId")]
        public Guid? WId { get; set; }

        /// <summary>
        /// 是否关联 UId 的用户信息
        /// </summary>
        [JsonProperty("isUInfo")]
        public bool? IsUInfo { get; set; }

        /// <summary>
        /// 是否关联BuId的用户信息
        /// </summary>
        [JsonProperty("isBuInfo")]
        public bool? IsBuInfo { get; set; }
    }
}