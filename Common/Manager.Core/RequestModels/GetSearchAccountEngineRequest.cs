using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetSearchAccountEngineRequest : QueryParameters
    {
        /// <summary>
        /// uId
        /// </summary>
        [JsonProperty("uId")]
        public Guid UId { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [JsonProperty("mail")]
        public string Mail { get; set; }

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