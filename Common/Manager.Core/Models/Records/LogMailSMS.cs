using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Logs
{
    public class LogMailSMS
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [JsonProperty("mail")]
        public string? Mail { get; set; }

        /// <summary>
        /// sms
        /// </summary>
        [JsonProperty("sms")]
        public string? Sms { get; set; }

        /// <summary>
        /// 0 登录  1 重置  2 注册
        /// </summary>
        [JsonProperty("type")]
        public sbyte? Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }
    }
}