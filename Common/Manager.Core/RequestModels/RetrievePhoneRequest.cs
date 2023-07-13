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
        public string Phone { get; set; }

        /// <summary>
        /// 新的密码
        /// </summary>
        [JsonProperty("pwd")]
        public string Pwd { get; set; }

        /// <summary>
        /// sms
        /// </summary>
        [JsonProperty("sms")]
        public string Sms { get; set; }
    }
}