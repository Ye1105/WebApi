using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class RetrieveMailRequest
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [JsonProperty("mail")]
        public string Mail { get; set; }

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