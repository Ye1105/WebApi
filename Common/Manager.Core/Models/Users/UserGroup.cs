using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Users
{
    /// <summary>
    /// 对关注的博客所属用户的分组
    /// </summary>
    public class UserGroup
    {
        [Key]
        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid UId { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("grp")]
        public string? Grp { get; set; }
    }
}