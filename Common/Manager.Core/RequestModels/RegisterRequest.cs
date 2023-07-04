using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.RequestModels
{
    public class RegisterRequest
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [JsonProperty("phone")]
        [RegularExpression(@"^1[3456789]\d{9}$")]
        public string Phone { get; set; }

        /// <summary>
        /// 腾讯sms
        /// </summary>
        [JsonProperty("sms")]
        [RegularExpression(@"^[0-9]{6}")]
        public string Sms { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [JsonProperty("nickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [JsonProperty("password")]
        public string Pssword { get; set; }
    }
}