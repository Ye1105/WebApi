using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Users
{
    public class UserTopic
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 话题Id
        /// </summary>
        [JsonProperty("topicId")]
        public Guid? TopicId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid UId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }
    }
}