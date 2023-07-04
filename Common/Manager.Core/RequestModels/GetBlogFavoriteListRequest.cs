using Manager.Core.Page;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class GetBlogFavoriteListRequest : QueryParameters
    {
        /// <summary>
        /// 当前网站的登录用户id
        /// </summary>
        [JsonProperty("wId")]
        public Guid WId { get; set; }
    }
}