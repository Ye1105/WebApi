using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.RequestModels
{
    public class RetrievePhoneRequest
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [JsonProperty("phone")]
        [Phone]
        public string Phone { get; set; }

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