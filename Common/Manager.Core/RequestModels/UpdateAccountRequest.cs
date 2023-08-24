using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.RequestModels
{
    public class UpdateAccountRequest
    {

        /// <summary>
        /// 手机
        /// </summary>
        [Phone]
        [JsonProperty("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress]
        [JsonProperty("mail")]
        public string? Mail { get; set; }
    }
}