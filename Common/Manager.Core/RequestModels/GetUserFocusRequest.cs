using Manager.Core.Enums;
using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetRelationRequest : QueryParameters
    {
        /// <summary>
        /// 0：关注 1：特别关注
        /// </summary>
        [JsonProperty("relation")]
        public sbyte? Relation { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("grp")]
        public string? Grp { get; set; }


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
    }
}