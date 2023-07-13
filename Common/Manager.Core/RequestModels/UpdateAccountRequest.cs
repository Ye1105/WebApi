using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.RequestModels
{
    public class UpdateAccountRequest
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// 账号名称
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

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