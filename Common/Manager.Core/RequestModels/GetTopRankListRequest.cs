using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetTopRankListRequest : QueryParameters
    {
        /// <summary>
        /// 当前登录网站的用户
        /// </summary>
        [JsonProperty("wId")]
        public Guid? WId { get; set; }
    }
}