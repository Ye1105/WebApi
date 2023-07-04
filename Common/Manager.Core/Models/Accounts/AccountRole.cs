using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Accounts
{
    /// <summary>
    /// 用户 和 角色 关系表
    /// </summary>
    public class AccountRole
    {
        [Key]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid UId { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        [JsonProperty("rId")]
        public Guid RId { get; set; }
    }
}