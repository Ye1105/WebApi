using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.RequestModels
{
    public class RetrieveMailRequest
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [EmailAddress]
        [JsonProperty("mail")]
        public string Mail { get; set; }

        /// <summary>
        /// 新的密码
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// sms
        /// </summary>
        [JsonProperty("sms")]
        public string Sms { get; set; }
    }
}