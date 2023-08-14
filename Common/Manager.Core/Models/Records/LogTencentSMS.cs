using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Logs
{
    /// <summary>
    /// 腾讯Sms
    /// </summary>
    public class LogTencentSMS
    {
        [Key]
        [JsonProperty("requestId")]
        public Guid RequestId { get; set; }

        [JsonProperty("sendStatusSet")]
        public string? SendStatusSet { get; set; }

        /// <summary>
        /// sms
        /// </summary>
        [JsonProperty("sms")]
        public string? Sms { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }
    }
}